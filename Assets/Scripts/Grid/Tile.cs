using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public MeshRenderer myMR;

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

    //test Elin 
    private bool isClicked = false;


    private void Start()
    {
        myMR = GetComponent<MeshRenderer>();
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
                Debug.Log("Stepped on pickup");
                CardManager.instance.AddNewCard(myPickUp.cardToAdd);
                Destroy(myPickUp.gameObject);
                myPickUp = null;
            }
        }
    }

    
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!blocked)
            myMR.material.color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!blocked)
            myMR.material.color = Color.white;
    }

    //TEST ELIN when you click the tile becomes blue
    public void OnPointerDown(PointerEventData eventData)
     {
       
        if (!blocked)
        {
            myMR.material.color = Color.blue;
            isClicked = true;
            HighlightNeighbours();

        }
            

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
        ItemSpawner.Instance.SpawnRandomItem(this);

    }

    //TEST ELIN when you click on a tile the neighbors gets blue
    private void HighlightNeighbours()
    {
       if (isClicked ==  true)
       {
            if (neighbourN != null)
                neighbourN.myMR.material.color = Color.blue;

            if (neighbourE != null)
                neighbourE.myMR.material.color = Color.blue;

            if (neighbourS != null)
                neighbourS.myMR.material.color = Color.blue;

            if (neighbourW != null)
                neighbourW.myMR.material.color = Color.blue;
       }
       else
       {
            if (neighbourN != null)
                neighbourN.myMR.material.color = Color.white;

            if (neighbourE != null)
                neighbourE.myMR.material.color = Color.white;

            if (neighbourS != null)
                neighbourS.myMR.material.color = Color.white;

            if (neighbourW != null)
                neighbourW.myMR.material.color = Color.white;
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!blocked)
        {
            myMR.material.color = Color.white;
            isClicked = false;
            HighlightNeighbours();
        }
            
        
    }
}
