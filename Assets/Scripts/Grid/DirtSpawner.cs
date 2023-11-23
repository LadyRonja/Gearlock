using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpawner : MonoBehaviour
{
    public static DirtSpawner Instance;

    [SerializeField] float yOffset = 4.5f;

    [SerializeField] GameObject dirtPrefab;
    [SerializeField] List<Material> dirtTops;
    [SerializeField] List<Material> dirtSides;


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
        foreach (Tile t in GridManager.Instance.tiles)
        {
            if(t.occupied) 
                continue;

            if(Random.Range(0, 2) == 0)
                SpawnDirt(t);
        }
    }

    public void SpawnDirt(Tile onTile)
    {
        // Spawn a piece of dirt
        GameObject dirtGameObject = Instantiate(dirtPrefab);
        Dirt dirt = dirtGameObject.GetComponent<Dirt>();

        // Randomize sides and top
        int randTop = Random.Range(0, dirtTops.Count);
        int randSideLeft = Random.Range(0, dirtSides.Count);
        int randSideRight = Random.Range(0, dirtSides.Count);
        int randFront = Random.Range(0, dirtSides.Count);

        dirt.top.material = dirtTops[randTop];
        dirt.front.material = dirtTops[randSideLeft];
        dirt.sideRight.material = dirtTops[randSideRight];
        dirt.sideLeft.material = dirtTops[randFront];

        // Set it's position properly
        Vector3 targetPos = onTile.transform.position;
        targetPos.y += yOffset;
        dirtGameObject.transform.position = targetPos;

        // Update the dirt covered tile
        onTile.dirt = dirt;
        onTile.containesDirt = true;
        onTile.blocked = true;
        onTile.targetable = true;
        if(onTile.occupant != null)
            onTile.occupant.Die();
        onTile.occupied = false;
    }
}
