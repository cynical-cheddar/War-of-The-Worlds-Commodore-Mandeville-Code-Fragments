using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : Health
{
    // Start is called before the first frame update

    public float incomingDamageMultiplier = 1f;

    public float rechargeRate = 2f;

    // the amount of time taknen after taking damage until chields canb begin recharging while under operation
    public float rechargeCooldown = 5f;

    public bool recharging = false;

    float baseAlpha = 10f;
    float maxBaseAlpha = 30f;

    

    Collider[] childColliders;

    float lastDamageTime = 0f;

    public ParticleSystem[] particleSystems;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    new void Start()
    {
        base.Start();
        childColliders = GetComponentsInChildren<Collider>();
        setChildCollidersStatus(true);
    }

    public void setIncomingDamageMultiplier(float amt){
        incomingDamageMultiplier = amt;
        float shieldAmount = (1/amt);
        
        baseAlpha = Mathf.Lerp(0, maxBaseAlpha, shieldAmount);
    }
     
    public override void applyDamage(float damage){
        hitpoints -= damage *incomingDamageMultiplier;
        lastDamageTime = Time.time;
        if(damage >0)recharging = false;
        if(hitpoints <= 0) die();
        if(hitpoints >= maxHitpoints)hitpoints = maxHitpoints;
        playHitEffects(damage);
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);
    }
    

    public override void applyDamage(float damage, Weapon.DamageType damageType)
    {
        if(damageType == Weapon.DamageType.kinetic)hitpoints -= damage *incomingDamageMultiplier*kineticResistanceMultiplier;
        else if(damageType == Weapon.DamageType.energy)hitpoints -= damage *incomingDamageMultiplier*energyResistanceMultiplier;
        else if(damageType == Weapon.DamageType.thermal)hitpoints -= damage *incomingDamageMultiplier*thermalResistanceMultiplier;
        else if(damageType == Weapon.DamageType.shield) hitpoints -= damage;
        else hitpoints -= damage *incomingDamageMultiplier;

        // shields weapons do not stop recharge 
        if(damageType != Weapon.DamageType.shield){
            lastDamageTime = Time.time;
            recharging = false;
        }
        
        if(hitpoints <= 0) die();
        if(hitpoints >= maxHitpoints)hitpoints = maxHitpoints;
        playHitEffects(damage);
        if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);

    }

    public void playHitEffects(float damage){
        float alphaIncrease = 0.5f * damage;
        if(alphaIncrease > 10) alphaIncrease = 10f;
        // get the particle systems and add to their alpha value
        foreach(ParticleSystem s in particleSystems){
             if(s.startColor.a < 255) s.startColor  = new Color(s.startColor.r, s.startColor.g, s.startColor.b, s.startColor.a + alphaIncrease);
             Debug.Log("starcolour " + s.startColor.a);
        }
       
    }

    public override void die(){
        // disable shield and recharge
        setChildCollidersStatus(false);
        recharging = true;
        foreach(ParticleSystem s in particleSystems){
     
                s.startColor =  new Color(s.startColor.r, s.startColor.g, s.startColor.b,0);
            }
    }

    void setChildCollidersStatus(bool set){
        foreach(Collider col in childColliders){
            col.enabled = set;
        }
    }
    void FixedUpdate(){
        if(recharging){
            if(hitpoints< maxHitpoints){
                hitpoints += rechargeRate*Time.deltaTime*(1/incomingDamageMultiplier) * (maxHitpoints/100);
                if(GetComponent<HealthBarOverlay>() != null) GetComponent<HealthBarOverlay>().setNumber(hitpoints, maxHitpoints);
            }
            if(hitpoints >= maxHitpoints){
                hitpoints=maxHitpoints;
                recharging = false;
                setChildCollidersStatus(true);
            } 
        }
        else{
            if(Time.time - lastDamageTime > rechargeCooldown) recharging = true;
        }
        if(!recharging){
            foreach(ParticleSystem s in particleSystems){
                if(s.startColor.a > baseAlpha/255)s.startColor  = new Color(s.startColor.r, s.startColor.g, s.startColor.b, s.startColor.a - Time.deltaTime*100 );
                if(s.startColor.a < baseAlpha/255)s.startColor =  new Color(s.startColor.r, s.startColor.g, s.startColor.b, baseAlpha/255);
            }
        }
        else{
            foreach(ParticleSystem s in particleSystems){
     
                s.startColor =  new Color(s.startColor.r, s.startColor.g, s.startColor.b,0);
            }
        }
        
    }


}
