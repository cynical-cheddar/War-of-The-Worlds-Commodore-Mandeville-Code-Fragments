using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enterBattleDialogueBox : MonoBehaviour
{
    string filepath;
    public void setShipFilePath(string set){
        filepath = set;
    }
    public void confirm(){
        GetComponentInParent<shipyard>().returnToScene();
    }

    public void cancel(){
        Destroy(gameObject);
    }
}
