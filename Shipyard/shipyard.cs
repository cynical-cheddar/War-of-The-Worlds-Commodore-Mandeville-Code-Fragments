using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Cinemachine;
using System.IO;
public class shipyard : MonoBehaviour
{
    public Text currentCreditsText;
    public Text currentTonnageText;
    public Text currentHealthText;
    public bool shopMode = true;
    int currentCredits = 0;
    public GameObject myShip;
    GameObject myShipInstance;

    public Transform shipPoint;

    public GameObject prows;
    public GameObject mains;
    public GameObject commands;
    public GameObject engines;
    public GameObject wings;

    public InputField shipName;

    public Transform statsMaster;

    public GameObject science;

    public GameObject currentHulls;

    public GameObject moduleButtonPrefab;
    public GameObject currentModuleButtonPrefab;

    public GameObject equipmentButtonPrefab;

    public GameObject detachButtonPrefab;

    public GameObject hardpointOverlayPrefab;

    public GameObject statsPrefab;
    public GameObject linePrefab;

    
    //EXAMPLE RECORD
    //   AftModule,0,SingularThrusterMk1-0.SingularThrusterMk1-1.SingularThrusterMk1-2
    List<string> modulesRecords = new List<string>();
    public bool displayAllParts = true;


    List<GameObject> moduleResourceList = new List<GameObject>();

    List<GameObject> currentModuleIcons = new List<GameObject>();

    public List<GameObject> availableProwModules;

    public List<GameObject> availableMainModules;

    public List<GameObject> availableCommandModules;

    public List<GameObject> availableEngineModules;


    public Transform equipmentPane;

    public List<Equipment> allEquipment = new List<Equipment>();
    Transform lookPoint;
    CinemachineFreeLook freecam;
    public Canvas uiCanvas;

    int curModuleId = 0;

    public GameObject successDialogue;

    public GameObject failureDialogue;

    public string returnSceneName;

    AudioSource equipSoundPlayer;
    public AudioClip soundAddEquipment;
    public AudioClip soundRemoveEquipment;

    public AudioClip soundAddModule;
    public AudioClip soundRemoveModule;

    public AudioClip soundSelectHardpoint;

    public AudioClip soundHoverHardpoint;
    public void resetEditor(){
        Application.LoadLevel(Application.loadedLevel);
    }

    public void back(){
        SceneManager.LoadScene(returnSceneName);
    }

