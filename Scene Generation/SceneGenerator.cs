using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using System.Linq;
public class SceneGenerator : MonoBehaviour
{
    //given a set of values, generate a space scene

    public bool readSceneTypeFromCrossSceneData = true;
    GameObject spaceSceneInstance;

    public string emptySpaceFolder;
    public string nebulaBlueFolder;
    public string nebulaRedFolder;

    public string planetaryFolder;

    public string specialFolder;

    GameObject[] prefabs;
 

    public crossSceneSpaceData.SpaceType spaceType;

    public void setSpaceType(crossSceneSpaceData.SpaceType typeOfSpace){
        spaceType = typeOfSpace;
    }

    public void generateSpace(){
        Debug.Log("generator method called");
        switch(spaceType) 
        {
            case crossSceneSpaceData.SpaceType.empty:
                
                prefabs = Resources.LoadAll(emptySpaceFolder).Cast<GameObject>().ToArray();
                break;
            case crossSceneSpaceData.SpaceType.blueNebula:
                prefabs = Resources.LoadAll(nebulaBlueFolder).Cast<GameObject>().ToArray();
                break;
            case crossSceneSpaceData.SpaceType.redNebula:
                prefabs = Resources.LoadAll(nebulaRedFolder).Cast<GameObject>().ToArray();
                break;
            case crossSceneSpaceData.SpaceType.planetary:
                prefabs = Resources.LoadAll(planetaryFolder).Cast<GameObject>().ToArray();
                break;

            case crossSceneSpaceData.SpaceType.special:
                prefabs = Resources.LoadAll(specialFolder).Cast<GameObject>().ToArray();
                break;
            default:
                prefabs = Resources.LoadAll(emptySpaceFolder).Cast<GameObject>().ToArray();
                break;
        }

        // now select a random prefab from the array
        GameObject spacePrefab = prefabs[Random.Range(0, prefabs.Length-1)];
        spaceSceneInstance = Instantiate(spacePrefab, Vector3.zero, Quaternion.identity);

    }
    void Awake()
    {   
        if(readSceneTypeFromCrossSceneData){
            spaceType = crossSceneSpaceData.nextSpaceType;
            generateSpace();
        }
        else{
            generateSpace();
        }
        
    }

    public void SetAndGenerateSpace(crossSceneSpaceData.SpaceType typeSpace){
        setSpaceType(typeSpace);
        generateSpace();
    }

    // Update is called once per frame

}
