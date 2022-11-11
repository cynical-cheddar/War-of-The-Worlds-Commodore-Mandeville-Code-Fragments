using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGridOpacity : MonoBehaviour
{
    Material gridMat;
    // Start is called before the first frame update
    void Start()
    {
        gridMat = GetComponent<Renderer>().material;
    }

    public void setOpacity(float amt){
        gridMat.color = Color.Lerp(Color.black, Color.blue, amt);
    }
}
