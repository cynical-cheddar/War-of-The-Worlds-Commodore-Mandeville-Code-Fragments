using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorsManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform SensorsCamera;
    Camera sensorCam;
    Transform freeCam;

    CameraLookController lookController;
    Transform lookpoint;
    void Start()
    {
        freeCam = Camera.main.transform;
        lookController = FindObjectOfType<CameraLookController>();
        lookpoint = lookController.getLookPoint();
        sensorCam = SensorsCamera.gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        lookpoint = lookController.getLookPoint();
        SensorsCamera.position = new Vector3(lookpoint.position.x, SensorsCamera.position.y, lookpoint.position.z);
        SensorsCamera.rotation = Quaternion.Euler(new Vector3(90, freeCam.eulerAngles.y, SensorsCamera.eulerAngles.z));
    }
    public void setSensorZoom(float amt){
        sensorCam.orthographicSize = amt;
    }
}
