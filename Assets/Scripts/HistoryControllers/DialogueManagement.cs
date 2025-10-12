using Esper.FeelSpeak;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueManagement : MonoBehaviour
{
    public static DialogueManagement Instance { get; private set; }

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
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
        VerifyStartDialogue();
    }


    private void VerifyStartDialogue()
    {
        if (!GameController.Instance.VerifyItemFound(GameDatabase.SkillEnumerator.Crowbar))
        {
            StartDialogue("First Dialogue");
        }
    }

    public void DialogueStart()
    {
        Time.timeScale = 1.0f;
    }
        
    public void DialogueEnd()
    {
        if (PauseController.Instance.TimeStopped)
        {
            Time.timeScale = 0f;
        }
    }

    public bool HasActiveDialogue()
    {
        return FeelSpeak.HasActiveDialogue;
    }

    public async void StartDialogue(string dialogueGraph)
    {
        await Task.Delay(200);
        FeelSpeak.TriggerDialogue(FeelSpeak.GetDialogueGraph(dialogueGraph));
    }
}
