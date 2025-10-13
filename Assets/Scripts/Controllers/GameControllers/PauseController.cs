using Esper.FeelSpeak;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    public enum PauseMode
    {
        Pause,
        SkillTree,
        GameOver,
    }

    public enum InternalPauseState
    {
        None,
        GamePaused,
        SkillTree,
        GameOver
    }

    public InternalPauseState currentState = InternalPauseState.None;

    private GameObject Panel;
    private GameObject GameOverPanel;
    private Volume GlobalVolume;

    public bool TimeStopped => currentState != InternalPauseState.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
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
        GlobalVolume = FindObjectOfType<Volume>();

        if (Panel != null) Panel.SetActive(false);
        if (GameOverPanel != null) GameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        currentState = InternalPauseState.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public bool ChangeFlowTime(PauseMode mode)
    {
        bool changedFlowTime = false;

        if (DialogueManagement.Instance.HasActiveDialogue())
        {
            return false;
        }

        switch (mode)
        {
            case PauseMode.Pause:

                if (currentState == InternalPauseState.None || currentState == InternalPauseState.GamePaused)
                {
                    bool isNowPaused = currentState == InternalPauseState.None;

                    currentState = isNowPaused ? InternalPauseState.GamePaused : InternalPauseState.None;
                    Panel.SetActive(isNowPaused);
                    changedFlowTime = true;
                }

                break;

            case PauseMode.SkillTree:
                if (currentState == InternalPauseState.None || currentState == InternalPauseState.SkillTree)
                {
                    currentState = (currentState == InternalPauseState.None) ? InternalPauseState.SkillTree : InternalPauseState.None;
                    changedFlowTime = true;
                }
                
                break;

            case PauseMode.GameOver:
                currentState = InternalPauseState.GameOver;
                GameOverPanel.SetActive(true);
                changedFlowTime = true;
                break;
        }

        if (changedFlowTime)
        {
            if (GlobalVolume.profile.TryGet(out DepthOfField dof))
            {
                dof.focusDistance.overrideState = TimeStopped;
            }

            Time.timeScale = TimeStopped ? 0f : 1f;
        }

        return changedFlowTime;
    }

    public void TogglePause()
    {
        ChangeFlowTime(PauseMode.Pause);
    }

    public async void ReloadScene()
    {
        currentState = InternalPauseState.None;
        GameController.Instance.ReloadScene(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        await Task.Delay(200);

        DialogueManagement.Instance.StartDialogue("YouDiedDialogue");
    }

    public void BackToMenu()
    {
        currentState = InternalPauseState.None; 
        Time.timeScale = 1f; 
        GameController.Instance.SaveGame();
        SceneManager.LoadScene(0);
    }
}