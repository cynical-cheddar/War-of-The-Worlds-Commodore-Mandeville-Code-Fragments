using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ObjectiveFinishScript))]
[RequireComponent(typeof(dataToAddToPersistFolder))]
public class ObjectiveManager : MonoBehaviour
{
    public int myTeamId = 0;
    public List<Objective> objectiveList = new List<Objective>();


    Objective currentObjectiveObject;

    int currentObjectiveIndex = 0;

    ObjectiveList objectiveListDisplay;

    ObjectiveFinishScript objectiveFinishScript;
    dataToAddToPersistFolder persistUpdater;

    public void setObjectiveFinishScript(ObjectiveFinishScript script){
        objectiveFinishScript = script;
    }
    public void setDataAddToPersistFolder(dataToAddToPersistFolder script){
        persistUpdater = script;
    }
    

    public void setCurrentObjective(Objective obj){
        obj.myTeamId = myTeamId;
        currentObjectiveObject = obj;
        obj.enableObjective();
        objectiveListDisplay.displayNewObjective(obj);
    }
    
    public void advanceObjective(){
        // mark the current objective as completed in the display
        objectiveListDisplay.completeObjective(currentObjectiveObject);

        currentObjectiveIndex++;
        // get the length of the objective list. if we have finished them all, then call the completed objectvie function
        if(currentObjectiveIndex >= objectiveList.Count){
            completedObjectiveList();
        }
        // otehrwise advance the objective
        else{
            setCurrentObjective(objectiveList[currentObjectiveIndex]);
        }
    }
    public void completedObjectiveList(){
        Debug.Log("Done all yer objectives mate");
        if(objectiveFinishScript == null) objectiveFinishScript = GetComponent<ObjectiveFinishScript>();
        if(persistUpdater == null) persistUpdater = GetComponent<dataToAddToPersistFolder>();
        objectiveFinishScript.objectivesFinishedAction();
        persistUpdater.saveToPersist();
        
        
    }

    
    // Start is called before the first frame update
    void Start()
    {
        objectiveListDisplay = FindObjectOfType<ObjectiveList>();
        if(objectiveList.Count>0) setCurrentObjective(objectiveList[0]);
    }
}
