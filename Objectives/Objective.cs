using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public int myTeamId = 0;
    protected ObjectiveManager objectiveManager;
    protected bool objectiveEnabled = false;

    public string objectiveText = "Generic Objective Text";
    

    
    public virtual void Start(){
        // get objectiveManager
        objectiveManager = FindObjectOfType<ObjectiveManager>();
    }

    // called via the objectvie manager
    public virtual void enableObjective(){
        //objectiveManager.setCurrentObjective(this);
        //objectiveEnabled = true;
        // sets up conditions to achive victory
    }

    // advances the current objective in the objective manager. Disables this objective
    // called by child
    public virtual void completeObjective(){
        objectiveManager.advanceObjective();
        objectiveEnabled = false;
    }
}
