using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiRetreat : AiClassBase
{
    public float proximityThreshold = 50f;

    public float retreatDistance = 800f;

    public bool aheadFull = false;


    public float speedFraction = 1f;

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
        moveVectorPos = animator.transform.position + animator.transform.forward * retreatDistance;
        // do a raycast. make sure that no ship is in the way
        if(performCollisionCheckNoTeams(animator.transform)){
            moveVectorPos = animator.transform.position + animator.transform.right * retreatDistance;
        }

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!collisionCheck(animator.transform)){ 
            
            if(step == 0) {
                
                behaviourDictionary.setSpeed(speedFraction);
                if(aheadFull){
                    behaviourDictionary.setAheadFull(true);
                }
                else{
                    behaviourDictionary.setAheadFull(false);
                }
                behaviourDictionary.moveToPosition(moveVectorPos, animator.gameObject, 0.5f, proximityThreshold, this);
                Debug.Log("moveToTransform: " + sensorManager.getCurrentTarget().ToString());
            }
            // We have arrived at the destination without interrruption
            // Advance state machine for our mutual recursion loop.
            if(step == 1){
                if(aheadFull){
                    behaviourDictionary.setAheadFull(false);
                }
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