using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameDatabase;
using static PauseController;
using System;

public class SkillTreeController : MonoBehaviour
{
    public static SkillTreeController Instance { get; private set; }

    private GameObject SkillTreePanel;
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
        buttons = SkillTreePanel.GetComponentsInChildren<Button>(true); // Inclui botões inativos na busca inicial

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
            SkillTreePanel.SetActive(PauseController.Instance.currentState.Equals(InternalPauseState.SkillTree));

            if (SkillTreePanel.activeSelf)
            {
                VerifyButtons();
            }
        }
    }

    public void VerifyButtons()
    {
        try
        {
            // ETAPA 1: Atualizar o estado de todos os botões (visibilidade, cor, etc.)
            foreach (var button in buttons)
            {
                var buttonScript = button.GetComponent<SkillTreeButton>();
                if (buttonScript == null) continue;

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
                }
            }

            UpdateAllConnectionLines();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao verificar os botões da Skill Tree: {ex}");
        }
    }

    /// <summary>
    /// Limpa as linhas de conexão antigas e recria todas as conexões para os botões visíveis.
    /// </summary>
    private void UpdateAllConnectionLines()
    {
        foreach (var line in connectionLines)
        {
            Destroy(line);
        }
        connectionLines.Clear();

        foreach (var button in buttons)
        {
            if (!button.gameObject.activeSelf)
            {
                continue;
            }

            var buttonScript = button.GetComponent<SkillTreeButton>();
            if (buttonScript == null || buttonScript.dependsOnAncestorItem == null || buttonScript.dependsOnAncestorItem.Length == 0)
            {
                continue;
            }

            foreach (var ancestorID in buttonScript.dependsOnAncestorItem)
            {
                var ancestorButton = FindButtonBySkillID(ancestorID);

                if (ancestorButton != null && ancestorButton.gameObject.activeSelf)
                {
                    GameObject lineObj = CreateLineBetween(button.transform, ancestorButton.transform);
                    connectionLines.Add(lineObj);
                }
            }
        }
    }

    private GameObject CreateLineBetween(Transform from, Transform to)
    {
        GameObject lineObj = new GameObject("SkillTreeConnectionLine", typeof(RectTransform), typeof(Image));

        lineObj.transform.SetParent(SkillTreePanel.transform, false);
        lineObj.transform.SetAsFirstSibling(); // Coloca a linha atrás de todos os outros elementos da UI

        RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
        Image image = lineObj.GetComponent<Image>();
        image.color = Color.black; // Cor padrão da linha

        Vector3 fromPos = from.position;
        Vector3 toPos = to.position;

        Vector3 midPoint = (fromPos + toPos) / 2f;
        rectTransform.position = midPoint;

        float distance = Vector3.Distance(fromPos, toPos);
        rectTransform.sizeDelta = new Vector2(distance, 5f);

        Vector3 dir = (toPos - fromPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
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