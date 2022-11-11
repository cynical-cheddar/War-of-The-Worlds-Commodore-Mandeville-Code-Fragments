using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthShip : Health
{
    public int bountyCredits = 0;
    Ship myShip;
    public GameObject corpse;

    public GameObject rammingExplosion;

    float collisionDamagePerNewtonSecond = 0.3f;

    ControlInterface shipControlInterface;

    public float maxDamageFromRam = 1000f;

    float lastImpact = 0f;

    public GameObject shipBody;

    public LayerMask hullbreachRayMask;
    public float destructionDuration = 5f;

    public int maxHullExplosions = 10;
    public int minHullExplosions = 5;
    public GameObject finalExplodePrefab;

    public List<GameObject> hullbreachPrefabs = new List<GameObject>();
    // Start is called before the first frame update

    GameObject corpseInstance;

    public List<AudioClip> deathsounds = new List<AudioClip>();
    public List<AudioClip> explosionSounds = new List<AudioClip>();

    // Update is called once per frame
    void Update()
    {
        
    }
    

    protected void OnCollisionEnter(Collision other) {
        if(other.gameObject.GetComponentInParent<Ship>()!=null && other.transform.root != transform.root && (Time.time - lastImpact > 1f)){
            lastImpact = Time.time;
            Debug.Log("COLLISION BABY YEAHHH!");
            Vector3 impulseChange = other.impulse;
            // get the mass of the other object as well as our mass. Distribute damage accordingly
            float ourmass = myShip.GetComponentInParent<Rigidbody>().mass;
            float theirmass = other.collider.transform.root.gameObject.GetComponent<Rigidbody>().mass;

            float impulseDamageProportion = 1 - ((ourmass)/(ourmass+theirmass));
            float rammingMultiplier = 1f;

            if(other.gameObject.GetComponentInParent<Module>()!=null){
                rammingMultiplier = other.gameObject.GetComponentInParent<Module>().rammingDamageMultiplier;
            }

            if(rammingExplosion!=null){
                GameObject ram = Instantiate(rammingExplosion, other.contacts[0].point, Quaternion.identity);
                Destroy(ram, 2f);
            }
            float dmg = impulseDamageProportion*collisionDamagePerNewtonSecond*impulseChange.magnitude*rammingMultiplier;
            if(dmg>maxDamageFromRam)dmg = maxDamageFromRam;
           applyDamage(dmg);
            StartCoroutine(delayedImpact(impulseDamageProportion*collisionDamagePerNewtonSecond*impulseChange.magnitude*rammingMultiplier));
            
            
        }
    }
    protected IEnumerator delayedImpact (float dmg){
        yield return new WaitForFixedUpdate();
        /*if(dmg>maxDamageFromRam)dmg = maxDamageFromRam;
        applyDamage(dmg);*/
    }

    new void Start(){
        base.Start();
        myShip = GetComponent<Ship>();
        // display current health on ui
        shipControlInterface = myShip.controlUI.GetComponent<ControlInterface>();
        shipControlInterface.setHealthBar(hitpoints, maxHitpoints);
    }

    public override void applyDamage(float damage)
    {
        base.applyDamage(damage);



        shipControlInterface.setHealthBar(hitpoints, maxHitpoints);
    }
    bool dead = false;
    public override void die(){
        if(FindObjectOfType<dataToAddToPersistFolder>()!=null && !dead){
            dead = true;
            dataToAddToPersistFolder persistUpdater = FindObjectOfType<dataToAddToPersistFolder>();
            persistUpdater.confirmKill(gameObject);
            //if(bountyCredits>0) persistUpdater.addCreditsToTotal(bountyCredits);
        }
        if(shipBody!=null){
            
            // we need to create a corpse object. Do do this, select the ship object and create a duplicate of it
            corpseInstance = shipBody;
            // remove all monobehaviours from this duplicate
            foreach (var script in gameObject.GetComponentsInChildren<Component>())
            {
                if ((script is MonoBehaviour))
                {
                    Destroy(script);
                }
            }
            // give a rigidbody to the duplicate. Give it the properties of the previous ship we just destroyed
            Rigidbody m_rb = GetComponent<Rigidbody>();
            Rigidbody c_rb = corpseInstance.AddComponent<Rigidbody>();
            c_rb.mass = m_rb.mass;
            c_rb.constraints = RigidbodyConstraints.None;
            c_rb.drag = m_rb.drag;
            c_rb.angularDrag = m_rb.angularDrag;
            c_rb.maxAngularVelocity = m_rb.maxAngularVelocity;
            
            c_rb.useGravity = false;
            c_rb.interpolation = RigidbodyInterpolation.Interpolate;
            //c_rb.velocity = m_rb.velocity;
            //c_rb.AddVelocity(c_rb.velocity);
            
            
            
        
            
            

            corpseInstance = Instantiate(corpseInstance, transform.position, transform.rotation);
            corpseInstance.GetComponent<Rigidbody>().AddForce(m_rb.velocity, ForceMode.VelocityChange);
           
            corpseInstance.transform.localScale = transform.lossyScale;
            // instantiate a random number of hull explosions
            CorpseScript c = corpseInstance.AddComponent<CorpseScript>() as CorpseScript;
            c.finalExplodePrefab = finalExplodePrefab;
            c.deathsounds = deathsounds;
            c.explosionSounds = explosionSounds;
            c.begin(hullbreachRayMask, destructionDuration, maxHullExplosions, minHullExplosions, hullbreachPrefabs);
            

            // extra

            // if it has modules, select a module at random and detach it from parent

                // foreach module, detatch a random amount of objects and give them rigidbodies

            // i
            Destroy(gameObject);
        }
        else{
            GameObject go = Instantiate(corpse, transform.position, transform.rotation);
            go.transform.localScale = transform.localScale;
        }
        Destroy(gameObject);
    }
    public void createCorpseHullBreach(){
        float range = 70f;
        // to do this, instantiate a gameobject x units away from the hull and at a random rotation
        GameObject dummy = new GameObject();
        dummy.transform.position = transform.position;
        
        //  create a dummy gameobject and set a random rotation. Instantiate gameobject at x units of normalised transform.forward
        dummy.transform.rotation = Random.rotation;
        GameObject raycaster = Instantiate(new GameObject(), dummy.transform.position + dummy.transform.forward * range, Quaternion.identity);
        
        //  make this object look at transform. Do a raycast from it (ignoring all but default) and get the contact point
        raycaster.transform.LookAt(transform);
        Ray ray = new Ray(raycaster.transform.position, raycaster.transform.forward);
                RaycastHit hit; //From camera to hitpoint, not as curent
                Transform hitTransform;
                Vector3 hitVector;
                hitTransform = FindClosestHitObject(ray, out hitVector);


                Physics.Raycast(ray.origin, ray.direction, out hit, range*2, hullbreachRayMask);
        
        //  instantiate an explosion at this position
        
        GameObject hullbreach = Instantiate(hullbreachPrefabs[Random.Range(0, hullbreachPrefabs.Count-1)]); 
        hullbreach.transform.parent = corpseInstance.transform;

        
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
