using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static GameDatabase;

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

    private bool isActive = false;
    public GameObject SkillTreePanel;
    private Volume GlobalVolume;

    private Button[] buttons;

    void Start()
    {
        buttons = SkillTreePanel.GetComponentsInChildren<Button>();
        GlobalVolume = FindObjectOfType<Volume>();
        SkillTreePanel.SetActive(isActive);

        VerifyButtons();
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
    }

    public void VerifyButtons(){

        for(int count = 0; count < buttons.Count(); count++){
            buttons[count].gameObject.SetActive(GameController.Instance.VerifyItemFound((ItemEnumerator)count));

            var buttonIcon = buttons[count].transform.Find("Icon");
        
            if (buttonIcon != null){
                
                if (buttonIcon.TryGetComponent<RawImage>(out var buttonImage))
                {
                    buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 
                    GameController.Instance.VerifyItemLearned((ItemEnumerator)count) ? 1f : 0.2f);
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
