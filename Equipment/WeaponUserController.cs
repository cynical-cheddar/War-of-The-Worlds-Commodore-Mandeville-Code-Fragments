using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

public class WeaponUserController : MonoBehaviour
{
    public LayerMask targetingMask;
    public bool friendlyFire = false;
    public bool lockWhenCantFire = true;
    public bool onlyLockOn = false;
    public toggleButtonGraphic autoAimButton;
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    float cursensitivityX = 15f;
    float cursensitivityY = 15f;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationX = 0F;
    float rotationY = 0F;
    Quaternion originalRotation;
    Weapon weapon;
    public CinemachineVirtualCameraBase weaponCam;

    Transform lookPoint;
    Transform cameraDummy;

    Transform camTransform;
    bool pausedOperation = false;


    public void invertControls(){
        sensitivityX *= -1;
        sensitivityY *= -1;
       
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
        
    }

    public IEnumerator pauseOp(){
        pausedOperation = true;
        yield return new WaitForSecondsRealtime(0.0f);
        pausedOperation = false;
    }
    public void invertCamera(){
        switch (weaponCam){
            case CinemachineVirtualCamera c:
                c.m_Lens.Dutch = 180;
            break;
            default:
            break;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponent<Weapon>();
        camTransform = weaponCam.transform;
        lookPoint = Instantiate(new GameObject().transform, transform.position, transform.rotation);
        lookPoint.gameObject.name = gameObject.name + "point";

        cameraDummy = Instantiate(new GameObject().transform, transform.position, transform.rotation);
        cameraDummy.transform.parent = transform;
        cursensitivityX = sensitivityX;
        cursensitivityY = sensitivityY;

    }
    public void forceCamLookat(Vector3 pos){
        lookPoint.position = pos;
        cameraDummy.transform.LookAt(pos);
        weaponCam.LookAt = lookPoint;
        rotationX = cameraDummy.localRotation.x;
        rotationY = cameraDummy.localRotation.y;

    }

    public void enableDirectWeaponControl(){
        // Get all cinemachine cameras and disable em. 
        // Enable the one we like
        if(GetComponentInParent<Ship>().currentlyControlled){
            disableAllVcams();
            
            weaponCam.enabled = true;
        //  lookPoint.position = cameraDummy.forward * 10000;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            cursensitivityX = sensitivityX;
            cursensitivityY = sensitivityY;
            if(GetComponent<WeaponTargetingOverlay>()!= null)GetComponent<WeaponTargetingOverlay>().checkForUpdate();
        }
    }

    

    void disableAllVcams(){
        CinemachineVirtualCameraBase[] allVcams = FindObjectsOfType<CinemachineVirtualCameraBase>();
        foreach(CinemachineVirtualCameraBase cam in allVcams){
            cam.enabled = false;
        }
    }
    public void restoreFreeCam(){
        // get call vcams, disable them, and return control to the freecam
         disableAllVcams();
        GameObject.FindObjectOfType<CameraLookController>().gameObject.GetComponent<CameraLookController>().enabled = true;
        GameObject.FindObjectOfType<CameraLookController>().gameObject.GetComponent<CinemachineFreeLook>().enabled = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        cursensitivityX = sensitivityX;
        cursensitivityY = sensitivityY;
        //GameObject.FindObjectOfType<CameraLookController>().gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;
    }

    public void toggleAutoAim(){
        if(GetComponent<WeaponAiController>() != null){
            GetComponent<Weapon>().toggleAutoControl();
            if(autoAimButton !=null) autoAimButton.toggle();
            GetComponent<WeaponAiController>().updateLists();
            if(autoAimButton== null) autoAimButton = transform.Find("AutoControl").gameObject.GetComponent<toggleButtonGraphic>();
            if(autoAimButton == null) autoAimButton = GetComponentInChildren<toggleButtonGraphic>();
            
        }
    }
    public void setAutoAimTrue(){
        if(GetComponent<WeaponAiController>() != null && weapon !=null){
            if(weapon.allowAutoTarget){
                GetComponent<Weapon>().autoControlled = true;
                
                GetComponent<WeaponAiController>().updateLists();
               // if(autoAimButton== null) autoAimButton = transform.Find("AutoControl").gameObject.GetComponent<toggleButtonGraphic>();
               // if(autoAimButton == null) autoAimButton = GetComponentInChildren<toggleButtonGraphic>();
               // if(autoAimButton!=null) autoAimButton.toggle();
            }
            
        }
    }

 
    

    void mouselook(){


         // Read the mouse input axis
         rotationX += Input.GetAxis("Mouse Y") * cursensitivityY * -1;
         rotationY += Input.GetAxis("Mouse X") * cursensitivityX;
         rotationX = ClampAngle (rotationX, minimumX, maximumX);
         rotationY = ClampAngle (rotationY, minimumY, maximumY);
         Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, transform.up);
         Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -transform.right);
         cameraDummy.localRotation = Quaternion.Euler(rotationX, rotationY, transform.rotation.z);
      //   Debug.Log(cameraDummy.localRotation);
         
         
        //  lookPoint.rotation =  originalRotation * xQuaternion * yQuaternion;
    }

    public static float ClampAngle (float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp (angle, min, max);
    }

    void lockOnGraphicHover(){
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit, weapon.range, targetingMask)) {
                // see if the target is attackable
                // traverse up the hierarchy and see if we have hit an enemy
                Ship theirShip = FindParentShip(hit.transform);
                
                if(theirShip != null){
                    if(theirShip.teamId != GetComponentInParent<Ship>().teamId) weapon.getGuiInterface().displayLockOn(true);
                    else if(friendlyFire)weapon.getGuiInterface().displayLockOn(true);
                    else weapon.getGuiInterface().displayLockOn(false);
                }
                else weapon.getGuiInterface().displayLockOn(false);
         }
         else weapon.getGuiInterface().displayLockOn(false);
    }
    Ship FindParentShip(Transform tPam)
    {
        Transform t = tPam;
        if(t.GetComponent<Ship>() != null) return t.GetComponent<Ship>();
        while (t.parent != null)
        {
        if (t.parent.GetComponent<Ship>() != null)
        {
            return t.parent.GetComponent<Ship>();
        }
        t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }
    void userSelectTarget(){
        //Debug.Log("ship targeted " + theirShip.ToString());
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit, weapon.range, targetingMask)) {
                // see if the target is attackable
                // traverse up the hierarchy and see if we have hit an enemy
                Ship theirShip = FindParentShip(hit.transform);
                
                if(theirShip != null){
                    if(theirShip.teamId != GetComponentInParent<Ship>().teamId || friendlyFire){
                        Debug.Log("ship targeted " + theirShip.ToString());
                        GetComponent<WeaponAiController>().userSetTarget(theirShip.transform);
                        toggleAutoAim();
                    } 
                }
         }
    }
    void Update()
    {
        if(weapon.selected){
            if(Input.GetButtonDown("Interact")){
                toggleAutoAim();
            }
            if(Input.GetKey(KeyCode.Tab)){
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else{
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                mouselook();
            }
            if(Time.timeScale >= 0.02){
                cursensitivityX = sensitivityX;
                cursensitivityY = sensitivityY;
            }
            // adjust cam rotation
            
            // temmp line
            if(Input.GetButtonDown("deselectWeapon")) weapon.deselectWeapon();
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if(scroll != 0)  Debug.Log("detected scroll input " + gameObject.name);
            if(scroll < 0){GetComponentInParent<shipWeaponList>().previousWeapon(lookPoint.position);}
            if(scroll > 0){GetComponentInParent<shipWeaponList>().nextWeapon(lookPoint.position);}
            // select prev weapon in our weapon list
            
           // if(Input.GetKeyDown(KeyCode.A)) GetComponentInParent<shipWeaponList>().previousWeapon();
           // if(Input.GetKeyDown(KeyCode.D)) GetComponentInParent<shipWeaponList>().nextWeapon();


            // Get input axes and set the look rotations
            // Do a raycast. Set the hitpoint as the lookat point;
            Vector3 point = Vector3.zero;
           /* Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit, Mathf.Infinity)) {
                lookPoint.position = hit.point;
            }
            else{
                lookPoint.position = Camera.main.transform.forward * 100f;
            }*/


            if(!weapon.autoControlled){
                // if we are allowed to do a lock on, then display the lock on graphic
                if(weapon.allowAutoTarget)lockOnGraphicHover();
                // fire controls for projectile weapons
                if(weapon.weaponType == Weapon.WeaponType.projectile){
                    if(Input.GetMouseButtonDown(0) && weapon.canFire()){
                        if(!onlyLockOn)weapon.Fire();
                        else userSelectTarget();
                        if(lockWhenCantFire && !weapon.canFire() && Time.timeScale < 0.02){
                            cursensitivityY = 0f;
                            cursensitivityX = 0f;
                        }
                    }
                }
                // fire controls for beams 
                else if(weapon.weaponType == Weapon.WeaponType.beam){
                    
                    if(Input.GetMouseButton(0) && weapon.canFire()){
                        weapon.Fire(); 
                    }
                    else{
                        weapon.ceaseFireBeam();
                    }
                }


                if(Input.GetMouseButtonDown(1) && weapon.allowAutoTarget){
                    userSelectTarget();
                } 
                
                if(!pausedOperation)lookPoint.position = cameraDummy.position + cameraDummy.forward * 10000;
                if(!pausedOperation)weaponCam.LookAt = lookPoint;  
                
                if (lockWhenCantFire && weapon.canFire() && Time.timeScale < 0.02){
                    cursensitivityX = sensitivityX;
                    cursensitivityY = sensitivityY;
                }

            }
            else{
                if(Input.GetMouseButtonDown(1) && weapon.allowAutoTarget) toggleAutoAim();
                if(GetComponent<WeaponAiController>() != null ){
                    if(!pausedOperation) lookPoint.position = GetComponent<WeaponAiController>().masterFirePoint.position + GetComponent<WeaponAiController>().masterFirePoint.forward * 1000;
                    cameraDummy.rotation = GetComponent<WeaponAiController>().masterFirePoint.rotation;
                    weaponCam.LookAt = lookPoint;  
                }
            }
            
           if(!pausedOperation)weapon.setLookDir(lookPoint.position, false);
            
            // Move virtual camera
            

            //weapon.setLookDir();
        }
        
    }
}
