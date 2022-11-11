using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeControlInterface : MonoBehaviour
{

    public List<GameObject> buttons;
    public List<float> timeRates;

    public AudioClip speedUp;
    public AudioClip slowDown;

    [SerializeField]
    int timeRateIndex;
    private float fixedDeltaTime;

    // Start is called before the first frame update
    void Start()
    {
        setTimeRateIndex(timeRateIndex);
    }
    void Awake(){
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            incrementTimeIndex();
        }
    }

    public void incrementTimeIndex(){
       // timeRateIndex += 1;
       // if(timeRateIndex > timeRates.Count -1)timeRateIndex = 0;
        setTimeRateIndex(timeRateIndex + 1);
    }

    public void setTimeRateIndex(int index){
        if(index > timeRates.Count -1)index = 0;
        if(index != timeRateIndex){
            if(timeRateIndex == 0 && !GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().PlayOneShot(speedUp);
            else if(index == 0 && !GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().PlayOneShot(slowDown);
        }
        timeRateIndex = index;

        Time.timeScale = timeRates[timeRateIndex];
        foreach(GameObject button in buttons){
            button.GetComponent<interactActionUI>().reset();
        }
        buttons[timeRateIndex].GetComponent<interactActionUI>().interact();
      //  Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
}
