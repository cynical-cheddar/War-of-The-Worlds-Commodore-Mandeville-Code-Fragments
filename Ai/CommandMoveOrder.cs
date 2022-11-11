using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMoveOrder : AiClassBase
{
    public float proximityThreshold = 100f;

    public float speedFractionWhileRotating = 1f;

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
        orderManager = animator.gameObject.GetComponent<OrderManager>();
        moveVectorPos = orderManager.executeOrder();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            moveVectorPos = orderManager.currentMoveOrder;
            if(step == 0) {
               // behaviourDictionary.setSpeed(speedFractionWhileRotating);
             //   behaviourDictionary.rotateToFaceVector( moveVectorPos, animator.gameObject, 1f, this);
                behaviourDictionary.MoveRotationTorque(animator.GetComponent<Rigidbody>(), moveVectorPos, 1f, this);
            }
            // We have rotated. Now let's plot a course
            if(step == 1) {
                behaviourDictionary.setSpeed(1f);
                behaviourDictionary.moveToPosition(moveVectorPos, animator.gameObject, 1f, proximityThreshold, this);

            }
            // We have arrived at the destination without interrruption
            // Advance state machine for our mutual recursion loop.
            if(step == 2){
                behaviourDictionary.setSpeed(0f);
                orderManager.completeOrder();
                
                animator.SetTrigger(nextState);  
                step ++; 
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