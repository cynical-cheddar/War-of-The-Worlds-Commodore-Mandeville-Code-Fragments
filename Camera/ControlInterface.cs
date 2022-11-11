using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ControlInterface : MonoBehaviour
{
    // Start is called before the first frame update

    //foreach ability object, create a button to do it
  //  public List<GameObject> standardAbilityObjects;


    public GameObject abilityButtonPrefab;
    List<Equipment> abilityObjects = new List<Equipment>();

    public RadialSlider rollControl;
    CaptialShipControl controls;

    public GameObject velocitySliderGo;
    public GameObject fuelGauge;


    float curSpeed = 0f;

    bool aheadFullEngaged = false;

    public UiBar shipHpBar;

    bool drawingArc = false;
    bool turning = false;

    [SerializeField]
    List<Vector3> arcVectors = new List<Vector3>(); 
    public float arcRadius = 10f;

    public Transform shipTransform;

    public GameObject turnCollider;
    GameObject turnColliderInstance;

    public LineRenderer lineDir;

    float angleDifference = 0f;
    Vector3 targetVectorRot = Vector3.zero;
    float shipRot = 0f;
    Vector3 compassPos = Vector3.zero;
    Vector3 shipPosStart = Vector3.zero;
    float vAngle;
    Vector3 dir = Vector3.forward;
     Vector3 point2 = Vector3.zero;

     public string curAxis = "y";

    public void addAbilityObject(Equipment abilityEquipment){
        abilityObjects.Add(abilityEquipment);
        displayAbilityIcons();
    }
    void displayAbilityIcons(){
        Transform abilitymaster = GetComponentInChildren<AbilityMaster>().transform;
        foreach(Transform child in abilitymaster){
            Destroy(child.gameObject);
        }
        foreach(Equipment e in abilityObjects){
            // create a button with a reference to the equipment item
            GameObject buttonInstance = Instantiate(abilityButtonPrefab, abilitymaster);
            buttonInstance.GetComponent<AbilityButton>().setAbilityObject(e);
            buttonInstance.GetComponent<Image>().sprite = e.weaponIcon;
        }
    }
    private void Start() {
        Physics.IgnoreLayerCollision(0, 9);
        controls  = GetComponentInParent<CaptialShipControl>();
    }

    public void tryToggleFire(){
        controls.toggleHoldFire();
    }

    public void setHealthBar(float currentHp, float maxHp){
        if(shipHpBar != null){
            Debug.Log("setting health bar2");
            float fraction = currentHp/maxHp;
            shipHpBar.setProgressBar(fraction);
            shipHpBar.setnumber(currentHp.ToString());
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(rollControl!=null){
            controls.setTargetRollDegrees(rollControl.getRollAngle());
        }
        
        if(Input.GetButtonDown("HeTurnV") && controls.currentlyControlled){
            Debug.Log("HAAAAAAAAAA");
            if(drawingArc){endTurnDraw(); endTurn();}
            else if(!turning) startTurnDraw("y");
            else if(turning) {endTurnDraw(); endTurn();}
            
        }
        if(Input.GetButtonDown("HeTurnH") && controls.currentlyControlled){
            if(drawingArc){endTurnDraw(); endTurn();}
            else if(!turning) startTurnDraw("x");
            else if(turning) {endTurnDraw(); endTurn();}
           
        }
        compassPos = shipTransform.position + shipTransform.rotation*shipTransform.GetComponent<Rigidbody>().centerOfMass;
       
        
        if(drawingArc){
            if(turnCollider != null){
               // turnColliderInstance.transform.position = shipTransform.GetComponent<Rigidbody>().centerOfMass;
                
                turnColliderInstance.transform.position = compassPos;
                shipPosStart = compassPos;
                if(curAxis == "y") turnColliderInstance.transform.rotation = shipTransform.rotation;
                if(curAxis == "x") turnColliderInstance.transform.rotation = Quaternion.Euler(shipTransform.rotation.eulerAngles.x, shipTransform.rotation.eulerAngles.y, shipTransform.rotation.eulerAngles.z + 90);
                lineDir = turnColliderInstance.GetComponentInChildren<LineRenderer>();
            }
            arcVectors.Clear();

            arcVectors.Add(compassPos);
            


            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9)){
                dir = (hit.point - compassPos).normalized;
                point2 = compassPos + dir * arcRadius;
                arcVectors.Add(point2);
                if(curAxis == "y") angleDifference = Vector3.SignedAngle(shipTransform.forward, point2 - shipTransform.position , shipTransform.up);
                else if(curAxis =="x") angleDifference = Vector3.SignedAngle(shipTransform.forward, point2 - shipTransform.position , shipTransform.right);
            }
            if(lineDir != null){
                lineDir.SetPosition(0, compassPos);
                lineDir.SetPosition(1, point2);
            }
        }

        if(drawingArc){
            if(Input.GetMouseButtonDown(0)){
                endTurnDraw();
                if(curAxis == "y") beginTurn(new Vector3(shipTransform.rotation.x, shipTransform.rotation.y + (angleDifference), shipTransform.rotation.z));
                else if(curAxis == "x") beginTurn(new Vector3(shipTransform.rotation.x + (angleDifference), shipTransform.rotation.y, shipTransform.rotation.z));
            }
        }
        if(!drawingArc && turning){
            if(turnCollider != null){
                turnColliderInstance.transform.position = compassPos;

            }
            if(lineDir != null){
                lineDir.SetPosition(0, compassPos);
                lineDir.SetPosition(1, compassPos + dir * arcRadius);
                arcVectors[1] = compassPos + dir * arcRadius;
            }
            shipRot =  shipTransform.eulerAngles.y;
            //if(shipRot <0) shipRot = shipRot + 360;
           // if(shipRot < targetVectorRot.y + 15 && shipRot > targetVectorRot.y - 15){
          //      endTurn();
          //  }
          vAngle = Vector3.Angle(shipTransform.forward, Vector3.Normalize(point2 - shipPosStart));
              if( vAngle < 10f){
                  endTurn();
              }
           }
    }
    
    void FixedUpdate()
    {
        if(!drawingArc && turning){
            vAngle = Vector3.Angle(shipTransform.forward, Vector3.Normalize(point2 - shipPosStart));
              if( vAngle < 10f){
                  endTurn();
              }
            //  if(Input.GetButtonDown("HeTurnH"))endTurn();
            //  if(Input.GetButtonDown("HeTurnV"))endTurn();
         }
     }
    public void startTurnDraw(string axis){
        if(drawingArc != true && !turning){
            drawingArc = true;
            compassPos = shipTransform.position + shipTransform.rotation*shipTransform.GetComponent<Rigidbody>().centerOfMass;
            if(turnColliderInstance!= null) Destroy(turnColliderInstance);
            if(axis == "y"){turnColliderInstance = Instantiate(turnCollider, compassPos, shipTransform.rotation); curAxis = "y";}
            if(axis == "x"){turnColliderInstance = Instantiate(turnCollider, compassPos, Quaternion.Euler(shipTransform.rotation.eulerAngles.x, shipTransform.rotation.eulerAngles.y, shipTransform.rotation.eulerAngles.z + 90)); curAxis = "x";}
            
        }
        
    }
    public void endTurnDraw(){
        if(drawingArc == true){
            drawingArc = false;
            
        }
    }
    public void beginTurn(Vector3 targetRotation){
        float dirSign = 0f;
        if(curAxis == "y") dirSign = (targetRotation.y - shipTransform.rotation.y);
        else if(curAxis == "x") dirSign = (targetRotation.x - shipTransform.rotation.x);
        targetVectorRot = targetRotation;

        // angleDifference
        /*float newY = shipTransform.rotation.y + angleDifference;
        if(newY < 0){
            //difference between ship rot and zero
            float difToZero = shipTransform.rotation.y;
            float flow = Mathf.Abs(angleDifference) - difToZero;
            targetVectorRot = new Vector3(targetVectorRot.x, 360-flow, targetVectorRot.z);
        }
        else if(newY > 360){
            float difTo360 = 360 - shipTransform.rotation.y;
            float flow = Mathf.Abs(angleDifference) - difTo360;
            targetVectorRot = new Vector3(targetVectorRot.x, flow, targetVectorRot.z);
        }*/

        
        controls.setHighEnergyTurn(true, dirSign, curAxis);
        turning = true;
    }
    
    public void endTurn(){
        Destroy(turnColliderInstance);
        controls.setHighEnergyTurn(false, 0, curAxis);
        turning = false;
    }
    
    public void OnSliderValueChanged(float value){
        curSpeed = value;
        controls.setSpeed(curSpeed);
    }
    public void toggleAheadFullButton(){
        setAheadFull(!aheadFullEngaged);
    }
    public void setAheadFull(bool isAheadFull){
        if(controls !=null){
            controls.setAheadFullEngaged(isAheadFull);
              aheadFullEngaged = isAheadFull;
             velocitySliderGo.GetComponent<Slider>().value = 1f;
        }
        
    }


    public void setFuel(float amt, float maxFuel){
        fuelGauge.GetComponent<RectTransform>().localScale = new Vector3(1, (amt/maxFuel), 1);
       // fuelGauge.GetComponent<RectTransform>().position = new Vector3(fuelGauge.GetComponent<RectTransform>().position.x, 0-(fuelGauge.GetComponent<RectTransform>().rect.height/2) -  , fuelGauge.GetComponent<RectTransform>().position.z)
    }
    public void updateSpeed(float fraction){
        if(aheadFullEngaged) controls.setAheadFullEngaged(true);
        //else controls.setSpeed(fraction);
        //velocitySlider.value = fraction;
    }
}
