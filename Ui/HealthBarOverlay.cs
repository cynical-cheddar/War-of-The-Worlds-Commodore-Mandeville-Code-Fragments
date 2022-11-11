using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarOverlay : Clickable
{

    public bool worldSpace = true;
        [Range(0.0f, 5.0f)]
    public float scaler = 1f;
    public bool overlayAlwaysOn = false;
    public bool changeBar = true;
    public bool changeNumber = true;
    Canvas uiCanvas;
    

    public GameObject barOverlayPrefab;
    GameObject barOverlayInstance;
    UiBar uiBarInterface;
    Ship myShip;

    CameraLookController cameraLookController;

    RectTransform rect;

    float healthCurrent = 1f;
    float maxHealth = 1f;

    bool mouseOver = false;

    Vector3 startScale = Vector3.one;

    public bool distanceScaling = true;
    float maxDist = 1000*3f;

    public void setNumber(float amt, float max){
        healthCurrent = amt;
        maxHealth = max;
    }

    public void setDisplay(bool set){
        mouseOver = set;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        myShip = GetComponentInParent<Ship>();
        

       if(worldSpace){
            if(FindObjectOfType<uiBarsCanvas>()!=null) uiCanvas = FindObjectOfType<uiBarsCanvas>().GetComponent<Canvas>();
            barOverlayInstance = Instantiate(barOverlayPrefab, transform.position, Quaternion.identity);
            barOverlayInstance.transform.parent = uiCanvas.transform;
            uiBarInterface = barOverlayInstance.GetComponent<UiBar>();
            rect = barOverlayInstance.GetComponent<RectTransform>();
            rect.localScale *= scaler;
            rect.localScale *= 0.1f;
            startScale = rect.localScale;
        }

        if(!worldSpace){
            barOverlayInstance = Instantiate(barOverlayPrefab, transform.position, Quaternion.identity);
            uiCanvas = FindObjectOfType<TimeControlInterface>().gameObject.GetComponent<Canvas>();
            barOverlayInstance.transform.SetParent(uiCanvas.transform, false);
            uiBarInterface = barOverlayInstance.GetComponent<UiBar>();


            rect = barOverlayInstance.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f);
            startScale = rect.localScale;
            rect.localScale *= scaler;
        }
        //barOverlayInstance.GetComponent<RectTransform>().localScale = uiCanvas.gameObject.GetComponent<RectTransform>().localScale;
        cameraLookController = FindObjectOfType<CameraLookController>();
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
        if (Input.GetButtonDown("Tab") && cameraLookController.directControl){
            toggleOverlay();
        }


        if(mouseOver){
            barOverlayInstance.SetActive(true);
            if(!worldSpace)updateOverlayPosition(true);
            if(worldSpace)worldBarupd(barOverlayInstance);
        }
        else if(overlayAlwaysOn){
            barOverlayInstance.SetActive(true);
            if(!worldSpace)updateOverlayPosition(false);
            if(worldSpace)worldBarupd(barOverlayInstance);
        }
        
        else{
            if(barOverlayInstance!=null) barOverlayInstance.SetActive(false);
        }

        mouseOver = false;
        
    }
    private void OnDestroy() {
        Destroy(barOverlayInstance);
    }


    void worldBarupd(GameObject bar){
        if(GetComponent<WeaponBarOverlay>())barOverlayInstance.transform.position = transform.position + transform.up * -2;
        else barOverlayInstance.transform.position = transform.position;
    
        barOverlayInstance.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        //barOverlayInstance.transform.rotation = Quaternion.Euler(new Vector3(barOverlayInstance.transform.rotation.x, barOverlayInstance.transform.rotation.y, 0));
       // barOverlayInstance.transform.rotation
        updateOverlayInfo(mouseOver);
    }

    void updateOverlayInfo(bool magnifyHealth){
        
        if(!worldSpace){
            if(distanceScaling) rect.localScale = startScale * scaler *  ((maxDist - Vector3.Distance(Camera.main.transform.position, myShip.transform.position))/maxDist);
                else rect.localScale = startScale * scaler;
                if(magnifyHealth) rect.localScale *= 2;
                if(changeBar) uiBarInterface.setProgressBar((healthCurrent)/maxHealth);
                if(changeNumber){
                    if(healthCurrent > 0.01) uiBarInterface.setnumber(Mathf.RoundToInt(healthCurrent).ToString() + " / " + Mathf.RoundToInt(maxHealth).ToString());
                    else uiBarInterface.setnumber("");
                }
        }
        else{
                if(magnifyHealth) rect.localScale = startScale * 3;
                if(!magnifyHealth) rect.localScale = startScale;
                if(changeBar) uiBarInterface.setProgressBar((healthCurrent)/maxHealth);
                if(changeNumber){
                    if(healthCurrent > 0.01) uiBarInterface.setnumber(Mathf.RoundToInt(healthCurrent).ToString() + " / " + Mathf.RoundToInt(maxHealth).ToString());
                    else uiBarInterface.setnumber("");
                }
        }
    }

    void updateOverlayPosition(bool magnifyHealth){
        //foreach target, calculate the aimpos
            Vector3 aimPos = transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(aimPos);
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCanvas.worldCamera, out movePos);
            rect.anchoredPosition = new Vector2(movePos.x, movePos.y - (10 * scaler));    
            updateOverlayInfo(magnifyHealth);
            
    }
}
