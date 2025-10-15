using UnityEngine;

public class DialogueTriggerScript : MonoBehaviour
{
    public string dialogueName;
    public bool canBePlayedOnce;
    public bool hasSpecificCondition;

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (canBePlayedOnce)
        {
            DialogueManagement.Instance.StartDialogueOnlyOnce(dialogueName);
        }
        else
        {
            DialogueManagement.Instance.StartDialogue(dialogueName);
        }
    }
}
