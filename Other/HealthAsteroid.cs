using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAsteroid : Health
{
    // Start is called before the first frame update
    public GameObject corpse;

    private void OnTriggerEnter(Collider other)
    {
        if(GetComponent<Rigidbody>() == null){
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 30f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

    }   
    public override void die(){
        GameObject go = Instantiate(corpse, transform.position, transform.rotation);
        go.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
}
