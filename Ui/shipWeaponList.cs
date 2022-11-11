using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipWeaponList : MonoBehaviour
{

    public List<Weapon> weaponList = new List<Weapon>();
    public int currentWeapon = 0;
    int weaponCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        refreshList();
    }

    

    public void nextWeapon(Vector3 prevAimPos){
        Debug.Log("NextWeapon");
        //disable current weapon
        int index = currentWeapon;
        int old = currentWeapon;
        weaponList[currentWeapon].semiDeselectWeapon();

        // enable next weapon
        
        index += 1;
        if(index >= weaponCount){
            index = 0;
        }

       // if(weaponList[index].equipmentSize == Equipment.partSize.Small) 
        StartCoroutine(selectWeaponNextFrame(index, old, prevAimPos));
        currentWeapon = index;
    }

    IEnumerator selectWeaponNextFrame(int index, int old, Vector3 prevAimPos)
    {
        //returning 0 will make it wait 1 frame
        weaponList[old].selected = false;
        yield return 0;
        weaponList[index].selectWeapon(prevAimPos);
        weaponList[old].selected = false;
    }
     public void previousWeapon(Vector3 prevAimPos){
        //disable current weapon
        int index = currentWeapon;
        int old = currentWeapon;
        weaponList[currentWeapon].semiDeselectWeapon();

        // enable next weapon
        
        index -= 1;
        if(index < 0){
            index = weaponCount-1;
        }
        StartCoroutine(selectWeaponNextFrame(index, old, prevAimPos));
        currentWeapon = index;
    }

    public void calculateCurrentWeaponIndex(){
     /*   int i = 0;
        foreach(Weapon weapon in weaponList){
            if(weapon.selected) currentWeapon = i;
            else i++;
        }*/

    }
    public void refreshList(){
        weaponList.Clear();
        Weapon[] arr = GetComponentsInChildren<Weapon>();
        foreach(Weapon w in arr){
            if(w.equipmentSize != Equipment.partSize.Small && w.switchable){
                weaponList.Add(w);
            }
            
        }
        weaponCount = weaponList.Count;
        
    }


}
