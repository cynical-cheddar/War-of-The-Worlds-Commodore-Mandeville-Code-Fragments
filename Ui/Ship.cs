using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Clickable
{
   // public int friendlyShipId = 0;
   public int aiShipCreditCost = 10000;
   public enum ShipClass
    {
        fighter,
        corvette,
        frigate,
        destroyer,

        cruiser,

        battleship

    }

    public ShipClass shipClass = ShipClass.frigate;


    public bool aiEnabled = false;
    public int teamId;
    public bool friendly = true;

    
    public bool controllable = true;
    public bool selected = false;

    public bool currentlyControlled = false;

    public GameObject controlUI; //A reference to the entire ship's hull (must be nested inside PlayerShip object) to be titled left and right locally as the ship turns.

    protected ShipSelectInterface shipSelectInterface;
    public virtual void  initialiseControls(){

    }

    protected void Awake(){
        shipSelectInterface = FindObjectOfType<ShipSelectInterface>();
    }

    public override void defaultClickAction(){
        
        // if controllable, then select this ship
        Debug.Log("default ship select");
        if(controllable){
            shipSelectInterface.selectShip(gameObject);
        }
    }
}
