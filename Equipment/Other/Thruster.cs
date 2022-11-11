using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : Equipment
{
    float thrustMultiplier =1f;
    public bool lateral;

    public bool right;

    public bool left;
    public bool vertical;

    public bool up;
    public bool down;

    public bool speed;
    public float thrusterForce = 20f;
    float thrusterForceDefault = 20f;
    public float thrusterForceMax = 20f;

    public AudioClip thrustStart;
    //public AudioSource thrustStartSource;

    public GameObject ThrustObject;

    public List<GameObject> thrustSpecialFx;
    

    float scaleFactorThruster = 1f;

    bool thrusting = false;
    bool full = false;

    Vector3 originalScale;
    bool disabled = false;

    float modifier = 1f;
    

    public float getCurrentThrusterForce(){
        return thrusterForce*thrustMultiplier;
    }
    public float getThrusterForceMax(){
        return thrusterForceMax*thrustMultiplier;
    }
    // Start is called before the first frame update
    public void setThrustMultiplier(float amt){
        thrustMultiplier = amt;
       // if(getCurrentThrusterForce() > 0.1)setScaleFactor(scaleFactorThruster);
        
    }
    public void addModifier(float value){
        modifier = modifier + (value - 1);
        if(modifier <= 0)modifier = 0;
    }
    public override void repairEquipment(){

        disabled = false;
    }
    public override void disableEquipment(){

        disabled = true;
    }
    void disableThrustSpecial(){
        foreach(GameObject fx in thrustSpecialFx){
            if(fx.GetComponent<ParticleSystem>()!= null)disableParticleFx(fx.GetComponent<ParticleSystem>());
            else{
                fx.SetActive(false);
                fx.transform.localScale = originalScale * 0;
                ThrustObject.GetComponent<AudioSource>().volume = 0;
            }
        }
    }
    void enableParticleFx(ParticleSystem thrustSpecial){
        foreach(Transform child in thrustSpecial.transform){
           if(child.gameObject.GetComponent<ParticleSystem>() != null){
               child.gameObject.GetComponent<ParticleSystem>().enableEmission = true;
               child.gameObject.GetComponent<ParticleSystem>().Play();
           }
        }
        thrustSpecial.enableEmission = true;
        thrustSpecial.Play();
    }
    void disableParticleFx(ParticleSystem thrustSpecial){
       foreach(Transform child in thrustSpecial.transform){
           if(child.gameObject.GetComponent<ParticleSystem>() != null){
               child.gameObject.GetComponent<ParticleSystem>().enableEmission = false;
              // child.gameObject.GetComponent<ParticleSystem>().Pause();
           }
        }
        thrustSpecial.enableEmission = false;
    }
    void enableThrustSpecial(){
        foreach(GameObject fx in thrustSpecialFx){
            if(fx.GetComponent<ParticleSystem>()!= null)enableParticleFx(fx.GetComponent<ParticleSystem>());
            else{
                fx.SetActive(true);
                fx.transform.localScale = originalScale * 1;
                ThrustObject.GetComponent<AudioSource>().volume = 1;
            }
        }

    }


    new void Start(){
        base.Start();
        getDataFromHardpoint();
        originalScale = ThrustObject.transform.localScale;
        disableThrustSpecial();
        thrusterForceDefault = thrusterForce;

        thrusterForceDefault*=10;
        thrusterForceMax*=10;
        thrusterForce*=10;
        setScaleFactor(0);
        Invoke("properStop", 0.5f);
    }
    void properStop(){
        setScaleFactor(0);
    }
    public void setScaleFactor(float scale){
        ThrustObject.transform.localScale = originalScale * scale * thrustMultiplier;
        
        if(ThrustObject.GetComponent<AudioSource>() != null) ThrustObject.GetComponent<AudioSource>().volume = 0;
    }
    void getDataFromHardpoint(){
        if(GetComponentInParent<HardpointThruster>() == null){
            //Hardpoint has no thruster script attached
            Debug.Log("Hardpoint has no thruster script attached");
        }
        else{
            HardpointThruster hp = GetComponentInParent<HardpointThruster>();
            lateral = hp.lateral;
            right = hp.right;
            left = hp.left;
            vertical = hp.vertical;
            up = hp.up;
            down = hp.down;
            speed = hp.speed;
        }
        GetComponentInParent<Ship>().initialiseControls();
    }
    public void enageThrust(float scale){
        setScaleFactor(scale);
        if(!thrusting){
            if(ThrustObject != null){
                
                ThrustObject.SetActive(true);
               // if(thrustStart != null)thrustStartSource.PlayOneShot(thrustStart);
                
                thrusting = true;
            }
        }
    }
    public void engageFull(){
        
        if(!full && !disabled){
            enageThrust(1);
            full = true;
            enableThrustSpecial();
        }
    }
    public void disengageFull(){
       // disengageThrust(0);
        if(full){
            full = false;
            disableThrustSpecial();
        }
    }
    public void disengageThrust(float scale){
        setScaleFactor(scale);
        if(thrusting){
            if(ThrustObject != null){
                ThrustObject.SetActive(false);
            // thrustStartSource.PlayOneShot(thrustStart);
                thrusting = false;
            }
        }
    }
}
