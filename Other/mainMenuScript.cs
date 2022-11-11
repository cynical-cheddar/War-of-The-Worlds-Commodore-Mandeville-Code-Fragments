using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public string skirmishScene = "";

    public string moviesScene = "";
    public void skirmish(){
        SceneManager.LoadScene(skirmishScene);
    }

    public void movies(){
        SceneManager.LoadScene(moviesScene);
    }

    public void quit(){
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
