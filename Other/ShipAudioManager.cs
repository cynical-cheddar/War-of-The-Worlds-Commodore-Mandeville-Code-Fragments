using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAudioManager : MonoBehaviour
{
    public GameObject audioSourcePreset;
    public AudioClip aheadFullStart;
    AudioSource aheadFullStartSource;

    public AudioClip aheadFullLoop;
    AudioSource aheadFullLoopSource;

    public AudioClip highEnergyTurnStart;
    AudioSource highEnergyTurnStartSource;

    public AudioClip highEnergyTurnLoop;
    public AudioClip highEnergyTurnStopClip;
    AudioSource highEnergyTurnLoopSource;


    public AudioClip increaceVelocityFromStart;
    public AudioClip increaseVelocity;
    AudioSource increaseVelocitySource;

    public AudioClip decreaseVelocity;
    AudioSource decreaseVelocitySource;

    public AudioClip movementLoop;
    AudioSource movementLoopSource;

    CaptialShipControl myShipControls;

    bool turning = false;
    bool aheadFull = false;
    float curFraction = 0f;

    public bool playIffControlled = true;
    void Start()
    {
        myShipControls = GetComponent<CaptialShipControl>();
        if(audioSourcePreset != null){
            instantiateSources();
            movementLoopSource.clip = movementLoop;
            movementLoopSource.loop = true;
            movementLoopSource.volume = 0;
            movementLoopSource.Play();
        }   
        
    }


    public void highEnergyTurnPlay(){
        highEnergyTurnStartSource.PlayOneShot(highEnergyTurnStart);
        highEnergyTurnLoopSource.loop = true;
        highEnergyTurnLoopSource.clip = highEnergyTurnLoop;
        highEnergyTurnLoopSource.Play();
    }
    public void highEnergyTurnStop(){
        highEnergyTurnLoopSource.loop = false;
        if(highEnergyTurnStopClip != null) highEnergyTurnStartSource.clip = highEnergyTurnStopClip;
        highEnergyTurnLoopSource.Stop();
        highEnergyTurnStartSource.Play();
    }
    public void increaseVelocityPlay(){
        if(!increaseVelocitySource.isPlaying) increaseVelocitySource.PlayOneShot(increaseVelocity);
    }
    public void startMovePlay(){
        if(!increaseVelocitySource.isPlaying) increaseVelocitySource.PlayOneShot(increaceVelocityFromStart);
    }


    void calculateMovementLoopVolume(){
        if(Time.timeScale < 0.02) movementLoopSource.volume = 0;
        else if(myShipControls.currentlyControlled)movementLoopSource.volume = myShipControls.curFraction / 3;
        else movementLoopSource.volume = 0;
    }

    void instantiateSources(){
        aheadFullLoopSource = instantiateSource();
        aheadFullLoopSource = instantiateSource();
        highEnergyTurnStartSource = instantiateSource();
        highEnergyTurnLoopSource = instantiateSource();
        increaseVelocitySource = instantiateSource();
        decreaseVelocitySource = instantiateSource();
        movementLoopSource= instantiateSource();
        
    }

    AudioSource instantiateSource(){
        GameObject a = Instantiate(audioSourcePreset, transform.position, Quaternion.identity);
        a.transform.parent = transform;
        return a.GetComponent<AudioSource>();
    }



    // Update is called once per frame
    void Update()
    {
        if(playIffControlled && myShipControls.currentlyControlled){
            calculateMovementLoopVolume();
            if(myShipControls.aheadFullEngaged != aheadFull){
                aheadFull = myShipControls.aheadFullEngaged;
                if(aheadFull == true) highEnergyTurnPlay();
                if(aheadFull == false) highEnergyTurnStop();
            }
            if(myShipControls.highEnergyTurnEngaged != turning){
                turning = myShipControls.highEnergyTurnEngaged;
                if(turning == true) highEnergyTurnPlay();
                if(turning == false) highEnergyTurnStop();
            }   
            
            if(myShipControls.curFraction > 0 && curFraction == 0){
                startMovePlay();
                curFraction = myShipControls.curFraction;
            }

            else if(myShipControls.curFraction > curFraction + 0.2){
                increaseVelocityPlay();
                curFraction = myShipControls.curFraction;
            }
        }

    }
}
