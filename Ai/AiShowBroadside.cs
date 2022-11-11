using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiShowBroadside : AiClassBase
{
    //public float proximityThreshold = 50f;

    public float maxDistance = 500f;

    public float rotationFraction = 1f;

    public float moveSpeed = 0.5f;

    public bool matchVerticalPlane = true;
    public float verticalCorrectionVelocity = 0.1f;

    Vector3 moveVectorPos = Vector3.zero;

    

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
        
            if(!collisionCheck(animator.transform)){ 
                if(matchVerticalPlane)behaviourDictionary.matchTransformVerticalPlaneNoCallback(sensorManager.getCurrentTarget(), animator.gameObject,verticalCorrectionVelocity, 20f);
                if(Vector3.Distance(animator.transform.position, sensorManager.getCurrentTarget().position) > maxDistance) animator.SetTrigger(closeGap);
                if(step == 0) {
                    behaviourDictionary.rotateToFaceTransformBroadside(sensorManager.getCurrentTarget(), animator.gameObject, rotationFraction, this);
                    behaviourDictionary.setSpeed(moveSpeed);
                }

                if(step == 1){
                    step ++;
                    animator.SetTrigger(nextState);   
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

