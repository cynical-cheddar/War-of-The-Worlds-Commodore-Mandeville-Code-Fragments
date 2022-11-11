using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BountyDisplay : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject bountyTextPrefab;
    
    public void displayBounty(int amt){
        GameObject objectiveTextInstance = Instantiate(bountyTextPrefab, transform);
        objectiveTextInstance.GetComponent<Text>().text = "Bounty: £" + amt;
        Destroy(objectiveTextInstance, 2f);
    }
}
