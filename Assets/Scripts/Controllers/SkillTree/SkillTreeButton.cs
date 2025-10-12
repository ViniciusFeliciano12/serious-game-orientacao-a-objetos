using Assets.Models;
using System.Collections.Generic;
using UnityEngine;
using static GameDatabase;

public class SkillTreeButton : MonoBehaviour
{
    public string title;
    public string iconPath;
    public SkillEnumerator itemID;
    public bool isInterface;
    public SkillEnumerator[] dependsOnAncestorItem;
    public List<Item> propriedades;
    public List<Item> metodos;
    public List<string> palavras;
    public List<DialogueTrigger> dialogueTriggers;

    private Transform xMark; // Guardar referência pro XMark

    private void Start()
    {
        xMark = transform.Find("XMark"); // Já pega o XMark no Start
    }

    public void Clicked()
    {
        if (xMark != null && xMark.gameObject.activeSelf)
        {
            return;
        }

        if (!GameController.Instance.VerifyItemLearned(itemID))
        {
            StartNewRecipeMinigame();
            return;
        }

        if (isInterface)
        {
            HandleInterfaceItem();
        }
        else
        {
            StartCraftItemMinigame();
        }
    }

    private void StartNewRecipeMinigame()
    {
        LearnNewRecipeMinigameController.Instance.StartNewGame(title, itemID, palavras, propriedades, metodos, dialogueTriggers, isInterface);
    }

    private void HandleInterfaceItem()
    {
        LearnNewRecipeMinigameController.Instance.ClearFields();
        UIController.Instance.SetTextTimeout("Classes abstratas não podem ser instanciadas");
    }

    private void StartCraftItemMinigame()
    {
        LearnNewRecipeMinigameController.Instance.CraftItem(title, itemID, propriedades, metodos, dialogueTriggers, iconPath);
    }

    public void StartCraftItemMinigameReusable(ItemDatabase item, int index)
    {
        SkillTreeController.Instance.ToggleSkillTree();

        LearnNewRecipeMinigameController.Instance.CraftItem(title, itemID, propriedades, metodos, dialogueTriggers, iconPath, item, index);
    }
}