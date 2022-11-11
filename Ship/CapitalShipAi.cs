using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalShipAi : MonoBehaviour
{
    //public bool issueOrders = true;
    int myTeam;
    CaptialShipControl shipControls;
    public WeaponAiController[] weaponList;
    List<Transform> enemyList = new List<Transform>();
    public List<Transform> targetList = new List<Transform>();

    float cooldown = 5f;
    float curCooldown = 5f;
    public float sensorRange = 200f;

    public bool targetSighted = false;

    Transform currentTarget;

    bool customTarget = false;
    Transform customTargetTransform;

    float customtargetDuration = 10f;
    float customTargetAssignTime = 0f;

    float updCooldown = 3f;

    public bool actuallyAiControlled = true; 

    float shortRange;
    float midRange;
    float longrange;

    public void setCustomTarget(Transform target, float duration){
        customtargetDuration = duration;
        customTargetAssignTime = Time.time;
        customTarget = true;
        customTargetTransform = target;
        currentTarget = target;
    }
    // Start is called before the first frame update
    Transform GetClosestEnemy(List<Transform> enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in enemies)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }
    public Transform getSingularEnemy(){
        Transform target = GetClosestEnemy(targetList);
        return target;
    }
    public Transform getCurrentTarget(){
        if(currentTarget==null)currentTarget = getSingularEnemy();
        return currentTarget;
    }
    void setCurrentTarget(Transform targetObject){
        currentTarget = targetObject;
    }
    void Start()
    {
        updCooldown = cooldown + Random.Range(-cooldown/5, cooldown/5);
        shipControls = GetComponent<CaptialShipControl>();
        myTeam = shipControls.teamId;
        // get default values from equipment;
        GameObject obj = Instantiate(new GameObject());
        Equipment e = obj.AddComponent<Equipment>();
        shortRange = e.shortRange;
        midRange = e.mediumRange;
        longrange = e.longRange;
        Destroy(obj);

        // that is janky but fine ^

        updateWeaponList();
        updateEnemyList();
        updateTargetList();
        StartCoroutine(sensorsUpdateLoop());
    }
    void updateWeaponList(){
        weaponList = GetComponentsInChildren<WeaponAiController>();
        foreach (WeaponAiController w in weaponList){
            w.weaponAiUpdateMethod();
            Debug.Log("Updated weapon ai: "+ w.ToString());
        }
    }
    void updateTargetPriority(){
        if(!customTarget && actuallyAiControlled && targetList.Count>0){
            // find a target that is both near to us and also rather weak (lowest health capital ship)

            List<Transform> shortMidtargets = new List<Transform>();
            List<Transform> zeroShortTargets = new List<Transform>();
            List<Transform> longRangeTargets = new List<Transform>();
            foreach(Transform potentialTarget in targetList){
                float dist = Vector3.Distance(transform.position, potentialTarget.position);
                if(dist > shortRange && dist < midRange){
                    shortMidtargets.Add(potentialTarget);
                }
                else if(dist < shortRange){
                    zeroShortTargets.Add(potentialTarget);
                }
                else if(dist > midRange){
                    longRangeTargets.Add(potentialTarget);
                }
            }

            // priority based on weapon ranges, prioritise short-mid, then 0-short, then mid-long
            // this way you cannot tie up ships

            // get all enemies in this band and pick the one with the lowest health
            if(shortMidtargets.Count>0){
                // select the target arbitaryily for now
                setCurrentTarget(shortMidtargets[0]);
            }
            else if(zeroShortTargets.Count>0){
                // select the target arbitaryily for now
                setCurrentTarget(zeroShortTargets[0]);
            }
            else if(longRangeTargets.Count>0){
                // select the target arbitaryily for now
                setCurrentTarget(longRangeTargets[0]);
            }
        }
    }
    void updateEnemyList(){
        enemyList.Clear();
        if(GameObject.FindObjectOfType<Ship>() != null){
            Ship[] ships = GameObject.FindObjectsOfType<Ship>();
            
            foreach(Ship s in ships){
                if(s.teamId != myTeam){
                    enemyList.Add(s.transform);
                }
            }
        }
    }
    void updateTargetList(){
        targetList.Clear();
        foreach(Transform enemy in enemyList){
            if(Vector3.Distance(enemy.position, transform.position) < sensorRange) targetList.Add(enemy);
        }
    }
    IEnumerator sensorsUpdateLoop(){
        while (true){
            
            yield return new WaitForSeconds(updCooldown);
            updateWeaponList();
            if(actuallyAiControlled) updateTargetPriority();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(customTarget){
            if(Time.time - customTargetAssignTime > customtargetDuration){
                // forget our target and find a new one 
                customTarget = false;
            }
        }
        if(!customTarget){
            curCooldown-=Time.deltaTime;
            if(curCooldown<0){
                updateEnemyList();
                updateTargetList();
                updateWeaponList();
                if(targetList.Count > 0) targetSighted = true;
                curCooldown  = cooldown;
            }
            
        }
    }
}
