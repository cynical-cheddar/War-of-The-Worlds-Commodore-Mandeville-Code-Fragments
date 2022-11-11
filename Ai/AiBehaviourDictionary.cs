using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBehaviourDictionary : MonoBehaviour
{
    protected CaptialShipControl movementScript;
    public float angleTolerance = 1f;
    public float movementThreshold = 10f;
 
    bool waiting = false;
    bool emergencyTurning = false;

    int movingState = 0;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        movementScript = GetComponent<CaptialShipControl>();
    }


    /*  public IEnumerator rotateToFaceVectorCoroutine(Vector3 targetPosition, GameObject ship, AiClassBase callback, float fractionRotationSpeed){
        Transform shipTransform = ship.transform;
        float angleDifferenceH = Vector3.SignedAngle(shipTransform.forward, targetPosition - shipTransform.position , shipTransform.up);
    
        // Distance to rotate vertically
        float angleDifferenceV = Vector3.SignedAngle(shipTransform.forward, targetPosition - shipTransform.position , shipTransform.right);

        while(Mathf.Abs(angleDifferenceH) > angleTolerance && Mathf.Abs(angleDifferenceV) > angleTolerance){
            rotateToFaceVector(targetPosition, ship, fractionRotationSpeed);
            yield return new WaitForFixedUpdate();
        }
        callback.completedAction();
        yield return null;   
        
    }*/
   /* public IEnumerator moveToPositionCoroutine(Vector3 position, GameObject ship, AiClassBase callback, float fractionRotationSpeed){
        
        if(Vector3.Magnitude(position - ship.transform.position) > movementThreshold){
            rotateToFaceVector(position, ship, fractionRotationSpeed);
            yield return new WaitForFixedUpdate();
        }
        else{
            callback.completedAction();
            yield return true;
        }
        
        
    }*/







    public void randomWait(float maxWait){
        if(!waiting){
            waiting = true;
            float waitTime = Random.Range(0, maxWait);
            float currentSpeed = movementScript.curFraction;
            setSpeed(0);
            StartCoroutine(resumeEngines(waitTime, currentSpeed));
        }
        
    }
    IEnumerator resumeEngines(float waitTime, float currentSpeed)
    {
        yield return new WaitForSeconds(waitTime);
        movementScript.setSpeed(currentSpeed);
        waiting = false;
    }
    public void emergencyTurnStart(float waitTime, GameObject ship,Vector3 position){
        if(!emergencyTurning){
            emergencyTurning = true;
            StartCoroutine(emergencyTurn(waitTime, ship, position));
        }
        
    }
    IEnumerator emergencyTurn(float waitTime, GameObject ship,Vector3 position)
    {
        while(waitTime > 0){
            rotateToFaceVectorNoCallback(position, ship, 1f);
            waitTime -= Time.deltaTime;
            yield return 0;
        }
        emergencyTurning = false;
    }

    public void moveToTransform(Transform target, GameObject ship, float fractionRotationSpeed, float proximityThreshold, AiClassBase callback){
        moveToPosition(target.position, ship, fractionRotationSpeed, proximityThreshold, callback);
    }

    public void moveToTransform(Transform target, GameObject ship, float fractionRotationSpeed, AiClassBase callback){
        moveToPosition(target.position, ship, fractionRotationSpeed, callback);
    }

    public void moveToPosition(Vector3 position, GameObject ship, float fractionRotationSpeed, float proximityThreshold , AiClassBase callback){
     //   rotateToFaceVectorNoCallback(position, ship, fractionRotationSpeed);
        MoveRotationTorqueNoCallback(ship.GetComponent<Rigidbody>(), position, 1f);
        if(Vector3.Magnitude(position - ship.transform.position) < proximityThreshold){
            callback.completedAction();
        }
        //
    }
    public void moveToPosition(Vector3 position, GameObject ship, float fractionRotationSpeed, AiClassBase callback){
        rotateToFaceVectorNoCallback(position, ship, fractionRotationSpeed);
        if(Vector3.Magnitude(position - ship.transform.position) < movementThreshold){
            callback.completedAction();
        }
        //
    }
    public void setSpeed(float fraction){
        movementScript.setSpeed(fraction);
    }
    public void setAheadFull(bool set){
        movementScript.setAheadFullEngaged(set);
    }

    public void MoveRotationTorque(Rigidbody rigidbody, Vector3 targetpos, float maxRate ,AiClassBase callback)
     {
         Quaternion targetRotation = Quaternion.LookRotation((targetpos -  rigidbody.transform.position ) , rigidbody.transform.up);
         rigidbody.maxAngularVelocity = 1000;
 
         Quaternion rotation = targetRotation * Quaternion.Inverse(rigidbody.rotation);
         var torque = new Vector3(rotation.x, rotation.y, rotation.z) * rotation.w * Time.fixedDeltaTime * maxRate;

         rigidbody.AddTorque(torque, ForceMode.VelocityChange);
        // rigidbody.angularVelocity = Vector3.zero;
         rigidbody.SetMaxAngularVelocity(1f);


         float angleDifferenceH = Vector3.SignedAngle(rigidbody.transform.forward, targetpos - rigidbody.transform.position , rigidbody.transform.up);
    
        // Distance to rotate vertically
        float angleDifferenceV = Vector3.SignedAngle(rigidbody.transform.forward, targetpos - rigidbody.transform.position , rigidbody.transform.right);

        

         if(Mathf.Abs(angleDifferenceH) < angleTolerance && Mathf.Abs(angleDifferenceV) <angleTolerance){
            callback.completedAction();
        }
     }
     public void MoveRotationTorqueNoCallback(Rigidbody rigidbody, Vector3 targetpos, float maxRate)
     {
         Quaternion targetRotation = Quaternion.LookRotation((targetpos -  rigidbody.transform.position ) , rigidbody.transform.up);
         rigidbody.maxAngularVelocity = 1000;
 
         Quaternion rotation = targetRotation * Quaternion.Inverse(rigidbody.rotation);
         var torque = new Vector3(rotation.x, rotation.y, rotation.z) * rotation.w * Time.fixedDeltaTime * maxRate;

         rigidbody.AddTorque(torque, ForceMode.VelocityChange);
        // rigidbody.angularVelocity = Vector3.zero;
         rigidbody.SetMaxAngularVelocity(1f);


         float angleDifferenceH = Vector3.SignedAngle(rigidbody.transform.forward, targetpos - rigidbody.transform.position , rigidbody.transform.up);
    
        // Distance to rotate vertically
        float angleDifferenceV = Vector3.SignedAngle(rigidbody.transform.forward, targetpos - rigidbody.transform.position , rigidbody.transform.right);

        

        
     }

    public void rotateToFaceVectorNoCallback(Vector3 targetPosition, GameObject ship, float fraction){
        
        float ControlHorizontal = 0;
		float ControlVertical = 0;
        Transform shipTransform = ship.transform;
        // Get our relative rotation

        // Distance to rotate horizontally
        float angleDifferenceH = Vector3.SignedAngle(shipTransform.forward, targetPosition - shipTransform.position , shipTransform.up);
    
        // Distance to rotate vertically
        float angleDifferenceV = Vector3.SignedAngle(shipTransform.forward, targetPosition - shipTransform.position , shipTransform.right);

        // if the angle is positive, then rotate clockwise
        if(Mathf.Abs(angleDifferenceH) > angleTolerance){
            if(angleDifferenceH > 0) ControlHorizontal =  -fraction;
            if(angleDifferenceH < 0) ControlHorizontal = fraction;
        }
        if(Mathf.Abs(angleDifferenceV) > angleTolerance){
            if(angleDifferenceV > 0) ControlVertical = -fraction;
            if(angleDifferenceV < 0) ControlVertical = fraction;
        }
      //  Debug.Log("H " + ControlHorizontal + "V " + ControlVertical);
        movementScript.moveUser(ControlHorizontal, ControlVertical);
    }

  public void rotateToFaceVector(Vector3 targetPosition, GameObject ship, float fraction, AiClassBase callback, bool sequential){
        movementScript.zeroRoll();
        float ControlHorizontal = 0;
		float ControlVertical = 0;
        Transform shipTransform = ship.transform;
        // Get our relative rotation

        // Distance to rotate horizontally
        float angleDifferenceH = Vector3.SignedAngle(shipTransform.forward, targetPosition - shipTransform.position , shipTransform.up);
    
        // Distance to rotate vertically
        float angleDifferenceV = Vector3.SignedAngle(shipTransform.forward, targetPosition - shipTransform.position , shipTransform.right);

        bool verticalRotate = true;

        float myY = ship.transform.position.y;
        float theirY = targetPosition.y;

        if(Mathf.Abs(angleDifferenceH) > angleTolerance){
            verticalRotate = false;
        }

        if(Mathf.Abs(theirY - myY) < 10f){
            verticalRotate = false;
        }


        // if the angle is positive, then rotate clockwise
        if(Mathf.Abs(angleDifferenceH) > angleTolerance){
            if(angleDifferenceH > 0) ControlHorizontal =  -fraction;
            if(angleDifferenceH < 0) ControlHorizontal = fraction;
        }
        if(Mathf.Abs(angleDifferenceV) > angleTolerance && verticalRotate){
            if(angleDifferenceV > 0) ControlVertical = -fraction;
            if(angleDifferenceV < 0) ControlVertical = fraction;
        }
      //  Debug.Log("H " + ControlHorizontal + "V " + ControlVertical);
        movementScript.moveUser(ControlHorizontal, ControlVertical);
        if(Mathf.Abs(angleDifferenceH) < angleTolerance && Mathf.Abs(angleDifferenceV) < angleTolerance){
            callback.completedAction();
        }
    }

    public void rotateToFaceVector(Vector3 targetPosition, GameObject ship, float fraction, AiClassBase callback){
        // if we are too close, then disable engines
        
        movementScript.zeroRoll();
        float ControlHorizontal = 0;
		float ControlVertical = 0;
        Transform shipTransform = ship.transform;
        // Get our relative rotation

        // Distance to rotate horizontally
        float angleDifferenceH = Vector3.SignedAngle(shipTransform.forward, targetPosition - shipTransform.position , shipTransform.up);
    
        // Distance to rotate vertically
        float angleDifferenceV = Vector3.SignedAngle(shipTransform.forward, targetPosition - shipTransform.position , shipTransform.right);

        bool verticalRotate = true;

        float myY = ship.transform.position.y;
        float theirY = targetPosition.y;
        if(Mathf.Abs(theirY - myY) < 5f){
            verticalRotate = false;
        }


        // if the angle is positive, then rotate clockwise
        if(Mathf.Abs(angleDifferenceH) > angleTolerance){
            if(angleDifferenceH > 0) ControlHorizontal =  -fraction;
            if(angleDifferenceH < 0) ControlHorizontal = fraction;
        }
        if(Mathf.Abs(angleDifferenceV) > angleTolerance && verticalRotate){
            if(angleDifferenceV > 0) ControlVertical = -fraction;
            if(angleDifferenceV < 0) ControlVertical = fraction;
        }
      //  Debug.Log("H " + ControlHorizontal + "V " + ControlVertical);
        movementScript.moveUser(ControlHorizontal, ControlVertical);
        if(Mathf.Abs(angleDifferenceH) < angleTolerance && Mathf.Abs(angleDifferenceV) < 10f){
            callback.completedAction();
        }
    }
    public void matchVerticalPlane(Vector3 targetPosition, GameObject ship, float correctionSpeed, AiClassBase callback, float threshold){
        float myY = ship.transform.position.y;
        float theirY = targetPosition.y;
        if(Mathf.Abs(theirY - myY) < threshold){
            if(theirY > myY){
                // slowly move up
                ship.GetComponent<Rigidbody>().AddForce(correctionSpeed * Vector3.up, ForceMode.VelocityChange);
             }
             if(theirY < myY){
                 ship.GetComponent<Rigidbody>().AddForce(-correctionSpeed * Vector3.up, ForceMode.VelocityChange);
             }
        }
        else{
            callback.completedAction();
        }
    }
    public void matchVerticalPlaneNoCallback(Vector3 targetPosition, GameObject ship, float correctionSpeed, float threshold){
        float myY = ship.transform.position.y;
        float theirY = targetPosition.y;
        if(Mathf.Abs(theirY - myY) < threshold){
            if(theirY > myY){
                // slowly move up
               
                ship.GetComponent<Rigidbody>().AddForce(correctionSpeed * Vector3.up, ForceMode.VelocityChange);
             }
             if(theirY < myY){
                 ship.GetComponent<Rigidbody>().AddForce(-correctionSpeed * Vector3.up, ForceMode.VelocityChange);
             }
        }
    }

    public void matchTransformVerticalPlane(Transform target, GameObject ship, float correctionSpeed, AiClassBase callback, float threshold){
        matchVerticalPlane(target.position, ship, correctionSpeed, callback, threshold);
    }
    public void matchTransformVerticalPlaneNoCallback(Transform target, GameObject ship, float correctionSpeed, float threshold){
        matchVerticalPlaneNoCallback(target.position, ship, correctionSpeed, threshold);
    }

    public void rotateToFaceVectorBroadside(Vector3 targetPosition, GameObject ship, float fraction, AiClassBase callback){
        float ControlHorizontal = 0;
		float ControlVertical = 0;
        Transform shipTransform = ship.transform;
        // Get our relative rotation

        // Distance to rotate horizontally
        float angleDifferenceH = Vector3.SignedAngle(shipTransform.right, targetPosition - shipTransform.position , shipTransform.up);
    
        // Distance to rotate on roll
        float angleDifferenceR = Vector3.SignedAngle(shipTransform.right, targetPosition - shipTransform.position , shipTransform.forward);
       // Debug.Log(angleDifferenceR);
        // if the angle is positive, then rotate clockwise
        if(Mathf.Abs(angleDifferenceH) > angleTolerance){
            if(angleDifferenceH > 0) ControlHorizontal =  -fraction;
            if(angleDifferenceH < 0) ControlHorizontal = fraction;
        }
      /*  if(Mathf.Abs(angleDifferenceR) > 5f){
            if(angleDifferenceR > 0)  movementScript.setTargetRollDegrees(-angleDifferenceR);
            if(angleDifferenceR < 0) movementScript.setTargetRollDegrees(-angleDifferenceR);
        }*/
      //  Debug.Log("H " + ControlHorizontal + "V " + ControlVertical);
        movementScript.moveUser(ControlHorizontal, 0);
       
        if(Mathf.Abs(angleDifferenceH) < angleTolerance){
            callback.completedAction();
        }
    }

    public void rotateToFaceTransform(Transform target, GameObject ship, float fractionSpeed, AiClassBase callback){
        rotateToFaceVector(target.position, ship, fractionSpeed, callback);
    }
    public void rotateToFaceTransform(Transform target, GameObject ship, float fractionSpeed, AiClassBase callback, bool sequential){
        rotateToFaceVector(target.position, ship, fractionSpeed, callback, sequential);
    }
    public void rotateToFaceTransformNoCallback(Transform target, GameObject ship, float fractionSpeed){
        rotateToFaceVectorNoCallback(target.position, ship, fractionSpeed);
    }

    public void rotateToFaceTransformBroadside(Transform target, GameObject ship, float fractionSpeed, AiClassBase callback){
       // rotateToFaceVectorBroadside(target.position, ship, fractionSpeed, callback);
        MoveRotationTorque( ship.GetComponent<Rigidbody>(), target.position, fractionSpeed, callback);
    }



}
