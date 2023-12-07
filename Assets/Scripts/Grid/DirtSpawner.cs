using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirtSpawner : MonoBehaviour
{
    public static DirtSpawner Instance;

    [Header("Generics")]
    [SerializeField] GameObject dirtPrefab;
    [SerializeField] List<Material> boulderMaterial;

    [Header("Spawning on load")]
    [SerializeField] bool spawnOnLoad = false;
    [SerializeField] List<Vector2Int> spawnPositions;

    [Header("Manage in editor")]
    [SerializeField] List<Tile> spawnOnTiles;
    [SerializeField] Tile placeOnTile;
    [SerializeField] Dirt toRemove;


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
        if (spawnOnLoad)
        {
            SpawnFromList(spawnPositions);
        }
           
        #region Spawnrandom
        /*foreach (Tile t in GridManager.Instance.tiles)
        {
            if(t.occupied) 
                continue;

            if(Random.Range(0, 2) == 0)
                SpawnDirt(t);
        }*/
        #endregion
    }

    public void SpawnDirt(Tile onTile)
    {
        // Spawn a piece of dirt
        GameObject dirtGameObject = Instantiate(dirtPrefab, this.transform);
        Dirt dirt = dirtGameObject.GetComponent<Dirt>();

        // Randomize boulder
        int rand = Random.Range(0, boulderMaterial.Count);

        dirt.gfx.material = boulderMaterial[rand];

        // Set it's position properly
        Vector3 targetPos = onTile.transform.position;
        targetPos.y += dirt.gfx.bounds.size.y/2f;
        targetPos.z += 0.3f;
        dirtGameObject.transform.position = targetPos;

        // Update the dirt covered tile
        dirt.myTile = onTile;
        onTile.dirt = dirt;
        onTile.containsDirt = true;
        onTile.blocked = true;
        onTile.targetable = true;
        if(onTile.occupant != null)
                onTile.occupant.Die();
        onTile.occupied = false;
    }

    [ContextMenu("Spawn Dirt On Selected Tiles")]
    public void SpawnDirt()
    {
        foreach (Tile t in spawnOnTiles)
        {
            if(!t.containsDirt)
                SpawnDirt(t);
        }
    }

    private void SpawnFromList(List<Vector2Int> spawnOn)
    {
        if(GridManager.Instance.tiles == null)
        {
            Debug.LogError("GridManager has no tiles, please update tile data");
            return;
        }

        for (int x = 0; x < GridManager.Instance.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < GridManager.Instance.tiles.GetLength(1); y++)
            {
                foreach (Vector2Int pos in spawnOn)
                {
                    if (GridManager.Instance.tiles[x, y].x == pos.x && GridManager.Instance.tiles[x, y].y == pos.y)
                    {
                        SpawnDirt(GridManager.Instance.tiles[x, y]);
                        break;
                    }
                    else if(GridManager.Instance.tiles[x, y].containsDirt)
                    {
                        GridManager.Instance.tiles[x, y].RemoveDirt();
                    }
                }
            }
        }
    }

    [ContextMenu("Destroy All Dirt")]
    public void RemoveAllDirt()
    {
        Object[] tiles = FindObjectsOfTypeAll(typeof(Tile));

        foreach (Object o in tiles)
        {
            o.GetComponent<Tile>().RemoveDirt();
        }
    }

    [ContextMenu("Remove Selected Dirt")]
    private void RemoveOnDirt()
    {

    }
}
