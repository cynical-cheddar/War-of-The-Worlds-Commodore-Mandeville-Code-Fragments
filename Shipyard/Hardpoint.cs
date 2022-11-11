using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Hardpoint : Clickable
{
    //public GameObject hardPointUI;
    

    protected int hardpointIndex = 0;
    protected GameObject hardPointUIinstance;

    protected Canvas uiCanvas;

    protected shipyard myShipyard;

    [SerializeField]
    public List<Equipment> attachableItems = new List<Equipment>();

    public Sprite uibarIcon;

    protected bool displayHardpointStats = false;

    // needed on each kind of hardpoint

    public void setIndex(int value){
        hardpointIndex = value;
    }
    public int getHardpointIndex(){
        return hardpointIndex;
    }

    public virtual void showAttachableItems(){
        attachableItems.Clear();
        foreach(Equipment item1 in myShipyard.allEquipment){
            switch (item1){
                case Equipment a:
                    attachableItems.Add(item1);
                break;
            }
        }
    }

    // when we are clicked, display the menu of parts we can add
    public override void defaultClickAction(){
        if(myShipyard != null){
           if(GetComponentInChildren<Equipment>() == null) myShipyard.displayEquipment(this, attachableItems);
           else{
              // display option to detatch weapon
              myShipyard.displayRemoveEquipmentMenu(this);
           }
        } 
    }
    public override void defaultMouseOverAction(){
        if(myShipyard != null) displayHardpointStats = true;
        
    }

    protected void LateUpdate()
    {
        if(displayHardpointStats) myShipyard.displayHardpointStats(this);
        displayHardpointStats = false;
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    protected void OnDestroy()
    {
        Destroy(hardPointUIinstance);
    }


    protected virtual void Start(){
        // upon start, and if there is a shipyard,display a clickable overlay to add a component
        if(FindObjectOfType<shipyard>() != null){
            uiCanvas = FindObjectOfType<uiBarsCanvas>().GetComponent<Canvas>();
            hardPointUIinstance = Instantiate(Resources.Load("HardpointOverlay") as GameObject);
            hardPointUIinstance.transform.SetParent(uiCanvas.transform);
            hardPointUIinstance.transform.position = transform.position;
            hardPointUIinstance.transform.rotation = transform.rotation;
            myShipyard = FindObjectOfType<shipyard>();
            hardPointUIinstance.GetComponent<RectTransform>().localScale = Vector3.one;
            if(uibarIcon != null) hardPointUIinstance.GetComponent<Image>().sprite = uibarIcon;
            if(GetComponent<Collider>() == null){
                SphereCollider a = gameObject.AddComponent<SphereCollider>();
                if(gameObject.GetComponent<Renderer>()!=null)a.radius = gameObject.GetComponent<Renderer>().bounds.size.magnitude * 10;
                else a.radius = 10f;
                
            } 
            if(FindObjectOfType<shipyard>() != null) showAttachableItems();
        }
    }
    
    public bool ventralControl = false;
    // Start is called before the first frame update
    

    // Update is called once per frame
    public virtual void requestSettings(GameObject weaponObject){
        
    }
}
