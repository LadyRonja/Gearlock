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
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        tiles = new Tile[coloumns, rows];

        Vector3 startPos = transform.position;
        GameObject temp = Instantiate(tilePrefab, startPos, Quaternion.identity, this.transform);
        MeshRenderer mr = temp.GetComponent<MeshRenderer>();
        float tileWidth = mr.bounds.size.x;
        float tileHeight = mr.bounds.size.z;
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
                tileScr.X = x;
                tileScr.Y = y;

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
            if (tile.X != 0)
            {
                tile.NeighbourW = tiles[tile.X - 1, tile.Y];
                tile.neighbours.Add(tile.NeighbourW);
            }

            // South
            if (tile.Y != 0)
            {
                tile.NeighbourS = tiles[tile.X, tile.Y - 1];
                tile.neighbours.Add(tile.NeighbourS);
            }

            // East
            if (tile.X != tiles.GetLength(0) - 1)
            {
                tile.NeighbourE = tiles[tile.X + 1, tile.Y];
                tile.neighbours.Add(tile.NeighbourE);
            }

            // North
            if (tile.Y != tiles.GetLength(1) - 1)
            {
                tile.NeighbourN = tiles[tile.X, tile.Y + 1];
                tile.neighbours.Add(tile.NeighbourN);
            }
        }
    }
}
