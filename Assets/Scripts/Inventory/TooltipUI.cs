using EJETAGame;
using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("Referências UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    private RectTransform rectTransform;
    private bool isShowing = false;

    private void Awake()
    {
        Instance = this;
        HideTooltip();
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            // Pega o retângulo do slot, e usa sua posição para calcular a do tooltip
            if (rectTransform != null)
            {
                Vector2 slotPos = rectTransform.position;

                // Ajuste a posição para que o tooltip apareça logo acima do slot
                Vector2 offset = new(0f, 230f);  // 40f é o deslocamento vertical para cima

                // Atualiza a posição do tooltip com o deslocamento
                tooltipPanel.GetComponent<RectTransform>().position = slotPos + offset;
            }
        }
    }

    public void ShowTooltip(ItemDatabase item, RectTransform rect)
    {
        if (isShowing || item == null) return;

        rectTransform = rect;
        isShowing = true;
        tooltipPanel.SetActive(true);

        // Customize as informações do tooltip com base nas propriedades do seu item
        string content = $"<b>{item.nome}</b>\n";
        foreach(var props in item.propriedades){
            content += $"{props.chave} {props.valor}, ";
        }

        content = content[..^2];

        content += "\n\n<i>Métodos disponíveis:</i>\n";

        foreach(var props in item.metodos){
            content += $"{props.chave} {props.valor}, ";
        }

        content = content[..^2];

        tooltipText.text = content;
    }

    public void HideTooltip()
    {
        isShowing = false;
        tooltipPanel.SetActive(false);
    }
}
