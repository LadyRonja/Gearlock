using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public MeshRenderer myMR;

    public Tile NeighbourN;
    public Tile NeighbourE;
    public Tile NeighbourS;
    public Tile NeighbourW;
    public List<Tile> neighbours = new List<Tile>();

    public int X;
    public int Y;

    //public Occupant Occupant;

    // Pathfinding
    public bool targetable = true;
    public bool blocked = false;

    public Tile cameFrom;
    public int G { get; set; }
    public int H { get; set; }
    public int F { get => (G + H); }

    private void Start()
    {
        myMR = GetComponent<MeshRenderer>();
    }

    #region Debug
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
        else
        {
            if (!targetable) return;

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
            }
        }
    }

    public void ResetColor()
    {
        Grid.Instance.tiles[0, 0].myMR.material.color = Color.white;

        if (!blocked)
            myMR.material.color = Color.white;
        else
            myMR.material.color = Color.black;
    }
    #endregion
}
