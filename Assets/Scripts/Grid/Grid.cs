using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid Instance;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] int rows;
    [SerializeField] int coloumns;

    public Tile[,] tiles;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }

    private void Start()
    {
        //GenerateGrid();
        if(tiles == null)
            FindGrid();
    }

    [ContextMenu("GenerateGrid")]
    private void GenerateGrid()
    {
        tiles = new Tile[coloumns, rows];

        Vector3 startPos = transform.position;
        GameObject temp = Instantiate(tilePrefab, startPos, Quaternion.identity, this.transform);
        MeshRenderer mr = temp.GetComponent<MeshRenderer>();
        float tileWidth = mr.bounds.size.x;
        float tileHeight = mr.bounds.size.z;

        bool destroyedInEditor = false;
#if UNITY_EDITOR
        DestroyImmediate(temp);
        destroyedInEditor = true;
#endif
        if(!destroyedInEditor)
            Destroy(temp);

        for (int x = 0; x < coloumns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 tilePos = Vector3.zero;

                float XOffset = tileWidth;
                float zOffset = tileHeight;

                tilePos = new Vector3(
                    startPos.x + XOffset * x,
                    startPos.y,
                    startPos.z + zOffset * y);

                GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, this.transform);
                Tile tileScr = tile.transform.GetComponent<Tile>();
                tileScr.x = x;
                tileScr.y = y;

                tile.transform.name = $"Square ({x}, {y})";
                tiles[x, y] = tileScr;
            }
        }

        CacheNeighbours();
    }

    
    private void CacheNeighbours()
    {
        foreach (Tile tile in tiles)
        {
            // West
            if (tile.x != 0)
            {
                tile.neighbourW = tiles[tile.x - 1, tile.y];
                tile.neighbours.Add(tile.neighbourW);
            }

            // South
            if (tile.y != 0)
            {
                tile.neighbourS = tiles[tile.x, tile.y - 1];
                tile.neighbours.Add(tile.neighbourS);
            }

            // East
            if (tile.x != tiles.GetLength(0) - 1)
            {
                tile.neighbourE = tiles[tile.x + 1, tile.y];
                tile.neighbours.Add(tile.neighbourE);
            }

            // North
            if (tile.y != tiles.GetLength(1) - 1)
            {
                tile.neighbourN = tiles[tile.x, tile.y + 1];
                tile.neighbours.Add(tile.neighbourN);
            }
        }
    }

    private void FindGrid()
    {
        tiles = new Tile[coloumns, rows];
        for (int x = 0; x < coloumns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject tile = GameObject.Find($"Square ({x}, {y})");
                if (tile != null)
                {
                    Tile xy = tile.GetComponent<Tile>();
                    if(xy != null)
                        tiles[x, y] = xy;
                }
            }
        }
    }
}
