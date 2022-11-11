using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
public class WeaponHardpoint : Hardpoint
{
    public bool autoAim = true;

    public bool fixedWeapons = false;
    public bool largeWeapons = true;
    public bool mediumWeapons = true;
    public bool smallWeapons = true;

    public bool scienceWeapons = false;

    public bool horns = false;
    public override void showAttachableItems(){
        attachableItems.Clear();
        foreach(Equipment item1 in myShipyard.allEquipment){
            switch (item1){
                case MountedTurret a:
                    
                    if(a.equipmentSize == Equipment.partSize.Large && largeWeapons)attachableItems.Add(item1);
                    else if(a.equipmentSize == Equipment.partSize.Medium && mediumWeapons)attachableItems.Add(item1);
                    else if(a.equipmentSize == Equipment.partSize.Small && smallWeapons)attachableItems.Add(item1);
                    else if(a.equipmentSize == Equipment.partSize.Fixed && fixedWeapons)attachableItems.Add(item1);
                    else if(a.equipmentSize == Equipment.partSize.Science && scienceWeapons)attachableItems.Add(item1);
                break;

                case TauntFunnel b:
                    if(horns) attachableItems.Add(item1);
                break;
            }
        }
        if(attachableItems.Count == 0) Debug.LogError("wtf you have no attachableItems on weaponHardpoint");
     //   Debug.LogError(attachableItems.Count + " attachableItems");
    }

    override protected void Start(){
        if(FindObjectOfType<shipyard>() != null){
            uiCanvas = FindObjectOfType<uiBarsCanvas>().GetComponent<Canvas>();
            hardPointUIinstance = Instantiate(Resources.Load("HardpointOverlay") as GameObject);
            hardPointUIinstance.transform.SetParent(uiCanvas.transform);
            hardPointUIinstance.transform.position = transform.position;
            hardPointUIinstance.transform.rotation = transform.rotation;
            myShipyard = FindObjectOfType<shipyard>();
            hardPointUIinstance.GetComponent<RectTransform>().localScale = Vector3.one;
            if(uibarIcon != null) hardPointUIinstance.GetComponent<Image>().sprite = uibarIcon;
            if(GetComponent<Collider>() == null){
                SphereCollider a = gameObject.AddComponent<SphereCollider>();
                a.radius = gameObject.GetComponent<Renderer>().bounds.size.magnitude * 10;
            } 
            if(FindObjectOfType<shipyard>() != null) showAttachableItems();
        }
    }

    

    // Update is called once per frame
    public override void requestSettings(GameObject weaponObject){
        if(ventralControl){
            weaponObject.GetComponent<WeaponUserController>().invertControls();
            weaponObject.GetComponent<WeaponUserController>().invertCamera();
            weaponObject.gameObject.transform.localScale = new Vector3(weaponObject.gameObject.transform.localScale.x, -weaponObject.gameObject.transform.localScale.y, weaponObject.gameObject.transform.localScale.z);
        }
        if(autoAim){
            if(weaponObject.GetComponent<Weapon>().allowAutoTarget && weaponObject.GetComponent<Weapon>().initiallyAutoControlled)weaponObject.GetComponent<WeaponUserController>().setAutoAimTrue();
            
        }
    }
}
