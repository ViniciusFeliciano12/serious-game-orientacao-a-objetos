using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static GameDatabase;
using static PauseController;
using System;

public class SkillTreeController : MonoBehaviour
{
    public static SkillTreeController Instance { get; private set; }

    private GameObject SkillTreePanel;
    private Volume GlobalVolume;
    private Button[] buttons;
    private List<GameObject> connectionLines = new List<GameObject>();

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

    private void Start()
    {
        SkillTreePanel = FindInactive.FindUIElement("SkillTreePanel");
        buttons = SkillTreePanel.GetComponentsInChildren<Button>();
        GlobalVolume = FindObjectOfType<Volume>();

        SkillTreePanel.SetActive(false);

        VerifyButtons();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleSkillTree();
        }
    }

    public void ToggleSkillTree()
    {
        if (PauseController.Instance.ChangeFlowTime(PauseMode.SkillTree))
        {
            LearnNewRecipeMinigameController.Instance.ClearFields();
            SkillTreePanel.SetActive(PauseController.Instance.pausedBySkillTree);

            if (GlobalVolume.profile.TryGet(out DepthOfField dof))
            {
                dof.focusDistance.overrideState = PauseController.Instance.pausedBySkillTree;
            }
        }
    }

    public void VerifyButtons()
    {
        try
        {
            for (int count = 0; count < buttons.Length; count++)
            {
                var button = buttons[count];
                var buttonScript = button.GetComponent<SkillTreeButton>();

                bool selfFound = GameController.Instance.VerifyItemFound(buttonScript.itemID);


                if (!selfFound)
                {
                    button.gameObject.SetActive(false);
                    continue;
                }

                bool canAppear = true;
                bool showXMark = false;
                bool isLearned = GameController.Instance.VerifyItemLearned(buttonScript.itemID);

                if (buttonScript.dependsOnAncestorItem != null && buttonScript.dependsOnAncestorItem.Length > 0)
                {
                    bool allAncestorsFound = true;
                    bool allAncestorsLearned = true;

                    foreach (var ancestor in buttonScript.dependsOnAncestorItem)
                    {
                        if (!GameController.Instance.VerifyItemFound(ancestor))
                        {
                            allAncestorsFound = false;
                            break;
                        }
                        if (!GameController.Instance.VerifyItemLearned(ancestor))
                        {
                            allAncestorsLearned = false;
                        }
                    }

                    if (!allAncestorsFound)
                    {
                        canAppear = false;
                    }
                    else if (!allAncestorsLearned)
                    {
                        showXMark = true;
                        isLearned = false;
                    }
                }

                button.gameObject.SetActive(canAppear);

                if (canAppear)
                {
                    var buttonIcon = button.transform.Find("Icon");
                    if (buttonIcon != null && buttonIcon.TryGetComponent<RawImage>(out var buttonImage))
                    {
                        buttonImage.color = new Color(
                            buttonImage.color.r,
                            buttonImage.color.g,
                            buttonImage.color.b,
                            isLearned ? 1f : 0.2f
                        );
                    }

                    var xMark = button.transform.Find("XMark");
                    if (xMark != null)
                    {
                        xMark.gameObject.SetActive(showXMark);
                    }

                    // Verifica se o botão possui ancestrais e cria as linhas se necessário
                    CreateConnectionLinesForButton(button, buttonScript);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        
    }

    private void CreateConnectionLinesForButton(Button button, SkillTreeButton buttonScript)
    {
        // Limpa as linhas de conexão antigas
        foreach (var line in connectionLines)
        {
            Destroy(line);
        }
        connectionLines.Clear();

        // Cria novas linhas de conexão
        if (buttonScript.dependsOnAncestorItem != null && buttonScript.dependsOnAncestorItem.Length > 0)
        {
            foreach (var ancestorID in buttonScript.dependsOnAncestorItem)
            {
                var ancestorButton = FindButtonBySkillID(ancestorID);

                // Verifica se o ancestral e o item atual estão visíveis (ativos)
                if (ancestorButton != null && ancestorButton.gameObject.activeSelf && button.gameObject.activeSelf)
                {
                    GameObject lineObj = CreateLineBetween(button.transform, ancestorButton.transform);
                    connectionLines.Add(lineObj);
                }
            }
        }
    }

    private GameObject CreateLineBetween(Transform from, Transform to)
    {
        // Cria um novo objeto para a linha
        GameObject lineObj = new GameObject("SkillTreeConnectionLine", typeof(RectTransform), typeof(Image));

        // Coloca a linha atrás dos botões
        lineObj.transform.SetParent(SkillTreePanel.transform, false);
        lineObj.transform.SetSiblingIndex(0);  // Coloca a linha atrás dos botões

        // Acessando componentes do RectTransform e Image
        RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
        Image image = lineObj.GetComponent<Image>();

        // Carregue o sprite de uma linha suave (faça isso no Unity e adicione a textura como um Sprite)
        Sprite lineSprite = Resources.Load<Sprite>("LineSprite"); // Substitua pelo caminho do seu sprite de linha

        if (lineSprite != null)
        {
            image.sprite = lineSprite;
        }
        else
        {
            // Se não encontrar o sprite, use uma cor sólida (preta)
            image.color = Color.black;
        }

        // Calculando a posição e o tamanho da linha
        Vector3 fromPos = from.position;
        Vector3 toPos = to.position;

        // Calculando o ponto médio para a linha
        Vector3 midPoint = (fromPos + toPos) / 2f;
        rectTransform.position = midPoint;

        // Calculando a distância entre os pontos para definir o tamanho da linha
        float distance = Vector3.Distance(fromPos, toPos);
        rectTransform.sizeDelta = new Vector2(distance, 5f);  // Largura da linha (ajustável)

        // Calculando a direção da linha (ângulo)
        Vector3 dir = (toPos - fromPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Rotacionando a linha para que ela fique alinhada entre os pontos
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        return lineObj;
    }


    private Button FindButtonBySkillID(SkillEnumerator id)
    {
        foreach (var button in buttons)
        {
            var buttonScript = button.GetComponent<SkillTreeButton>();
            if (buttonScript != null && buttonScript.itemID == id)
            {
                return button;
            }
        }
        return null;
    }
}
