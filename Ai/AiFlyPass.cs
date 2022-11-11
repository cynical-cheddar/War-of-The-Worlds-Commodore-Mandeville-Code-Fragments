using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFlyPass : AiClassBase
{
    public float proximityThreshold = 100f;

    public Vector3 flypassOffset = new Vector3(0, 100, 0);

    public float speedFractionWhileRotating = 1f;
    public float speedFraction = 1f;



    Vector3 moveVectorPos = Vector3.zero;

    float maxTime = 10f;

    Rigidbody rigidbody;

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
        rigidbody = animator.gameObject.GetComponent<Rigidbody>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if(!collisionCheck(animator.transform)){ 
            if(step == 0) {
               behaviourDictionary.setSpeed(speedFractionWhileRotating);
                behaviourDictionary.MoveRotationTorque(rigidbody ,sensorManager.getCurrentTarget().position + flypassOffset, 10f,this);
                Debug.Log("rotating to face: " + sensorManager.getCurrentTarget().ToString());
            }
            // We have rotated. Now let's plot a course
            if(step == 1) {
                
                behaviourDictionary.setSpeed(speedFraction);
                behaviourDictionary.moveToPosition(sensorManager.getCurrentTarget().position + flypassOffset, animator.gameObject, 1f, proximityThreshold, this);
                Debug.Log("moveToTransform: " + sensorManager.getCurrentTarget().ToString());
            }
            // We have arrived at the destination without interrruption
            // Advance state machine for our mutual recursion loop.
            if(step == 2){
                
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