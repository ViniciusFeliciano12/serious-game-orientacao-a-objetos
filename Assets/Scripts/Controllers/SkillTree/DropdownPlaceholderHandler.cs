using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class DropdownPlaceholderHandler : MonoBehaviour, IPointerClickHandler
{
    private TMP_Dropdown dropdown;
    private List<TMP_Dropdown.OptionData> originalOptions;

    private bool hasPlaceholder = true;

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        // Guarda uma cópia das opções completas (incluindo o placeholder)
        originalOptions = new List<TMP_Dropdown.OptionData>(dropdown.options);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Quando o usuário clica no dropdown, removemos o placeholder
        if (hasPlaceholder && dropdown.options.Count > 0)
        {
            dropdown.options.RemoveAt(0);
            hasPlaceholder = false;

            // Opcional: seleciona o primeiro item real, mas não mostra no caption
            // dropdown.value = 0;
            dropdown.RefreshShownValue();
        }
    }

    public void RestorePlaceholder()
    {
        if (!hasPlaceholder)
        {
            dropdown.options = new List<TMP_Dropdown.OptionData>(originalOptions);
            dropdown.value = 0;
            dropdown.RefreshShownValue();
            hasPlaceholder = true;
        }
    }
}
