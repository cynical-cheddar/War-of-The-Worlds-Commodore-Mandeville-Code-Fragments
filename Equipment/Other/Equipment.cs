using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Clickable
{
    public bool hasAbility = false;
    
    public bool showInShipyard = true;
    

    public enum partSize
    {
        Small,
        Medium,
        Large,
        Fixed,

        Science
    }

    public enum standardisedRange
    {
        Short,
        Medium,
        Long,
        Artillery,
        Infinite
    }
    [HideInInspector]
    public float shortRange = 800;
    [HideInInspector]
    public float mediumRange = 1500;
    [HideInInspector]

    public float longRange = 2300;
    [HideInInspector]

    public float artilleryRange = 3000;
    

    public float InfiniteRange = 99999999;

    public standardisedRange standardisedRangeSetting = standardisedRange.Short;
    public partSize equipmentSize;
    public string weaponName = "the gun";
    public Sprite weaponIcon;
    protected CaptialShipControl myShip;
    public int hardpointIndex = 0;
    public int cost = 100;

    protected bool inShipyard = false;

    public virtual void doAbility(){
        Debug.Log("Doing Ability");

    }
    public virtual void doneAbility(){
        Debug.Log("Done Ability");

    }
    
    
    protected void Start(){
        
        myShip = GetComponentInParent<CaptialShipControl>();
        if(FindObjectOfType<shipyard>()){
            if(GetComponent<Collider>()!=null) GetComponent<Collider>().enabled = false;
        }  
        if(FindObjectOfType<shipyard>()) inShipyard = true;
        if(hasAbility){
            // if we have an ability, then register it with the ability master
            transform.root.gameObject.GetComponentInChildren<ControlInterface>().addAbilityObject(this);
        }
    }
    
    public virtual void disableEquipment(){

    }
    public virtual void repairEquipment(){

    }

    public virtual List<string> getStats(){
        List<string> list = new List<string>();
        list.Add("Cost: £" + cost.ToString());
        return list;
    }

    public virtual void addToInventory(){
        
    }
}