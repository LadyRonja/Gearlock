using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UIElements;

public abstract class PlayCard : MonoBehaviour //lägg till abstract
{
    public bool selectCard;
    public bool playCard;
    public int distance;
    public CardType currentCardType;


    //protected Tile clickedTile;
    protected UnitSelector chosenUnit;
    protected AttackCard attackCard;
    protected DigCard digCard;
    protected Unit botName;
    

    protected virtual void Start()
    {
        chosenUnit = GetComponent<UnitSelector>();
    }

    protected virtual void Update()
    {
        //TODO
        //Check if card is selected
        //if not activated (the card) return 
        //bort med enum och switch
        //lägg till enum och switch för att välja robot

        ClickOnUnit();

    }


    public enum CardType
    {
        Dig,
        Attack
    }

    public void ChooseCard()
    {
        switch (currentCardType)
        {
            case CardType.Dig:
                // Create and use DigCard
                DigCard digCard = gameObject.AddComponent<DigCard>();
                digCard.Dig();
                Debug.Log("You dig the dirt");
                break;

            case CardType.Attack:
                // Create and use AttackCard
                AttackCard attackCard = gameObject.AddComponent<AttackCard>();
                attackCard.Attack();
                Debug.Log("You hit an enemy!");
                break;

            default:
                Debug.LogError("Unknown card type");
                break;
        }
    }



    public void ClickOnUnit()
    {
        if (!selectCard)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;

            // Cast a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            // Perform a raycast to check for objects at the click position
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the clicked object has a Tile component
                Tile clickedTile = hit.collider.GetComponent<Tile>();

                if (clickedTile != null)
                {
                    // Use GridManager to find the player's unit
                    Unit playerUnit = FindPlayerUnit();

                    if (playerUnit != null)
                    {
                        // Calculate the distance between the player's unit and the clicked tile
                        //distance = Vector3.Distance(playerUnit.transform.position, clickedTile.transform.position);
                        distance = Pathfinding.GetDistance(playerUnit.standingOn, clickedTile);

                        // Log the distance
                        Debug.Log($"Distance to clicked tile: {distance}");
                    }
                    else
                    {
                        Debug.LogWarning("No player unit found.");
                    }
                }
                
                if (distance == 1) //om avståndet från player och clicked tile är mindre än 20 dvs 2 rutor
                {
                    playCard = true;
                    Debug.Log("You can play this card");

                    if (playCard == true)
                    {
                        if (currentCardType == CardType.Attack)
                        {
                            attackCard.Attack();
                            Debug.Log("You are attacking");
                        }

                        else if (currentCardType == CardType.Dig)
                        {
                            if(botName.GetType() == typeof(Digger))
                            {
                                digCard.Dig();
                            }

                            /*if (botName.unitName != "Digger")
                            {
                                Debug.Log("This bot cannot dig");
                            }
                            else
                            {
                                //digCard.Dig();
                                Debug.Log($"{botName.unitName} is digging");
                            }*/
                        }

                    }


                    /*if(currentCardType == CardType.Attack)
                    {
                        attackCard.Attack();

                    }

                    if (currentCardType == CardType.Dig) 
                    {
                        if (botName.unitName == "Digger")
                        {
                            digCard.Dig();
                        }
                        else
                        {
                            Debug.Log("This bot cannot dig");
                        }
                    }*/

                }
                else
                {
                    playCard = false;
                    Debug.Log("You can NOT play this card");
                }
            }
        }
    }


    private Unit FindPlayerUnit()
    {
        
        GridManager gridManager = GridManager.Instance;

        if (gridManager != null && gridManager.tiles != null)
        {
            for (int x = 0; x < gridManager.tiles.GetLength(0); x++)
            {
                for (int y = 0; y < gridManager.tiles.GetLength(1); y++)
                {
                    Tile tile = gridManager.tiles[x, y];
                    if (tile != null && tile.occupant != null && tile.occupant.playerBot)
                    {
                        return tile.occupant;
                    }
                }
            }
        }

        return null;
    }

  

    public virtual void Play()
    {

        selectCard = true;
        Debug.Log("you have selceted a card");

        ClickOnUnit();
        ChooseCard();
        
    
    }
}

   

