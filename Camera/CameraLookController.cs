using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
public class CameraLookController : MonoBehaviour
{
    //public List<CinemachineVirtualCameraBase> CameraBases;
    bool panning = false;
    Transform selectedShip;
    Transform lookpoint;
    public CinemachineFreeLook freeLookCam;
    int mouseButton = 1;
    public float speedX;
    public float speedY;

    public bool directControl = true;

    public bool strategicCamera = false;

    public bool superStrategicCamera = false;

    public float scrollSensitivity = 1f;

    float lastZoomLevel = 0.5f;

    float curscroll;

    mainCamOverlays mainCamScripts;

    [SerializeField]
   // [Range(0.0f, 1f)]
    public float zoomLevel = 0.5f;

    float targetZoomLevel = 0.5f;

    float strategicZoomLevel = 0.0f;
    public Transform getLookPoint(){
        if(lookpoint!=null)return lookpoint;
        else return selectedShip;
        
    }
    public void setDirectControlBool(bool set){
        directControl = set;
    }
    public void nullSelectedShip(){
        selectedShip = null;
    }
    public OrbitDetails[] m_OrbitsFar = new OrbitDetails[3] 
    { 
     // These are the default orbits
        new OrbitDetails(20f, 60f),
        new OrbitDetails(0f, 60f),
        new OrbitDetails(-20f, 60f)
    };

    public OrbitDetails[] m_OrbitsNear = new OrbitDetails[3] 
    { 
     // These are the default orbits
        new OrbitDetails(20f, 10f),
        new OrbitDetails(0f, 10f),
        new OrbitDetails(-20f, 10f)
    };

    public OrbitDetails[] m_OrbitsStrategicNear = new OrbitDetails[3] 
    { 
     // These are the default orbits
        new OrbitDetails(600f, 400f),
        new OrbitDetails(0f, 400f),
        new OrbitDetails(-600f, 400f)
    };

    public OrbitDetails[] m_OrbitsStrategicFar = new OrbitDetails[3] 
    { 
     // These are the default orbits
        new OrbitDetails(1400f, 900f),
        new OrbitDetails(0f, 900f),
        new OrbitDetails(-1400f, 900f)
    };

    public OrbitDetails[] m_SuperOrbitsStrategicFar = new OrbitDetails[3] 
    { 
     // These are the default orbits
        new OrbitDetails(4000f, 4000f),
        new OrbitDetails(2000f, 4000f),
        new OrbitDetails(-1000f, 4000f)
    };
    
     [Serializable]
     public struct OrbitDetails 
        { 
            /// <summary>Height relative to target</summary>
            public float m_Height; 
            /// <summary>Radius of orbit</summary>
            public float m_Radius; 
            /// <summary>Constructor with specific values</summary>
            public OrbitDetails(float h, float r) { m_Height = h; m_Radius = r; }
    }
    public bool shipyardMode = false;
    private void Start() {
        selectCamera(freeLookCam);
        mainCamScripts = Camera.main.GetComponent<mainCamOverlays>();
        if(FindObjectOfType<shipyard>() != null) shipyardMode = true;
        
    }

    public void selectShip(GameObject shipObject){
        selectedShip = shipObject.transform;
        
        if(!strategicCamera && !superStrategicCamera){
            freeLookCam.LookAt = selectedShip;
            freeLookCam.Follow = selectedShip;
            if(lookpoint!=null)lookpoint.position = selectedShip.position;
            
        }
    }


