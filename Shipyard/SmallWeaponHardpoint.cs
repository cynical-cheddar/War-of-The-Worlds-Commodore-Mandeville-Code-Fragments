using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallWeaponHardpoint : WeaponHardpoint
{
    // Start is called before the first frame update
    




    public override void showAttachableItems(){
        largeWeapons = false;
        mediumWeapons = false; 
        smallWeapons = true;
        attachableItems.Clear();
        foreach(Equipment item1 in myShipyard.allEquipment){
            switch (item1){
                case MountedTurret a:
                    if(a.equipmentSize == Equipment.partSize.Large && largeWeapons)attachableItems.Add(item1);
                    else if(a.equipmentSize == Equipment.partSize.Medium && mediumWeapons)attachableItems.Add(item1);
                    else if(a.equipmentSize == Equipment.partSize.Small && smallWeapons)attachableItems.Add(item1);
                break;
            }
        }

    }

    

    private void Awake() {
       largeWeapons = false;
       mediumWeapons = false; 
       smallWeapons = true;
    }


    public override void requestSettings(GameObject weaponObject){
        if(ventralControl){
           
            weaponObject.GetComponent<WeaponUserController>().invertControls();
            weaponObject.GetComponent<WeaponUserController>().invertCamera();
            weaponObject.gameObject.transform.localScale = new Vector3(weaponObject.gameObject.transform.localScale.x, -weaponObject.gameObject.transform.localScale.y, weaponObject.gameObject.transform.localScale.z);
        }
        if(autoAim){
            weaponObject.GetComponent<WeaponUserController>().setAutoAimTrue();
        }
    }
}
