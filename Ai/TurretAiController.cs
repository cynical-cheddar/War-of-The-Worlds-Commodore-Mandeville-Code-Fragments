using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAiController : WeaponAiController
{
     
    // Start is called before the first frame update

    // using LateUpdate to avoid class mishmashing of methods
    public float angleFireTolerance = 10f;
    void LateUpdate()
    {
        if(target != null && GetComponent<Weapon>().autoControlled){
            
           Vector3 aimPos;
           if(target != null) aimPos = target.position;
           else if(target == transform) aimPos = transform.forward;
           else aimPos = transform.forward;
           if(fireControl) aimPos = calculateFireControl(target, controlledWeapon.muzzleVelocity, masterFirePoint.position);
           controlledWeapon.setLookDir(aimPos, false);
           if(controlledWeapon.canFire()){
               // check if, in a straight line from the barrel, the target is close enough by a dot product
              // masterFirePoint.transform.localRotation = originalFirepointRotation;
               Vector3 forwardGun = masterFirePoint.forward;
               Vector3 gunToTarget = aimPos - masterFirePoint.position;
               if(Vector3.Angle(forwardGun, gunToTarget) < angleFireTolerance && Vector3.Distance(masterFirePoint.position, target.position) < weaponRange){
                   
                   controlledWeapon.Fire();
               }
               
           }
        }
    }


}
