using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiClassBase : StateMachineBehaviour
{
    // Start is called before the first frame update
    float maxrangeCollisionTest = 100;
    float closeRangeCollisionTest = 50;

    float r = 0;
    float maxCrashWait = 2f;
    float minCrashWait = 2f;
    protected string nextState = "nextState";
    protected string targetFound = "TargetFound";
    protected string LowHealth = "LowHealth";

    protected string closeGap = "closeGap";
    protected CapitalShipAi sensorManager;
    protected CaptialShipControl controls;

    protected AiBehaviourDictionary behaviourDictionary;

    protected OrderManager orderManager;

    float collisionCooldown = 1f;
    float currentCooldown = 1f;

    bool checkResult = false;

    protected bool performCollisionCheckNoTeams(Transform myShip){
        bool maycrash = false;
                // check if we are going to crash
                Transform front = myShip.gameObject.GetComponentInParent<CaptialShipControl>().front;

                RaycastHit hit;
                Transform hitTransform = myShip;
                // get the first object that is not us
                if (Physics.Raycast(front.position, front.forward, out hit, maxrangeCollisionTest))
                {
                    hitTransform = hit.transform;
                }
                // overlap stuff
                if(hitTransform == myShip){
                    // now test the overlap
                    
        
                    Collider[] colliders = Physics.OverlapSphere(front.position, r);
                    Debug.Log("colliders" + colliders.ToString());
                    bool done = false;
                    foreach (Collider col in colliders){
                    //    Debug.Log(col.ToString());
                        Transform test = col.transform;
                        if(!done){
                            if(!test.IsChildOf(myShip)){
                                Vector3 overlap = col.ClosestPointOnBounds(front.position);
                            //  Gizmos.DrawWireSphere(overlap, 0.1f);
                                if(Vector3.Angle(front.forward ,overlap -front.position) < 180){
                                    Debug.Log("collider detection2");
                                    if(crashDetectNoTeam(test, myShip, overlap)){
                                        done = true;
                                    }
                                }
                                
                            }

                        }
                        
                    }
                }
                // ray stuff
                if(hitTransform != myShip){
                    maycrash = crashDetect(hitTransform, myShip, hit.point);

                }
                if(maycrash)Debug.Log("MAY CRASH!!!!!!");
                return maycrash;
    }
    protected bool performCollisionCheck(Transform myShip){
        bool maycrash = false;
                // check if we are going to crash
                Transform front = myShip.gameObject.GetComponentInParent<CaptialShipControl>().front;

                RaycastHit hit;
                Transform hitTransform = myShip;
                // get the first object that is not us
                if (Physics.Raycast(front.position, front.forward, out hit, maxrangeCollisionTest))
                {
                    hitTransform = hit.transform;
                }
                // overlap stuff
                if(hitTransform == myShip){
                    // now test the overlap
                    
        
                   /* Collider[] colliders = Physics.OverlapSphere(front.position, r);
                    Debug.Log("colliders" + colliders.ToString());
                    bool done = false;
                    foreach (Collider col in colliders){
                    //    Debug.Log(col.ToString());
                        Transform test = col.transform;
                        if(!done){
                            if(!test.IsChildOf(myShip)){
                                Vector3 overlap = col.ClosestPointOnBounds(front.position);
                            //  Gizmos.DrawWireSphere(overlap, 0.1f);
                                if(Vector3.Angle(front.forward ,overlap -front.position) < 180){
                                    Debug.Log("collider detection2");
                                    if(crashDetect(test, myShip, overlap)){
                                        done = true;
                                    }
                                }
                                
                            }

                        }
                        
                    }*/
                }
                // ray stuff
                if(hitTransform != myShip){
                    maycrash = crashDetect(hitTransform, myShip, hit.point);

                }
                if(maycrash)Debug.Log("MAY CRASH!!!!!!");
                return maycrash;
    }
    protected bool collisionCheck(Transform myShip){
     //  currentCooldown -= Time.deltaTime;
     //  if(currentCooldown <= 0){
           checkResult = performCollisionCheck(myShip);
     //      currentCooldown = collisionCooldown;
     //  }
       return checkResult;
    }

     bool crashDetectNoTeam(Transform hitTransform, Transform myShip, Vector3 hitPoint){
           bool maycrash = false;
                    // If the object is a nearby friendly ship, then stop and wait a random amount of time for them to pass
            Ship s = hitTransform.GetComponentInParent<Ship>();
            Health h = hitTransform.GetComponentInParent<Health>();
            if(s != null){
                
                    if(Vector3.Distance(hitPoint, myShip.position) < closeRangeCollisionTest){
                        // adjust heading
                        behaviourDictionary.randomWait(maxCrashWait);
                       // Vector3 turnPos = myShip.position + myShip.transform.right * 100;
                      //  behaviourDictionary.emergencyTurnStart(3f, myShip.gameObject, turnPos);
                        maycrash = true;
                    }
                    else if(Vector3.Distance(hitPoint, myShip.position) < maxrangeCollisionTest){
                        maycrash = true;
                        behaviourDictionary.rotateToFaceVectorNoCallback(myShip.position + myShip.transform.right  + myShip.transform.forward, myShip.gameObject, 1f);
                    }
            }
            else if(s == null && h == null){
            // If the object is static and far, adjust heading 
                if(Vector3.Distance(hitPoint, myShip.position) < closeRangeCollisionTest){
                    Vector3 turnPos = myShip.position + myShip.transform.right * 100;
                    float angleH = Vector3.SignedAngle(myShip.forward, (hitPoint - myShip.position), myShip.up);
                    float angleV = Vector3.SignedAngle(myShip.forward, (hitPoint - myShip.position), myShip.right);
                    // highest magnitude angle
                    if(Mathf.Abs(angleH) > Mathf.Abs(angleV)){
                        // if the hitpoint is to your left
                         if(angleH < 0) turnPos = myShip.position + myShip.transform.right * 100;
                        // right
                        if(angleH > 0) turnPos = myShip.position + myShip.transform.right * -100;
                    }
                    else{
                        // above 
                        if(angleV > 0) turnPos = myShip.position + myShip.transform.up * -100;

                        // below
                        if(angleV < 0) turnPos = myShip.position + myShip.transform.up * 100 ;
                    }
                    behaviourDictionary.randomWait(Random.Range(0f, 1f));
                    behaviourDictionary.rotateToFaceVectorNoCallback(turnPos, myShip.gameObject, 1f);
                    //behaviourDictionary.emergencyTurnStart(5f, myShip.gameObject, turnPos);
                    maycrash = true;
                }
                else if(Vector3.Distance(hitPoint, myShip.position) > closeRangeCollisionTest){
                    
                    behaviourDictionary.rotateToFaceVectorNoCallback(myShip.position + myShip.transform.right  + myShip.transform.forward, myShip.gameObject, 0.5f);
                    maycrash = true;
                }
            }
            return maycrash;
    }

    bool crashDetect(Transform hitTransform, Transform myShip, Vector3 hitPoint){
           bool maycrash = false;
                    // If the object is a nearby friendly ship, then stop and wait a random amount of time for them to pass
            Ship s = hitTransform.GetComponentInParent<Ship>();
            Health h = hitTransform.GetComponentInParent<Health>();
            /* if(s != null){
                if(s.teamId == myShip.gameObject.GetComponent<Ship>().teamId){
                    if(Vector3.Distance(hitPoint, myShip.position) < closeRangeCollisionTest){
                        // adjust heading
                        behaviourDictionary.randomWait(maxCrashWait);
                       // Vector3 turnPos = myShip.position + myShip.transform.right * 100;
                      //  behaviourDictionary.emergencyTurnStart(3f, myShip.gameObject, turnPos);
                        maycrash = true;
                    }
                    else if(Vector3.Distance(hitPoint, myShip.position) < maxrangeCollisionTest){
                        maycrash = true;
                      //  behaviourDictionary.rotateToFaceVectorNoCallback(myShip.position + myShip.transform.right  + myShip.transform.forward, myShip.gameObject, 1f);
                       // behaviourDictionary.MoveRotationTorque(myShip.GetComponent<Rigidbody>(), myShip.position + myShip.transform.right * 100, 10f, this);
                    }
                }
            }*/
            if(s == null && h == null){
            // If the object is static and far, adjust heading 
                if(Vector3.Distance(hitPoint, myShip.position) < closeRangeCollisionTest){
                    Vector3 turnPos = myShip.position + myShip.transform.right * 100;
                    float angleH = Vector3.SignedAngle(myShip.forward, (hitPoint - myShip.position), myShip.up);
                    float angleV = Vector3.SignedAngle(myShip.forward, (hitPoint - myShip.position), myShip.right);
                    // highest magnitude angle
                    if(Mathf.Abs(angleH) > Mathf.Abs(angleV)){
                        // if the hitpoint is to your left
                         if(angleH < 0) turnPos = myShip.position + myShip.transform.right * 100;
                        // right
                        if(angleH > 0) turnPos = myShip.position + myShip.transform.right * -100;
                    }
                    else{
                        // above 
                        if(angleV > 0) turnPos = myShip.position + myShip.transform.up * -100;

                        // below
                        if(angleV < 0) turnPos = myShip.position + myShip.transform.up * 100 ;
                    }
                    behaviourDictionary.randomWait(Random.Range(1f, 1f));
                    behaviourDictionary.MoveRotationTorqueNoCallback(myShip.GetComponent<Rigidbody>(), turnPos, 3f);
                    //behaviourDictionary.emergencyTurnStart(5f, myShip.gameObject, turnPos);
                    maycrash = true;
                }
                else if(Vector3.Distance(hitPoint, myShip.position) > closeRangeCollisionTest){
                    Vector3 turnPos = myShip.position + myShip.transform.right * 100;
                    float angleH = Vector3.SignedAngle(myShip.forward, (hitPoint - myShip.position), myShip.up);
                    float angleV = Vector3.SignedAngle(myShip.forward, (hitPoint - myShip.position), myShip.right);
                    // highest magnitude angle
                    if(Mathf.Abs(angleH) > Mathf.Abs(angleV)){
                        // if the hitpoint is to your left
                         if(angleH < 0) turnPos = myShip.position + myShip.transform.right * 100;
                        // right
                        if(angleH > 0) turnPos = myShip.position + myShip.transform.right * -100;
                    }
                    else{
                        // above 
                        if(angleV > 0) turnPos = myShip.position + myShip.transform.up * -100;

                        // below
                        if(angleV < 0) turnPos = myShip.position + myShip.transform.up * 100 ;
                    }
                    behaviourDictionary.randomWait(Random.Range(1f, 1f));
                    behaviourDictionary.MoveRotationTorqueNoCallback(myShip.GetComponent<Rigidbody>(), turnPos, 3f);
                    //behaviourDictionary.emergencyTurnStart(5f, myShip.gameObject, turnPos);
                    maycrash = true;
                }
            }
            return maycrash;
    }

        
    protected int step = 0;

    public void completedAction(){
        step += 1;
    }
    
}