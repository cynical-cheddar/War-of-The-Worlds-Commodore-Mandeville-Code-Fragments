using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
public class ShipInstantiator : MonoBehaviour
{
    // Start is called before the first frame update

    public string fleetFilePath;
    public List<string> shipPathsToSpawn;

    public Transform spawnPointMaster;
    List<Transform> spawnPoints = new List<Transform>();

    public GameObject shipBasePrefab;

    private void Awake() {
        fleetFilePath = crossSceneShipData.fleetFilePath;
        if(fleetFilePath == null) fleetFilePath = Application.persistentDataPath + "/" + FindObjectOfType<FleetManager>().relativeFleetFolder;
        if(fleetFilePath == "") fleetFilePath = Application.persistentDataPath + "/" + FindObjectOfType<FleetManager>().relativeFleetFolder;
        loadDirectory();
    }

    private void Start() {
        Invoke("spawnFleet", 0.1f);
    }

    void loadDirectory(){
        string fleetFolder = fleetFilePath;
        Debug.Log(fleetFolder);
        string shipName = "";
        // load ships in the folder
        
        if(Directory.Exists(fleetFolder)){
            DirectoryInfo dir = new DirectoryInfo(fleetFolder);
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info){
                if(f.Extension == ".txt"){
                    string path = f.FullName;
                    
                    if(path != "" && f.Name != "persist.txt"){
                        StreamReader inp_stm = new StreamReader(path);
                        int i = 0;

                        while(!inp_stm.EndOfStream)
                        {
                            string inp_ln = inp_stm.ReadLine();
                            // Do Something with the input. 
                            if(i== 0) shipName = inp_ln;
                            //else {moduleDescription.Add(inp_ln); modulesRecords.Add(inp_ln);}
                            i++;
                            
                        }
                        shipPathsToSpawn.Add(path);
                        inp_stm.Close( );  
                        
                    }
                }
            }
        }
        // it doesn't exist, then make the folder
        else{
            var folder = Directory.CreateDirectory(fleetFolder); // returns a DirectoryInfo object
        }
    }
    

    public void spawnFleet(){
        int i = 0;
        spawnPoints.Clear();
        foreach(Transform child in spawnPointMaster){
            spawnPoints.Add(child);
        }
        foreach(string path in shipPathsToSpawn){
            GameObject shipBaseInstance = shipBasePrefab;
            
            shipBaseInstance.GetComponent<shipBuilderPlayer>().myFilePath = path;
            GameObject a = Instantiate(shipBaseInstance, spawnPoints[i].position, spawnPoints[i].rotation);
            a.name = Path.GetFileNameWithoutExtension(path); 
            i++;
        }
    }

    // Update is called once per frame
    public void setShipPathsToSpawn(List<string> l){
        shipPathsToSpawn = l;
    }
}
