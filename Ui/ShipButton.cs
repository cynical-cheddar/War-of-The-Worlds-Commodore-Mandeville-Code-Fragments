using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipButton : Clickable
{
    public string shipFilePath;
    public FleetManager fleetManager;
    public override void defaultMouseOverAction(){
        // show icon
    }
    public override void defaultClickAction(){
        // open shipyard via fleetManager
        fleetManager.openShipyard(shipFilePath);
    }
}
