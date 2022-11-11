using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.Linq;
using System.IO;
public class shipBuilderPlayer : MonoBehaviour
{
    
List<string> partIds = new List<string>();
public bool buildship = true;

    string shipName = "";
    List<string> modulesRecords = new List<string>();
    [Header("Transforms (in Asset)")]
    public Transform moduleMaster;

    
    [Header("Current Ship State")]

    public List<string> moduleDescription;

    List<MainModule> mainModuleDescriptors;

    [Header("Current Modules")]
    public List<GameObject> mainModuleInstances;
    public List<GameObject> aftModuleInstances;

    public List<GameObject> prowModuleInstances;

    public void setModulesRecords(List<string> newList){
        moduleDescription = newList;
    }
    IEnumerator postSetRot(Transform equipmentInstance, Transform hardpointParent){
        yield return new WaitForFixedUpdate();
        equipmentInstance.transform.rotation = hardpointParent.rotation;
    }
    public string myFilePath = "";
    void Awake()
    {
        if(buildship){
        // Load Data from JSON
            if(crossSceneShipData.CrossSceneInformation != "" && myFilePath == "") myFilePath = crossSceneShipData.CrossSceneInformation;
        
            if(FindObjectOfType<shipyard>() == null && myFilePath != "" && myFilePath != null) loadShip(myFilePath);
            
            if(FindObjectOfType<shipyard>() == null) constructShip();
            if(GetComponent<Ship>() != null)GetComponent<Ship>().initialiseControls();
        }
    }
    void addEquipment(GameObject moduleInstance, string[] equipmentList){
        moduleDescriptor descriptor = moduleInstance.GetComponent<moduleDescriptor>();
        descriptor.equipmentList.Clear();
        Module module =  moduleInstance.GetComponent<Module>();
        int i = 0;
        foreach(string equipmentRecord in equipmentList){
            // Get its hardpointID
            string[] equipmentAtt = equipmentRecord.Split('-');
            GameObject eqipmentInstance = Instantiate(Resources.Load<GameObject>(equipmentAtt[0]), module.transform.position, Quaternion.identity);
            Equipment type = eqipmentInstance.GetComponent<Equipment>();
          //  float xScale = eqipmentInstance.transform.localScale.x;
          //  eqipmentInstance.transform.localScale = new Vector3(xScale, xScale, xScale);
            int hardPointId = Int32.Parse(equipmentAtt[1]);
            switch (type){
                case Weapon weapon:
                    eqipmentInstance.transform.parent = module.hardpoints[hardPointId];
                    eqipmentInstance.GetComponent<Equipment>().hardpointIndex = hardPointId;
                    eqipmentInstance.transform.position = module.hardpoints[hardPointId].position;
                    
                    StartCoroutine(postSetRot(eqipmentInstance.transform, module.hardpoints[hardPointId]));
                    descriptor.equipmentList.Add(equipmentRecord);
                break;
                case Thruster thruster:
                    eqipmentInstance.transform.parent = module.eqipmentPoints[hardPointId];
                    eqipmentInstance.GetComponent<Equipment>().hardpointIndex = hardPointId;
                    eqipmentInstance.transform.position = module.eqipmentPoints[hardPointId].position;
                    eqipmentInstance.transform.rotation = module.eqipmentPoints[hardPointId].rotation;
                    descriptor.equipmentList.Add(equipmentRecord);
                break;
                default:
                    eqipmentInstance.transform.parent = module.eqipmentPoints[hardPointId];
                    eqipmentInstance.GetComponent<Equipment>().hardpointIndex = hardPointId;
                    eqipmentInstance.transform.position = module.eqipmentPoints[hardPointId].position;
                    eqipmentInstance.transform.rotation = module.eqipmentPoints[hardPointId].rotation;
                    descriptor.equipmentList.Add(equipmentRecord);
                break;

            }


            
            
        }
    }
    public void loadShip(string path){
        // Read saved ship files and load into variable fields
        Debug.Log(path);
        if(path != ""){
            modulesRecords.Clear();
            moduleDescription.Clear();
            StreamReader inp_stm = new StreamReader(path);
            int i = 0;

            while(!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                // Do Something with the input. 
                if(i== 0) shipName = inp_ln;
                else {moduleDescription.Add(inp_ln); modulesRecords.Add(inp_ln);}
                i++;
            }

            inp_stm.Close( );  
        }
    }
    int getModuleSizeFromRecord(string record){
        string[] attributes = record.Split(','); 
        return(getModuleSize(attributes[0]));
    }
    int getModuleSize(string moduleName){
        GameObject moduleInstance = Resources.Load<GameObject>(moduleName);
        Module moduleType = moduleInstance.GetComponent<Module>();
        int moduleSize = moduleType.moduleOrderSize;
        return moduleSize;
    }

