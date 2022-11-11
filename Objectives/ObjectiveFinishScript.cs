using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveFinishScript : MonoBehaviour
{

    public enum finishAction
    {
        loadScene

    }

    public string nextSceneName;
    public GameObject victoryInfoScreen;

    public finishAction actionWhenObjectivesCompleted;
    public void objectivesFinishedAction(){
        Debug.Log("objectivesFinishedAction");
        
        if(actionWhenObjectivesCompleted == finishAction.loadScene){
            displayVictoryInformation();
            
        }
    }

    void displayVictoryInformation(){
        // get kills from dataToAddToPersistFOlder
       // Time.timeScale = 0.001f;
       // disable all current canvases
        foreach (Canvas o in Object.FindObjectsOfType<Canvas>()) {
             o.enabled = false;
         }
        GameObject victoryInfoScreenInstance = Instantiate(victoryInfoScreen, Vector3.zero, Quaternion.identity);
        VictoryScreen victoryScreenScript = victoryInfoScreenInstance.GetComponent<VictoryScreen>();
        victoryScreenScript.objectiveFinishScript = this;
        dataToAddToPersistFolder data = FindObjectOfType<dataToAddToPersistFolder>();
        victoryScreenScript.setKills(data.fightersKilled, data.corvettesKilled, data.frigatesKilled, data.destroyersKilled, data.cruisersKilled, data.battleshipsKilled);
        
    }

    public void loadNextScene(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
    
    
}
