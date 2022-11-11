using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class addComponentShipyard : partButton
{
    
    public int componentCost = 100;
    public int index = 0;

    
    // Start is called before the first frame update
    

    // Update is called once per frame


    public void interact(){
        // get the shipyard script and attempt to add the component
        GameObject thing = Resources.Load(componentName) as GameObject;
        if(thing.GetComponent<Module>() != null) hostShipyard.addHull(componentName);

        


    }
}
