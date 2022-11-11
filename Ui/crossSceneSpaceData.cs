using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class crossSceneSpaceData {
    public enum SpaceType{
        empty,
        blueNebula,
        redNebula,
        planetary,
        special

    }

    public static SpaceType nextSpaceType { get; set; }

   

}