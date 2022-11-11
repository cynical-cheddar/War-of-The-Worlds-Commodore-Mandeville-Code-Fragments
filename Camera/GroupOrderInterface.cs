using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
public class GroupOrderInterface : MonoBehaviour
{
    public GameObject groupSelectInterfacePrefab;
    GameObject groupSelectInterface;
    public List<GameObject> selectables = new List<GameObject>();
    public List<GameObject> controllableShips = new List<GameObject>();

    public List<GameObject> selectedShips = new List<GameObject>();
    List<Vector3> posList = new List<Vector3>();
    List<GameObject> sortedSelected = new List<GameObject>();
    private bool MoveMode = false; // enables movement
    private bool Vertical = false; // toggles between vertical and horizontal movement
    public GameObject beacon;      // prefab that contains the line renders a visual represntation of the system
    GameObject currentbeacon; //  the instanced navbeacon
    private LineRenderer horizLinerenderer; // horizantal line render that goes from the nav beacon to the selected unit
    private LineRenderer vertLinerenderer;    // vertical line render that goes from the nav beacon up or down in line with the unit
    private float xinput; // used modify the nav beacons x pos                    
    private float zinput; // used modify the nav beacons x pos
    private float yinput; // used modify the nav beacons x pos
    public float Damp = 1f;  // used modify the nav beacons x,y,z pos

    bool lastMovemode = false;

    public GameObject movementPlanePrefab;
    GameObject movementPlaneInstance;

    int clickCount = 0;

    public float singleClickThreshold = 0.2f;
    float currentThreshold = 0.1f;

    Vector3 curPos = Vector3.zero;

    CameraLookController lookController;

    [Tooltip("Canvas is set automatically if not set in the inspector")]
    Canvas canvas;
    [Tooltip("The Image that will function as the selection box to select multiple objects at the same time. Without this you can only click to select.")]
    public Image selectionBox;
    [Tooltip("The key to add/remove parts of your selection")]
    public KeyCode copyKey = KeyCode.LeftControl;
 
    private Vector3 startScreenPos;
 
    private BoxCollider worldCollider;
 
    private RectTransform rt;
 
    private bool isSelecting;
    


    public void setSelectables(List<GameObject> lst){
        selectables = lst;
    }

    void Awake()
    {
       

        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
 
        if (selectionBox != null)
        {
            //We need to reset anchors and pivot to ensure proper positioning
            rt = selectionBox.GetComponent<RectTransform>();
            rt.pivot = Vector2.one * .5f;
            rt.anchorMin = Vector2.one * .5f;
            rt.anchorMax = Vector2.one * .5f;
            selectionBox.gameObject.SetActive(false);
        }
    }
    public bool getMoveMode(){
        return MoveMode;
    }
    int stage = 0;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        for(int i = 0; i < 22; i++){
            Physics.IgnoreLayerCollision(i, 9);
            Physics.IgnoreLayerCollision(9, i);
        }
        StartCoroutine(lateSet());
        lookController = FindObjectOfType<CameraLookController>();