    List<string> orderCommandModules(){
        List<string> commandOrder = new List<string>();
        //loop through and add all of the command modules
         foreach(string module in moduleDescription){
           string[] attributes = module.Split(','); 
           GameObject moduleInstance = Resources.Load<GameObject>(attributes[0]);
           Module moduleType = moduleInstance.GetComponent<Module>();
           switch (moduleType){
               case CommandModule c:
                    // get size 
                    int size = moduleType.moduleOrderSize;
                    int position = 0;
                    // get size of current modules
                    if(commandOrder.Count > 0){
                        foreach(string prevModule in commandOrder){
                            int prevSize = getModuleSizeFromRecord(prevModule);
                            if(prevSize >= size) position += 1;
                        }
                    }
                    // add record at position
                    commandOrder.Insert(position, module);
               break;
               default:
               break;
             }
         }
        return commandOrder;
    }

    List<string> orderMainModules(){
        List<string> mainOrder = new List<string>();
        //loop through and add all of the command modules
         foreach(string module in moduleDescription){
           string[] attributes = module.Split(','); 
           GameObject moduleInstance = Resources.Load<GameObject>(attributes[0]);
           Module moduleType = moduleInstance.GetComponent<Module>();
           switch (moduleType){
               case MainModule c:
                    // get size 
                    int size = moduleType.moduleOrderSize;
                    int position = 0;
                    // get size of current modules
                    if(mainOrder.Count > 0){
                        foreach(string prevModule in mainOrder){
                            int prevSize = getModuleSizeFromRecord(prevModule);
                            if(prevSize >= size) position += 1;
                        }
                    }
                    // add record at position
                    mainOrder.Insert(position, module);
               break;
               default:
               break;
             }
         }
        return mainOrder;
    }
    
    List<string> orderProwModules(){
        List<string> prowOrder = new List<string>();
        //loop through and add all of the command modules
         foreach(string module in moduleDescription){
           string[] attributes = module.Split(','); 
           GameObject moduleInstance = Resources.Load<GameObject>(attributes[0]);
           Module moduleType = moduleInstance.GetComponent<Module>();
           switch (moduleType){
               case ProwModule c:
                    // get size 
                    int size = moduleType.moduleOrderSize;
                    int position = 0;
                    // get size of current modules
                    if(prowOrder.Count > 0){
                        foreach(string prevModule in prowOrder){
                            int prevSize = getModuleSizeFromRecord(prevModule);
                            if(prevSize >= size) position += 1;
                        }
                    }
                    // add record at position
                    prowOrder.Insert(position, module);
               break;
               default:
               break;
             }
         }
        return prowOrder;
    }
    
    List<string> orderAftModules(){
        List<string> aftOrder = new List<string>();
        //loop through and add all of the command modules
         foreach(string module in moduleDescription){
           string[] attributes = module.Split(','); 
           GameObject moduleInstance = Resources.Load<GameObject>(attributes[0]);
           Module moduleType = moduleInstance.GetComponent<Module>();
           switch (moduleType){
               case AftModule c:
                    // get size 
                    int size = moduleType.moduleOrderSize;
                    int position = 0;
                    // get size of current modules
                    if(aftOrder.Count > 0){
                        foreach(string prevModule in aftOrder){
                            int prevSize = getModuleSizeFromRecord(prevModule);
                            if(prevSize >= size) position += 1;
                        }
                    }
                    // add record at position
                    aftOrder.Insert(position, module);
               break;
               default:
               break;
             }
         }
        return aftOrder;
    }
    
    List<string> orderModuleList(){
        List<string> commandOrder = orderCommandModules();
        List<string> mainOrder = orderMainModules();
        List<string> prowOrder = orderProwModules();
        List<string> aftOrder = orderAftModules();

        List<string> order = commandOrder;

        order.AddRange(mainOrder);
        order.AddRange(prowOrder);
        order.AddRange(aftOrder);
        
        return order;
    }
    void recentreShip(List<GameObject> currentModules){
        // get distance from engines to prow
        float diff = getSmallestProw(currentModules).transform.position.z - getSmallestAft(currentModules).transform.position.z;

     //   moduleMaster.Translate(new Vector3(0,0,diff/3), Space.Self);
    }


