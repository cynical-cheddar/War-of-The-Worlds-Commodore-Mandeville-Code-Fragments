using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class WeaponUi : MonoBehaviour
{


    Weapon parentWeapon;
    public Sprite weaponIcon;


    public Sprite crossHair;

    public Sprite lockonCrossHair;
    public Sprite shellIcon;

    public string weaponName = "the gun";

   [Header("Prefab")]
    public GameObject shellPrefab;

    [Header("Instances on the canvas to set")]
    public Image weaponImage;
    public Image crossHairImage;

    public Image lockonCrossHairImage;
    public GameObject shellDisplayMaster;

    public Text nameHolder;
    public Text reloadProgress;

    public Text clipCount;


    
    // Start is called before the first frame update
    public void displayLockOn(bool set){
        lockonCrossHairImage.enabled = set;
    }
    public void setWeaponIcon(Sprite icon){
        weaponIcon = icon;
        weaponImage.sprite = weaponIcon;
    }

    public void setshellIcon(Sprite icon){
        shellIcon = icon;
    }
    public void setCrossHair(Sprite crosshair){
        crossHair = crosshair;
        crossHairImage.sprite = crossHair;
    }
    public void setLockOnCrossHair(Sprite lockOnCrosshairLocal){
        lockonCrossHair = lockOnCrosshairLocal;
        lockonCrossHairImage.sprite = lockonCrossHair;
    }
    public void setName(string name){
        weaponName = name;
        nameHolder.text = weaponName;
    }

    public void displayReloadProgress(float cooldown){
        if(cooldown < 0.1)reloadProgress.text = "0";
        else reloadProgress.text = Mathf.FloorToInt(cooldown + 1).ToString();
    }
    public void displayCurrentShells(int amt, int max){
        clipCount.text = amt.ToString() + "/" + max.ToString();
        foreach(Transform child in shellDisplayMaster.transform){
            Destroy(child.gameObject);
        }
        for (int i = 0; i < amt; i++){
            GameObject shell = Instantiate(shellPrefab, shellDisplayMaster.transform);
            shell.transform.position = shellDisplayMaster.transform.position;
            shell.GetComponent<RectTransform>().localPosition = Vector3.zero + new Vector3(shell.GetComponent<RectTransform>().rect.width * 2 *i, 0, 0);
            shell.GetComponent<Image>().sprite = shellIcon;
        }
        
    }
    void Start()
    {
        parentWeapon = GetComponentInParent<Weapon>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(parentWeapon.selected == false) gameObject.SetActive(false);
    }
}
