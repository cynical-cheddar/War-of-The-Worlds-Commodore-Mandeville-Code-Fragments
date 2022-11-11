using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Module : MonoBehaviour
{
    public struct prerequesites{
        public bool combat;
        public bool science;
        

    }

   // public prerequesites modulePrerequistes;

   public bool prerequesite_combat = false;

   public bool prerequesite_science = false;

    public float bonusHealth = 500f;
    public float tonnage = 2500f;
    public float rammingDamageMultiplier = 1f;
    public Sprite icon;
    string id = "0";
    public int moduleOrderSize = 1;
    public Transform frontConnector;
    public Transform rearConnector;

    public int baseCost = 100;
    public float armourRating = 50f;
    float curArmour = 50f;

    public List<Transform> hardpoints;

    public List<Transform> eqipmentPoints;
    
    public List<string> statList;

    int hpIndex = 0;
    public void setID(string newID){
        id = newID;
    }
    public string getId(){
        return id;
    }

    public virtual List<string> getStats(){
        List<string> list = new List<string>();
        
        
        list.Add(gameObject.name);
        list.Add("Cost: £" + baseCost.ToString());
        list.Add("Armour Rating: " + armourRating.ToString());
        list.Add("Hardpoints " + hardpoints.Count.ToString());
        list.Add("Tonnage: " + tonnage.ToString() + " tonnes");
        list.Add("Bonus Health:" + bonusHealth.ToString() + " hp");
        list.AddRange(statList);
        //if(GetComponent<ModuleHealth>() != null) list.Add("Module Health " + GetComponent<ModuleHealth>().hitpoints.ToString());
        return list;
    }


    // Start is called before the first frame update
    protected void Awake()
    {
            WeaponHardpoint[] hardpointsArr = GetComponentsInChildren<WeaponHardpoint>();
            foreach(WeaponHardpoint h in hardpointsArr){
                hardpoints.Add(h.transform);
        }
            Hardpoint[] hardpointAllsArr = GetComponentsInChildren<Hardpoint>();
            foreach(Hardpoint h in hardpointAllsArr){
                eqipmentPoints.Add(h.transform);
                h.setIndex(hpIndex);
                hpIndex++;
            }
    }
    public void returnEquipmentToInventory(){
        Equipment[] myParts = GetComponentsInChildren<Equipment>();
        foreach(Equipment part in myParts){
            part.addToInventory();
        }
    }

    protected void Start() {
        int i = 0;
        eqipmentPoints.Clear();
        foreach(Transform t in eqipmentPoints){
            t.GetComponent<Hardpoint>().setIndex(i);
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
