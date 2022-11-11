using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCloseDistance : AiClassBase
{
    public float proximityThreshold = 300f;

   // public bool horizontalRotationOnly = true;

    public float speedFractionWhileRotating = 1f;
    public float speedFraction = 1f;

    Vector3 moveVectorPos = Vector3.zero;

    public bool matchVerticalPlane = false;

    public float verticalCorrectionVelocity = 1f;

    public bool disableCollisionCheck = false;

    public bool aheadFull = false;



    // Search should just be a random (ish) movement pattern. It will zigzag
    // It gets the position of a valid enemy target and draws a direction vector
    // Offset this direction vector by a random angle
    // Normalise new vector and multiply by a random(ish) distance
    // Move to new position

    // This behaviour will be interrupted when an enemy is in range / we are attacked



    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        step = 0;
        behaviourDictionary = animator.gameObject.GetComponent<AiBehaviourDictionary>();
        sensorManager = animator.gameObject.GetComponent<CapitalShipAi>();

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!collisionCheck(animator.transform) || disableCollisionCheck){ 
            if(step == 0) {
                behaviourDictionary.setSpeed(speedFractionWhileRotating);
               // behaviourDictionary.rotateToFaceTransform(sensorManager.getCurrentTarget(), animator.gameObject, 1f, this);
                behaviourDictionary.MoveRotationTorque(animator.GetComponent<Rigidbody>(), sensorManager.getCurrentTarget().position, 10f, this);
                //else behaviourDictionary.rotateToFaceTransform(sensorManager.getCurrentTarget(), animator.gameObject, 1f, this);
                
                Debug.Log("rotating to face: " + sensorManager.getCurrentTarget().ToString());
            }
            if(step == 1) {
                if(aheadFull) behaviourDictionary.setAheadFull(true);
                behaviourDictionary.setSpeed(speedFraction);
                if(matchVerticalPlane) behaviourDictionary.matchTransformVerticalPlaneNoCallback(sensorManager.getCurrentTarget(), animator.gameObject, verticalCorrectionVelocity, 50f);
            //    behaviourDictionary.rotateToFaceTransform(sensorManager.getCurrentTarget(), animator.gameObject, 1f, this, true);
                behaviourDictionary.moveToTransform(sensorManager.getCurrentTarget(), animator.gameObject, 6f, proximityThreshold, this);
                //else behaviourDictionary.rotateToFaceTransform(sensorManager.getCurrentTarget(), animator.gameObject, 1f, this);
                
                Debug.Log("rotating to face: " + sensorManager.getCurrentTarget().ToString());
            }
            // We have arrived at the destination without interrruption
            // Advance state machine for our mutual recursion loop.
            if(step == 2){
                if(aheadFull) behaviourDictionary.setAheadFull(false);
                animator.SetTrigger(nextState);  
                step ++; 
            }
        }
        
        
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
    }
}
