using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavLogic : MonoBehaviour {

	public void LoadMenu()
    {
        SceneManager.LoadScene("Select");
    }

    public void LoadStart()
    {
        SceneManager.LoadScene("Start");
    }

    public void Generate() {
        SceneManager.LoadScene("MapGenScene");
    }

    public void Return()
    {
        string currScene = SceneManager.GetActiveScene().name;
        if (currScene == "MapGenScene")
        {
            LoadMenu();
        }
        else if (currScene == "Select")
        {
            LoadStart();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
