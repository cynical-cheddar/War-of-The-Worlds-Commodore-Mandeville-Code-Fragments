using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GroupInteractionInterface : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> selectedShips = new List<GameObject>();
    bool currentAheadFull = false;
   
    public void setGroupControlChildren(List<GameObject> lst){
        GetComponentInChildren<Joystick>().setGroupControlChildren(lst);
        selectedShips = lst;
        determineAheadFull();
    }
    void determineAheadFull(){
        // determine if they are all ahead full
        bool allAhead = true;
        foreach(GameObject ship in selectedShips){
            if(!ship.GetComponent<CaptialShipControl>().aheadFullEngaged) allAhead = false; 
        }
        // set the button to be on
        if(allAhead){
            currentAheadFull = true;
        }
        else{
            currentAheadFull = false;
        }
    }

    public void toggleGroupAheadFull(){
        if(selectedShips.Count > 0){
            currentAheadFull = !currentAheadFull;
            foreach(GameObject ship in selectedShips){
                ship.GetComponent<CaptialShipControl>().setAheadFullEngaged(currentAheadFull);
            }
        }
    }


}
