using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraInteractionController : MonoBehaviour
{

    bool shipyardMode = false;
    Camera myCam;
    List<Weapon> litWeapons = new List<Weapon>();

    GroupOrderInterface groupOrderInterface;

    ShipSelectInterface shipSelectInterface;

    CameraLookController lookController;

    float timeThreshold = 0.2f;
    float lastTime = 0f;

    public LayerMask mask = -1;

    void Start()
    {
        myCam = GetComponent<Camera>();
        if(FindObjectOfType<shipyard>() != null) shipyardMode = true;
        groupOrderInterface = FindObjectOfType<GroupOrderInterface>();
        shipSelectInterface = FindObjectOfType<ShipSelectInterface>();
        lookController = FindObjectOfType<CameraLookController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Cursor.lockState != CursorLockMode.Locked) GetMouseInfo();
        if(Input.GetMouseButtonDown(0))lastTime = Time.time;
        if(Input.GetMouseButtonUp(0))click();
    }

    bool getIsMoveMode(){
        if(groupOrderInterface != null){
            return  groupOrderInterface.getMoveMode();
        }
        else return false;
    }

    void click(){
        if(!getIsMoveMode()){
            Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
        
            if (Physics.Raycast(ray,out hit, Mathf.Infinity, mask))
            {
                if(!EventSystem.current.IsPointerOverGameObject())clickProcess(hit.collider.gameObject);
            }
           // else if (!EventSystem.current.IsPointerOverGameObject())    // is the touch on the GUI
           // {
           //     clickProcess(hit.collider.gameObject);
          //  }
            else if(lookController.strategicCamera){
               // shipSelectInterface.deselectAll();
            }
        }
    }
    void clickProcess(GameObject hitItem){
        if(hitItem.GetComponent<Clickable>() != null)//This detects the colliders transform if you just use hit.transform it gets the root parent at least thats what I found.
             {

                 // call highlight function on clickable
                 switch (hitItem.GetComponent<Clickable>()){
                     case Weapon weapon:
                        if(!shipyardMode){
                            if(!lookController.superStrategicCamera){
                                Weapon w = hitItem.GetComponent<Weapon>();
                                if(w.gameObject.GetComponentInParent<Ship>().currentlyControlled) w.selectWeapon();
                            }
                            // otherwise just select the ship from a distance by doing the default action
                            else{
                                Clickable a =hitItem.GetComponent<Clickable>();

                                if(hitItem.GetComponentInParent<Ship>() != null){
                                    Ship s = hitItem.GetComponentInParent<Ship>();
                                    if(s.controllable && !s.currentlyControlled && !shipyardMode) s.defaultClickAction();
                                }
                                
                                a.defaultClickAction();
                            }
                           
                        }
                     break;
                     

                     default:
                        // if the clickable object is part of a contollable ship, then control it
                        Clickable c =hitItem.GetComponent<Clickable>();

                        if(hitItem.GetComponentInParent<Ship>() != null){
                            Ship s = hitItem.GetComponentInParent<Ship>();
                            if(s.controllable && !s.currentlyControlled && !shipyardMode) s.defaultClickAction();
                        }
                        
                        c.defaultClickAction();
                     break;

                 }
             }
             if(hitItem.GetComponent<Clickable>() == null)//This detects the colliders transform if you just use hit.transform it gets the root parent at least thats what I found.
             {
                 Transform hitTransform = hitItem.transform;
                 while(hitTransform.parent != null){
                     hitTransform = hitTransform.parent;
                   //  Debug.Log("test object "+ hitTransform.name);
                     if(hitTransform.gameObject.GetComponent<Clickable>() != null){clickProcess(hitTransform.gameObject); Debug.Log("FOUND!!!"); break;}
                 }
             }
    }

    void mouseOver(GameObject hitObject){
               //  Debug.Log("Mouse is over " + hit.collider + ".");
                 // call highlight function on clickable
                 bool weaponFound = false;
                 
                 //get list of clickables
                 Clickable[] clickables = hitObject.GetComponents<Clickable>();
                 foreach(Clickable clickable in clickables){
                     switch (clickable){
                     case Weapon weapon:
                        if(!shipyardMode){
                            Weapon w = hitObject.GetComponent<Weapon>();
                            w.setHighlight(true);
                            weaponFound = true;
                        }
                        break;
                     case HealthBarOverlay overlay:
                        overlay.setDisplay(true);
                     break;

                      default:
                        clickable.defaultMouseOverAction();
                     break;
                 }
                 }
    }
     void GetMouseInfo()
     {
         Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
         RaycastHit hit;
     
         if (Physics.Raycast(ray,out hit, Mathf.Infinity, mask))
         {
           //  Debug.Log(hit.collider.gameObject);
             if(hit.collider.GetComponent<Clickable>() != null)//This detects the colliders transform if you just use hit.transform it gets the root parent at least thats what I found.
             {
                 mouseOver(hit.collider.gameObject);
             }
             // if not clickable, loop up the hierachy
             if(hit.collider.GetComponent<Clickable>() == null)//This detects the colliders transform if you just use hit.transform it gets the root parent at least thats what I found.
             {
                 Transform hitTransform = hit.transform;
                 while(hitTransform.parent != null){
                     hitTransform = hitTransform.parent;
                   //  Debug.Log("test object "+ hitTransform.name);
                     if(hitTransform.gameObject.GetComponent<Clickable>() != null){mouseOver(hitTransform.gameObject); Debug.Log("FOUND!!!"); break;}
                 }
             }
         }
     
     }

     void resetHighlight(){
         if(litWeapons.Count > 0){
             litWeapons[0].setHighlight(false);
             litWeapons.Clear();
          }
     }
}
