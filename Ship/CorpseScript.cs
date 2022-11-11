using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseScript : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask hullbreachRayMask;
    public float destructionDuration = 5f;

    public int maxHullExplosions = 10;
    public int minHullExplosions = 5;

    public List<GameObject> hullbreachPrefabs = new List<GameObject>();
    List<GameObject> breaches = new List<GameObject>();

    public List<AudioClip> deathsounds = new List<AudioClip>();
    public List<AudioClip> explosionSounds = new List<AudioClip>();
    public GameObject finalExplodePrefab;
    public bool recursivelyRemoveChildren = false;

    public void begin(LayerMask mask, float dur, int maxExp, int minExp, List<GameObject> breachtypes){
        Debug.Log("begin");
        playDeathSound();
        hullbreachRayMask = mask;
        destructionDuration = dur;
        maxHullExplosions = maxExp;
        minHullExplosions = minExp;
        hullbreachPrefabs = breachtypes;
        int hullExplosionCount = Random.Range(minHullExplosions, maxHullExplosions);
        for(int i = 0; i< hullExplosionCount; i++){
            Invoke("createCorpseHullBreach", (destructionDuration*i)/hullExplosionCount);
        }
        Invoke("explode", destructionDuration);
        
    }
    void removeChildRecursive(Transform parent){
        foreach(Transform child in parent){
            if(child.gameObject.GetComponent<ShieldHealth>() != null) Destroy(child.gameObject);
            child.parent = null;
            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.mass = 100f;
            rb.maxAngularVelocity = 1f;
            TimedObjectDestructor t = child.gameObject.AddComponent<TimedObjectDestructor>() as TimedObjectDestructor;
            rb.useGravity = false;
            
            if(child.gameObject.GetComponent<Collider>() == null) child.gameObject.AddComponent<SphereCollider>();
            if(child.childCount > 0) removeChildRecursive(child);
        }
    }
    void explode(){
        
        if(!recursivelyRemoveChildren){
            foreach(Transform child in transform){
                child.parent = null;
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.mass = 100f;
                rb.maxAngularVelocity = 1f;
                TimedObjectDestructor t = child.gameObject.AddComponent<TimedObjectDestructor>() as TimedObjectDestructor;
                rb.useGravity = false;
                if(child.gameObject.GetComponent<Collider>() == null) child.gameObject.AddComponent<SphereCollider>();
            }
        }
        else if(recursivelyRemoveChildren){
            removeChildRecursive(transform);
        }
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, 100);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(3000, explosionPos, 1000, 1F);
        }
        if(finalExplodePrefab) Instantiate(finalExplodePrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    void playDeathSound(){
        GameObject fire = Instantiate(new GameObject(), transform.position, Quaternion.identity);
        AudioSource a = fire.AddComponent<AudioSource>();
        a.spatialBlend = 1f;


        a.dopplerLevel = 0f; 
        a.rolloffMode = AudioRolloffMode.Logarithmic;
        a.maxDistance = 2000f;
        a.minDistance = 600f;
        int index = Random.RandomRange(0, deathsounds.Count -1);
        a.PlayOneShot(deathsounds[index], 1f);
        fire.AddComponent<TimedObjectDestructor>();
    }
    void playExplosionSound(){
        GameObject fire = Instantiate(new GameObject(), transform.position, Quaternion.identity);
        AudioSource a = fire.AddComponent<AudioSource>();
        a.spatialBlend = 1f;


        a.dopplerLevel = 0f; 
        a.rolloffMode = AudioRolloffMode.Logarithmic;
        a.maxDistance = 2000f;
        a.minDistance = 600f;
        int index = Random.RandomRange(0, explosionSounds.Count -1);
        a.PlayOneShot(explosionSounds[index], 1f);
        fire.AddComponent<TimedObjectDestructor>();
    }
    public void createCorpseHullBreach(){
        playExplosionSound();
        Debug.Log("breaching");
        float range = 150f;
        // to do this, instantiate a gameobject x units away from the hull and at a random rotation
        GameObject dummy = new GameObject();
        dummy.transform.position = transform.position;
        
        //  create a dummy gameobject and set a random rotation. Instantiate gameobject at x units of normalised transform.forward
        dummy.transform.rotation = Random.rotation;
        GameObject raycaster = Instantiate(new GameObject(), dummy.transform.position + dummy.transform.forward * range, Quaternion.identity);
        breaches.Add(raycaster);
        //  make this object look at transform. Do a raycast from it (ignoring all but default) and get the contact point
        raycaster.transform.LookAt(transform);
        Ray ray = new Ray(raycaster.transform.position, raycaster.transform.forward);
                RaycastHit hit; //From camera to hitpoint, not as curent
                Transform hitTransform;
                Vector3 hitVector;
                hitTransform = FindClosestHitObject(ray, out hitVector);


                Physics.Raycast(ray.origin, ray.direction, out hit, range*2, hullbreachRayMask);
        
        //  instantiate an explosion at this position
        
        GameObject hullbreach = Instantiate(hullbreachPrefabs[Random.Range(0, hullbreachPrefabs.Count-1)], hitVector, Quaternion.identity); 
        hullbreach.transform.LookAt(raycaster.transform);
        hullbreach.transform.parent = transform;
        breaches.Add(hullbreach);

        
    }

    protected Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint)
    {

        RaycastHit[] hits = Physics.RaycastAll(ray);

        Transform closestHit = null;
        float distance = 0;
        hitPoint = Vector3.zero;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.root == this.transform && (closestHit == null || hit.distance < distance))
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
