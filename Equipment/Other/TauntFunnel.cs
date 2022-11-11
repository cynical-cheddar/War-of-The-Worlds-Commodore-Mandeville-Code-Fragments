using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntFunnel : mountableEquipment
{
    public AudioClip[] funnelSound;
    public AudioSource audioSource;

    public GameObject smokeFxPrefab;
    public GameObject smokeFxPrefabNoRIng;

    public Transform smokeFxPoint;
    public void funnelFx(){
        Invoke("doFxOnceRing", 0f);
        Invoke("doFxOnce", 3f);
        Invoke("doFxOnce", 6f);
        Invoke("doFxOnce", 10f);
    }
    void doFxOnce(){
        audioSource.PlayOneShot(funnelSound[Random.Range(0, funnelSound.Length-1)]);
        GameObject s = Instantiate(smokeFxPrefabNoRIng, smokeFxPoint.position, smokeFxPoint.rotation);
        s.transform.SetParent(smokeFxPoint);
        Destroy(s, 3f);
    }
    void doFxOnceRing(){
        audioSource.PlayOneShot(funnelSound[Random.Range(0, funnelSound.Length-1)]);
        GameObject s = Instantiate(smokeFxPrefab, smokeFxPoint.position, smokeFxPoint.rotation);
        s.transform.SetParent(smokeFxPoint);
        Destroy(s, 3f);
    }

    public override void doAbility(){
        GetComponent<Ability>().doAbility();
        GetComponent<Ability>().doneAbility();
        
    }
}
