using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDivertManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float baseReactorMultiplier = 1f;

    float enginesMultipler = 1f;
    float gunsMultiplier = 1f;
    float shieldsMultiplier = 1f;

    public float divertOverload= 1.5f;
    public void setMultipliers(Vector3 powerDistribution){
        // we have got a decimal representation of fractions
        //engines, guns, shields
        enginesMultipler = powerDistribution[0];
        gunsMultiplier = powerDistribution[1];
        shieldsMultiplier = powerDistribution[2];

        // get all things, set multiplier

        Thruster[] thrusters = GetComponentsInChildren<Thruster>();
        foreach(Thruster t in thrusters){
            t.setThrustMultiplier(enginesMultipler);
        }


        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach(Weapon w in weapons){
            w.setDamageMultiplier(gunsMultiplier);
            w.setReloadTimeMultiplier(gunsMultiplier);
        }

        ShieldHealth[] shields = GetComponentsInChildren<ShieldHealth>();
        foreach(ShieldHealth s in shields){
            s.setIncomingDamageMultiplier(1/(shieldsMultiplier));
        }
        
    }
}
