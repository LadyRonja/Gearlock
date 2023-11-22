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
            blocked = false;
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
        if (Input.GetKey(KeyCode.LeftControl))
        {
            blocked = !blocked;
            targetable = !targetable;

            if (blocked)
                myMR.material.color = Color.black;
            else
                myMR.material.color = Color.white;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            UnitSelector.Instance.UpdateSelectedUnit(occupant);
        }
        else
        {
            if (!targetable) return;

            MovementManager.Instance.MoveUnit(UnitSelector.Instance.selectedUnit, this);

            /*
            List<Tile> path = Pathfinding.FindPath(Grid.Instance.tiles[0, 0], this);
            if (path != null)
            {
                Grid.Instance.tiles[0, 0].myMR.material.color = Color.blue;
                Grid.Instance.tiles[0, 0].Invoke("ResetColor", 1f);
                //Debug.DrawLine(HexGrid.Instance.tiles[new HexCoordinates(0, 0)].transform.position, path[0].transform.position, Color.green, 3f);
                for (int i = 1; i < path.Count; i++)
                {
                    Debug.DrawLine(path[i - 1].transform.position, path[i].transform.position, Color.magenta, 3f);
                    path[i].myMR.material.color = Color.blue;
                    path[i].Invoke("ResetColor", 1f);
                }
            }
            else
            {
                Grid.Instance.tiles[0, 0].myMR.material.color = Color.yellow;
                myMR.material.color = Color.yellow;
                Invoke("ResetColor", 1f);
            }*/
        }

        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKey(KeyCode.W))
            {
                Debug.Log("Setting start tile: " + name);
                GridManager.Instance.startTile = this;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("Setting End tile: " + name);
                GridManager.Instance.endTile = this;
            }
            else if (Input.GetKey(KeyCode.R))
            {
                Debug.Log($"Distance between start({GridManager.Instance.startTile}) and end({GridManager.Instance.endTile}): {Pathfinding.GetDistance(GridManager.Instance.startTile, GridManager.Instance.endTile)}");
            }
        }
    }

    public void RemoveDirt()
    {
        //TODO:
        // check if covered in dirt
        // set covered to false
        // remove dirt block object
        // spawn card/enemy
    }
}
