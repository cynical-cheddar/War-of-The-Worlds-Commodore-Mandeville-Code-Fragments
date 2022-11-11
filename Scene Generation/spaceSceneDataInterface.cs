using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spaceSceneDataInterface : MonoBehaviour
{
    public List<crossSceneSpaceData.SpaceType> spaceTypes;

    void Start(){
        crossSceneSpaceData.nextSpaceType = spaceTypes[0];
    }
    public void setNextSpaceType(int spaceTypeIndex){
        crossSceneSpaceData.SpaceType space = spaceTypes[spaceTypeIndex];
        crossSceneSpaceData.nextSpaceType = space;
    }
}
