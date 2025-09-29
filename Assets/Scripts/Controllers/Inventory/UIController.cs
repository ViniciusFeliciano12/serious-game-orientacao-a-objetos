using System.Collections;
using EJETAGame;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("Referências UI")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject playerInfoPanel;

    private TextMeshProUGUI tooltipText;
    private TextMeshProUGUI textAppear;
    private TextMeshProUGUI textAppearWithTimeout;
    private TextMeshProUGUI lifeText;
    private TextMeshProUGUI scrollsText;
    private RectTransform currentSlotRect;
    private bool isShowing;

    // Constantes de tempo para fade
    private const float FadeDuration = 0.5f;
    private const float VisibleTime = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Init();
    }

    private void Update()
    {
        RepositionTooltip();
    }

    #region Inicialização

    private void Init()
    {
        HideTooltip();
        LoadInterfaceItems();
    }
    private void LoadInterfaceItems()
    {
        if (!tooltipPanel || !interactionPanel || !playerInfoPanel)
        {
            Debug.LogError("Algum painel não foi atribuído na cena!", this);
            enabled = false;
            return;
        }

        tooltipText = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>(true);
        if (!tooltipText)
        {
            Debug.LogError("Texto do tooltip não encontrado!", this);
            enabled = false;
            return;
        }

        var interactionTexts = interactionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
        if (interactionTexts.Length >= 2)
        {
            textAppear = interactionTexts[0];
            textAppearWithTimeout = interactionTexts[1];
        }
        else
        {
            Debug.LogError("Painel de interação precisa de 2 textos!", this);
            enabled = false;
            return;
        }

        var infoTexts = playerInfoPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
        if (infoTexts.Length >= 2)
        {
            lifeText = infoTexts[0];
            scrollsText = infoTexts[1];
        }
        else
        {
            Debug.LogError("Painel do jogador precisa de 2 textos!", this);
            enabled = false;
        }
    }

    #endregion

    #region Player HUD

    public void UpdateHUD(int? lifes = null, int? scrolls = null)
    {
        if (lifeText && lifes != null) lifeText.SetText($"{lifes}/10");
        if (scrollsText && scrolls != null) scrollsText.SetText($"{scrolls}");
    }

    #endregion

    #region Interaction Text

    public void UpdateTextAppear(bool active) => textAppear?.gameObject.SetActive(active);

    public void SetText(string text) => textAppear?.SetText(text);

    public void SetTextTimeout(string text)
    {
        if (!textAppearWithTimeout) return;
        StopAllCoroutines();
        StartCoroutine(FadeTextRoutine(text));
    }

    private IEnumerator FadeTextRoutine(string text)
    {
        textAppearWithTimeout.SetText(text);
        Color baseColor = textAppearWithTimeout.color;

        yield return FadeRoutine(0f, 1f, FadeDuration, baseColor); // Fade-in
        yield return new WaitForSeconds(VisibleTime);
        yield return FadeRoutine(1f, 0f, FadeDuration, baseColor); // Fade-out
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, Color baseColor)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(from, to, timer / duration);
            textAppearWithTimeout.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        textAppearWithTimeout.color = new Color(baseColor.r, baseColor.g, baseColor.b, to);
    }

    #endregion

    #region Tooltip

    private void RepositionTooltip()
    {
        if (tooltipPanel.activeSelf && currentSlotRect != null)
        {
            Vector2 offset = new(0f, 230f);
            tooltipPanel.GetComponent<RectTransform>().position = currentSlotRect.position + (Vector3)offset;
        }
    }

    public void ShowTooltip(ItemDatabase item, RectTransform rect)
    {
        if (isShowing || item == null) return;

        currentSlotRect = rect;
        isShowing = true;
        tooltipPanel.SetActive(true);

        string props = string.Join(", ", item.propriedades.ConvertAll(p => $"{p.chave} {p.valor}"));
        string methods = string.Join(", ", item.metodos.ConvertAll(m => $"{m.chave} {m.valor}"));

        tooltipText.text = $"<b>{item.nome}</b>\n{props}\n\n<i>Métodos disponíveis:</i>\n{methods}";
    }

    public void HideTooltip()
    {
        isShowing = false;
        tooltipPanel.SetActive(false);
    }

    #endregion
}