using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    string cancelTriggerName = "cancel";

    public Vector3 currentMoveOrder = Vector3.zero;

    public List<Vector3> moveOrderQueue = new List<Vector3>();

    public GameObject lineRendererPrefab;
    GameObject lineRendererInstance;

    LineRenderer lineRenderer;

    private void Start() {
        lineRendererInstance = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
        lineRenderer = lineRendererInstance.GetComponent<LineRenderer>();
    }
    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        Destroy(lineRendererInstance);
    }
    public void addMoveOrder(Vector3 pos){
        moveOrderQueue.Add(pos);
    }

    public void cancel(){
        moveOrderQueue.Clear();
        GetComponent<Animator>().SetTrigger(cancelTriggerName);
        currentMoveOrder = Vector3.zero;
    }

    public void setMoveOrder(Vector3 pos){
        moveOrderQueue.Clear();
        GetComponent<Animator>().SetTrigger(cancelTriggerName);
        moveOrderQueue.Add(pos);
        currentMoveOrder = pos;
    } 

    private void LateUpdate() {
        // if moveOrderQueue[0] is not Vector3.zero, draw a line to where we are going
        if(moveOrderQueue.Count > 0){
            lineRenderer.positionCount = moveOrderQueue.Count + 1;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentMoveOrder); 
            for(int i = 2; i <= moveOrderQueue.Count; i++){
                lineRenderer.SetPosition(i, moveOrderQueue[i-1]);
            }
        }
        else{
            lineRenderer.enabled = false;
        }
    }

    public Vector3 executeOrder(){
        if(moveOrderQueue.Count > 0){
            currentMoveOrder = moveOrderQueue[0];
            
            return currentMoveOrder;
        }
        else return currentMoveOrder;
        
    }
    public void completeOrder(){
        moveOrderQueue.RemoveAt(0);
    }
}
