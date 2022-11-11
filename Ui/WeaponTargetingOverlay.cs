using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTargetingOverlay : MonoBehaviour
{
    
    Canvas uiCanvas;
    public bool fireControlOverlay = true;
    WeaponAiController weaponAi;

    Weapon controlledWeapon;

    protected float updateCooldown = 1f;
    protected float upd = 0f;

    int listLength = 0;

    List<GameObject> targetOverlays = new List<GameObject>();

    public GameObject targetOverlayPrefab;

    // Start is called before the first frame update
    void Start()
    {
        uiCanvas = GetComponentInChildren<WeaponUi>().GetComponent<Canvas>();
        controlledWeapon = GetComponent<Weapon>();
        weaponAi = GetComponent<WeaponAiController>();
        refreshTargetOverlays();
        uiCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(controlledWeapon.selected){
            checkForUpdate();
            upd -= Time.deltaTime;
            if(upd<=0){
                if(!controlledWeapon.autoControlled) weaponAi.updateLists();
                
                upd = updateCooldown;
            }
            
        }
    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        if(weaponAi.targetList.Count > 0 && targetOverlays.Count > 0)updateOverlayPositions();
    }

    public void checkForUpdate(){
        if(weaponAi.targetList.Count > 0){
            int newListLength = weaponAi.targetList.Count;
            if(newListLength != listLength){
                listLength = newListLength;
                refreshTargetOverlays();
                if(weaponAi.targetList.Count > 0) updateOverlayPositions();
            }
        }
        else{
            foreach(GameObject i in targetOverlays){
                Destroy(i);
            }
            targetOverlays.Clear();
        }
        
    }

    void updateOverlayPositions(){
        //foreach target, calculate the aimpos
        int i = 0;
        foreach(Transform t in weaponAi.targetList){
            Vector3 aimPos = weaponAi.calculateFireControl(t, controlledWeapon.muzzleVelocity, weaponAi.masterFirePoint.position);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(aimPos);
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCanvas.worldCamera, out movePos);
            
            targetOverlays[i].GetComponent<RectTransform>().anchoredPosition = movePos;

            if(Mathf.Abs(Vector3.SignedAngle(Camera.main.transform.forward,  aimPos - Camera.main.transform.position, transform.up)) > 90f){
                targetOverlays[i].SetActive(false);
            }
            else{
                targetOverlays[i].SetActive(true);
                //show
            }
            i++;
        }
        
    }

    void refreshTargetOverlays(){
        // foreach enemy target, create a target reticule
        foreach(GameObject i in targetOverlays){
            Destroy(i);
        }
        targetOverlays.Clear();
        foreach(Transform t in weaponAi.targetList){
            GameObject target = Instantiate(targetOverlayPrefab, transform.position, Quaternion.identity);
            target.transform.parent = uiCanvas.transform;
            targetOverlays.Add(target);
            target.GetComponent<RectTransform>().localPosition = new Vector3(target.GetComponent<RectTransform>().localPosition.x, target.GetComponent<RectTransform>().localPosition.y, 0f);
        }
    }
}