    public void constructShip(){
        modulesRecords = orderModuleList();
        partIds.Clear();
        foreach(string line in modulesRecords){
            string[] atts = line.Split(',');
            
            partIds.Add(atts[1]);
        }

        
        List<GameObject> currentModules = new List<GameObject>();

        int mainCount = 0;
        int idCount = 0;
        foreach(string module in modulesRecords){
           string[] attributes = module.Split(','); 
           string[] equipment;

           // string split into type, pd, and equipment
           bool hasEquipment = true;
           if(attributes[2] == "null") hasEquipment = false;
           

           if(hasEquipment){
                equipment = attributes[2].Split('.');
           }
           else{
                equipment = new string[1];
           } 
           
           

           GameObject moduleInstance = Instantiate(Resources.Load<GameObject>(attributes[0]), transform.position, Quaternion.identity);
           Module moduleType = moduleInstance.GetComponent<Module>();
           moduleDescriptor descriptor = moduleInstance.GetComponent<moduleDescriptor>();
           
           //descriptor.type = attributes[0];
           //descriptor.pointDefence = int.Parse(attributes[1]);

           
            moduleInstance.transform.rotation = moduleMaster.rotation;
            moduleInstance.transform.parent = moduleMaster;

           switch (moduleType){
               case CommandModule command:
               // if this is the first module, put it in default position
                if(currentModules.Count <= 0){
                    moduleInstance.transform.position = moduleMaster.position;
                }
                else{

                 //   moduleInstance.transform.position = currentModules.Last().GetComponent<Module>().frontConnector.position;
                    GameObject oldModule3 = getSmallestCommand(currentModules);
                    connectModulePositions(moduleInstance, oldModule3, true);
                }
                // otherwise add it to the front of the current module
                // now add equipment
                if(hasEquipment)addEquipment(moduleInstance, equipment);
               break;
               case AftModule a:
               // add behind current module
               
              //  moduleInstance.transform.position = currentModules.First().GetComponent<Module>().rearConnector.position;

                // get the command module with tyhe highest priority
                
                GameObject oldModule2 = getBiggestCommand(currentModules);
                connectModulePositions(moduleInstance, oldModule2, false);
                // now add equipment
                if(hasEquipment)addEquipment(moduleInstance, equipment);
                break;

                
               case MainModule m:
               mainCount ++;
               if(currentModules.Count <= 0){
                        moduleInstance.transform.parent = moduleMaster;
                    }
                    else{
                    // add to the front of the current module
                        
                        if(mainCount == 1) connectModulePositions(moduleInstance, getSmallestCommand(currentModules), true);
                        else connectModulePositions(moduleInstance, getBiggestMain(currentModules), true);
                        
                    }
                    if(hasEquipment)addEquipment(moduleInstance, equipment);
                break;
                
              case ProwModule p:
                // add to the front of the current module
                GameObject oldModule;
                // add to the front of the current module
                if(mainCount > 0) oldModule  = getSmallestMain(currentModules);
                else oldModule = getSmallestCommand(currentModules);
                connectModulePositions(moduleInstance, oldModule, true);
                if(hasEquipment)addEquipment(moduleInstance, equipment);
                break;
           }

           moduleType.setID(partIds[idCount]);
           idCount++;
           currentModules.Add(moduleInstance);
           recentreShip(currentModules);
           // set tonnage
           setTonnage();
           setHealth();
        }
    }
    void setHealth(){
        float sum = 0f;
        CommandModule[] commandModules = GetComponentsInChildren<CommandModule>();
        // take max command module Health
        float maxHealth = 0f;
        foreach(CommandModule mod in commandModules){
            if(mod.coreHealth > maxHealth) maxHealth = mod.coreHealth;
        }
        sum = maxHealth;
        Module[] modules = GetComponentsInChildren<Module>();
        foreach(Module mod in modules){
            sum += mod.bonusHealth;
        }
        GetComponent<HealthShip>().setMaxHealth(sum);
        GetComponent<HealthShip>().resetHealth();
    }
    void setTonnage(){
        float sum  = 0f;
        Module[] modules = GetComponentsInChildren<Module>();
        foreach(Module m in modules){
            sum += m.tonnage;
        }
        GetComponent<Rigidbody>().mass = sum;
    }
    GameObject getBiggestCommand(List<GameObject> currentModules){
        GameObject biggestCommand = currentModules[0];
                int i = 0;
                int curMax = 0;
                foreach(GameObject mod in currentModules){
                    Module moduleType1 = mod.GetComponent<Module>();
                    switch (moduleType1){
                        case CommandModule command:
                            if(command.moduleOrderSize > curMax) {curMax = command.moduleOrderSize; biggestCommand = currentModules[i];}
                            
                        break;
                    }
                    i++;
                }
                return biggestCommand;
    }
    GameObject getSmallestCommand(List<GameObject> currentModules){
        GameObject biggestCommand = currentModules[0];
                int i = 0;
                int curMax = 999999999;
                foreach(GameObject mod in currentModules){
                    Module moduleType1 = mod.GetComponent<Module>();
                    switch (moduleType1){
                        case CommandModule command:
                            if(command.moduleOrderSize <= curMax) {curMax = command.moduleOrderSize; biggestCommand = currentModules[i];}
                            
                        break;
                    }
                    i++;
                }
                return biggestCommand;
    }
    GameObject getSmallestAft(List<GameObject> currentModules){
        GameObject biggestCommand = currentModules[0];
                int i = 0;
                int curMax = 999999999;
                foreach(GameObject mod in currentModules){
                    Module moduleType1 = mod.GetComponent<Module>();
                    switch (moduleType1){
                        case AftModule command:
                            if(command.moduleOrderSize <= curMax) {curMax = command.moduleOrderSize; biggestCommand = currentModules[i];}
                            
                        break;
                    }
                    i++;
                }
                return biggestCommand;
    }

