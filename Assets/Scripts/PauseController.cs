using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    private GameObject Panel;
    public bool pausedGame = false;
    public bool pausedBySkillTree = false;

    public bool timeStopped 
    {
        get => pausedGame || pausedBySkillTree;
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
        Panel = FindInactive.FindUIElement("PausePanel");
        Panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            PauseUnPause();
        }
    }

    public bool ChangeFlowTime(PauseMode mode){
        switch(mode){
            case PauseMode.Pause: 
                if (!pausedBySkillTree)
                    pausedGame = !pausedGame; 
                else return false;
            break;

            case PauseMode.SkillTree: 
                if(!pausedGame)
                    pausedBySkillTree = !pausedBySkillTree; 
                else return false;
            break;
        }

        Time.timeScale = timeStopped ? 0.00001f : 1f;
        return true;
    }

    public void PauseUnPause(){
        ChangeFlowTime(PauseMode.Pause);
        Panel.SetActive(pausedGame);
    }

    public void BackToMenu(){
        SceneManager.LoadScene(0);
    }

    public enum PauseMode{
        Pause,
        SkillTree
    }
}
