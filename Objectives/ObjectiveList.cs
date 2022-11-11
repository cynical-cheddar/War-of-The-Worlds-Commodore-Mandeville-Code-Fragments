using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveList : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject objectiveTextPrefab;
    public Transform layoutGroupTransform;

    List<Objective> displayedObjectives = new List<Objective>();
    List<GameObject> displayedObjectiveInstances = new List<GameObject>();
    

    public void displayNewObjective(Objective obj){
        displayedObjectives.Add(obj);
        GameObject objectiveDisplayInstance = Instantiate(objectiveTextPrefab, layoutGroupTransform);
        objectiveDisplayInstance.GetComponent<Text>().text = obj.objectiveText;
        displayedObjectiveInstances.Add(objectiveDisplayInstance);
    }

    public void completeObjective(Objective obj){
        // loop through displayed objectives to find obj
        // get index
        // mark displayed objective instance as done
        int i = 0;
        foreach(Objective objective in displayedObjectives){
            if(obj == objective){
                break;
            }
            i++;
        }
        displayedObjectiveInstances[i].GetComponent<Text>().text = displayedObjectiveInstances[i].GetComponent<Text>().text + "[ x ]";
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
