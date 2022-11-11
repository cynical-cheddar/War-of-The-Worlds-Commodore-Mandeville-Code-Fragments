using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    // Start is called before the first frame update
    Equipment abilityEquipment;
    Ability abilityObject;

    public Text cooldowntext;

    string curCooldownText = "";

    public void setAbilityObject(Equipment obj){
        abilityEquipment = obj;
        abilityObject = abilityEquipment.gameObject.GetComponent<Ability>();
        abilityObject.setAbilityButton(this);
    }
    public void updateCooldown(string amt){
        curCooldownText = amt;
        if(cooldowntext!=null)cooldowntext.text = amt;
    }
    public void doAbility(){
        // check if the ability object
        if(abilityObject.canExecute()) abilityEquipment.doAbility();
       
    }
}
