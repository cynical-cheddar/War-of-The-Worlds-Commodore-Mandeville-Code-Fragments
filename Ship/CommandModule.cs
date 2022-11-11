using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandModule : Module
{
    public float coreHealth = 4000f;
    public Transform[] wingConnector;
    public struct moduleType{
        public bool combat;
        public bool science;
        

    }

   // public moduleType commandType;

    public bool command_combat = true;
    public bool command_science = true;

    // Start is called before the first frame update

    public override List<string> getStats(){
        List<string> list = new List<string>();
        
        
        list.Add(gameObject.name);
        list.Add("Cost: £" + baseCost.ToString());
        list.Add("Armour Rating: " + armourRating.ToString());
        list.Add("Hardpoints " + hardpoints.Count.ToString());
        list.Add("Tonnage: " + tonnage.ToString() + " tonnes");
        list.Add("Core Health: " + coreHealth.ToString() + " hp");
        list.Add("Bonus Health:" + bonusHealth.ToString() + " hp");
        list.AddRange(statList);
        //if(GetComponent<ModuleHealth>() != null) list.Add("Module Health " + GetComponent<ModuleHealth>().hitpoints.ToString());
        return list;
    }

}
