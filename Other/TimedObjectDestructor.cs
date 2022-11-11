using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestructor : MonoBehaviour
{
    // Start is called before the first frame update
    public float time = 25f;
    void Start()
    {
        Destroy(gameObject, time);
    }
}
