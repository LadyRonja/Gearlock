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

    public Unit occupant;
    public bool containesDirt;
    public Dirt dirt;

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
    }

    public void UpdateOccupant(Unit newOccupant)
    {
        occupant = newOccupant;

        if (newOccupant == null)
        {
            blocked = containesDirt;
            occupied = false;
            targetable = true;
        }
        else
        {
            blocked = true;
            occupied = true;
            targetable = false;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        // Select My Unit
        if(occupant != null)
        {
            UnitSelector.Instance.UpdateSelectedUnit(occupant);
        }

        // Toggle Blocked (Debug)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            TileClicker.Instance.ToggleBlockedDebug(this);
        }

        // Move Selected Unit Here
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!targetable) return;
            TileClicker.Instance.MoveSelectedUnitHere(this);
        }

        // Toggle Dirt Here
        if (Input.GetKey(KeyCode.D))
        {
            if(!containesDirt)
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

        containesDirt = false;
        blocked = false;
        bool remoevedInEditor = false;
#if UNITY_EDITOR
        DestroyImmediate(dirt.gameObject);
        remoevedInEditor = true;
#endif
        if (!remoevedInEditor)
            Destroy(dirt.gameObject);

        dirt = null;

        // TODO
        // Spawn item/enemy
    }
}
