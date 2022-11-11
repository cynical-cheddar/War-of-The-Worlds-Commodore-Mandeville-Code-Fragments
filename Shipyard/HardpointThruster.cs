using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardpointThruster : Hardpoint
{

    public bool lateral;

    public bool right;

    public bool left;
    public bool vertical;

    public bool up;
    public bool down;

    public bool speed;

    public override void defaultClickAction(){
        if(myShipyard != null) myShipyard.displayEquipment(this, attachableItems);
    }
    public override void showAttachableItems(){
        attachableItems.Clear();
        foreach(Equipment item1 in myShipyard.allEquipment){
            switch (item1){
                case Thruster a:
                    attachableItems.Add(item1);
                break;
            }
        }

    }
}
