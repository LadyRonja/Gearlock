using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;
    [SerializeField] float spawnYOffset = 3f;
    [SerializeField] GameObject pickupPrefab;
    [SerializeField] List<GameObject> spawnableCards = new();
    [SerializeField] List<GameObject> spawnableCardsFinite = new();
    public bool useBadLuckProtection = false; // TODO: Make more flexible

    public float delay = 0.5f;
    
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
        if (onTile.containsDirt)
            return;

        if (!spawnableCards.Any())
        {
            Debug.LogError("No items in the item spawner");
            return;
        }

        if (useBadLuckProtection)
        {
            if (BadLuckProtectionInjection(onTile))
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
        if (cardPickUpScript == null)
        {
            Debug.LogError("No pickup script on Card Pick-up");
            return;
        }
        cardPickUpScript.cardToAdd = spawnableCards[rand];
        onTile.myPickUp = cardPickUpScript;
    }

    public void SpawnFiniteCard(Tile onTile)
    {
        if (onTile.containsDirt)
            return;

        if (!spawnableCardsFinite.Any())
        {
            SpawnRandomItem(onTile);
            return;
        }

        if (onTile.myPickUp != null)
        {
            Destroy(onTile.myPickUp);
            onTile.myPickUp = null;
        }

        if (useBadLuckProtection)
        {
            if (BadLuckProtectionInjection(onTile))
                return;
        }

        int randomCard;
        if (spawnableCardsFinite.Count > 3)
            randomCard = Random.Range(0, 3);
        else
            randomCard = Random.Range(0, spawnableCardsFinite.Count);

        Vector3 spawnPos = onTile.transform.position;
        spawnPos.y += spawnYOffset;
        GameObject cardPickUpObject = Instantiate(pickupPrefab, spawnPos, Quaternion.identity);
        CardPickUp cardPickUpScript = cardPickUpObject.GetComponent<CardPickUp>();
        if (cardPickUpScript == null)
        {
            Debug.LogError("No pickup script on Card Pick-up");
            return;
        }
        cardPickUpScript.cardToAdd = spawnableCardsFinite[randomCard];
        spawnableCardsFinite.RemoveAt(randomCard);
        onTile.myPickUp = cardPickUpScript;
        return;
    }


    private bool BadLuckProtectionInjection(Tile onTile)
    {
        if (GameStats.Instance.GetRocksMined() != 2)
            return false;

        // Check if any of the top 3 cards at this point is a fighter bot
        if (spawnableCardsFinite.Count < 3)
            return false;

        List<GameObject> topThreeSpawnables = new() { spawnableCardsFinite[0], spawnableCardsFinite[1], spawnableCardsFinite[2] };
        for(int i = 0; i < topThreeSpawnables.Count; i++)
        {
            if (topThreeSpawnables[i].GetComponent<Card>().GetType() == typeof(SpawnUnitCard))
            {
                if (topThreeSpawnables[i].GetComponent<SpawnUnitCard>().myType == Card.CardType.FighterBot)
                {
                    // Spawn that fightbot
                    Vector3 spawnPos = onTile.transform.position;
                    spawnPos.y += spawnYOffset;
                    GameObject cardPickUpObject = Instantiate(pickupPrefab, spawnPos, Quaternion.identity);
                    CardPickUp cardPickUpScript = cardPickUpObject.GetComponent<CardPickUp>();
                    if (cardPickUpScript == null)
                    {
                        Debug.LogError("No pickup script on Card Pick-up");
                        return false;
                    }
                    cardPickUpScript.cardToAdd = topThreeSpawnables[i];
                    spawnableCardsFinite.RemoveAt(i);
                    onTile.myPickUp = cardPickUpScript;
                    return true;
                }
            }
        }

        return false;
    }

}
