using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft;
public class ShipJsonManager : MonoBehaviour
{


    public List<string> moduleDescription;



    void Update() {
        if(Input.GetKeyDown(KeyCode.S)){
            describeShip();
        }
    }
    public void describeShip(){
        // find all children of type moduleDescriptor
        
        moduleDescription.Clear();
        moduleDescriptor[] descriptors = GetComponentsInChildren<moduleDescriptor>();
        foreach(moduleDescriptor descriptor in descriptors){
            moduleDescription.Add(descriptor.describeModule());
        }

        
    }
}
