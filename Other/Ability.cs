using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public float cooldown = 60f;
    protected float cooldownRemaining = 0f;

    protected AbilityButton abilityButton;

    public void setAbilityButton(AbilityButton btn){
        abilityButton = btn;
    }


    // using fixed update for efficiency
    protected virtual void FixedUpdate(){
        if(cooldownRemaining >= 0){
            cooldownRemaining -= Time.deltaTime;
            if(abilityButton!=null) abilityButton.updateCooldown(Mathf.RoundToInt(cooldownRemaining).ToString());
        }
    }

    public virtual void doAbility(){ 
        doneAbility();
    }

    // a callback function to reset cooldown
    public virtual void doneAbility(){
        cooldownRemaining = cooldown;
    }
    public virtual bool canExecute(){ 
        return true;
    }
}
