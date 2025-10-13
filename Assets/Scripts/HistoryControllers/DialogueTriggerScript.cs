using System;
using UnityEngine;

public class DialogueTriggerScript : MonoBehaviour
{
    public string dialogueName;
    public bool canBePlayedOnce;
    public bool hasSpecificCondition;

    void Start()
    {
        if (GameController.Instance.DialogueAlreadyPlayed(dialogueName))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DialogueManagement.Instance.StartDialogue(dialogueName);

        if (canBePlayedOnce)
        {
            GameController.Instance.SaveDialoguePlayed(dialogueName);
            Destroy(gameObject);
        }
    }
}
