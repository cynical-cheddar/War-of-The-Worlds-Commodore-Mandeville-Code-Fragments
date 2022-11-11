using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
public class SkirmishSetupManager : MonoBehaviour
{
    public List<string> defaultPersistInfo = new List<string>();
    public string relativeFleetFolder = "exampleFleet";
    public string fleetFolderFull;
    public string fleetManager;

    protected int currentCredits = 0;

    public Dropdown drpFaction;
    public Dropdown drpCredits;

    public Dropdown drpEnemyCredits;

    public Dropdown drpEnemyFleetComposition;

    public string nextScene;

    public bool replaceFolderOnStartup = false;

    public bool enterBattleScene = false;
    public GameObject noShipsWarningPrefab;
    public Transform noShipsParent;

    
    void Start(){
        crossSceneShipData.fleetFilePath = fleetFolderFull;
        crossSceneShipData.fleetFilePathRelative = relativeFleetFolder;
        loadDirectory();
        if(replaceFolderOnStartup) changeEnemyCredits(0);
    }

    public void enterFleetmanager(){
        SceneManager.LoadScene(fleetManager);
    }

    public void enterBattle(){

        // check if we have any ships
        if(enterBattleScene){
            DirectoryInfo dir = new DirectoryInfo(fleetFolderFull);
            FileInfo[] info = dir.GetFiles();
            if(info.Length < 2){
                GameObject a = Instantiate(noShipsWarningPrefab, noShipsParent);
                Destroy(a, 2f);
            }
            else{
                crossSceneShipData.fleetFilePath = fleetFolderFull;
                SceneManager.LoadScene(nextScene);
            }
        }
        else{
            crossSceneShipData.fleetFilePath = fleetFolderFull;
                SceneManager.LoadScene(nextScene);
        }
        // do not tlet them enter without ships
        
    }

    public void menu(){
        //crossSceneShipData.fleetFilePath = fleetFolderFull;
        SceneManager.LoadScene("menu");
    }

    public void dropdownChangeCredits(int id){
        string amt = drpCredits.options[id].text;
        changeCredits(amt);
    }

    public void changeCredits(string amt){
        // loop through default info and change the credits to amt
        List<string> newDefaultInfo = new List<string>();
        string newRecord = "";
        foreach(string record in defaultPersistInfo){
            string[] atts = record.Split(',');
            if(atts[0] == "credits"){
                 newRecord = "credits," + amt.ToString();
                newDefaultInfo.Add(newRecord);
            }
            else newDefaultInfo.Add(record);
        }

        defaultPersistInfo = newDefaultInfo;

        loadDirectory();

        
    }

    public void changeEnemyCredits(int id){
        string amt = drpEnemyCredits.options[id].text;
        crossSceneShipData.enemyBudget = int.Parse(amt);
    }
    public void changeEnemyFleet(int id){
      //  string amt = drpEnemyFleetComposition.options[id].text;
        crossSceneShipData.nextFleetComposition = id;
    }


    void loadDirectory(){
        string fleetFolder = Application.persistentDataPath + "/" + relativeFleetFolder + "/";
        fleetFolderFull = fleetFolder;
        Debug.Log(fleetFolder);
        string shipName = "";
        // load ships in the folder
        
        if(File.Exists(fleetFolder+"persist.txt")){
            // if a file exists in the fleet folder, then delete everything
            if(replaceFolderOnStartup){
                System.IO.DirectoryInfo di = new DirectoryInfo(fleetFolder);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete(); 
                }
            }
        }
        // it doesn't exist, then make the folder
        if(!File.Exists(fleetFolder+"persist.txt")){
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
            //creditsCount.text = "£"+currentCredits.ToString();
        }
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
}

