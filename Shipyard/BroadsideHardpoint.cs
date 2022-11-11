using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadsideHardpoint : WeaponHardpoint
{
    // Start is called before the first frame update
    public override void showAttachableItems(){
        attachableItems.Clear();
        foreach(Equipment item1 in myShipyard.allEquipment){
            switch (item1){
                case MountedBroadside a:
                    if(a.equipmentSize == Equipment.partSize.Large && largeWeapons)attachableItems.Add(item1);
                    else if(a.equipmentSize == Equipment.partSize.Medium && mediumWeapons)attachableItems.Add(item1);
                    else if(a.equipmentSize == Equipment.partSize.Small && smallWeapons)attachableItems.Add(item1);
                break;
            }
        }
        if(attachableItems.Count == 0) Debug.LogError("wtf you have no attachableItems on weaponHardpoint");

    }
}
