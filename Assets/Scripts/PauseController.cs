using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    public GameObject Panel;
    public bool pausedGame = false;
    public bool pausedByMinigame = false;

    public bool timeStopped 
    {
        get => pausedGame || pausedByMinigame;
    }

    private void Awake()
    {
        if(Instance!= null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

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

    public void ChangeFlowTime(){
        Time.timeScale = timeStopped ? 0.00001f : 1f;
    }

    public void PauseUnPause(){
        if (pausedByMinigame){
            return;
        }
        
        pausedGame = !pausedGame;
        ChangeFlowTime();
        Panel.SetActive(pausedGame);
    }

    public void BackToMenu(){
        SceneManager.LoadScene(0);
    }
}
