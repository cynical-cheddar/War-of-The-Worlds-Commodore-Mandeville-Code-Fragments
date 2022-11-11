using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class FleetManager : MonoBehaviour
{

    
    
    // Start is called before the first frame update
    public List<string> defaultPersistInfo = new List<string>();
    public string relativeFleetFolder = "exampleFleet";
    public string fleetFolderFull;
    public string shipyardScene;
    public GameObject shipButtonPrefab;
    public Transform shipListMaster;
    public List<GameObject> shipButtons = new List<GameObject>();

    public int currentCredits = 6000;

    public Text creditsCount;

    List<string> shipPathsToSpawn = new List<string>();

    public string nextScene = "";
    public void enterBattle(){
        crossSceneShipData.fleetFilePath = fleetFolderFull;
        SceneManager.LoadScene(nextScene);
    }
    public void openShipyardNewShip(){
        crossSceneShipData.fleetFilePath = relativeFleetFolder;
        crossSceneShipData.CrossSceneInformation = "";
        SceneManager.LoadScene(shipyardScene);
        
    }
    void Start()
    {
        if(crossSceneShipData.fleetFilePath != "" && crossSceneShipData.fleetFilePath != null)fleetFolderFull = crossSceneShipData.fleetFilePath;
        if(crossSceneShipData.fleetFilePathRelative != "" && crossSceneShipData.fleetFilePathRelative != null)relativeFleetFolder = crossSceneShipData.fleetFilePathRelative;
        loadDirectory();
        
        GetComponent<ShipInstantiator>().setShipPathsToSpawn(shipPathsToSpawn);
        GetComponent<ShipInstantiator>().fleetFilePath = fleetFolderFull;
       // GetComponent<ShipInstantiator>().spawnFleet();
        if(creditsCount!=null) creditsCount.text = "£"+currentCredits.ToString();
    }
    public void openShipyard(string shipPath){
        crossSceneShipData.fleetFilePath = relativeFleetFolder;
        crossSceneShipData.CrossSceneInformation = shipPath;
        SceneManager.LoadScene(shipyardScene);
    }
    void loadPersistFile(string path){
                        if(path != ""){
                            StreamReader inp_stm = new StreamReader(path);
                            int i = 0;

                            while(!inp_stm.EndOfStream)
                            {
                                string inp_ln = inp_stm.ReadLine();
                                // deconstruct line via , delimiter
                                string[] atts = inp_ln.Split(',');
                                // if the prefix is credits, load credits
                                if(atts[0] == "credits") currentCredits = int.Parse(atts[1]);                                
                            }
                            inp_stm.Close( );  
                            
                        }
                        crossSceneShipData.currentCredits = currentCredits;
    }
    void loadDirectory(){
        string fleetFolder = Application.persistentDataPath + "/" + relativeFleetFolder + "/";
        fleetFolderFull = fleetFolder;
        Debug.Log(fleetFolder);
        string shipName = "";
        // load ships in the folder
        
        if(File.Exists(fleetFolder+"persist.txt")){
            Debug.Log("got here");
            DirectoryInfo dir = new DirectoryInfo(fleetFolder);
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info){
                if(f.Extension == ".txt"){
                    if(f.Name != "persist.txt"){
                        string path = f.FullName;
                        if(path != ""){
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
                            GameObject shipButtonInstance = Instantiate(shipButtonPrefab, shipListMaster);
                            shipButtonInstance.GetComponent<ShipButton>().fleetManager = this;
                            shipButtonInstance.GetComponent<ShipButton>().shipFilePath = path;
                            shipButtonInstance.GetComponent<Text>().text = f.Name;
                            shipPathsToSpawn.Add(path);
                            inp_stm.Close( );  
                            
                        }
                    }
                    else{
                        // this is the persist file, load current gamestate
                        loadPersistFile(f.FullName);
                    }
                }
            }
        }
        // it doesn't exist, then make the folder
        else{
            var folder = Directory.CreateDirectory(fleetFolder); // returns a DirectoryInfo object
            // now make the persist file
            // save each line to a list
            string m_Path = fleetFolder + "persist" + ".txt";
            Debug.Log(m_Path);
            // now write em
            StreamWriter writer = new StreamWriter(m_Path , false);
        
            foreach (string record in defaultPersistInfo){
                writer.WriteLine(record);
            }

            writer.Close();
            // update persist credits
            // get credits amount we just wrote

            loadPersistFile(m_Path);
            crossSceneShipData.currentCredits = currentCredits;
            creditsCount.text = "£"+currentCredits.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
