using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static GameDatabase;
using static PauseController;

public class SkillTreeController : MonoBehaviour
{
    public static SkillTreeController Instance { get; private set; }

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

    private GameObject SkillTreePanel;
    private Volume GlobalVolume;

    private Button[] buttons;

    void Start()
    {
        SkillTreePanel = FindInactive.FindUIElement("SkillTreePanel");
        buttons = SkillTreePanel.GetComponentsInChildren<Button>();
        GlobalVolume = FindObjectOfType<Volume>();
        SkillTreePanel.SetActive(false);

        VerifyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I)){
            HandleSkillTree();
        }
    }

    public void HandleSkillTree(){
        if(PauseController.Instance.ChangeFlowTime(PauseMode.SkillTree)){
            LearnNewRecipeMinigameController.Instance.ClearFields();

            SkillTreePanel.SetActive(PauseController.Instance.pausedBySkillTree);

            if (GlobalVolume.profile.TryGet(out DepthOfField dof)){
                dof.focusDistance.overrideState = PauseController.Instance.pausedBySkillTree;
            }
        }
    }

    public void VerifyButtons(){

        for(int count = 0; count < buttons.Count(); count++){
            buttons[count].gameObject.SetActive(GameController.Instance.VerifyItemFound((SkillEnumerator)count));

            var buttonIcon = buttons[count].transform.Find("Icon");
        
            if (buttonIcon != null){
                
                if (buttonIcon.TryGetComponent<RawImage>(out var buttonImage))
                {
                    buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 
                    GameController.Instance.VerifyItemLearned((SkillEnumerator)count) ? 1f : 0.2f);
                }
                else{
                    Debug.Log("Não encontrado componente RawImage do \"Icon\" do botão da Skill Tree");
                }
            }
            else{
                Debug.Log("Não encontrado \"Icon\" do botão da Skill Tree");
            }
        }
    }
}
