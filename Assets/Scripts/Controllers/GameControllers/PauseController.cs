using Esper.FeelSpeak;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    public enum PauseMode
    {
        Pause,
        SkillTree,
        GameOver,
        DeletingItem,
        EndGame
    }

    public enum InternalPauseState
    {
        None,
        GamePaused,
        SkillTree,
        GameOver,
        DeletingItem,
        EndGame
    }

    public InternalPauseState currentState = InternalPauseState.None;

    private GameObject Panel;
    private GameObject GameOverPanel;
    private RawImage PainelFade;
    private Volume GlobalVolume;

    public bool TimeStopped => currentState != InternalPauseState.None;

    public float duracaoFade = 1.5f;

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
        PainelFade = FindInactive.FindUIElement("PainelFade").GetComponent<RawImage>();
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
            case PauseMode.EndGame:
                currentState = InternalPauseState.EndGame;
                break;

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

            case PauseMode.DeletingItem:
                if (currentState == InternalPauseState.None || currentState == InternalPauseState.DeletingItem)
                {
                    currentState = (currentState == InternalPauseState.None) ? InternalPauseState.DeletingItem : InternalPauseState.None;
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

    public void FinishGame()
    {
        Time.timeScale = 0f;
        ChangeFlowTime(PauseMode.EndGame);
        IniciarFadeOut();
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

    #region Screen Management

    public void IniciarFadeOut()
    {
        StartCoroutine(RotinaFadeOut());
    }

    public void IniciarFadeIn()
    {
        StartCoroutine(RotinaFadeIn());
    }

    private IEnumerator RotinaFadeOut()
    {
        float tempoDecorrido = 0f;
        PainelFade.gameObject.SetActive(true);

        Color corDoPainel = PainelFade.color;

        while (tempoDecorrido < duracaoFade)
        {
            tempoDecorrido += Time.unscaledDeltaTime;

            float novoAlpha = Mathf.Clamp01(tempoDecorrido / duracaoFade);
            PainelFade.color = new Color(corDoPainel.r, corDoPainel.g, corDoPainel.b, novoAlpha);

            yield return null;
        }

        PainelFade.color = new Color(corDoPainel.r, corDoPainel.g, corDoPainel.b, 1f);

        MostrarHistoria(
            "John não apenas fugiu da prisão; ele a quebrou moralmente, de dentro para fora. Diante dele, o Minotauro, um guardião de eras, tombou não apenas por força, mas pelo brilho avassalador dos novos poderes que despertaram em sua alma.\n\n" +
            "Sua saga não foi simplesmente contada, mas entoada como um hino de liberdade. Sussurrada por crianças em seus leitos e aclamada por anciãos ao redor de fogueiras crepitantes, seu nome transformou-se em sinônimo de esperança.\n\n" +
            "Pois os anos que se seguiram foram de uma escuridão profunda, uma noite que ameaçava consumir o mundo. Mas os feitos de John tornaram-se um farol nessa treva, a centelha que permitiu a muitos não apenas sobreviver, mas lutar e acreditar em um novo amanhecer...\n\n" +
            "Ele se tornou a lenda que andava entre os homens.");
    }

    private IEnumerator RotinaFadeIn()
    {
        float tempoDecorrido = 0f;
        Color corDoPainel = PainelFade.color;

        while (tempoDecorrido < duracaoFade)
        {
            tempoDecorrido += Time.unscaledDeltaTime;

            float novoAlpha = 1f - Mathf.Clamp01(tempoDecorrido / duracaoFade);
            PainelFade.color = new Color(corDoPainel.r, corDoPainel.g, corDoPainel.b, novoAlpha);

            yield return null;
        }

        PainelFade.color = new Color(corDoPainel.r, corDoPainel.g, corDoPainel.b, 0f);
    }

    public async void MostrarHistoria(string historia)
    {
        foreach (char caracter in historia)
        {
            PainelFade.transform.Find("HistoriaText").GetComponent<TextMeshProUGUI>().text += caracter;

            await Task.Delay(25);
        }

        await Task.Delay(2000);

        SceneManager.LoadScene("CreditsScene");
    }

    #endregion

}