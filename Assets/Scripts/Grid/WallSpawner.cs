using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    [SerializeField] GameObject wallPrefab;
    [SerializeField] List<Tile> onTiles;

    [ContextMenu("Remove All Walls")]
    public void RemoveAllWalls()
    {
        Object[] tiles = FindObjectsOfType(typeof(Tile));
        Object[] walls = GameObject.FindGameObjectsWithTag("Wall");

        foreach (Object o in tiles)
        {
            o.GetComponent<Tile>().RemoveWall();
        }

        foreach (GameObject w in walls)
        {
            DestroyImmediate(w);
        }
    }

    [ContextMenu("Spawn Walls on selected Tiles")]
    public void SpawnWalls()
    {
        foreach (Tile t in onTiles)
        {
            GameObject wall = Instantiate(wallPrefab, t.transform.position, Quaternion.identity, this.transform);
            t.SetWalled();
            float yOffset = wall.GetComponentInChildren<MeshRenderer>().bounds.max.y/2f;
            wall.transform.localPosition += new Vector3(0, -yOffset, 0);
        }
    }
}
