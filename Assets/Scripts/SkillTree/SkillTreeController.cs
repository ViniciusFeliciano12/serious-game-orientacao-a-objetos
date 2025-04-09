using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SkillTreeController : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isActive = false;
    public GameObject SkillTreePanel;
    private Volume GlobalVolume;

    private Button[] buttons;

    void Start()
    {
        buttons = SkillTreePanel.GetComponentsInChildren<Button>();
        GlobalVolume = FindObjectOfType<Volume>();
        SkillTreePanel.SetActive(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I)){
            isActive = !isActive;
            SkillTreePanel.SetActive(isActive);

            if (GlobalVolume.profile.TryGet(out DepthOfField dof)){
                dof.focusDistance.overrideState = isActive;
            }

            Debug.Log("Skill tree " + (isActive ? "active" : "inactive"));
        }

        VerifyButtonsEnabled();
    }

    private void VerifyButtonsEnabled(){
        buttons[0].interactable = GameController.instance.VerifyRecipesUnlocked(GameController.Recipes.Key);
        buttons[1].interactable = GameController.instance.VerifyRecipesUnlocked(GameController.Recipes.Sword);
    }
}
