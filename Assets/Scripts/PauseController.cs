using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject Panel;
    private bool pausedGame = false;
    void Start()
    {
        Panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            PauseUnPause();
        }
    }

    public void PauseUnPause(){
        pausedGame = !pausedGame;
        Panel.SetActive(pausedGame);
        Time.timeScale = pausedGame ? 0.00001f : 1f;
    }
}
