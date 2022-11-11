using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    public ObjectiveFinishScript objectiveFinishScript;

    public Transform killLayout;

    public GameObject killcountPrefab;

    public AudioSource killcountSources;

    public AudioClip killcountSound;

    public int curShipclass = 0;

    int fighterKills;
    int corvetteKills;
    int frigateKills;

    int destroyerKills;

    int cruiserKills;

    int battleshipKills;

    public void setKills(int fighterKillsLocal, int corvetteKillsLocal, int frigateKillsLocal, int destroyerKillsLocal ,int cruiserKillsLocal, int battleshipKillsLocal){
        // add the kills into a queue
        // instantiate and display
        fighterKills = fighterKillsLocal;
        corvetteKills = corvetteKillsLocal;
        frigateKills = frigateKillsLocal;
        destroyerKills = destroyerKillsLocal;
        cruiserKills = cruiserKillsLocal;
        battleshipKills = battleshipKillsLocal;
        startCoroutineMethod(curShipclass);
    }

    void startCoroutineMethod(int shipclass){
        if(shipclass == 0) StartCoroutine(updateKillCount(fighterKills, 0.2f, "Fighters Killed:"));
        if(shipclass == 1) StartCoroutine(updateKillCount(corvetteKills, 0.2f, "Corvettes Destroyed:"));
        if(shipclass == 2) StartCoroutine(updateKillCount(frigateKills, 0.2f, "Frigates Neutralised:"));
        if(shipclass == 3) StartCoroutine(updateKillCount(destroyerKills, 0.2f, "Destroyers Sunk:"));
        if(shipclass == 4) StartCoroutine(updateKillCount(cruiserKills, 0.2f, "Cruisers Smashed:"));
        if(shipclass == 5) StartCoroutine(updateKillCount(battleshipKills, 0.2f, "Battleships Obliterated:"));
    }
    IEnumerator updateKillCount(int kills, float updTime, string tagtext){
        int cur = 0;
        GameObject killcountInstance = Instantiate(killcountPrefab, killLayout);
        killcountInstance.GetComponentInChildren<uitag>().gameObject.GetComponent<Text>().text = tagtext;
        while(cur <= kills){
            killcountInstance.GetComponentInChildren<number>().gameObject.GetComponent<Text>().text = cur.ToString();
            killcountSources.PlayOneShot(killcountSound);
            cur++;
            yield return new WaitForSecondsRealtime(updTime);
        }
        curShipclass ++;
        startCoroutineMethod(curShipclass);
    }

    public void nextScene(){
        objectiveFinishScript.loadNextScene();
    }
}
