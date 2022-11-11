using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleHealth : Health
{

    HealthShip parentHealth;

    // Start is called before the first frame update
    new void Start()
    {
        maxHitpoints = hitpoints;
        parentHealth = transform.GetComponentInParent<Ship>().gameObject.GetComponent<HealthShip>();
    }

    // Update is called once per frame
    public override void applyDamage(float damage)
    {

        

        
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);

        Debug.Log("Module took hit");
        parentHealth.applyDamage(damage);

        
    }
    public override void applyDamage(float damage, Weapon.DamageType damageType)
    {
      //  Debug.Log("Generic damge to generic health");
        if(!dead){
            if(damageType == Weapon.DamageType.kinetic)hitpoints -= damage *kineticResistanceMultiplier;
            else if(damageType == Weapon.DamageType.energy)hitpoints -= damage *energyResistanceMultiplier;
            else if(damageType == Weapon.DamageType.thermal)hitpoints -= damage *thermalResistanceMultiplier;
            else hitpoints -= damage;


            
            if(hitpoints <= 0) die();
        }
        
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);
        parentHealth.applyDamage(damage);
    }   


}
