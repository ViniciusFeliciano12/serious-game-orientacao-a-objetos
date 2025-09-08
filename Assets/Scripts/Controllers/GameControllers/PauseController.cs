using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    private GameObject Panel;
    private GameObject GameOverPanel;
    public bool pausedGame = false;
    public bool pausedBySkillTree = false;
    public bool isDead = false;
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
        GameOverPanel = FindInactive.FindUIElement("GameOver Panel");
        Panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            PauseUnPause();
        }

        Time.timeScale = timeStopped ? 0.00001f : 1f;
    }

    public bool ChangeFlowTime(PauseMode mode){
        switch (mode)
        {
            case PauseMode.Pause:
                if (!pausedBySkillTree)
                    pausedGame = !pausedGame;
                else return false;
                break;

            case PauseMode.SkillTree:
                if (!pausedGame)
                    pausedBySkillTree = !pausedBySkillTree;
                else return false;
                break;

            case PauseMode.GameOver:
                pausedGame = true;
                isDead = true;
                GameOverPanel.SetActive(true);
                break;
        }

        return true;
    }

    public void PauseUnPause(){
        if (!isDead)
        {
            ChangeFlowTime(PauseMode.Pause);
            Panel.SetActive(pausedGame);
        }
    }

    public void ReloadScene()
    {
        isDead = false;
        PauseUnPause();
        GameController.Instance.ReloadScene();
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public enum PauseMode
    {
        Pause,
        SkillTree,
        GameOver
    }
}
