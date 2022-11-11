using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHealth : Health
{
    Health parentHealth;

    Equipment mountedEquipment;
    public float equipmentToModuleDamageRatio = 0.5f;

    // Start is called before the first frame update
    new void Start()
    {
        mountedEquipment = GetComponent<Equipment>();
        maxHitpoints = hitpoints;
        parentHealth = transform.GetComponentInParent<Health>();
    }

    // Update is called once per frame
    public override void applyDamage(float damage)
    {/*
        if(!dead){
            hitpoints -= damage;
            
            if(hitpoints <= 0) die();
        }
        
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);

        Debug.Log("Equipment took hit");
        parentHealth.applyDamage(damage);*/
    }
    public void repair(){
        if(dead){
            GetComponent<Equipment>().repairEquipment();
        }
        dead = false;
    }

    public override void die(){
        if(!dead){
            GetComponent<Equipment>().disableEquipment();
        }
        dead = true;
    }
}