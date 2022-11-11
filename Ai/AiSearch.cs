using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSearch : AiClassBase
{

    public float rangeMove = 200f;
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

        moveVectorPos = new Vector3(sensorManager.transform.position.x + Random.Range(-rangeMove, rangeMove), sensorManager.transform.position.y + Random.Range(-25f, 25f), sensorManager.transform.position.z + Random.Range(-rangeMove, rangeMove));

        Debug.DrawLine(animator.gameObject.transform.position, moveVectorPos, Color.green, 10f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            if(sensorManager.targetSighted) animator.SetTrigger(targetFound);

            if(step == 0) {
                behaviourDictionary.setSpeed(0f);
                behaviourDictionary.rotateToFaceVector(moveVectorPos, animator.gameObject, 0.3f, this);
            }
            // We have rotated. Now let's plot a course
            if(step == 1) {
                if(!collisionCheck(animator.transform)){ 
                    Debug.Log("Finished Rotation, cur step =  " + step);
                    behaviourDictionary.setSpeed(0.5f);
                    behaviourDictionary.moveToPosition(moveVectorPos, animator.gameObject, 0.2f, this);
                }
            }
            // We have arrived at the destination without interrruption
            // Advance state machine for our mutual recursion loop.
            if(step == 2){
                step ++;
                animator.SetTrigger(nextState);   
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

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