    void selectCamera(CinemachineVirtualCameraBase cam){
        speedX = freeLookCam.m_XAxis.m_MaxSpeed;
        speedY = freeLookCam.m_YAxis.m_MaxSpeed;
    }
    void Update()
    {
        if(!directControl) strategicCamera = false;
        if(directControl){
        // Rotation
            if(Input.GetMouseButton(mouseButton)){
                if(strategicCamera){
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else{
                    /*Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;*/
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                }
                freeLookCam.m_XAxis.m_MaxSpeed = speedX;
                freeLookCam.m_YAxis.m_MaxSpeed = speedY;
            }
            else{
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                freeLookCam.m_XAxis.m_MaxSpeed = 0;
                freeLookCam.m_YAxis.m_MaxSpeed = 0;
            }
            // Zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            // create a target zoom level. Add scroll to it
            targetZoomLevel -= scroll * scrollSensitivity;
            if(targetZoomLevel > 3) targetZoomLevel = 3;
            if(targetZoomLevel < 0) targetZoomLevel  =0;
            if(targetZoomLevel<1 && selectedShip == null)targetZoomLevel = 1;
            // lerp between currentzoom and target zoom
            zoomLevel = Mathf.Lerp(zoomLevel, targetZoomLevel, Time.unscaledDeltaTime * scrollSensitivity * 4);
            
            if(targetZoomLevel != zoomLevel){
                // calculate new orbit radius

               // zoomLevel -= scroll;
                if(zoomLevel < 0)zoomLevel = 0f;
                if(zoomLevel < 1f && selectedShip != null){
                    lookpoint = Instantiate(new GameObject(), selectedShip.transform.position, freeLookCam.m_LookAt.rotation).transform;
                   
                    strategicCamera = false;
                    superStrategicCamera = false;
                }
                else if(zoomLevel < 1f && selectedShip == null){
                    strategicCamera = false;
                    superStrategicCamera = false;
                    zoomLevel = 1f;
                }
                else if(zoomLevel >= 1 && zoomLevel < 2){
                    strategicCamera = true;
                    superStrategicCamera = false;
                }
                else if(zoomLevel >=2){
                    strategicCamera = false;
                    superStrategicCamera = true;
                    if(zoomLevel > 3) zoomLevel = 3;
                }

                if(!strategicCamera && !superStrategicCamera){
                    
                  //  freeLookCam.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
                    mainCamScripts.setGridLevel(0);
                    freeLookCam.m_Orbits = new CinemachineFreeLook.Orbit[3] 
                    { 
                        // These are the default orbits
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsNear[0].m_Height, m_OrbitsFar[0].m_Height, zoomLevel), Mathf.Lerp(m_OrbitsNear[0].m_Radius, m_OrbitsFar[0].m_Radius, zoomLevel)),
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsNear[1].m_Height, m_OrbitsFar[1].m_Height, zoomLevel), Mathf.Lerp(m_OrbitsNear[1].m_Radius, m_OrbitsFar[1].m_Radius, zoomLevel)),
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsNear[2].m_Height, m_OrbitsFar[2].m_Height, zoomLevel), Mathf.Lerp(m_OrbitsNear[2].m_Radius, m_OrbitsFar[2].m_Radius, zoomLevel))
                    };
                }
                if(strategicCamera && !superStrategicCamera){
                    if(lookpoint == null){
                        lookpoint = Instantiate(new GameObject(), freeLookCam.m_LookAt.position, freeLookCam.m_LookAt.rotation).transform;
                        freeLookCam.LookAt = lookpoint;
                        freeLookCam.Follow = lookpoint;
                    }
                    mainCamScripts.setStrategicCam(true);
                    mainCamScripts.setSensorCam(false);
                    
                    freeLookCam.m_Orbits = new CinemachineFreeLook.Orbit[3] 
                    { 
                        
                        // These are the default orbits
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsStrategicNear[0].m_Height, m_OrbitsStrategicFar[0].m_Height, zoomLevel-1), Mathf.Lerp(m_OrbitsStrategicNear[0].m_Radius, m_OrbitsStrategicFar[0].m_Radius, zoomLevel-1)),
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsStrategicNear[1].m_Height, m_OrbitsStrategicFar[1].m_Height, zoomLevel-1), Mathf.Lerp(m_OrbitsStrategicNear[1].m_Radius, m_OrbitsStrategicFar[1].m_Radius, zoomLevel-1)),
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsStrategicNear[2].m_Height, m_OrbitsStrategicFar[2].m_Height, zoomLevel-1), Mathf.Lerp(m_OrbitsStrategicNear[2].m_Radius, m_OrbitsStrategicFar[2].m_Radius, zoomLevel-1))
                    };
                }
                else if(!strategicCamera && superStrategicCamera){
                    if(lookpoint == null){
                        lookpoint = Instantiate(new GameObject(), freeLookCam.m_LookAt.position, freeLookCam.m_LookAt.rotation).transform;
                        freeLookCam.LookAt = lookpoint;
                        freeLookCam.Follow = lookpoint;
                    }

                    mainCamScripts.setSensorCam(true);
                    
                    //mainCamScripts.setGridLevel(zoomLevel -1);
                    mainCamScripts.setGridLevel(zoomLevel -2);
                    freeLookCam.m_Orbits = new CinemachineFreeLook.Orbit[3] 
                    { 
                        
                        // These are the default orbits
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsStrategicFar[0].m_Height, m_SuperOrbitsStrategicFar[0].m_Height, zoomLevel-2), Mathf.Lerp(m_OrbitsStrategicFar[0].m_Radius, m_SuperOrbitsStrategicFar[0].m_Radius, zoomLevel-2)),
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsStrategicFar[1].m_Height, m_SuperOrbitsStrategicFar[1].m_Height, zoomLevel-2), Mathf.Lerp(m_OrbitsStrategicFar[1].m_Radius, m_SuperOrbitsStrategicFar[1].m_Radius, zoomLevel-2)),
                        new CinemachineFreeLook.Orbit(Mathf.Lerp(m_OrbitsStrategicFar[2].m_Height, m_SuperOrbitsStrategicFar[2].m_Height, zoomLevel-2), Mathf.Lerp(m_OrbitsStrategicFar[2].m_Radius, m_SuperOrbitsStrategicFar[2].m_Radius, zoomLevel-2))
                    };
                }
            }
            if(lastZoomLevel != zoomLevel){
                if(zoomLevel > 1 && lastZoomLevel < 1){
                    // we have entered strategic mode
                    // override the lookat
                    /*
                    lookpoint.position = freeLookCam.m_LookAt.position;

                    freeLookCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
                    freeLookCam.LookAt = lookpoint;
                    freeLookCam.Follow = lookpoint;*/


                    lookpoint = Instantiate(new GameObject(), freeLookCam.m_LookAt.position, freeLookCam.m_LookAt.rotation).transform;
                    selectedShip = null;
                    selectedShip = FindObjectOfType<ShipSelectInterface>().getcurrentlyControlledShip().transform;
                    // check if the ship is in view
                    
                    
                    freeLookCam.LookAt = lookpoint;
                    freeLookCam.Follow = lookpoint;


                    Debug.Log("strategic");
                    StartCoroutine(cameraPanUnscaledTime());
                }
                else if(zoomLevel < 1 && lastZoomLevel > 1){
                    // we have entered shipmode
                    
                    // reset the lookat
                    /*
                    freeLookCam.LookAt = selectedShip;
                    freeLookCam.Follow = selectedShip;
                    lookpoint.position = selectedShip.position;

                    freeLookCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace; */
                   // selectedShip = FindObjectOfType<ShipSelectInterface>().getcurrentlyControlledShip().transform;

                    if(selectedShip != null){
                        freeLookCam.LookAt = selectedShip;
                        freeLookCam.Follow = selectedShip;
                        Destroy(lookpoint.gameObject);
                    }
                    Debug.Log("tactical");
                }
            }

            
            lastZoomLevel = zoomLevel;
        }
        if(strategicCamera && !panning){
            StartCoroutine(cameraPanUnscaledTime());
        }
        else if(superStrategicCamera && !panning){
            StartCoroutine(cameraPanUnscaledTime());
        }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        // if we are in strategic camera mode, allow panning
            
    }

    IEnumerator cameraPanUnscaledTime(){
           
           while(strategicCamera || selectedShip == null || superStrategicCamera){
               panning = true;
                if(lookpoint == null){
                        lookpoint = Instantiate(new GameObject(), freeLookCam.m_LookAt.position, freeLookCam.m_LookAt.rotation).transform;
                        freeLookCam.LookAt = lookpoint;
                        freeLookCam.Follow = lookpoint;
                }
                float ControlHorizontal = 0;
		        float ControlVertical = 0;

                if(Input.GetKey(KeyCode.W)) ControlVertical = 10;
                else if(Input.GetKey(KeyCode.S)) ControlVertical = -10;

                if(Input.GetKey(KeyCode.A)) ControlHorizontal = -10;
                else if(Input.GetKey(KeyCode.D)) ControlHorizontal = 10;

                //get the forward direction of the camera and flatten it
                Vector3 forwardDir = Camera.main.transform.forward;
                Vector3 rightDir = Camera.main.transform.right;
                forwardDir = new Vector3(forwardDir.x, 0, forwardDir.z);
                forwardDir.Normalize();

                rightDir = new Vector3(rightDir.x, 0, rightDir.z);
                rightDir.Normalize();

                Vector3 newpos = lookpoint.position + forwardDir*ControlVertical + rightDir*ControlHorizontal;
                
                if(lookpoint!=null) lookpoint.position = newpos;
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
            panning = false;
            
    }
    public void setLookatStrategicMode(Transform target){
        if(lookpoint != null && (strategicCamera || superStrategicCamera)){
            lookpoint.position = target.position;
            lookpoint.rotation = target.rotation;
            freeLookCam.LookAt = lookpoint;
            freeLookCam.Follow = lookpoint;
        }
        
    }
}

