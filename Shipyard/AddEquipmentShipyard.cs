using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEquipmentShipyard : partButton
{

    public int componentCost = 100;
    public string moduleIndex = "0";



    public Hardpoint hardpoint;




    // Start is called before the first frame update


    // Update is called once per frame


    public void interact(){
        // get the shipyard script and attempt to add the component
        GameObject thing = Resources.Load(componentName) as GameObject;
       
        if(thing.GetComponent<Equipment>() != null) hostShipyard.addEquipment(componentName, moduleIndex, hardpoint.getHardpointIndex());

        


    }
}
