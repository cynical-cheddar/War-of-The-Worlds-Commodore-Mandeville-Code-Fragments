using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    public float hitpoints = 100f;
    protected float maxHitpoints = 100f;

    protected bool dead = false;

    public float kineticResistanceMultiplier = 1f;
    public float energyResistanceMultiplier = 1f;

    public float thermalResistanceMultiplier = 1f;

    protected void Start(){
        maxHitpoints = hitpoints;
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);
    }
    public void setMaxHealth(float amt){
        maxHitpoints = amt;
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);
    }
    public void resetHealth(){
      hitpoints = maxHitpoints;
      if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);
    }
    public virtual void applyDamage(float damage)
    {
      //  Debug.Log("Generic damge to generic health");
        hitpoints -= damage;
        
        if(hitpoints <= 0) die();
        
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);

    }

    

    public virtual void applyDamage(float damage, Weapon.DamageType damageType)
    {
      //  Debug.Log("Generic damge to generic health");
        if(damageType == Weapon.DamageType.kinetic)hitpoints -= damage *kineticResistanceMultiplier;
        else if(damageType == Weapon.DamageType.energy)hitpoints -= damage *energyResistanceMultiplier;
        else if(damageType == Weapon.DamageType.thermal)hitpoints -= damage *thermalResistanceMultiplier;
        else if(damageType == Weapon.DamageType.shield){
          if(transform.root.gameObject.GetComponentInChildren<ShieldHealth>()!=null){
            transform.root.gameObject.GetComponentInChildren<ShieldHealth>().applyDamage(damage);
          }
        }
        else hitpoints -= damage;



        
        if(hitpoints <= 0) die();
        
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);

    }
    public virtual void die(){
        dead = true;
    }

    // Update is called once per frame

}
