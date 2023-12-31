using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tile : MonoBehaviour, IPointerDownHandler
{
    public MeshRenderer myMR;
    public SpriteRenderer myHighligther;

    public Tile neighbourN;
    public Tile neighbourE;
    public Tile neighbourS;
    public Tile neighbourW;
    public List<Tile> neighbours = new List<Tile>();

    public int x;
    public int y;

    public bool highlighted = false;

    public float delay = 0.4f;

    // Contents
    public Unit occupant;
    public bool containsDirt;
    public Dirt dirt;
    public CardPickUp myPickUp;
    public bool highlightedForMovement = false;

    // Pathfinding
    public bool targetable = true;
    public bool blocked = false;
    public bool occupied = false;

    public Tile cameFrom;
    public int G { get; set; }
    public int H { get; set; }
    public int F { get => (G + H); }



    private void Start()
    {
        myMR = GetComponent<MeshRenderer>();
        myHighligther = GetComponentInChildren<SpriteRenderer>();
        if (myHighligther == null)
            Debug.LogError("Tile Missing Hightligther: " + transform.name);
        else
            UnHighlight();
    }

    public void UpdateOccupant(Unit newOccupant)
    {
        occupant = newOccupant;

        if (newOccupant == null)
        {
            blocked = containsDirt;
            occupied = false;
            targetable = true;
        }
        else
        {
            blocked = true;
            occupied = true;
            targetable = false;

            if (newOccupant.playerBot && myPickUp != null)
            {
                CardManager.instance.AddNewCard(myPickUp.cardToAdd);
                Destroy(myPickUp.gameObject);
                myPickUp = null;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
     {
        // Being called from the HoverManager now, leaving it in until sure it works fine
        //OnClick();
     }

    public void OnClick()
    {   
        // Select My Unit
        if (occupant != null)
        {
            TileClicker.Instance.UpdateSelectedUnit(this);
        }
        TileClicker.Instance.HandleMoveClick(this);

        // Toggle Blocked (Debug)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            TileClicker.Instance.ToggleBlockedDebug(this);
        }
    }

    public void RemoveDirt(bool canSpawnCard)
    {
        if(dirt == null)
        {
            Debug.Log("Attempted to remove non-existing dirt");
            return;
        }

        containsDirt = false;
        blocked = occupied;
        AudioHandler.PlaySoundEffect(dirt.getMinedSound);
        bool remoevedInEditor = false;
#if UNITY_EDITOR
        Destroy(dirt.gameObject, delay);
        remoevedInEditor = true;
#endif
        if (!remoevedInEditor)
        {
            
            Destroy(dirt.gameObject, delay);
        }
            

        dirt = null;


        // TODO: See item spawner todo
        //ItemSpawner.Instance.SpawnRandomItem(this);
        if(canSpawnCard)
            if(ItemSpawner.Instance != null)
               ItemSpawner.Instance.SpawnFiniteCard(this);
                           
    }
    public void RemoveDirt()
    {
        RemoveDirt(true);
    }


    public void Highlight(Color highlightColor)
    {
        if(myHighligther == null)
        {
            Debug.LogError("Tile Missing Hightligther: " + transform.name);
            return;
        }

        highlighted = true;
        myHighligther.gameObject.SetActive(true);
        myHighligther.color = highlightColor;
    }
    public void Highlight()
    {
        Highlight(Color.white);
    }

    public void UnHighlight()
    {
        if (myHighligther == null)
        {
            Debug.LogError("Tile Missing Hightligther: " + transform.name);
            return;
        }

        if(dirt != null)
            dirt.UnHighlight();

        myHighligther.color = new Color(0, 0, 0, 0.3f);
        highlighted = false;    
        //myHighligther.gameObject.SetActive(false);
    }

    
}