    public void returnToScene(){
        string filename = shipName.text;
        string m_Path = Application.persistentDataPath +  "/" + crossSceneShipData.fleetFilePath + "/" + filename + ".txt";
        crossSceneShipData.CrossSceneInformation = m_Path;
        SceneManager.LoadScene(returnSceneName);
    }
    public void importShip(){
        string filepath = crossSceneShipData.CrossSceneInformation;
        // load ship info to records
        if(filepath != ""){
            modulesRecords.Clear();
           
            StreamReader inp_stm = new StreamReader(filepath);
            int i = 0;

            while(!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                // Do Something with the input. 
                if(i== 0) shipName.text = inp_ln;
                else {modulesRecords.Add(inp_ln);}
                i++;
            }

            inp_stm.Close( );  
        }
    }
    public void exportShip(){
        // firstly, check that we have engines
        if(myShipInstance.GetComponentInChildren<ProwModule>()!=null && myShipInstance.GetComponentInChildren<AftModule>()!=null){

            string filename = shipName.text;
        //  string m_Path = Application.persistentDataPath + "/" + filename + ".txt";
            string m_Path = Application.persistentDataPath + "/" + crossSceneShipData.fleetFilePath + "/" + filename + ".txt";
            
            Debug.Log("Export ship path: "+m_Path);
            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(m_Path , false);
        // writer.WriteLine("{");
            writer.WriteLine(shipName.text);
            foreach (string record in modulesRecords){
                writer.WriteLine(record);
            }
        // writer.WriteLine("{");
            
            writer.Close();


            // now save our credits to the persist file

            // save each line to a list
            m_Path = Application.persistentDataPath + "/" + crossSceneShipData.fleetFilePath + "/" + "persist" + ".txt";


            List<string> oldList = new List<string>();
            List<string> newList = new List<string>();
            StreamReader inp_stm = new StreamReader(m_Path);
            int i = 0;

            while(!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                oldList.Add(inp_ln);
            }
            inp_stm.Close( ); 


            // now edit all the lines
            // find the credits one
            foreach(string line in oldList){
                string[] atts = line.Split(',');
                if(atts[0] == "credits"){
                    newList.Add("credits," + currentCredits.ToString());
                }
                else{
                    newList.Add(line);
                }
            }
            // now write em
            writer = new StreamWriter(m_Path , false);
        
            foreach (string record in newList){
                writer.WriteLine(record);
            }

            writer.Close();
            // update persist credits
            crossSceneShipData.currentCredits = currentCredits;


            GameObject go = Instantiate(successDialogue, transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<RectTransform>().rect.Set(0,0,400, 400);
            
        }
        else{
            GameObject go = Instantiate(failureDialogue, transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<RectTransform>().rect.Set(0,0,400, 400);
        }
    }

    public void removeModule(string moduleId){
        bool pass = true;
        calculateNewId();
        int i = 0;
        int refundAmt = 0;
      
        List<string> newModuleRecords = new List<string>();
        if(modulesRecords.Count > 0){
            // find our id
            foreach(string record in modulesRecords){
                string[] atts = record.Split(',');
                if(atts[1] == moduleId){
                    GameObject mod = Resources.Load(atts[0]) as GameObject;
                    refundAmt = mod.GetComponent<Module>().baseCost;
                    if(mod.GetComponent<CommandModule>() != null && myShipInstance.GetComponentsInChildren<CommandModule>().Length == 1 ){
                        pass = false;
                        Debug.Log("command module required");
                    }
                    
                    // this is the module we want to remove
                    //  refundModuleEquipment(moduleId);
                   Debug.Log("Not adding module " + atts[1]);
                }
                else newModuleRecords.Add(record);
                i++;
            }
        }

        if(pass){
            refundModuleEquipment(moduleId);
            currentCredits += refundAmt;
            updateCreditsText();
            modulesRecords = newModuleRecords;
            construct();
            Invoke("createCurrentIcons", 0.01f);
            equipSoundPlayer.PlayOneShot(soundRemoveModule);
        }
      //  createCurrentIcons();
    }
    public void refundModuleEquipment(string moduleId){
        Module[] currentModules = myShipInstance.GetComponentsInChildren<Module>();
        foreach(Module m in currentModules){
            if(m.getId() == moduleId){
                m.returnEquipmentToInventory();
            }
        }
    }

    // Displayed when hovering over a hardpoint
    public void displayHardpointStats(Hardpoint hp){
        // create an overlay with stats on it
           // equipSoundPlayer.PlayOneShot(soundHoverHardpoint);
            GameObject overlay = Instantiate(hardpointOverlayPrefab, uiCanvas.transform);
            overlay.transform.position = hp.transform.position;

            overlay.transform.rotation = Quaternion.LookRotation(overlay.transform.position - Camera.main.transform.position);


            if(overlay.GetComponentInChildren<Text>() != null) overlay.GetComponentInChildren<Text>().text = hp.name;
            StartCoroutine(destroyNextFrame(overlay));
    }
    // Displayed when hovering over an equipment button
    public void displayEquipmentStats(List<string> stats){
      //  equipSoundPlayer.PlayOneShot(soundHoverHardpoint);
            Debug.Log("0ver");
            GameObject statsInstance = Instantiate(statsPrefab, statsMaster);
            statsInstance.transform.position = statsMaster.transform.position;
            foreach(string line in stats){
                GameObject lineObject = Instantiate(linePrefab, statsMaster.GetComponentInChildren<VerticalLayoutGroup>().transform);
                lineObject.GetComponent<Text>().text = line;
            }

            StartCoroutine(destroyNextFrame(statsInstance));
    }
    IEnumerator destroyNextFrame(GameObject obj){

        yield return new WaitForEndOfFrame();
        
        Destroy(obj);
    }

    public void removeDisplayEquipment(){
        foreach(Transform child in equipmentPane){
            Destroy(child.gameObject);
        }
    }

    // DISPLAY EQIUUPMENT ICONS TO ADD THE ONES IN THE SHOP <----
    // Displayed when clicked a hardpoint
    public void displayEquipment(Hardpoint hardpoint, List<Equipment> partsCanAdd){
     //   Debug.LogError(partsCanAdd.Count + " partsCanAdd");
        equipSoundPlayer.PlayOneShot(soundSelectHardpoint);
        foreach(Transform child in equipmentPane){
            Destroy(child.gameObject);
        }
        foreach(Equipment item in partsCanAdd){
            GameObject part = Instantiate(equipmentButtonPrefab, equipmentPane);
            part.GetComponent<AddEquipmentShipyard>().hardpoint = hardpoint;
            part.GetComponent<AddEquipmentShipyard>().componentName = item.gameObject.name;
            part.GetComponent<AddEquipmentShipyard>().componentCost = item.cost;
            part.GetComponent<AddEquipmentShipyard>().moduleIndex = hardpoint.transform.GetComponentInParent<Module>().getId();
            if(item.GetComponent<Equipment>().weaponIcon != null) part.GetComponent<Image>().sprite = item.GetComponent<Equipment>().weaponIcon;
        }
    }
    // Displayed when clicked a hardpoint in place
    public void displayRemoveEquipmentMenu(Hardpoint hardpoint){
        equipSoundPlayer.PlayOneShot(soundAddEquipment);
        removeDisplayEquipment();
        // get equipment in hardpoint
        Equipment item = hardpoint.transform.GetComponentInChildren<Equipment>();
        GameObject part = Instantiate(detachButtonPrefab, equipmentPane);
        part.GetComponent<RemoveEquipmentShipyard>().hardpoint = hardpoint;
        part.GetComponent<RemoveEquipmentShipyard>().componentName = item.gameObject.name;
        part.GetComponent<RemoveEquipmentShipyard>().componentCost = item.cost;
        part.GetComponent<RemoveEquipmentShipyard>().moduleIndex = hardpoint.transform.GetComponentInParent<Module>().getId();
        part.GetComponent<RemoveEquipmentShipyard>().equipmentReference = item;
        part.GetComponent<RemoveEquipmentShipyard>().setMouseOver(true);
        if(item.GetComponent<Equipment>().weaponIcon != null) part.GetComponent<Image>().sprite = item.GetComponent<Equipment>().weaponIcon;
        // add detatch icon
    }
    // Start is called before the first frame update
    public void loadAllEquipment(){
        uiCanvas = FindObjectOfType<uiBarsCanvas>().GetComponent<Canvas>();
        allEquipment.Clear();
    //    UnityEngine.Object[] equipmentCatalogue = Resources.LoadAll("", typeof(Equipment));
        Equipment[] equipmentCatalogue = Resources.LoadAll<Equipment>("");
        foreach (Equipment go in equipmentCatalogue)
        {
            if(go.showInShipyard)allEquipment.Add(go);
            
        }
        if(allEquipment.Count < 2) Debug.LogError("wtf you have no equipment in the shipyard");
//        Debug.LogError(allEquipment.Count + " loadAllEquipment");
        Resources.UnloadUnusedAssets();
    }
    // add to this when we have more ship types
    bool havePreRequisiteModules(string moduleName){
        bool passed = false;
        GameObject moduleTemp = Resources.Load(moduleName) as GameObject;
        Module m = moduleTemp.GetComponent<Module>();
        

        // if we have no command modules and the module we are testing is not a command module
        if(myShipInstance.GetComponentInChildren<CommandModule>() == null && m.gameObject.GetComponent<CommandModule>() == null) return false;
        if(m.prerequesite_combat == false && m.prerequesite_science == false) return true; // no prerequestites
        CommandModule[] commandModules = myShipInstance.GetComponentsInChildren<CommandModule>();
        if(commandModules.Length == 0) return false;   
        
        foreach(CommandModule commandModule in commandModules){
            if(m.prerequesite_combat && commandModule.command_combat) return true;
            if(m.prerequesite_science && commandModule.command_science) return true;
        }
        return passed;
    }
    void createIcons(List<GameObject> parts, Transform parent){
        foreach (Transform child in parent){
            Destroy(child.gameObject);
        }
            int i = 0;
            foreach(GameObject a in parts){
                string componentName = a.name.Replace("(clone)","").Trim();
                // now load the prefab and check its prerequesites
                if(havePreRequisiteModules(componentName)){


                    GameObject buttonInstance = Instantiate(moduleButtonPrefab);
                    buttonInstance.GetComponent<addComponentShipyard>().componentName = a.name.Replace("(clone)","").Trim();
                    buttonInstance.transform.parent = parent;
                //   buttonInstance.transform.localPosition = new Vector3(0, 0, 0);
                //  buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * buttonInstance.GetComponent<RectTransform>().rect.width, 0);
                    buttonInstance.GetComponent<RectTransform>().localScale = Vector3.one;
                    if(a.GetComponent<Module>().icon != null) buttonInstance.GetComponent<Image>().sprite = a.GetComponent<Module>().icon;
                    
                    i++;
                }
            }
    }

    public void createCurrentIcons(){
        
        List<GameObject> parts = new List<GameObject>();
        Module[] modulesInShip = myShipInstance.GetComponentsInChildren<Module>();
        foreach(Module m in modulesInShip){
            parts.Add(m.gameObject);
        }

        foreach(Transform child in currentHulls.transform){
            Destroy(child.gameObject);
        }
        int i = 0;
         foreach(GameObject a in parts){
                string newName = a.name.Replace("(Clone)","").Trim();
                newName = newName.Replace("(clone)","").Trim();
                Debug.Log(newName);
                GameObject buttonInstance = Instantiate(currentModuleButtonPrefab);
                buttonInstance.GetComponent<partButton>().componentName = newName;
                buttonInstance.transform.parent = currentHulls.transform;
                
                
                buttonInstance.GetComponent<currentModuleButton>().moduleId = a.GetComponent<Module>().getId();
                if(a.GetComponent<Module>().icon != null) buttonInstance.GetComponent<Image>().sprite = a.GetComponent<Module>().icon;

            //    buttonInstance.GetComponent<addComponentShipyard>().componentName = a.name;
           //     buttonInstance.GetComponent<addComponentShipyard>().index = i;

                // create a button for  each of the equipment pieces in the module
                

                i++;
            }
    }
    void Awake(){
        // take in a file if loading a premade ship file
        
    }
    void displayModuleIcons(){
        if(displayAllParts){
        //prow Modules
            createIcons(availableCommandModules, commands.transform);
            createIcons(availableMainModules, mains.transform);
            createIcons(availableProwModules, prows.transform);
            createIcons(availableEngineModules, engines.transform);

        }
    }
    void Start()
    {
        
        currentCredits = crossSceneShipData.currentCredits;
        equipSoundPlayer = GetComponent<AudioSource>();
        lookPoint = Instantiate(new GameObject()).transform;
        freecam = FindObjectOfType<CinemachineFreeLook>();
        
        construct();
        calculateNewId();
        loadAllEquipment();
        if(crossSceneShipData.CrossSceneInformation != null && crossSceneShipData.CrossSceneInformation != ""){
            importShip();
            
        } 
        
        displayModuleIcons();

    }
    void calculateNewId(){
        if(modulesRecords.Count > 0){
            // find top id
           // foreach(string record in modulesRecords){
           //     string[] atts = record.Split(',');
           //     int id = int.Parse(atts[1]);
           //     if (id > curModuleId) curModuleId = id; 
           // }
            curModuleId = UnityEngine.Random.Range(0, 99999999);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateCreditsText();
    }

   /* List<GameObject> getResourcesOfType(string type){
        List<GameObject> objectsTyped = new List<GameObject>();
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
           
            if(go.GetComponent<ProwModule>() != null) objectsTyped.Add(go);
            if(go.GetComponent(Type.GetType(type)) != null)  Debug.Log("yes");
        }
        return objectsTyped;
    }*/

    
    public void addHull(string hullName){
        Debug.Log("adding hull " + hullName);
        bool pass = true;
        GameObject a = Resources.Load(hullName) as GameObject;

        // firstly, check if we can actuall afford this module
        int cost = a.GetComponent<Module>().baseCost;
        if(currentCredits - cost <= 0 && shopMode) pass = false;

        calculateNewId();
        //if a prow, check that we do not already have one
        
        if(myShipInstance == null){
            
            if(a.GetComponent<CommandModule>() == null) pass = false;
        }
        if(myShipInstance != null){
            if(a.GetComponent<CommandModule>() == null && myShipInstance.GetComponentInChildren<CommandModule>() == null) pass = false;
            if(myShipInstance.GetComponentInChildren<ProwModule>() != null && a.GetComponent<ProwModule>() != null) pass = false;
            if(myShipInstance.GetComponentInChildren<AftModule>() != null && a.GetComponent<AftModule>() != null) pass = false;
            // GameObject a = Resources.Load(hullName) as GameObject;
        }
        if(pass){
            equipSoundPlayer.PlayOneShot(soundAddModule);
            if(pass) modulesRecords.Add(concatenateStrings(hullName, curModuleId.ToString(), ""));
            
            if(pass) construct();
            if(shopMode) currentCredits -= cost;
            curModuleId++;
        }

        // foreach current module record, add a new icon
        // this is ordered in the order of adding components
        
        /*foreach(string record in modulesRecords){
            string[] split = record.Split(',');
            GameObject module = Resources.Load(split[0]) as GameObject;
            currentModuleList.Add(module);
        }*/
        Invoke("createCurrentIcons", 0.01f);
        displayModuleIcons();
        
        

    }
    

    public void construct(){
        if(myShipInstance != null) Destroy(myShipInstance);

        
        myShipInstance = Instantiate(myShip, shipPoint.position, shipPoint.rotation);
        myShipInstance.GetComponent<shipBuilderPlayer>().setModulesRecords(modulesRecords);
        myShipInstance.GetComponent<shipBuilderPlayer>().constructShip();
        //shipPoint.position = myShipInstance.GetComponent<Rigidbody>().worldCenterOfMass;

        ShipConnector[] modules = myShipInstance.GetComponentsInChildren<ShipConnector>();
         Vector3 point1 = Vector3.zero;
         Vector3 point2 = Vector3.zero;
        if(myShipInstance.GetComponentInChildren<Module>() != null){
             point1 = FindFurthestObject(modules).transform.position;
             point2 = FindClosestObject(modules).transform.position;
        }
        
        

        lookPoint.position = (point1 + point2)/2;
        freecam.LookAt =lookPoint;
        freecam.Follow =lookPoint;

        

        

        

        
        Invoke("updateHealthAndTonnage",0.1f);
    }
    void updateHealthAndTonnage(){
        currentTonnageText.text = myShipInstance.GetComponent<Rigidbody>().mass.ToString() + " tonnes";
        currentHealthText.text = myShipInstance.GetComponent<HealthShip>().hitpoints.ToString() + " hp";
    }
     GameObject FindFurthestObject(ShipConnector[] GameObjectList)
    {
        float FurthestDistance = 0;
        GameObject FurthestObject = null;
        foreach(ShipConnector go in GameObjectList)
        {
            float ObjectDistance = Vector3.Distance(freecam.transform.position, go.transform.position);
            if (ObjectDistance > FurthestDistance)
            {
                FurthestObject = go.gameObject;
                FurthestDistance = ObjectDistance;
            }
        }
        return FurthestObject;
 }
 GameObject FindClosestObject(ShipConnector[] GameObjectList)
    {
        float FurthestDistance = 9999999999999f;
        GameObject FurthestObject = null;
        foreach(ShipConnector go in GameObjectList)
        {
            float ObjectDistance = Vector3.Distance(freecam.transform.position, go.transform.position);
            if (ObjectDistance < FurthestDistance)
            {
                FurthestObject = go.gameObject;
                FurthestDistance = ObjectDistance;
            }
        }
        return FurthestObject;
 }
 void updateCreditsText(){
     if(currentCreditsText!=null) currentCreditsText.text = "£"+currentCredits.ToString();
 }


    public void removeEquipment(string partName, string moduleIndex, int hardpointIndex, Equipment equipmentReference){
        // find the module with the index. 
        equipSoundPlayer.PlayOneShot(soundRemoveEquipment);
        
        bool pass = true;
        List<string> moduleRecordsNew = new List<string>();
        int i = 0;
        foreach(string record in modulesRecords){
            string[] atts = record.Split(',');
            string id = atts[1];
            // dealing with our module
            if(id == moduleIndex){

                string[] equipmentOnModule = atts[2].Split('.');
                string newRecord = "";
                if(equipmentOnModule.Length == 1){
                    // if there is one equipment, then replace string with null
                    newRecord = atts[0] + "," + atts[1] + "," + "null";
                }
                // otherwise remove the equipment at our hardpoint index
                else{
                    string newRecordappend = "";
                    foreach(string item in equipmentOnModule){
                        // split the record in two with delimter '-'. if the hardpoint matches our index
                        string[] itemAtts = item.Split('-');
                        if(itemAtts[1] == hardpointIndex.ToString()){

                        }
                        else{
                            newRecordappend = newRecordappend + item + ".";
                        }
                    }
                    // redefines as name, id, equipment
                   newRecord =  atts[0] + "," + atts[1] + "," + newRecordappend;
                      
                   
                }
                if(newRecord.EndsWith(".")) newRecord = newRecord.Remove(newRecord.Length - 1, 1); 
                moduleRecordsNew.Add(newRecord);
                
                
            }

            // dealting with another module
            else{
                moduleRecordsNew.Add(record);
            }
            i++;
        }
        currentCredits += equipmentReference.cost;
        updateCreditsText();
        modulesRecords.Clear();
        modulesRecords = moduleRecordsNew;
        construct();
        removeDisplayEquipment();
    }

    public void addEquipment(string partName, string moduleIndex, int hardpointIndex){


        equipSoundPlayer.PlayOneShot(soundAddEquipment);

        

        // find the module with the index. 
        bool pass = true;
        int cost = 0;

        if(shopMode){
            GameObject e = Resources.Load(partName) as GameObject;
            cost = e.GetComponent<Equipment>().cost;
            if(currentCredits - cost < 0) pass = false;
        }
        if(pass){
            List<string> moduleRecordsNew = new List<string>();
            int i = 0;
            foreach(string record in modulesRecords){
                string[] atts = record.Split(',');
                string id = atts[1];
                if(id == moduleIndex){
                    if(atts[2] == "null")  moduleRecordsNew.Add(atts[0] + "," + atts[1] + "," + partName + "-" + hardpointIndex.ToString());
                    else{
                    //  moduleRecordsNew.Add(modulesRecords[i] + "." +  partName + "-" + hardpointIndex.ToString());
                        // check that the hardpoint index has not been taken
                        string[] currentEquipment = atts[2].Split('.');
                        foreach(string thing in currentEquipment){
                            string[] equ = thing.Split('-');
                            if(equ[1] == hardpointIndex.ToString()) pass = false;
                        }
                        if(pass) moduleRecordsNew.Add(modulesRecords[i] + "." +  partName + "-" + hardpointIndex.ToString());
                        else moduleRecordsNew.Add(record);
                    } 
                }
                else{
                    moduleRecordsNew.Add(record);
                }
                i++;
            }
            if(shopMode && pass) currentCredits -= cost;
            modulesRecords.Clear();
            modulesRecords = moduleRecordsNew;
            construct();
        }
        
        removeDisplayEquipment();
    }


   /* public string describeModule(){
        


        string description = "";
        
        string equipmentString = "";
        int i = 1;
        foreach(string str in equipmentList){
            equipmentString = equipmentString + str;
            if(i != equipmentList.Count){
                equipmentString = equipmentString + ".";
            }
            i ++;
        }

        description = concatenateStrings(description, type, pointDefence.ToString(), equipmentString);
        return description;

    }*/

    string concatenateStrings(string type, string pointDefenceStr, string equipmentList){
        string desc = "";
        if(equipmentList != ""){
           desc = type + "," + pointDefenceStr + "," + equipmentList;
        }
        else{
            desc = type + "," + pointDefenceStr + "," + "null";
        }
        return desc;
    }

}