        groupSelectInterface = Instantiate(groupSelectInterfacePrefab, transform.position, Quaternion.identity);
        groupSelectInterface.SetActive(false);

    }
    IEnumerator lateSet(){
        return new WaitForSecondsRealtime(0.1f);
        List<GameObject> a = new List<GameObject>();
        a.Add(GetComponent<ShipSelectInterface>().currentlyControlledShip);
        setSelectedShips(a);
    }

    public void setControllableShips(List<GameObject> ships){
        controllableShips = ships;
    }
    public void setSelectedShips(List<GameObject> ships){
        
        // disable selected marker
        foreach(GameObject ship in selectedShips){
            if(ship.GetComponentInChildren<HighlightMarker>()!=null)ship.GetComponentInChildren<HighlightMarker>().gameObject.GetComponent<MeshRenderer>().enabled = false;
            
        }

        if(ships.Count > 0) selectedShips = ships;
        // if we have selected more than one ship, set the currently controlled flag of all of em to false and bring up the group control interface
        // set a marker on them to say that they are selected
        foreach(GameObject ship in selectedShips){
           if(ship.GetComponentInChildren<HighlightMarker>()!=null) ship.GetComponentInChildren<HighlightMarker>().gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
         finaliseSelection();
        
    }
    public void finaliseSelection(){
        
        foreach(GameObject shipObject in selectables){
            shipObject.GetComponent<CaptialShipControl>().setCurrentlyControlled(false);
        }
        if(selectedShips.Count == 0) groupSelectInterface.SetActive(false);
        if(selectedShips.Count == 1){
            groupSelectInterface.SetActive(false);
            selectedShips[0].GetComponent<CaptialShipControl>().setCurrentlyControlled(true);
            
        }
        // enavble group controls
        else if(selectedShips.Count > 1){
            groupSelectInterface.SetActive(true);
            groupSelectInterface.GetComponent<GroupInteractionInterface>().setGroupControlChildren(selectedShips);
            //groupSelectInterface.GetComponentInChildren<Joystick>().ActivateJoystick();
        }
    }
    bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
   
    Vector2 a = p2 - p1;
    Vector2 b = p3 - p4;
    Vector2 c = p1 - p3;
   
    float alphaNumerator = b.y*c.x - b.x*c.y;
    float alphaDenominator = a.y*b.x - a.x*b.y;
    float betaNumerator  = a.x*c.y - a.y*c.x;
    float betaDenominator  = a.y*b.x - a.x*b.y;
   
    bool doIntersect = true;
   
    if (alphaDenominator == 0 || betaDenominator == 0) {
        doIntersect = false;
    } else {
       
        if (alphaDenominator > 0) {
            if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) {
                doIntersect = false;
               
            }
        } else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) {
            doIntersect = false;
        }
       
        if (doIntersect && betaDenominator > 0) {
            if (betaNumerator < 0 || betaNumerator > betaDenominator) {
                doIntersect = false;
            }
        } else if (betaNumerator > 0 || betaNumerator < betaDenominator) {
            doIntersect = false;
        }
    }
 
    return doIntersect;
    }
    bool doIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) 
    { 
        // Find the four orientations needed for general and 
        // special cases 
        float o1 = orientation(p1, q1, p2); 
        float o2 = orientation(p1, q1, q2); 
        float o3 = orientation(p2, q2, p1); 
        float o4 = orientation(p2, q2, q1); 
    
        // General case 
        if (o1 != o2 && o3 != o4) 
            return true; 
    
        // Special Cases 
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
        if (o1 == 0 && onSegment(p1, p2, q1)) return true; 
    
        // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
        if (o2 == 0 && onSegment(p1, q2, q1)) return true; 
    
        // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
        if (o3 == 0 && onSegment(p2, p1, q2)) return true; 
    
        // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
        if (o4 == 0 && onSegment(p2, q1, q2)) return true; 
    
        return false; // Doesn't fall in any of the above cases 
    } 
    bool onSegment(Vector2 p, Vector2 q, Vector2 r) 
    { 
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) && 
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y)) 
        return true; 
    
        return false; 
    } 
    float orientation(Vector2 p, Vector2 q, Vector2 r) 
    { 
        // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
        // for details of below formula. 
        float val = (q.y - p.y) * (r.x - q.x) - 
                (q.x - p.x) * (r.y - q.y); 
    
        if (val == 0) return 0; // colinear 
    
        return (val > 0)? 1: 2; // clock or counterclock wise 
    } 
    private Vector3 GetMeanVector(List<Vector3> positions)
    {
     if (positions.Count == 0)
         return Vector3.zero;
     float x = 0f;
     float y = 0f;
     float z = 0f;
     foreach (Vector3 pos in positions)
     {
         x += pos.x;
         y += pos.y;
         z += pos.z;
     }
     return new Vector3(x / positions.Count, y / positions.Count, z / positions.Count);
    }
    
    void issueMoveOrder(Vector3 pos){
        // get all the selected ships
        // calculate an array of positions 

        // get the direction from the mean position of ships and pos. 
        List<Vector3> shipPositions = new List<Vector3>();
        foreach(GameObject ship in selectedShips){
            shipPositions.Add(ship.transform.position);
        }
        Vector3 sourcePos = GetMeanVector(shipPositions);
        
      //  Vector3 dir = sourcePos - pos;
      //  dir.Normalize();
        //then get the normal vector to that and add ofsets
        //GameObject destinationDummy = Instantiate(new GameObject(), pos, Quaternion.Euler())



        posList.Clear();
        int i = 0;
        float positionOffset = 200f;
        currentbeacon.transform.LookAt(sourcePos);
        currentbeacon.transform.position = pos;
        foreach(GameObject ship in selectedShips){
            Vector3 newPos = currentbeacon.transform.position + currentbeacon.transform.right * i * positionOffset;
            i++;
            posList.Add(newPos);
        }
        // now order the destinations in the order such that each ship's path doesn't intersect
       sortedSelected = selectedShips;
       
       // to do this, we get the 2d start and end points (x,z) of each ship's order and check they do not overlap
       bool success = false;
       // if they overlap, then shuffle the posList
       int maxTrials = 30000;
       int trials = 0;
       while (!success){
           
           bool noIntersections = true;
           posList = posList.OrderBy( x => Random.value ).ToList( );
           int j = 0;
           foreach(GameObject ship in selectedShips){
               Vector2 startpos = new Vector2(ship.transform.position.x, ship.transform.position.z);
               Vector2 endPos = new Vector2(posList[j].x, posList[j].z);
               int k = 0;
               foreach(GameObject otherShip in selectedShips){
                    Vector2 startpos2 = new Vector2(otherShip.transform.position.x, otherShip.transform.position.z);
                    Vector2 endPos2 = new Vector2(posList[k].x, posList[k].z);
                    // if the startpos endPos line isn't equal to startpos2 endPos2
                    if(j!=k){
                        if(FasterLineSegmentIntersection(startpos, endPos, startpos2, endPos2))noIntersections = false;
                        
                    }
                    
                    k++;
                   
               }
               j++;
           }
           
           
           
           if(noIntersections) {success = true; Debug.Log("No intersections on move order, bitch"); break;}
           if(trials > maxTrials){success = true; break;}
           trials ++;

           
           
       }



        


        i = 0;
        // go through the move orders. each ship takes the closest position. that one is then removed from the list
        foreach(GameObject ship in sortedSelected){
            if(Input.GetKey(KeyCode.LeftShift)){
               // Vector3 destination = closestMoveOrder(posList, ship.transform);
                //posList.Remove(destination);
               // ship.GetComponent<OrderManager>().addMoveOrder(destination);
               ship.GetComponent<OrderManager>().setMoveOrder(posList[i]);
            }
            else{
              //  Vector3 destination = closestMoveOrder(posList, ship.transform);
               // posList.Remove(destination);
               // ship.GetComponent<OrderManager>().setMoveOrder(destination);
               ship.GetComponent<OrderManager>().setMoveOrder(posList[i]);
            }
            i++;
            
        }

        // set their destination via ai

        // maintain their line renderer until they reach destination
        stage = 0;
    }
    
    Vector3 closestMoveOrder(List<Vector3> posList, Transform shipTransform){
        Vector3 closest = Vector3.zero;
        float minDistance = Mathf.Infinity;
        foreach(Vector3 pos in posList){
            float dist = Vector3.Distance(pos, shipTransform.position);
            if(dist<minDistance) closest = pos;
        }
        
        return closest;

    }



    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.F) && selectedShips.Count == 1){
                FindObjectOfType<CameraLookController>().setLookatStrategicMode(selectedShips[0].transform);
                FindObjectOfType<CameraLookController>().selectShip(selectedShips[0]);
        }
        else if(Input.GetKeyDown(KeyCode.F) && selectedShips.Count > 1){
            List<Vector3> selectedPositions = new List<Vector3>();
            foreach(GameObject ship in selectedShips){
                selectedPositions.Add(ship.transform.position);
            }
            Vector3 averagePos = GetMeanVector(selectedPositions);
            Transform newTransform = Instantiate(new GameObject(), averagePos, Quaternion.identity).transform;
            newTransform.rotation = Camera.main.transform.rotation;
            FindObjectOfType<CameraLookController>().setLookatStrategicMode(newTransform);
        }


        if(selectedShips.Count == 1){
            selectedShips[0].GetComponent<CaptialShipControl>().setCurrentlyControlled(true);
        }
        
        if (Input.GetMouseButtonDown(0) && !MoveMode && !EventSystem.current.IsPointerOverGameObject())
        {
           // if(!EventSystem.current.IsPointerOverGameObject()){
                Ray mouseToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                //Shoots a ray into the 3D world starting at our mouseposition
                if (Physics.Raycast(mouseToWorldRay, out hitInfo))
                {
                    //We check if we clicked on an object with a Selectable component
                    Ship s = hitInfo.collider.GetComponentInParent<Ship>();
                    if (s != null)
                    {
                        //While holding the copyKey, we can add and remove objects from our selection
                        //If we clicked on a Selectable, we don't want to enable our SelectionBox
                        s.defaultClickAction();
                        return;
                    }
                    else if (EventSystem.current.IsPointerOverGameObject())    // is the touch on the GUI
                    {
                        return;
                    }
                    
                }

                
    
                if (selectionBox == null)
                    return;
                //Storing these variables for the selectionBox
                startScreenPos = Input.mousePosition;
                isSelecting = true;
           // }
            
        }
 
        //If we never set the selectionBox variable in the inspector, we are simply not able to drag the selectionBox to easily select multiple objects. 'Regular' selection should still work
        if (selectionBox == null)
            return;
 
        //We finished our selection box when the key is released
        if (Input.GetMouseButtonUp(0) && !MoveMode && !EventSystem.current.IsPointerOverGameObject())
        {
            isSelecting = false;
            if(!EventSystem.current.IsPointerOverGameObject()){
                
                /*
                if(selectedShips.Count == 1){
                    // select the one ship
                    GetComponent<ShipSelectInterface>().selectShip(selectedShips[0]);
                }*/
                finaliseSelection();
            }
        }
 
        selectionBox.gameObject.SetActive(isSelecting);
 
        if (isSelecting && !MoveMode)
        {
            Bounds b = new Bounds();
            //The center of the bounds is inbetween startpos and current pos
            b.center = Vector3.Lerp(startScreenPos, Input.mousePosition, 0.5f);
            //We make the size absolute (negative bounds don't contain anything)
            b.size = new Vector3(Mathf.Abs(startScreenPos.x - Input.mousePosition.x),
                Mathf.Abs(startScreenPos.y - Input.mousePosition.y),
                0);
 
            //To display our selectionbox image in the same place as our bounds
            rt.position = b.center;
            rt.sizeDelta = canvas.transform.InverseTransformVector(b.size);
            List<GameObject> sShips = new List<GameObject>();
            //Looping through all the selectables in our world (automatically added/removed through the Selectable OnEnable/OnDisable)
            foreach (GameObject selectable in selectables)
            {
                //If the screenPosition of the worldobject is within our selection bounds, we can add it to our selection
                Vector3 screenPos = Camera.main.WorldToScreenPoint(selectable.transform.position);
                screenPos.z = 0;
                //UpdateSelection(selectable, (b.Contains(screenPos)));
                // if the box contains em
                if(b.Contains(screenPos))sShips.Add(selectable);
                
            }
            setSelectedShips(sShips);
            
        }
        
        if(!MoveMode) stage = 0;
        // detect a single right mouse click. 

        // if nothing is going on, check if the list of selected ships is not empty

        // if it is not, then initiate the move flag
        
        if(currentThreshold >= 0) currentThreshold -= Time.unscaledDeltaTime;
        if(Input.GetMouseButtonDown(1) && !MoveMode  && lookController.directControl){
            currentThreshold = singleClickThreshold;
            
        }

        


        
        


        
        if((Input.GetKeyUp(KeyCode.LeftShift) || Input.GetMouseButtonDown(0)) && MoveMode && stage == 1){
            Vertical = !Vertical; // toggles vertical move
        }


  

        if(((Input.GetMouseButtonUp(0) &&  stage == 2)|| Input.GetKeyUp(KeyCode.M)) && MoveMode ){
                issueMoveOrder(curPos);
                MoveMode = !MoveMode;                     // toggles the move mode
                Cursor.visible = MoveMode;                // hides the cursor
                EnterMoveMode(MoveMode);                  // this function instances the nev beacon and initializes the variables, when false destroys the beacon
        }


        //lastmovemode is updated at the end of lateUpdate
        if(lastMovemode != MoveMode){
            //instantiate the movement plane
            if(MoveMode == true){
                movementPlaneInstance = Instantiate(movementPlanePrefab, selectedShips[0].transform.position, Quaternion.Euler(Vector3.zero));
            }
            // destroy the movement plane
            if(MoveMode == false){
                Destroy(movementPlaneInstance);
            }
        }

        
        // check if we need to make/destroy a movement plane
    }


    void horizontalMovementPlane(){
        movementPlaneInstance.transform.rotation = Quaternion.Euler(Vector3.zero);
        movementPlaneInstance.transform.Find("VerticalExtra").gameObject.GetComponentInChildren<Collider>().enabled = false;
    }
    void verticalMovementPlane(){
        stage = 2;
        // set vertical movement plane to be along the horizontal component of the ship and beacon
        Vector3 dir = new Vector3 (horizLinerenderer.GetPosition(1).x, horizLinerenderer.GetPosition(0).y, horizLinerenderer.GetPosition(1).z)  - horizLinerenderer.GetPosition(0);
       // dir.Normalize();
        float angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
        movementPlaneInstance.transform.rotation = Quaternion.Euler(new Vector3(0f,angle,90f));
        movementPlaneInstance.transform.Find("VerticalExtra").gameObject.GetComponentInChildren<Collider>().enabled = true;
    }

    private void EnterMoveMode(bool enter){
        if(enter && beacon){
            stage = 1;
            GameObject go = GameObject.Instantiate(beacon, selectedShips[0].transform.position, Quaternion.identity) as GameObject;
            currentbeacon = go;
            go.SetActive(true);
            horizLinerenderer = go.GetComponent<LineRenderer>();
            vertLinerenderer = go.transform.GetChild(0).GetComponent<LineRenderer>();
 
            currentbeacon.transform.position = selectedShips[0].transform.position;
        }
        if(!enter){
            Vertical = false;
            Destroy(currentbeacon);
            stage = 0;
        }
    }

    private void upDateLineRenderers(){

        horizLinerenderer.SetPosition(0, selectedShips[0].transform.position);
        horizLinerenderer.SetPosition(1, currentbeacon.transform.position);
        
        vertLinerenderer.SetPosition(0,new Vector3(currentbeacon.transform.position.x, selectedShips[0].transform.position.y,currentbeacon.transform.position.z));
        vertLinerenderer.SetPosition(1,currentbeacon.transform.position);
        curPos = currentbeacon.transform.position;
    }

    void LateUpdate()
    {
        bool done = false;
        //sections moved due to execution order
        if(((Input.GetMouseButtonUp(1) && currentThreshold > 0  && Input.GetAxis("Mouse X") == 0) || Input.GetKeyUp(KeyCode.M)) && selectedShips.Count > 0  && !MoveMode){
            MoveMode = !MoveMode;                     // toggles the move mode
            Cursor.visible = MoveMode;                // hides the cursor
            EnterMoveMode(MoveMode);                  // this function instances the nev beacon and initializes the variables, when false destroys the beacon#
            done = true;
        }

        else if(Input.GetMouseButtonUp(1) &&  (stage != 0) && MoveMode ){

                MoveMode = !MoveMode;                     // toggles the move mode
                Cursor.visible = MoveMode;                // hides the cursor
                EnterMoveMode(MoveMode);                  // this function instances the nev beacon and initializes the variables, when false destroys the beacon
        }

        // complete and issue order

        


        if(MoveMode && !Vertical)
        {
            /*
            xinput = Input.GetAxis("Mouse X")*Damp;
            zinput = Input.GetAxis("Mouse Y")*Damp;
            yinput = selectedShips[0].transform.position.y;

            //get direction vector of maincam to selected ship
            Vector3 dir = selectedShips[0].transform.position - Camera.main.transform.position;

            //flatten the vector
            dir = new Vector3(dir.x, 0, dir.z);
            //normalise the vector
            dir.Normalize();*/
            
            // instantiate a movement plane along the 2d world and get a raycast position
            horizontalMovementPlane();

            // do a raycast
            Vector3 newpos = Vector3.zero;
            if(currentbeacon != null) newpos = currentbeacon.transform.position;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9)){
                newpos = hit.point;
            }

            if(currentbeacon != null){
                currentbeacon.transform.position = newpos;
            }
        }
        if(MoveMode && Vertical)
        {
         //   xinput = selectedShips[0].transform.position.y;
           // zinput = selectedShips[0].transform.position.z;
            verticalMovementPlane();
            Vector3 newpos = Vector3.zero;
            if(currentbeacon != null) newpos = currentbeacon.transform.position;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9)){
                newpos = hit.point;
            }

            if(currentbeacon != null){
                currentbeacon.transform.position = new Vector3(currentbeacon.transform.position.x, newpos.y, currentbeacon.transform.position.z);
            }  
        }
        if(MoveMode && currentbeacon){
            upDateLineRenderers();
        }

        lastMovemode = MoveMode;
    }
}
