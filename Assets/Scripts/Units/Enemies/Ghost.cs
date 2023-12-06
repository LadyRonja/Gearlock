using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Unit
{
    public GameObject infoTextGhost;

    private PlayCard card;
    private AttackCard attackCard;

    public void Start()
    {
        infoTextGhost.SetActive(false);
    }
    public override Unit FindTargetUnit()
    {
        return FindNearestPlayerUnit(true);
    }

    public override List<Tile> CalculatePathToTarget(Tile targetTile)
    {
        List<Tile> output = Pathfinding.FindPath(standingOn, targetTile, movePointsCur, true);
        if (output == null)
        {
            Debug.Log("No path found");
            return null;
        }
        if(output.Count == 0)
        {
            Debug.Log("Path is 0 long");
            return null;
        }

        if (output[output.Count - 1] == targetTile)
            output.RemoveAt(output.Count - 1);

        return output;
    }

    public void OnMouseEnter()
    {
        PlayCard currentCard = ActiveCard.Instance.transform.GetComponentInChildren<PlayCard>();

        infoTextGhost.SetActive(true);

        if (currentCard != null)
        {
            if (currentCard.GetType().Equals(typeof(AttackCard)))
            {
                MouseControl.instance.Fight();
                Debug.Log("Changing cursor to Fight");
            }
        }
        // if (card.myState == CardState.SelectingTile)
        //{
        
           // }
        



    }

    public void OnMouseExit()
    {
        MouseControl.instance.Default();
        infoTextGhost.SetActive(false);
    }
}
