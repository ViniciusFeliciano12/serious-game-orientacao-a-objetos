using Esper.FeelSpeak;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

delegate bool DestroyDialogue();

public class DialogueManagement : MonoBehaviour
{
    public static DialogueManagement Instance { get; private set; }

    private Dictionary<string, DestroyDialogue> triggerDialoguesCondition;

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

        triggerDialoguesCondition = new Dictionary<string, DestroyDialogue>
        {
             { "EnemyInSightDialogue", EnemyInSightDialogue },
        };
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

    #region World Trigger Dialogues

    public bool VerifyDialogueStillExists(string dialogueGraph)
    {
        if (triggerDialoguesCondition.TryGetValue(dialogueGraph, out DestroyDialogue useAction))
        {
            return useAction.Invoke();
        }

        return false;
    }

    private bool EnemyInSightDialogue()
    {
        return GameController.Instance.GetInventoryCount() > 4;
    }

    #endregion
}
