using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManualControl : Ability
{
    // Start is called before the first frame update


    public float minimumPercentageToFire = 0.5f;
    public enum CooldownType
    {
        independentCooldown,
        displayChargePercentage,
        displaySalvoSize

    }



    public CooldownType cooldownType = CooldownType.independentCooldown;


    public override void doAbility(){ 
        GetComponent<Weapon>().selectWeapon();
    }

    protected override void FixedUpdate(){
        switch (cooldownType){
            // independent cooldown
            case (CooldownType.independentCooldown):
                if(cooldownRemaining >= 0){
                    cooldownRemaining -= Time.deltaTime;
                    if(abilityButton!=null){
                         abilityButton.updateCooldown(Mathf.RoundToInt(cooldownRemaining).ToString());
                         if(Mathf.RoundToInt(cooldownRemaining) == 0) abilityButton.updateCooldown("");
                    }
                }
            break;
            case (CooldownType.displayChargePercentage):
                   // cooldownRemaining = GetComponent<Weapon>().getSalvoInfo()[0];
                   float cur = GetComponent<Weapon>().getSalvoInfo()[0];
                   float max = GetComponent<Weapon>().getSalvoInfo()[1];
                    if(abilityButton!=null) abilityButton.updateCooldown(((cur / max)*100).ToString() + "%");
            break;
            case (CooldownType.displaySalvoSize):
                    cooldownRemaining = GetComponent<Weapon>().getSalvoInfo()[0];
                    if(abilityButton!=null) abilityButton.updateCooldown(Mathf.RoundToInt(cooldownRemaining).ToString());
            break;

        }
    }


    // a callback function to reset cooldown
    public override void doneAbility(){
        cooldownRemaining = cooldown;
    }
    public override bool canExecute(){ 
        switch (cooldownType){
            // independent cooldown
            case (CooldownType.independentCooldown):
                if(cooldownRemaining <= 0){
                    return true;
                }
            break;
            case (CooldownType.displayChargePercentage):
                if(GetComponent<Weapon>().getSalvoInfo()[0] >= GetComponent<Weapon>().getSalvoInfo()[1]*minimumPercentageToFire) return true;
            break;
            case (CooldownType.displaySalvoSize):
                 if(GetComponent<Weapon>().getSalvoInfo()[0]  >= GetComponent<Weapon>().getSalvoInfo()[1]*minimumPercentageToFire) return true;
            break;

        }
        return false;
    }


}
