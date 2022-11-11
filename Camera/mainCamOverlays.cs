using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCamOverlays : MonoBehaviour
{
    BackgroundGridOpacity strategicGrid;
    Camera stratOverlayCam;
    Camera radarOverlayCam;
    // Start is called before the first frame update
    void Start()
    {
        strategicGrid = FindObjectOfType<BackgroundGridOpacity>();
        stratOverlayCam = GetComponentInChildren<stratcam>().GetComponent<Camera>();
        radarOverlayCam = GetComponentInChildren<radarcam>().GetComponent<Camera>();
    }

    // Update is called once per frame
    public void setGridLevel(float amt){
        if(strategicGrid != null){
            strategicGrid.setOpacity(amt);
        } 
        if(amt > 0.1) stratOverlayCam.enabled = true;
        else stratOverlayCam.enabled = false;
    }
    public void setStrategicCam(bool set){
        stratOverlayCam.enabled = set;
    }

    public void setSensorCam(bool set){
        radarOverlayCam.enabled = set;
    }
}
