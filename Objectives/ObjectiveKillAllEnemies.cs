using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveKillAllEnemies : Objective
{
    List<GameObject> enemyList = new List<GameObject>();
    float updateCooldown = 5f;
    // Start is called before the first frame update
    

    // Update is called once per frame


    public override void enableObjective(){
        Debug.Log("checking if enemies are dead0");
        base.enableObjective();
        objectiveEnabled = true;
        StartCoroutine(periodicUpdate());
        
    }


    IEnumerator periodicUpdate(){
        Debug.Log("checking if enemies are dead");
        while(objectiveEnabled){
            Debug.Log("checking if enemies are dead2");
            yield return new WaitForSeconds(updateCooldown);
            updateEnemyList();
            // if there are no enemies, then advance execution
            if(enemyList.Count == 0) completeObjective();
        }
    }
    void updateEnemyList(){
        enemyList.Clear();
        if(GameObject.FindObjectOfType<Ship>() != null){
            Ship[] ships = GameObject.FindObjectsOfType<Ship>();
            
            foreach(Ship s in ships){
                if(s.teamId != myTeamId){
                    enemyList.Add(s.gameObject);
                }
            }
        }
    }
}
