using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTauntFunnel : Ability
{
    Ship myShip;
    public float range = 300f;

    public float abilityDuration = 10f;
    List<Ship> shipsInRange = new List<Ship>();
    Ship [] array;
    public override void doAbility(){ 
        // do ability fx
        if(GetComponent<TauntFunnel>()!=null) GetComponent<TauntFunnel>().funnelFx();

        myShip = GetComponentInParent<Ship>();
        array = FindObjectsOfType<Ship>();
        shipsInRange = new List<Ship>();
        // do an overlap sphere
        foreach (Ship s in array){
            
            float distanceSqr = Vector3.Distance(transform.position, s.transform.position);
            if(distanceSqr < range && myShip.teamId != s.teamId)
               shipsInRange.Add(s);
        }
        
        // foreach enemy, set their target to be this ship
        foreach(Ship s in shipsInRange){

            s.gameObject.GetComponent<CapitalShipAi>().setCustomTarget(myShip.transform, abilityDuration);
           // foreach weapon ai controller, set their target to be this ship
           WeaponAiController[] weapons = s.gameObject.GetComponentsInChildren<WeaponAiController>();
           foreach(WeaponAiController weapon in weapons){
               weapon.userSetTarget(myShip.transform, abilityDuration);
           }
        }
        
    }

    protected override void FixedUpdate(){
        if(cooldownRemaining >= 0){
            cooldownRemaining -= Time.deltaTime;
            if(abilityButton!=null){
                abilityButton.updateCooldown(Mathf.RoundToInt(cooldownRemaining).ToString());
                if(Mathf.RoundToInt(cooldownRemaining) == 0) abilityButton.updateCooldown("");
            }
        }
    }


    // a callback function to reset cooldown
    public override void doneAbility(){
        cooldownRemaining = cooldown;
    }
    public override bool canExecute(){ 

        if(cooldownRemaining <= 1){
            Debug.Log("true");
            return true;
        }
        else return false;
        
    }
}
