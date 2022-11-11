using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class ShipSelectInterface : MonoBehaviour
{
    public List<GameObject> controllableShips = new List<GameObject>();
    
    public GameObject currentlyControlledShip;
     CinemachineFreeLook freecam;

    public Transform iconMaster;
    public GameObject shipIconPrefab;

    public GameObject freecamPrefab;

    CameraLookController lookController;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        freecam = FindObjectOfType<CameraLookController>().gameObject.GetComponent<CinemachineFreeLook>();
        lookController = freecam.GetComponent<CameraLookController>();
    }

    public void addShipToList(GameObject shipObject){
        if(!controllableShips.Contains(shipObject)) controllableShips.Add(shipObject);
        if(GetComponent<GroupOrderInterface>()!=null)GetComponent<GroupOrderInterface>().setSelectables(controllableShips);
        displayIcons();
        
    }

    public void deselectAll(){
        foreach(GameObject ship in controllableShips){
            CaptialShipControl c = ship.GetComponent<CaptialShipControl>();
            c.setCurrentlyControlled(false);
        }
        lookController.nullSelectedShip();
    }

    public GameObject getcurrentlyControlledShip(){
        if(currentlyControlledShip!=null){
            return currentlyControlledShip;
        }
        else{
            return null;
        }
    }

    public void removeShipFromList(GameObject shipToRemove){
        controllableShips.Remove(shipToRemove);
        if(GetComponent<GroupOrderInterface>()!=null)GetComponent<GroupOrderInterface>().setSelectables(controllableShips);
        displayIcons();
    }
    void displayIcons(){

        foreach(Transform child in iconMaster){
            Destroy(child.gameObject);
        }
        foreach(GameObject shipObject in controllableShips){
            GameObject shipIcon = Instantiate(shipIconPrefab, iconMaster);
            shipIcon.GetComponent<selectShipIcon>().ship = shipObject;

            // TODO - change to actual name
            shipIcon.GetComponent<selectShipIcon>().setName(shipObject.name);
        }
        // make a pass through the list of ships. If none of them are controlled, then choose one arbritarily
        bool pass = true;
        foreach(GameObject shipObject in controllableShips){
            if(shipObject.GetComponent<Ship>().currentlyControlled == true){pass = false; currentlyControlledShip = shipObject;}
        }
        if(pass) selectShip(controllableShips[0]);
        
    }

    void findCurrentlyControlled(){
        foreach(GameObject shipObject in controllableShips){
            if(shipObject.GetComponent<Ship>().currentlyControlled == true){currentlyControlledShip = shipObject;}
        }
    }
    public void selectShip(GameObject shipObject){
        // disable direct control for current ships
        // also disable their huds
        findCurrentlyControlled();
        // TODO - change later. We need a list of selected ships
        
        foreach(GameObject ship in controllableShips){
            CaptialShipControl c = ship.GetComponent<CaptialShipControl>();
            c.setCurrentlyControlled(false);
        }

        // enable our ship

        freecam.GetComponent<CameraLookController>().selectShip(shipObject);
      //          freecam.Follow = shipObject.transform;

       // freecam.LookAt = shipObject.transform;
        
       // 


      
      findCurrentlyControlled();

      if(GetComponent<GroupOrderInterface>()!= null){
            List<GameObject> selectedShips = new List<GameObject>();
            currentlyControlledShip = shipObject;
            selectedShips.Add(currentlyControlledShip);
            GetComponent<GroupOrderInterface>().setSelectedShips(selectedShips);
        }

       

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
