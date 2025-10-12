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

    // Update is called once per frame
    void Update()
    {

    }

    private void VerifyStartDialogue()
    {
        if (!GameController.Instance.VerifyItemFound(GameDatabase.SkillEnumerator.Crowbar))
        {
            StartDialogue("First Dialogue");
        }
    }

    public void ChangeFlowTime()
    {
        //PauseController.Instance.ChangeFlowTime(PauseController.PauseMode.Dialogue);
    }

    public async void StartDialogue(string dialogueGraph)
    {
        await Task.Delay(200);
        FeelSpeak.TriggerDialogue(FeelSpeak.GetDialogueGraph(dialogueGraph));
    }
}