    GameObject getBiggestMain(List<GameObject> currentModules){
        GameObject biggestMain = currentModules[0];
                int i = 0;
                int curMax = 0;
                foreach(GameObject mod in currentModules){
                    Module moduleType1 = mod.GetComponent<Module>();
                    switch (moduleType1){
                        case MainModule command:
                            if(command.moduleOrderSize >= curMax) {curMax = command.moduleOrderSize; biggestMain = currentModules[i];}
                            
                        break;
                    }
                    i++;
                }
                return biggestMain;
    }
    GameObject getSmallestProw(List<GameObject> currentModules){
        GameObject biggestMain = currentModules[0];
                int i = 0;
                int curMax = 999999999;
                foreach(GameObject mod in currentModules){
                    Module moduleType1 = mod.GetComponent<Module>();
                    switch (moduleType1){
                        case ProwModule command:
                            if(command.moduleOrderSize <= curMax) {curMax = command.moduleOrderSize; biggestMain = currentModules[i];}
                            
                        break;
                    }
                    i++;
                }
                return biggestMain;
    }
    GameObject getSmallestMain(List<GameObject> currentModules){
        GameObject biggestMain = currentModules[0];
                int i = 0;
                int curMax = 999999999;
                foreach(GameObject mod in currentModules){
                    Module moduleType1 = mod.GetComponent<Module>();
                    switch (moduleType1){
                        case MainModule command:
                            if(command.moduleOrderSize <= curMax) {curMax = command.moduleOrderSize; biggestMain = currentModules[i];}
                            
                        break;
                    }
                    i++;
                }
                return biggestMain;
    }
    void connectModulePositions(GameObject moduleInstance, GameObject oldModule, bool connectToOldFront){
        if(connectToOldFront){
            moduleInstance.transform.position = oldModule.GetComponent<Module>().frontConnector.position;
            // get difference betwen front and back positions
            float xDiff = moduleInstance.GetComponent<Module>().rearConnector.position.x - oldModule.GetComponent<Module>().frontConnector.position.x;
            float yDiff = moduleInstance.GetComponent<Module>().rearConnector.position.y - oldModule.GetComponent<Module>().frontConnector.position.y;
            float zDiff = moduleInstance.GetComponent<Module>().rearConnector.position.z - oldModule.GetComponent<Module>().frontConnector.position.z;

            moduleInstance.transform.position = moduleInstance.transform.position - (new Vector3(xDiff, yDiff, zDiff));
        }
        else{
            moduleInstance.transform.position = oldModule.GetComponent<Module>().rearConnector.position;
            // get difference betwen front and back positions
            float xDiff = moduleInstance.GetComponent<Module>().frontConnector.position.x - oldModule.GetComponent<Module>().rearConnector.position.x;
            float yDiff = moduleInstance.GetComponent<Module>().frontConnector.position.y - oldModule.GetComponent<Module>().rearConnector.position.y;
            float zDiff = moduleInstance.GetComponent<Module>().frontConnector.position.z - oldModule.GetComponent<Module>().rearConnector.position.z;

            moduleInstance.transform.position = moduleInstance.transform.position - (new Vector3(xDiff, yDiff, zDiff));
        }
    }

    

    void deleteList(List<GameObject> moduleList){
        foreach(GameObject module in moduleList){
            Destroy(module);
        }
    }
}
