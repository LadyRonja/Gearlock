using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
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

    // Contents
    public Unit occupant;
    public bool containsDirt;
    public Dirt dirt;
    public CardPickUp myPickUp;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightHoverEnterManager();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightHoverExitManager();
    }

    public void OnPointerDown(PointerEventData eventData)
     {
        // Select My Unit
        if (occupant != null)
         {
             UnitSelector.Instance.UpdateSelectedUnit(occupant);
         }
         TileClicker.Instance.HandleMoveClick(this);

         // Toggle Blocked (Debug)
         if (Input.GetKey(KeyCode.LeftControl))
         {
             TileClicker.Instance.ToggleBlockedDebug(this);
         }

         // Toggle Dirt Here
         if (Input.GetKey(KeyCode.D))
         {
             if(!containsDirt)
                 TileClicker.Instance.SpawnDirt(this);
             else
                 RemoveDirt();
         }

     }

    public void RemoveDirt()
    {
        if(dirt == null)
        {
            Debug.Log("Attempted to remove non-existing dirt");
            return;
        }

        containsDirt = false;
        blocked = false;
        bool remoevedInEditor = false;
#if UNITY_EDITOR
        DestroyImmediate(dirt.gameObject);
        remoevedInEditor = true;
#endif
        if (!remoevedInEditor)
            Destroy(dirt.gameObject);

        dirt = null;

        // TODO: See item spawner todo
        //ItemSpawner.Instance.SpawnRandomItem(this);
        if(ItemSpawner.Instance != null)
            ItemSpawner.Instance.SpawnRandomCardDelete(this);
    }

    private void HighlightHoverEnterManager()
    {
        // If a card is not being played
        // Highlight white or red depending on blocked status
        // Yellow/blue takes higher priority for enemy/friendly units
        if (ActiveCard.Instance.transform.childCount == 0)
        {
            if (myHighligther.color == Color.blue || myHighligther.color == Color.yellow)
            { }// Do nothings
            else if (!blocked)
                Highlight();
            
            else
                Highlight(Color.red);
        }
        else if (myHighligther.gameObject.activeSelf == true)
        {
            // If a card is being played, highlight as green if it's not on the selected unit

            if(myHighligther.color != Color.blue)
                Highlight(Color.green);
        }
    }

    private void HighlightHoverExitManager()
    {
        // If a card is not being played
        // Remove highlight
        if (ActiveCard.Instance.transform.childCount == 0)
        {
            if(myHighligther.color != Color.blue && myHighligther.color != Color.yellow)
                UnHighlight();
        }
        else if (myHighligther.gameObject.activeSelf == true)
        {
            // If a card is being played
            // Keep it highligthed
            // Keep it blue for the selected unit

            if(myHighligther.color != Color.blue)
                Highlight();
        }
    }

    public void Highlight(Color highlightColor)
    {
        if(myHighligther == null)
        {
            Debug.LogError("Tile Missing Hightligther: " + transform.name);
            return;
        }

        myHighligther.gameObject.SetActive(true);
        myHighligther.color = highlightColor;
    }
    public void Highlight()
    {
        Highlight(Color.white);
        
        //test elins mouse cursor
        MouseControl.instance.Walk();
    }

    public void UnHighlight()
    {
        if (myHighligther == null)
        {
            Debug.LogError("Tile Missing Hightligther: " + transform.name);
            return;
        }

        myHighligther.color = Color.white;
        myHighligther.gameObject.SetActive(false);

        //test elins mouse cursor
        MouseControl.instance.Default();
    }
}
