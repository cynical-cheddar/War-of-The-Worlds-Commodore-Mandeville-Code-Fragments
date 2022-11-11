using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class partButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected shipyard hostShipyard;
    public string componentName = "unnamedHull";
    protected GameObject part;

    protected List<string> partInfo = new List<string>();

    protected bool mouseOver = false;

    public void setMouseOver(bool set){
        mouseOver = set;
    }
    protected void Start()
    {
        hostShipyard = GetComponentInParent<shipyard>();
        componentName = componentName.Replace("(Clone)","").Trim();
        part = Resources.Load(componentName) as GameObject;
        if(part.GetComponent<Weapon>() != null){
            // if the part in question is a piece of equipment, get its details;
            partInfo = part.GetComponent<Weapon>().getStats();
        }
        if(part.GetComponent<Module>() != null){
            // if the part in question is a piece of equipment, get its details;
            partInfo = part.GetComponent<Module>().getStats();
        }
    }
    

    public void displayInfo(){
        if(partInfo.Count > 0) hostShipyard.displayEquipmentStats(partInfo);
    }

    protected void LateUpdate(){
        
        if(mouseOver)displayInfo();
        
    }
    public void OnPointerEnter(PointerEventData eventData)
     {

         mouseOver = true;
     }
     public void OnPointerExit(PointerEventData eventData)
     {
         mouseOver = false;
     }

}
