using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptialShipControl : Ship
{
    OrderManager orderManager;
    CameraLookController camControls;
    public Transform front;
    float targetRollDegrees = 0f;
    public bool torqueBasedRotation = false;
    public float torqueBasedRotationMultiplier = 10f;
    
    public float highEnergyTurnForceFactor = 25f;

    public float aheadFullForceFactor = 2f;
    public float fuelRegenRate = 0.2f;
    public float maxFuel = 10f;
    float currentFuel = 10f;
    public List<Transform> VerticalThrusters;
    public List<Transform> LateralThrusters;

    public List<Transform> SpeedThrusters;
	//public float MaxSpeed = 40f; 

    [SerializeField]


	private Rigidbody rb;

    public bool aheadFullEngaged = false;

    public float curFraction = 0f;

    public bool highEnergyTurnEngaged = false;

    bool turnLeft = false;
    bool turnRight = false;

    bool turnUp = false;
    bool turnDown = false;

    string turnAxis = "y";

    public bool holdFire = false;

    

	// Use this for initialization
	protected void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
        currentFuel = maxFuel;
        if(front == null) front = transform.Find("Front");
        if(FindObjectOfType<shipyard>() != null){
            rb.isKinematic = true;
         //   transform.position = Vector3.zero;
        } 
        addToSelectableShips();
        camControls = FindObjectOfType<CameraLookController>();
        orderManager = GetComponent<OrderManager>();
	}

    public void addToSelectableShips(){
         if(FindObjectOfType<ShipSelectInterface>()!=null && controllable){
            shipSelectInterface = FindObjectOfType<ShipSelectInterface>();
            shipSelectInterface.addShipToList(gameObject);
         } 
    }
    private void OnDestroy() {
        if(shipSelectInterface) shipSelectInterface.removeShipFromList(gameObject);
    }

    public void toggleHoldFire(){
        holdFire = !holdFire;
    }
    public void zeroRoll(){
        targetRollDegrees = 0f;
    }
    public void setTargetRollDegrees(float amt){
        targetRollDegrees = amt;
    }
    // Get the references to thrusters and other equipment.
    public override void initialiseControls(){
        //Find all thrusters;
        SpeedThrusters.Clear();
        VerticalThrusters.Clear();
        LateralThrusters.Clear();
        Thruster[] thrusters = GetComponentsInChildren<Thruster>();
        foreach(Thruster thruster in thrusters){
            if(thruster.speed) SpeedThrusters.Add(thruster.transform);
            if(thruster.vertical) VerticalThrusters.Add(thruster.transform);
            if(thruster.lateral) LateralThrusters.Add(thruster.transform);
           // thruster.disengageThrust(0);
           // thruster.disengageFull();
        }
        if(!currentlyControlled)setCurrentlyControlled(false);
        else setCurrentlyControlled(true);

    }

    public void setCurrentlyControlled(bool set){
        if(currentlyControlled!=set){
            if(FindObjectOfType<shipyard>() == null){
                currentlyControlled = set;
                if(!currentlyControlled){
                    controlUI.GetComponent<Canvas>().enabled = false;
                    if(GetComponentInChildren<selectedMarker>()!=null) GetComponentInChildren<selectedMarker>().gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                else{
                    controlUI.GetComponent<Canvas>().enabled = true;
                    if(GetComponentInChildren<selectedMarker>()!=null){
                        GetComponentInChildren<selectedMarker>().gameObject.GetComponent<MeshRenderer>().enabled = true;
                    // GetComponentInChildren<selectedMarker>().gameObject.transform.position = GetComponent<Rigidbody>().centerOfMass;
                    } 
                }
            }
        }
    }

    public void setAheadFullEngaged(bool set){
        aheadFullEngaged = set;
        if(set == false){
             foreach(Transform thruster in SpeedThrusters){
                Thruster t = thruster.GetComponent<Thruster>();
                t.disengageFull();
            }
        }
    }
   // public void setAheadFull(){
   //     if(aheadFullEngaged){
   //     }
   // }
     public void setHighEnergyTurn(bool set, float angle, string axis){
        turnAxis = axis;
        if(highEnergyTurnEngaged != set){
            setAheadFullEngaged(false);
            highEnergyTurnEngaged = set;
            if(set){
                if(turnAxis == "y"){
                    turnUp = false; turnDown = false;
                    if(angle >0) {turnRight = true; turnLeft = false;}
                    if(angle <0) {turnRight = false; turnLeft = true;}
                }
                else if(turnAxis == "x"){
                    turnRight = false; turnLeft = false;
                    if(angle >0) {turnUp = true; turnDown = false;}
                    if(angle <0) {turnUp = false; turnDown = true;}
                }
            }
            else{
                turnLeft = false;
                turnRight = false;
                turnUp = false;
                turnDown = false;
            }

            if(axis == "y"){
                if(set == false){
                    foreach(Transform thruster in LateralThrusters){
                        Thruster t = thruster.GetComponent<Thruster>();
                        t.disengageFull();
                    }
                }
            }
            else if(axis == "x"){
                 if(set == false){
                    foreach(Transform thruster in VerticalThrusters){
                        Thruster t = thruster.GetComponent<Thruster>();
                        t.disengageFull();
                    }
                }
            }
        }
    }
  
    public void setSpeed(float fraction){
        curFraction = fraction;
    }
    void standardMove(float fraction){
        if(!aheadFullEngaged){
            foreach(Transform thruster in SpeedThrusters){
                Thruster t = thruster.GetComponent<Thruster>();
                float f = Mathf.Lerp(0, t.getCurrentThrusterForce(), fraction);
                if(fraction > 0) t.enageThrust(fraction);
                else t.disengageThrust(0f);

                // now actually do the force thing
                Vector3 direction = transform.forward * f;
            // rb.AddForceAtPosition(direction.normalized, thruster.position, ForceMode.Impulse);
                rb.AddForce(direction, ForceMode.Impulse);
            }
        }
    }

    void highEnergyTurnMove(){
        currentFuel -= Time.deltaTime;
        if(currentFuel > 0){
            if(turnAxis == "y"){
                foreach(Transform thruster in LateralThrusters){
                    
                    Thruster t = thruster.GetComponent<Thruster>();
                    if(turnLeft && t.left){
                        t.engageFull();
                        //t.enageThrust(1f);

                        rb.AddTorque(transform.up * t.getThrusterForceMax() * -highEnergyTurnForceFactor, ForceMode.Impulse);
                    //    rb.AddForceAtPosition(thruster.forward * t.thrusterForceMax * -4 , thruster.position, ForceMode.Impulse);
                    }
                    else if(turnRight && t.right){
                        t.engageFull();
                        //t.enageThrust(1f);

                        rb.AddTorque(transform.up  * t.getThrusterForceMax() * highEnergyTurnForceFactor, ForceMode.Impulse);
                    //  rb.AddForceAtPosition(thruster.forward * t.thrusterForceMax * -4, thruster.position, ForceMode.Impulse);
                    }
                }
            }
            else if(turnAxis == "x"){
                 foreach(Transform thruster in VerticalThrusters){
                    
                    Thruster t = thruster.GetComponent<Thruster>();
                    if(turnUp && t.down){
                        t.engageFull();
                        //t.enageThrust(1f);

                        rb.AddTorque(transform.right * t.getThrusterForceMax() * highEnergyTurnForceFactor, ForceMode.Impulse);
                    //    rb.AddForceAtPosition(thruster.forward * t.thrusterForceMax * -4 , thruster.position, ForceMode.Impulse);
                    }
                    else if(turnDown && t.up){
                        t.engageFull();
                        //t.enageThrust(1f);

                        rb.AddTorque(transform.right  * t.getThrusterForceMax() * -highEnergyTurnForceFactor, ForceMode.Impulse);
                    //  rb.AddForceAtPosition(thruster.forward * t.thrusterForceMax * -4, thruster.position, ForceMode.Impulse);
                    }
                }
            }
        }
        else{
            setHighEnergyTurn(false, 0, turnAxis);
            controlUI.GetComponent<ControlInterface>().endTurn();
        }
    }

    void aheadFullMove(){
        currentFuel -= Time.deltaTime;
        if(currentFuel > 0){
            foreach(Transform thruster in SpeedThrusters){
                Thruster t = thruster.GetComponent<Thruster>();
                t.engageFull();
                // now actually do the force thing
                Vector3 direction = transform.forward * t.getThrusterForceMax() * aheadFullForceFactor;
                // rb.AddForceAtPosition(direction.normalized, thruster.position, ForceMode.Impulse);
                rb.AddForce(direction, ForceMode.Impulse);
            }
        }
        else{
            setAheadFullEngaged(false);
            controlUI.GetComponent<ControlInterface>().setAheadFull(false);
        }
    }
    void Update()
    {
        
        

    }
    void FixedUpdate(){
        if(currentlyControlled && !highEnergyTurnEngaged && Time.timeScale > 0.1 && !camControls.strategicCamera && !camControls.superStrategicCamera){
            // use our movement controller
            float ControlHorizontal = Input.GetAxis ("Horizontal")*-1;
		    float ControlVertical = Input.GetAxis ("Vertical");
            if(ControlHorizontal != 0) orderManager.cancel();
            if(ControlVertical != 0) orderManager.cancel();
            moveUser(ControlHorizontal, ControlVertical);
        }
        
        if(currentFuel < maxFuel) currentFuel += fuelRegenRate * Time.deltaTime;
        if(controlUI != null){controlUI.GetComponent<ControlInterface>().setFuel(currentFuel, maxFuel);}
        if(aheadFullEngaged){
            setHighEnergyTurn(false, 0, turnAxis);
            aheadFullMove();
           
        }
        if(highEnergyTurnEngaged) highEnergyTurnMove();
        standardMove(curFraction);
        //YAW CODE.
        if(!highEnergyTurnEngaged){
        var r = transform.eulerAngles;
        if(Mathf.Abs(r.z - targetRollDegrees) > 3f)MoveRotationTorque(rb, Quaternion.Euler(r.x, r.y, targetRollDegrees));
        }
    }
    void disengageThrusters(List<Transform> disList){
        foreach(Transform lThruster in disList){
            Thruster t = lThruster.GetComponent<Thruster>();
            t.disengageThrust(0f);
        }
    }
    float curH;
    float curV;
    

    public void moveUser(float ControlHorizontal, float ControlVertical){
        curH = ControlHorizontal;
        curV = ControlVertical;
		//ELSE ASSIGN VIA AI

      //  Vector3 vDiff = transform.forward * MaxSpeed * ControlThrust - rb.velocity; //Difference between current velocity and intended velocity.

	//	rb.AddForce (vDiff , ForceMode.VelocityChange);
        
        if(ControlHorizontal == 0)disengageThrusters(LateralThrusters);
        if(ControlVertical == 0)disengageThrusters(VerticalThrusters);
        // Turn
        if(ControlVertical != 0){
            int dir = 1;
            // Add a torque at the thruster
            foreach(Transform vThruster in VerticalThrusters){
                Vector3 direction = Vector3.zero;
                Thruster t = vThruster.GetComponent<Thruster>();
                // up
                if(ControlVertical >0){
                    if(t.up){
                        dir = -1;
                        direction = vThruster.forward * t.getCurrentThrusterForce() * ControlVertical * dir;
                        t.enageThrust(1f);
                    }
                    else{
                        t.disengageThrust(0f);
                    }
                }
                else if(ControlVertical < 0){
                    if(t.down){
                        dir = 1;
                        direction = vThruster.forward * t.getCurrentThrusterForce() * ControlVertical * dir;
                        t.enageThrust(1f);
                    }
                    else{
                        t.disengageThrust(0f);
                    }
                }
                
                if(!torqueBasedRotation) rb.AddForceAtPosition(direction, vThruster.position, ForceMode.Impulse);
                if(torqueBasedRotation){
                    rb.AddTorque(torqueBasedRotationMultiplier* transform.right  * direction.magnitude  * dir * Vector3.Magnitude(vThruster.position - (transform.position + transform.rotation*transform.GetComponent<Rigidbody>().centerOfMass)), ForceMode.Impulse);
                  //  Debug.Log("Adding force");
                } 
            }
            

        }
        if(ControlHorizontal != 0){
            int dir = 1;
            foreach(Transform lThruster in LateralThrusters){
                Vector3 direction = Vector3.zero;
                // Get the thrusters that corresponnt to our turning
                Thruster t = lThruster.GetComponent<Thruster>();
                //Right
                if(ControlHorizontal > 0){
                    //Get all left thrusters
                    if(t.left){
                        dir = -1;
                        direction = lThruster.forward * t.getCurrentThrusterForce() * ControlHorizontal * -1;
                     //   Debug.Log(direction.magnitude);
                        t.enageThrust(1f);
                    }
                    else{
                        t.disengageThrust(0f);
                    }
                }

                //Left
                else if(ControlHorizontal < 0){
                    if(t.right){
                        dir = 1;
                        direction = lThruster.forward * t.getCurrentThrusterForce() * ControlHorizontal;
                        t.enageThrust(1f);
                    }
                    else{
                        t.disengageThrust(0f);
                    }
                }     
                if (!torqueBasedRotation)rb.AddForceAtPosition(direction, lThruster.position, ForceMode.Impulse);
                if(torqueBasedRotation) rb.AddTorque( torqueBasedRotationMultiplier*transform.up  * direction.magnitude  *dir* Vector3.Magnitude(lThruster.position - (transform.position + transform.rotation*transform.GetComponent<Rigidbody>().centerOfMass)), ForceMode.Impulse);
            }
            
        }

        
        


		
    }

    
    public void MoveRotationTorque(Rigidbody rigidbody, Quaternion targetRotation)
     {
         rigidbody.maxAngularVelocity = 1000;
 
         Quaternion rotation = targetRotation * Quaternion.Inverse(rigidbody.rotation);
         var torque = new Vector3(rotation.x, rotation.y, rotation.z) * rotation.w * Time.fixedDeltaTime * 20;

         rigidbody.AddTorque(torque, ForceMode.VelocityChange);
        // rigidbody.angularVelocity = Vector3.zero;
        rigidbody.SetMaxAngularVelocity(1f);
     }


    private Vector3 RectifyAngleDifference(Vector3 angdiff)
 {
     if (angdiff.x > 180) angdiff.x -= 360;
     if (angdiff.y > 180) angdiff.y -= 360;
     if (angdiff.z > 180) angdiff.z -= 360;
     return angdiff;
 }
    IEnumerator rotationlerp(float to, float duration){
        float durationElapsed = 0f;

        while (durationElapsed <= duration){
            var r = transform.eulerAngles;
            float lerp = Mathf.LerpAngle(r.z, to, Time.deltaTime);
            lerp = r.z - lerp;
           if (lerp > 180) lerp -= 360;
           // transform.rotation = Quaternion.Euler(r.x, r.y, lerp);
            transform.Rotate(new Vector3(0f,0f, lerp), Space.Self);
            durationElapsed+=Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
       

    }
}
