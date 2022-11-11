using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferRay : BeamTurret
{
    // Start is called before the first frame update
    ShieldHealth myShields;
    public float efficency = 1f;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    new protected void Awake()
    {
        base.Awake();
        disableCooldown = fireCooldown * 1.2f;
        disableCooldownCur = disableCooldown;
        
        
    }
    // || 
    public override void Fire(){
        myShields = transform.root.GetComponentInChildren<ShieldHealth>();
        Physics.IgnoreLayerCollision(10,10);
        if((currentSalvo<=0 )&& forgetTargetAfterFire) forgetTarget();
        if(currentSalvo > 0 && currentFireCooldown <= 0){
            disableCooldownCur = disableCooldown;
            if(beamFiring == false && !depletedFire() && currentSalvo >= salvoSize*minimumChargeToFireMultiplier) startFireBeam();

            if(beamFiring){
            decrementSalvo();
            if(currentSalvo <= 0 && forgetTargetAfterFire) forgetTarget();
            foreach(Transform firePoint in firePoints){
                

                Ray ray = new Ray(firePoint.position, firePoint.forward);
                RaycastHit hit; //From camera to hitpoint, not as curent
                Transform hitTransform;
                Vector3 hitVector;
                hitTransform = FindClosestHitObject(ray, out hitVector);


                Physics.Raycast(ray.origin, ray.direction, out hit, range, rayMask);
               

                if (hitTransform == null)
                {
                    hit.point = firePoint.position + (firePoint.forward * 1500f);
                }
                
                if(oldHitpoint == null) oldHitpoint = firePoint.position;
               

                
              //  ShootBeamInDir(firePoint.position, firePoint.forward);
                StartCoroutine(LerpHitAndStart(oldHitpoint, hit.point, fireCooldown, firePoint));
                oldHitpoint = hit.point;
                

                // now do the damagy stuff
                if(hitTransform!=null) applyDamageToHit(hit, hitTransform);


            }
            }
            currentFireCooldown = fireCooldown;
        }


    }
    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>


    protected new void applyDamageToHit(RaycastHit hit, Transform hitObject){
            //transform.DetachChildren();
            float multiplier = 1f;
            multiplier = calculateDamageRampup(Vector3.Distance(firePoints[0].position, hit.point));


            
            
            //Debug.DrawRay(hit.contacts[0].point, hit.contacts[0].normal * 1, Color.yellow);
            
            // get the collider we have hit and traverse up the tree to find the first health script we come across.
            Health impactHealthScript = FindParentHealth(hit.collider.transform);

            
            
            
            if(impactHealthScript != null){
                // determine if we are locked onto a friendly ship or enemy ship
                if(impactHealthScript.gameObject.transform.root != transform.root){

                    switch (impactHealthScript){
                        // case of shields
                        case ShieldHealth h:

                            GameObject impactParticle = Instantiate(impactParticlePrefab, hit.point+firePoints[0].forward*3, Quaternion.identity) as GameObject;
                            impactParticle.transform.localScale = new Vector3(impactParticle.transform.localScale.x * multiplier, impactParticle.transform.localScale.y * multiplier,impactParticle.transform.localScale.z * multiplier);
                            AudioSource a = impactParticle.GetComponent<AudioSource>();
                            if(!selected)a.spatialBlend = 0.9f;
                            else a.spatialBlend = 0f;
                            a.volume = fireVolume ;
                            a.dopplerLevel = 0.1f; 
                            a.rolloffMode = AudioRolloffMode.Logarithmic;
                            a.maxDistance = 2000f;
                            a.minDistance = 600f;
                            if(impactSounds != null)impactParticle.GetComponent<AudioSource>().PlayOneShot(impactSounds[Random.RandomRange(0, impactSounds.Count-1)]);
                            // transfer energy
                            if(h.transform.root.gameObject.GetComponent<Ship>().teamId == transform.root.gameObject.GetComponent<Ship>().teamId){
                                if(myShields.hitpoints <= 10) forgetTarget();
                                impactHealthScript.applyDamage(-damage * multiplier, damageType);
                                myShields.applyDamage(damage * (1/efficency), damageType);
                            }
                            // drain energy
                            else{
                                impactHealthScript.applyDamage(damage * multiplier, damageType);
                                myShields.applyDamage(-damage, damageType);
                            }
                            Destroy(impactParticle, 2f);
                            break;
                        // IFF the other ship is ours and has no shields, we may transfer energy
                        default:
                            if(impactHealthScript.transform.root.gameObject.GetComponent<Ship>().teamId == transform.root.gameObject.GetComponent<Ship>().teamId){
                                if(myShields.hitpoints <= 10) forgetTarget();
                                impactHealthScript.transform.root.gameObject.GetComponentInChildren<ShieldHealth>().applyDamage(-damage * multiplier, damageType);
                               
                                myShields.applyDamage(damage * (1/efficency), damageType);
                            }
                        break;
                    }
                    

                }
            }

            //yield WaitForSeconds (0.05);
            
           
            
            
    }
}
