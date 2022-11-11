using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoLauncher : MountedTurret
{
    // Start is called before the first frame update
    public List<AudioClip> impactSounds;
    public List<AudioClip> shieldImpactSounds;

    public int maxAmmo = 8;
    


    public float damage = 150f;
    public float cooldown = 1f;
    float currentCooldown = 1f;

    public float fireCooldown = 1f;
    float currentFireCooldown = 0f;
    public GameObject projectile;
    public List<Transform> firePoints;

    public AudioClip[] fireSound;

    
    public float cooldownDeviation = 0.2f;

    public float fireCooldownDeviation = 0.2f;
    public float projectileDeviation = 1f;


    public override void doAbility(){
        // take manual control
        Debug.Log("Beam ability");
        GetComponent<Ability>().doAbility();
        
    }
    public override bool canFire(){
        if(currentFireCooldown <= 0 && currentSalvo > 0 && !myShip.holdFire){
            // do firing arc shizzle
            
            return true;
        }
        return false;
    }
    public override void Fire(){
        Physics.IgnoreLayerCollision(10,10);
        Debug.Log("Torpedoes away");
        if(currentSalvo > 0 && currentFireCooldown <= 0 && maxAmmo >= 0){
            playFireSound();
            decrementSalvo();
            if(hasAbility) GetComponent<Ability>().doneAbility();
            maxAmmo--;
            
            foreach(Transform firePoint in firePoints){
                GameObject projectileInstance = Instantiate(projectile, firePoint.position, firePoint.rotation) as GameObject;
                projectileInstance.GetComponent<ProjectileScript>().setDamage(damage * damageMultiplier);
                
                projectileInstance.GetComponent<ProjectileScript>().setHostWeapon(this);
                //Get all colliders in our ship. 
                // Ignore Collision with the ship
                
                Collider[] shipColliders = transform.root.gameObject.GetComponentsInChildren<Collider>();
             //   Debug.Log(shipColliders);
                foreach(Collider shipCollider in shipColliders){
                    Physics.IgnoreCollision(projectileInstance.GetComponent<Collider>(), shipCollider);
                }
                projectileInstance.transform.rotation = Quaternion.Euler(projectileInstance.transform.rotation.eulerAngles.x + Random.RandomRange(-projectileDeviation, projectileDeviation), projectileInstance.transform.rotation.eulerAngles.y + Random.RandomRange(-projectileDeviation, projectileDeviation), projectileInstance.transform.rotation.eulerAngles.z + Random.RandomRange(-projectileDeviation, projectileDeviation));
                
                projectileInstance.GetComponent<Rigidbody>().AddForce(projectileInstance.transform.forward * muzzleVelocity, ForceMode.VelocityChange);
                projectileInstance.GetComponent<ProjectileScript>().setImpactSounds(impactSounds);
                projectileInstance.GetComponent<ProjectileScript>().setDamageType(damageType);
                if(shieldImpactSounds.Count > 0) projectileInstance.GetComponent<ProjectileScript>().setShieldImpactSounds(shieldImpactSounds);
            }
            currentFireCooldown = fireCooldown + Random.Range(-fireCooldownDeviation, fireCooldownDeviation);
            doneAbility();
        }
    }
    public override void doneAbility(){
        if(GetComponent<Ability>()!=null){
            GetComponent<Ability>().doneAbility();
        }
    }

    void playFireSound(){
        GameObject fire = Instantiate(new GameObject(), firePoints[0].position, firePoints[0].rotation);
        AudioSource a = fire.AddComponent<AudioSource>();
        if(!selected)a.spatialBlend = 1f;
        else a.spatialBlend = 0f;

        a.dopplerLevel = 0.1f; 
        a.rolloffMode = AudioRolloffMode.Logarithmic;
        a.maxDistance = 2000f;
        a.minDistance = 600f;
        int index = Random.RandomRange(0, fireSound.Length -1);
        a.PlayOneShot(fireSound[index], fireVolume);
        Destroy(fire, 2f);
    }
    // using lateUpdate to avoid overriding mounted turret behaviour
    new void LateUpdate()
    {
       base.LateUpdate();
       if(currentCooldown>=0 && currentSalvo < salvoSize){
            currentCooldown -= Time.deltaTime * reloadSpeedMultiplier;
            weaponUiInterface.displayReloadProgress(currentCooldown);
            weaponCooldownFreelookHud(currentCooldown + cooldown*((salvoSize-1) - currentSalvo), cooldown*(salvoSize));
       }
       if(currentSalvo == salvoSize){
           weaponCooldownFreelookHud(0,1);
       }
       if(currentCooldown<=0 && currentSalvo < salvoSize){
           currentCooldown = cooldown + Random.Range(-cooldownDeviation, cooldownDeviation);
           reloadShells(1);
       } 
       if(currentFireCooldown>=0){
           currentFireCooldown -= Time.deltaTime * reloadSpeedMultiplier;
           
       }
       
    }
    public override List<string> getStats(){
        List<string> list = new List<string>();
        list.Add(weaponName);
        list.AddRange(base.getStats());
        list.Add("Weapon Type: " + damageType);
        list.Add("Damage per shell: " + damage.ToString());
        list.Add("Shells per shot: " + firePoints.Count.ToString());
        list.Add("Salvo size: " + salvoSize.ToString());  
        list.Add("Reload time: " + cooldown.ToString() + "s");
        list.Add("Projectile Deviation: " + projectileDeviation.ToString() + " degrees");
        return list;
    }
}
