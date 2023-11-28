using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;
    [SerializeField] float spawnYOffset = 3f;
    [SerializeField] GameObject pickupPrefab;
    [SerializeField] List<GameObject> spawnableCards = new();

    //TODO:
    // Spawn enemies sometimes
    // Weighted spawns
    // Prevent enemies spawning too early

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }

    public void SpawnRandomItem(Tile onTile)
    {
        // Error proofing
        if (onTile.containesDirt)
            return;

        if (!spawnableCards.Any())
        {
            Debug.LogError("No items in the item spawner");
            return;
        }

        if (onTile.myPickUp != null)
        {
            Destroy(onTile.myPickUp);
            onTile.myPickUp = null;
        }

        // Pick random spawnable item
        int rand = Random.Range(0, spawnableCards.Count);
        Vector3 spawnPos = onTile.transform.position;
        spawnPos.y += spawnYOffset;
        GameObject cardPickUpObject = Instantiate(pickupPrefab, spawnPos, Quaternion.identity);
        CardPickUp cardPickUpScript = cardPickUpObject.GetComponent<CardPickUp>();
        if(cardPickUpScript == null)
        {
            Debug.LogError("No pickup script on Card Pick-up");
            return;
        }
        cardPickUpScript.cardToAdd = spawnableCards[rand];
        onTile.myPickUp = cardPickUpScript;
    }

    // new function to spawn pickup for dead robots here?


}
