using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class selectShipIcon : MonoBehaviour
{


    public GameObject ship;
    // Start is called before the first frame update
    public void selectShipCallback(){
        GetComponentInParent<ShipSelectInterface>().selectShip(ship);
    }

    public void setName(string name){
        GetComponentInChildren<Text>().text = name;
    }
}
