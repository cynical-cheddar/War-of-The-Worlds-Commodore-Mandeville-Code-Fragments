using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WeaponBarOverlay : MonoBehaviour
{
    public bool worldSpace = true;
    [Range(0.0f, 1.0f)]
    public float scaler = 1f;
    public bool overlayAlwaysOn = false;
    public bool changeBar = true;
    public bool changeNumber = true;
    Canvas uiCanvas;
    public bool weaponBarOverlay = true;

    public GameObject barOverlayPrefab;
    GameObject barOverlayInstance;
    Weapon controlledWeapon;
    UiBar uiBarInterface;
    Ship myShip;
Vector3 startScale = Vector3.one;
    CameraLookController cameraLookController;

    RectTransform rect;

    float remainingTime = 0f;
    float maxTime = 1f;

    float maxDist = 300f;

    List<int> salvoInfo = new List<int>();
    public bool distanceScaling = true;
    public void setRemainingTime(float remainingTimeLocal, float maxTimeLocal){
        remainingTime = remainingTimeLocal;
        maxTime = maxTimeLocal;
    }
    // Start is called before the first frame update
    void Start()
    {
        controlledWeapon = GetComponent<Weapon>();
        myShip = GetComponentInParent<Ship>();
        
       
        
        if(worldSpace){
            uiCanvas = FindObjectOfType<uiBarsCanvas>().GetComponent<Canvas>();
            barOverlayInstance = Instantiate(barOverlayPrefab, transform.position, Quaternion.identity);
            barOverlayInstance.transform.parent = uiCanvas.transform;
            uiBarInterface = barOverlayInstance.GetComponent<UiBar>();
            rect = barOverlayInstance.GetComponent<RectTransform>();
            rect.localScale *= scaler;
            rect.localScale *= 0.07f;
        }

        if(!worldSpace){
            uiCanvas = GetComponentInParent<Ship>().controlUI.GetComponent<Canvas>();
             barOverlayInstance = Instantiate(barOverlayPrefab, uiCanvas.transform.position, Quaternion.identity);
            barOverlayInstance.transform.parent = uiCanvas.transform;
            uiBarInterface = barOverlayInstance.GetComponent<UiBar>();


            rect = barOverlayInstance.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f);

            startScale = rect.localScale;

            rect.localScale *= scaler;
        }
        //barOverlayInstance.GetComponent<RectTransform>().localScale = uiCanvas.gameObject.GetComponent<RectTransform>().localScale;
        cameraLookController = FindObjectOfType<CameraLookController>();
        salvoInfo = controlledWeapon.getSalvoInfo();
        // draw divider lines
        if(salvoInfo != null){
            for(int i = 0; i<salvoInfo[1]; i++){
                float fraction = (float)i/salvoInfo[1];
                uiBarInterface.setDivider(fraction, 1);
            }
            
        }
    }   
    void toggleOverlay(){
        overlayAlwaysOn = !overlayAlwaysOn;
    }


    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        if(myShip!=null){
            if(salvoInfo.Count > 0){salvoInfo = controlledWeapon.getSalvoInfo();}
            if (Input.GetButtonDown("Tab") && !controlledWeapon.selected && myShip.controllable && cameraLookController.directControl){
                toggleOverlay();
            }


            if(!controlledWeapon.selected && myShip.currentlyControlled && controlledWeapon.highlighted){
                barOverlayInstance.SetActive(true);
                if(!worldSpace) updateOverlayPosition();
                if(worldSpace) worldBarupd(barOverlayInstance);
            }
            if(overlayAlwaysOn){
                barOverlayInstance.SetActive(true);
                if(!worldSpace) updateOverlayPosition();
                if(worldSpace) worldBarupd(barOverlayInstance);
            }
            
            else{
               if(barOverlayInstance!=null) barOverlayInstance.SetActive(false);
            }
            
        }
    }
    private void OnDestroy() {
        Destroy(barOverlayInstance);
    }

    void worldBarupd(GameObject bar){
        barOverlayInstance.transform.position = transform.position;
    
        barOverlayInstance.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        //barOverlayInstance.transform.rotation = Quaternion.Euler(new Vector3(barOverlayInstance.transform.rotation.x, barOverlayInstance.transform.rotation.y, 0));
       // barOverlayInstance.transform.rotation
        updateOverlayInfo();
    }


    void updateOverlayInfo(){
                    if(changeBar){
                if(salvoInfo == null)uiBarInterface.setProgressBar((maxTime-remainingTime)/maxTime);
                else uiBarInterface.setProgressBar((maxTime-remainingTime)/(maxTime));
            } 
            if(changeNumber){
                if(remainingTime > 0.01) uiBarInterface.setnumber(Mathf.RoundToInt(remainingTime).ToString());
                else uiBarInterface.setnumber("");
            }
    }
    void updateOverlayPosition(){
        //foreach target, calculate the aimpos
            Vector3 aimPos = transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(aimPos);
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCanvas.worldCamera, out movePos);
            rect.anchoredPosition = movePos;    

            if(distanceScaling) rect.localScale = startScale * scaler *  ((maxDist - Vector3.Distance(cameraLookController.transform.position, aimPos))/maxDist);
            else rect.localScale = startScale * scaler;

            updateOverlayInfo();



    }
}
