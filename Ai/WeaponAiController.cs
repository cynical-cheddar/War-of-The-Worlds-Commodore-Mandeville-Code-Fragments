using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAiController : MonoBehaviour
{
    // Start is called before the first frame update

    bool userSpecifiedTarget = false;

    public bool autoReseektarget = true;
    public Weapon controlledWeapon;
    List<Transform> enemyList = new List<Transform>();
    public List<Transform> targetList = new List<Transform>();

    public List<Ship.ShipClass> targetPriority = new List<Ship.ShipClass>();

    public bool fireControl = true;

    int myTeam;
    protected float weaponRange = 400f;

    public float fireCone = 360f;

    public Transform masterFirePoint;

    public Transform target;
    public float updateCooldown = 3f;
    protected float upd = 3f;

    float targetExpiry = 0f;

    CapitalShipAi sensorsManager;

    protected Quaternion originalFirepointRotation = Quaternion.identity;

    bool friendlyTarget = false;

    public void forgetTarget(){
        //userSpecifiedTarget = false;
        target = null;
        enemyList.Clear();
        targetList.Clear();
        GetComponent<Weapon>().autoControlled = false;
    }
    public void userSetTarget(Transform targetLocal){
        userSpecifiedTarget = true;
        target = targetLocal;
    }
    public void userSetTarget(Transform targetLocal, float duration){
        userSpecifiedTarget = true;
        target = targetLocal;
        targetExpiry = duration;
        if(targetLocal.gameObject.GetComponent<Ship>().teamId == myTeam) friendlyTarget = true;
        else friendlyTarget = false;
    }
    public Vector3 calculateFireControl(Transform targetTransform, float muzzleVelocity, Vector3 firePoint){
        Vector3 newPos = targetTransform.position;
        Vector3 targetVel = targetTransform.gameObject.GetComponent<Rigidbody>().velocity;
        float estimatedTime = Vector3.Magnitude(targetTransform.position - firePoint) / muzzleVelocity;
        newPos += targetVel * estimatedTime;



        return newPos;
    }

    protected void Start()
    {
        sensorsManager = GetComponentInParent<CapitalShipAi>();
        originalFirepointRotation = masterFirePoint.rotation;
        controlledWeapon = gameObject.GetComponent<Weapon>();
        if(gameObject.GetComponentInParent<Ship>()) myTeam = gameObject.GetComponentInParent<Ship>().teamId;
        updateCooldown = updateCooldown + Random.Range(-updateCooldown/5, updateCooldown/5);
        upd = updateCooldown;

        getRangeFronEquipment();
        
        //if(FindObjectOfType<shipyard>() != null ) 
    }

    void getRangeFronEquipment(){
        
        switch (controlledWeapon.standardisedRangeSetting){
            case Equipment.standardisedRange.Short: weaponRange = controlledWeapon.shortRange; break;
            case Equipment.standardisedRange.Medium: weaponRange = controlledWeapon.mediumRange; break;
            case Equipment.standardisedRange.Long: weaponRange = controlledWeapon.longRange; break;
            case Equipment.standardisedRange.Artillery: weaponRange = controlledWeapon.artilleryRange; break;
            case Equipment.standardisedRange.Infinite: weaponRange = controlledWeapon.InfiniteRange; break;
            default: weaponRange = controlledWeapon.shortRange; break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(targetExpiry > 0){
            targetExpiry -= Time.deltaTime;
            if(targetExpiry <= 0 && !userSpecifiedTarget) forgetTarget();
        }
        // if the user has specified a target, but it's either out of range or dead, then go back to auto targeting
        


        // only let the turret act autonomously if it is auto controlled
        // allows specific weapons to be fired in the main ai script
        

        
    }
    public void weaponAiUpdateMethod(){
        if(controlledWeapon.autoControlled){
            updateEnemyList();
            updateTargetList();
            if(target == null){
                
                if(!userSpecifiedTarget)selectTarget();  
                if(userSpecifiedTarget && autoReseektarget){
                
                  if((target == null || !targetList.Contains(target)) && !friendlyTarget){
                      userSpecifiedTarget = false;
                      selectTarget();
                  }
                  else if((target == null || !targetList.Contains(target)) && friendlyTarget){
                      
                  }
                }
               
            }
            else if(target != null){
                float dist = Vector3.Distance(target.position, transform.position);
                if(dist > weaponRange) forgetTarget();
                // if we still have a target, then reassess priority
                if(target!=null && !userSpecifiedTarget){
                    target = getTargetByClass(targetList);
                }
                
            }
        }
    }
    
    public void updateLists(){
        // get a list of enemies
        updateEnemyList();
        updateTargetList();
               
        // leave the rest up to the derived class
    }

    void updateEnemyList(){
        enemyList.Clear();
        // get all enemies from the ship's sensors
        

        if(sensorsManager.targetList.Count > 0){
            
            foreach(Transform s in sensorsManager.targetList){
                if(s!=null){
                    if(s.gameObject.GetComponent<Ship>().teamId != myTeam){
                        enemyList.Add(s);
                    }
                }
            }
        }
    }
    void updateTargetList(){
        targetList.Clear();
        foreach(Transform enemy in enemyList){
            float dist = Vector3.Distance(enemy.position, transform.position);
            // calculate dot product of our rotation vector and their position
            float targetAngle = Vector3.SignedAngle(transform.forward, (enemy.position - transform.position ), transform.up);
            if(dist < weaponRange && Mathf.Abs(targetAngle) < fireCone) targetList.Add(enemy);
        }
    }

    void selectTarget(){
        // arbritraily choose first target for now
        target = null;
        if(targetList.Count > 0 && autoReseektarget){
            // get the biggest enemy 
            target = getTargetByClass(targetList);

        }
        
    }

    Transform getTargetByClass(List<Transform> enemies){
        Transform tMin = null;
        List<Transform> potentialTargets = new List<Transform>();
        bool foundclass = false;

            // make a pass through each shiptype to get priority
            foreach(Ship.ShipClass c in targetPriority){
               foreach (Transform t in enemies)
                 {
                    Ship s = t.gameObject.GetComponent<Ship>();
                    if(s.shipClass == c){
                        foundclass = true;
                        potentialTargets.Add(t);
                    }
                }
                if(foundclass) break;
            }

            
           return GetClosestEnemy(potentialTargets);
        }

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
}
