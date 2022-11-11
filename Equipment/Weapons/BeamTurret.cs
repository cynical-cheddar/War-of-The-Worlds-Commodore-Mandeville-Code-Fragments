using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTurret : MountedTurret
{
    // Start is called before the first frame update
    
    public LayerMask rayMask;

    [Range(0, 1)]
    public float minimumChargeToFireMultiplier = 0.5f;
    public List<AudioClip> impactSounds;

    public bool forgetTargetAfterFire = false;
  
    public float damage = 150f;
    public float cooldown = 1f;
    protected float currentCooldown = 1f;

    public float fireCooldown = 1f;
    protected float currentFireCooldown = 0f;
    public GameObject beamPrefab;
    public List<Transform> firePoints;

    public AudioClip[] fireSound;

    public bool firesound2d = false;
    public bool playImpactOnShieldHit = true;

    
    public GameObject[] beamLineRendererPrefab;
    public GameObject[] beamStartPrefab;
    public GameObject[] beamEndPrefab;

    protected LineRenderer line;

    protected int currentBeam = 0;
    protected GameObject beamStart;
    protected GameObject beamEnd;
    protected GameObject beam;
    [Header("Adjustable Variables")]
    public float beamEndOffset = 1f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
    public float textureLengthScale = 3; //Length of the beam texture

    protected Vector3 oldHitpoint;
    protected Vector3 CurHitPoint;

    protected Vector3 oldStart;
    protected Vector3 CurStart;

    public GameObject impactParticlePrefab;
    public GameObject impactParticlePrefabShield;


    protected bool beamFiring = false;

    protected GameObject beamInstance;

    protected float disableCooldown;
    protected float disableCooldownCur;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    /// 
    /// 
    

    public override void doAbility(){
        // take manual control
        Debug.Log("Beam ability");
        GetComponent<Ability>().doAbility();
        
    }
    protected void forgetTarget(){
        GetComponent<WeaponAiController>().forgetTarget();
    }

    new protected void Awake()
    {
        base.Awake();
        disableCooldown = fireCooldown * 1.2f;
        disableCooldownCur = disableCooldown;
    }
    protected void Update() {
        disableCooldownCur -= Time.deltaTime;
        if(disableCooldownCur<=0){
            ceaseFireBeam();
            disableCooldownCur = disableCooldown;
        }
    }
    public override bool canFire(){
        if(currentFireCooldown <= 0 && currentSalvo > 0 && !myShip.holdFire){
            // do firing arc shizzle
            
            return true;
        }
        return false;
    }
    public bool depletedFire(){
        if(currentSalvo <= 0 || myShip.holdFire){
            // do firing arc shizzle
            if(forgetTargetAfterFire)forgetTarget();
            return true;
        }
        return false;
    }

    public void startFireBeam(){
        beamFiring = true;
        // when we start firing the beam, instantiate a beam prefab and set a start and endpoint in update
        beamStart = Instantiate(beamStartPrefab[currentBeam], firePoints[0].position, Quaternion.identity) as GameObject;
        beamEnd = Instantiate(beamEndPrefab[currentBeam], firePoints[0].position, Quaternion.identity) as GameObject;
        beam = Instantiate(beamLineRendererPrefab[currentBeam], firePoints[0].position, Quaternion.identity) as GameObject;
        line = beam.GetComponent<LineRenderer>();
        line.startWidth = line.startWidth * damageMultiplier;
        line.endWidth = line.endWidth * damageMultiplier;
        oldHitpoint = firePoints[0].position;
        playFireSound();
    }
    public override void ceaseFireBeam(){
        if(beamFiring){
            beamFiring = false;
            Destroy(beamStart);
            Destroy(beamEnd);
            Destroy(beam);
        }
    }
    public override void Fire(){
        
        Physics.IgnoreLayerCollision(10,10);
        if(currentSalvo<=0 && forgetTargetAfterFire) forgetTarget();
        if(currentSalvo > 0 && currentFireCooldown <= 0){
            disableCooldownCur = disableCooldown;
            if(beamFiring == false && !depletedFire() && currentSalvo >= salvoSize*minimumChargeToFireMultiplier) startFireBeam();

            if(beamFiring){
            decrementSalvo();
            if(currentSalvo <= 0 && forgetTargetAfterFire) forgetTarget();
            foreach(Transform firePoint in firePoints){
                
                /*GameObject projectileInstance = Instantiate(projectile, firePoint.position, firePoint.rotation) as GameObject;
                projectileInstance.GetComponent<ProjectileScript>().setDamage(damage * damageMultiplier);
                //Get all colliders in our ship. 
                // Ignore Collision with the ship
                
                Collider[] shipColliders = transform.root.gameObject.GetComponentsInChildren<Collider>();
             //   Debug.Log(shipColliders);
                foreach(Collider shipCollider in shipColliders){
                    Physics.IgnoreCollision(projectileInstance.GetComponent<Collider>(), shipCollider);
                }*/

                /*projectileInstance.transform.rotation = Quaternion.Euler(projectileInstance.transform.rotation.eulerAngles.x + Random.RandomRange(-projectileDeviation, projectileDeviation), projectileInstance.transform.rotation.eulerAngles.y + Random.RandomRange(-projectileDeviation, projectileDeviation), projectileInstance.transform.rotation.eulerAngles.z + Random.RandomRange(-projectileDeviation, projectileDeviation));
                
                projectileInstance.GetComponent<Rigidbody>().AddForce(projectileInstance.transform.forward * muzzleVelocity, ForceMode.VelocityChange);
                projectileInstance.GetComponent<ProjectileScript>().setImpactSounds(impactSounds);*/

                // do a raycast from the firepoint forward. 
                

                // if something is hit, that is the endpoint

                // otherwise send it off into the distance and set the beam to taper out

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
    protected void OnDestroy()
    {
        Destroy(beam);
        Destroy(beamEnd);
        Destroy(beamStart);
    }

    protected void applyDamageToHit(RaycastHit hit, Transform hitObject){
            //transform.DetachChildren();
            float multiplier = 1f;
            multiplier = calculateDamageRampup(Vector3.Distance(firePoints[0].position, hit.point));
            GameObject impactParticle;
            if(hitObject.gameObject.GetComponent<ShieldHealth>()!=null){
                if(impactParticlePrefabShield!= null) impactParticle = Instantiate(impactParticlePrefabShield, hit.point+firePoints[0].forward*3, Quaternion.identity) as GameObject;
                else impactParticle = Instantiate(impactParticlePrefab, hit.point+firePoints[0].forward*3, Quaternion.identity) as GameObject;
            }
            else impactParticle = Instantiate(impactParticlePrefab, hit.point+firePoints[0].forward*3, Quaternion.identity) as GameObject;
            impactParticle.transform.localScale = new Vector3(impactParticle.transform.localScale.x * multiplier, impactParticle.transform.localScale.y * multiplier,impactParticle.transform.localScale.z * multiplier);
            
            AudioSource a = impactParticle.GetComponent<AudioSource>();
            if(!selected)a.spatialBlend = 0.9f;
            else a.spatialBlend = 0f;
            a.volume = fireVolume ;
            a.dopplerLevel = 0.1f; 
            a.rolloffMode = AudioRolloffMode.Logarithmic;
            a.maxDistance = 2000f;
            a.minDistance = 600f;
            if(impactSounds != null && playImpactOnShieldHit)impactParticle.GetComponent<AudioSource>().PlayOneShot(impactSounds[Random.RandomRange(0, impactSounds.Count-1)]);
            Destroy(impactParticle, 2f);
            //Debug.DrawRay(hit.contacts[0].point, hit.contacts[0].normal * 1, Color.yellow);
            
            // get the collider we have hit and traverse up the tree to find the first health script we come across.
            Health impactHealthScript = FindParentHealth(hit.collider.transform);

            
            
            
            if(impactHealthScript != null){
                if(impactHealthScript.gameObject.transform.root != transform.root)impactHealthScript.applyDamage(damage * multiplier, damageType);
            }

            //yield WaitForSeconds (0.05);
            
           
            Destroy(impactParticle, 5f);
            
    }
    public Health FindParentHealth(Transform tPam)
    {
        Transform t = tPam;
        if(t.GetComponent<Health>() != null) return t.GetComponent<Health>();

        return t.GetComponentInParent<Health>();

        return null; // Could not find a parent with given tag.
    }
    protected IEnumerator LerpHitAndStart(Vector3 oldPos, Vector3 newPos, float time, Transform Barrel) // We need to pass this the old and new vector positions of the hitpoint.
    {
        float ElapsedTime = 0f;
        while (ElapsedTime <= time)
        { // until one second passed

            ElapsedTime += Time.deltaTime;
            CurHitPoint = Vector3.Lerp(oldPos, newPos, (ElapsedTime / time)); // lerp from A to B in one second
            Vector3 startPos = Barrel.position + (Barrel.GetComponent<Rigidbody>().velocity * ElapsedTime);
            
            beamStart.transform.position = startPos;
           // beamStart.transform.localPosition = Vector3.zero;
            line.SetPosition(0, startPos);


            line.SetPosition(1, CurHitPoint);
            beamEnd.transform.position = CurHitPoint;
            beamStart.transform.LookAt(beamEnd.transform.position);
            beamEnd.transform.LookAt(beamStart.transform.position);

            float distance = Vector3.Distance(Barrel.transform.position, CurHitPoint);
            line.sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
            line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
            yield return new WaitForEndOfFrame(); // wait for next frame
        }
    }
    protected void ShootBeamInDir(Vector3 start, Vector3 dir)
    {
        line.SetVertexCount(2);
        line.SetPosition(0, start);
        beamStart.transform.position = start;

        Vector3 end = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(start, dir, out hit))
            end = hit.point - (dir.normalized * beamEndOffset);
        else
            end = transform.position + (dir * 100);




        // Replace above with own code - Raycast from camera.
        beamEnd.transform.position = end;
        line.SetPosition(1, end);

        beamStart.transform.LookAt(beamEnd.transform.position);
        beamEnd.transform.LookAt(beamStart.transform.position);

        float distance = Vector3.Distance(start, end);
        line.sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
        line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
    }

    protected void playFireSound(){
        GameObject fire = Instantiate(new GameObject(), firePoints[0].position, firePoints[0].rotation);
        AudioSource a = fire.AddComponent<AudioSource>();
        if(!selected)a.spatialBlend = 1f;
        
        else a.spatialBlend = 0f;
        if(firesound2d) a.spatialBlend = 0.1f;

        a.dopplerLevel = 0.1f; 
        a.rolloffMode = AudioRolloffMode.Logarithmic;
        a.maxDistance = 2000f;
        a.minDistance = 600f;
        int index = Random.RandomRange(0, fireSound.Length -1);
        a.PlayOneShot(fireSound[index], fireVolume);
        Destroy(fire, 5f);
    }
    // using lateUpdate to avoid overriding mounted turret behaviour
    new protected void LateUpdate()
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
           currentCooldown = cooldown;
           reloadShells(1);
       } 
       if(currentFireCooldown>=0){
           currentFireCooldown -= Time.deltaTime;
           
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
       // list.Add("Projectile Deviation: " + projectileDeviation.ToString() + " degrees");
        return list;
    }



    protected Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint)
    {

        RaycastHit[] hits = Physics.RaycastAll(ray);

        Transform closestHit = null;
        float distance = 0;
        hitPoint = Vector3.zero;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.root != this.transform && (closestHit == null || hit.distance < distance))
            {
                // We have hit something that is:
                // a) not us
                // b) the first thing we hit (that is not us)
                // c) or, if not b, is at least closer than the previous closest thing

                closestHit = hit.transform;
                distance = hit.distance;
                hitPoint = hit.point;
            }
        }

        // closestHit is now either still null (i.e. we hit nothing) OR it contains the closest thing that is a valid thing to hit

        return closestHit;

    }
}