using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using static GameDatabase;
using System.Linq;
using EJETAGame;

public class LearnNewRecipeMinigameController : MonoBehaviour
{
    public static LearnNewRecipeMinigameController Instance { get; private set; }

    // Prefabs e elementos de UI
    public GameObject fieldPrefab;
    public GameObject wordPrefab;
    public GameObject dropdownPrefab;
    private RectTransform propriedadesArea;
    private RectTransform metodosArea;
    private RectTransform palavrasArea;
    private RectTransform minigameArea;
    private Button learnButton;
    private Button criarButton;

    // Dados do jogo
    private List<Item> propriedades;
    private List<Item> metodos;
    private List<string> palavras;
    private Texture texture;
    private SkillEnumerator itemID;
    private int correctWords = 0;
    private TextMeshProUGUI title;

    // Estado
    private List<object> dropdowns = new List<object>();

    private const float padding = 20f;

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
        title = FindInactive.FindUIElement("Nome").GetComponent<TextMeshProUGUI>();
        minigameArea = FindInactive.FindUIElement("PointAndClickMinigame").GetComponent<RectTransform>();
        palavrasArea = FindInactive.FindUIElement("SpawnArea").GetComponent<RectTransform>();
        propriedadesArea = FindInactive.FindUIElement("PropriedadesArea").GetComponent<RectTransform>();
        metodosArea = FindInactive.FindUIElement("MetodosArea").GetComponent<RectTransform>();
        learnButton = FindInactive.FindUIElement("AprenderButton").GetComponent<Button>();
        criarButton = FindInactive.FindUIElement("CriarButton").GetComponent<Button>();

        criarButton.onClick.AddListener(CriarJson);
        criarButton.gameObject.SetActive(false);
        learnButton.gameObject.SetActive(false);
        minigameArea.gameObject.SetActive(false);
    }

    #region Shared

    public void VerifyComplete()
    {
        correctWords++;
        bool learnButtonVisible = correctWords == propriedades.Count + metodos.Count;
        learnButton.gameObject.SetActive(learnButtonVisible);
        if (learnButtonVisible){
            ClearArea(palavrasArea);
        }
    }

    public void ClearFields()
    {
        ClearArea(propriedadesArea);
        ClearArea(metodosArea);
        ClearArea(palavrasArea);
        minigameArea.gameObject.SetActive(false);
    }

    private void ClearArea(RectTransform area, string tag = null)
    {
        foreach (Transform child in area)
        {
            if (tag == null || child.CompareTag(tag))
            {
                Destroy(child.gameObject);
            }
        }
    }

    #endregion

    #region LearnRecipe Minigame

    public void StartNewGame(string titleText, SkillEnumerator itemID, List<string> palavras, List<Item> propriedades, List<Item> metodos, bool isInterface)
    {
        correctWords = 0;
        title.text = titleText;
        this.itemID = itemID;
        this.palavras = palavras;
        this.propriedades = propriedades;
        this.metodos = metodos;

        ClearFields();

        if (isInterface)
        {
            SpawnCompleted();
        }
        else
        {
            Spawn();
        }

        learnButton.gameObject.SetActive(isInterface);
        criarButton.gameObject.SetActive(false);
        minigameArea.gameObject.SetActive(true);
    }

    public void Spawn()
    {
        SpawnPalavras();
        SpawnCampos(propriedades, propriedadesArea);
        SpawnCampos(metodos, metodosArea);
    }

    private void SpawnCompleted()
    {
        List<GameObject> palavrasObjs = CreateWords();

        CreateFields(propriedades, palavrasObjs, propriedadesArea);
        CreateFields(metodos, palavrasObjs, metodosArea);

        ClearRemainingWords();
    }

    private List<GameObject> CreateWords()
    {
        List<GameObject> palavrasObjs = new List<GameObject>();

        foreach (string palavra in palavras)
        {
            GameObject wordObj = Instantiate(wordPrefab, minigameArea);
            wordObj.GetComponentInChildren<TextMeshProUGUI>().text = palavra;
            palavrasObjs.Add(wordObj);
            wordObj.SetActive(false); // Deixa invisível temporariamente
        }

        return palavrasObjs;
    }

    private void CreateFields(List<Item> items, List<GameObject> palavrasObjs, RectTransform area)
    {
        foreach (Item item in items)
        {
            GameObject fieldObj = Instantiate(fieldPrefab, area);
            var dropZone = fieldObj.GetComponent<DropZone>();

            var wordObj = palavrasObjs.Find(obj => obj.GetComponentInChildren<TextMeshProUGUI>().text == item.name);
            if (wordObj != null)
            {
                wordObj.SetActive(true);
                dropZone.SetCorrectWord(wordObj);
            }
        }
    }

    private void ClearRemainingWords()
    {
        foreach (Transform child in minigameArea)
        {
            if (child.CompareTag("MinigameWord"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void SpawnCampos(List<Item> palavras, RectTransform parentArea)
    {
        foreach (Item palavra in palavras)
        {
            GameObject fieldObj = Instantiate(fieldPrefab, parentArea);
            fieldObj.GetComponent<DropZone>().acceptedWord = palavra.name;
        }
    }

    public void SpawnPalavras()
    {
        foreach (string palavra in palavras)
        {
            GameObject wordObj = Instantiate(wordPrefab, palavrasArea);
            wordObj.GetComponentInChildren<TextMeshProUGUI>().text = palavra;

            RectTransform wordRect = wordObj.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(wordRect);

            float maxX = palavrasArea.rect.width / 2f - wordRect.rect.width / 2f - padding;
            float maxY = palavrasArea.rect.height / 2f - wordRect.rect.height / 2f - padding;

            float randomX = Random.Range(-maxX, maxX);
            float randomY = Random.Range(-maxY, maxY);

            wordRect.anchoredPosition = new Vector2(randomX, randomY);
        }
    }

    public void LearnSkill()
    {
        GameController.Instance.AddSkillLearned(itemID);
        SkillTreeController.Instance.ToggleSkillTree();
        InteractionText.instance.SetTextTimeout("Habilidade aprendida! Visite sua árvore novamente para criar um novo item.");
    }

    #endregion

    #region Craft Minigame

    public void CraftItem(string titleText, SkillEnumerator itemID, List<Item> propriedades, List<Item> metodos, Texture texture)
    {
        title.text = titleText;
        this.itemID = itemID;
        this.propriedades = propriedades;
        this.metodos = metodos;
        this.texture = texture;
        correctWords = 0;
        dropdowns.Clear();

        ClearFields();

        SpawnCraft(propriedades, propriedadesArea);
        SpawnCraft(metodos, metodosArea);

        learnButton.gameObject.SetActive(false);
        criarButton.gameObject.SetActive(false);
        minigameArea.gameObject.SetActive(true);
    }

    private void SpawnCraft(List<Item> items, RectTransform parentArea)
    {
        foreach (Item item in items)
        {
            if (item.options != null && item.options.Any())
            {
                dropdowns.Add(CreateDropdownComponentFor(item, parentArea));
            }
            else
            {
                dropdowns.Add(CreateLabelComponentFor(item, parentArea));
            }
        }

        int totalDropdowns = propriedades.Count(item => item.options != null && item.options.Any()) + metodos.Count(item => item.options != null && item.options.Any());

        criarButton.gameObject.SetActive(correctWords >= totalDropdowns);
    }

    private TMP_Dropdown CreateDropdownComponentFor(Item item, RectTransform parentArea)
    {
        GameObject fieldObj = Instantiate(dropdownPrefab, parentArea);
        if (!fieldObj.TryGetComponent<TMP_Dropdown>(out var dropdown))
        {
            Debug.LogError("Prefab de Dropdown não contém o componente TMP_Dropdown!");
            Destroy(fieldObj);
            return null;
        }

        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData(item.name));
        dropdown.options.AddRange(item.options.Select(option => new TMP_Dropdown.OptionData(option)));

        dropdown.value = 0;
        dropdown.RefreshShownValue();
        StylePlaceholder(dropdown);

        dropdown.onValueChanged.AddListener(index => HandleDropdownChange(dropdown, item));

        return dropdown;
    }

    private TextMeshProUGUI CreateLabelComponentFor(Item item, RectTransform parentArea)
    {
        GameObject fieldObj = Instantiate(wordPrefab, parentArea);

        var textComponent = fieldObj.GetComponentInChildren<TextMeshProUGUI>();
        var draggableComponent = fieldObj.GetComponentInChildren<DraggableText>();

        if (textComponent != null)
        {
            textComponent.text = item.name;
        }

        if (draggableComponent != null)
        {
            draggableComponent.enabled = false;
        }

        return textComponent;
    }

    private void StylePlaceholder(TMP_Dropdown dropdown)
    {
        if (dropdown.captionText != null)
        {
            dropdown.captionText.fontStyle = FontStyles.Italic;
            dropdown.captionText.color = new Color32(160, 160, 160, 255);
        }
    }

    private void HandleDropdownChange(TMP_Dropdown dropdown, Item item)
    {
        if (dropdown.options[0].text == item.name)
        {
            dropdown.options.RemoveAt(0);
            correctWords++;
        }

        dropdown.captionText.fontStyle = FontStyles.Normal;
        dropdown.captionText.color = Color.black;

        int totalDropdowns = propriedades.Count(item => item.options != null && item.options.Any()) + metodos.Count(item => item.options != null && item.options.Any());

        criarButton.gameObject.SetActive(correctWords >= totalDropdowns);
    }

    private void CriarJson()
    {
        if (GameController.Instance.GetInventory().Count > 9)
        {
            InteractionText.instance.SetTextTimeout("Inventário lotado, exclua um item para criar novos...");
        }
        else
        {
            ItemDatabase itemDatabase = new()
            {
                nome = title.text,
                skillID = itemID,
                icon = texture
            };

            int index = 0;

            // Propriedades
            AddPropertiesToDatabase(itemDatabase, ref index);

            // Métodos
            AddMethodsToDatabase(itemDatabase, ref index);

            string jsonStr = JsonUtility.ToJson(itemDatabase, true);
            Debug.Log(jsonStr);

            GameController.Instance.AddItemDatabase(itemDatabase);
            InteractionText.instance.SetTextTimeout("Item criado!");
        }
    }

    private void AddPropertiesToDatabase(ItemDatabase itemDatabase, ref int index)
    {
        foreach (var item in propriedades)
        {
            string selecionado = "";

            if (item.options.Count > 0)
            {
                var element = dropdowns[index++];

                if (element is TMP_Dropdown dropdown)
                {
                    selecionado = dropdown.captionText.text;
                }
            }

            itemDatabase.propriedades.Add(new()
            {
                chave = item.name,
                valor = selecionado
            });
        }
    }

    private void AddMethodsToDatabase(ItemDatabase itemDatabase, ref int index)
    {
        foreach (var item in metodos)
        {
            string selecionado = "";

            if (item.options.Count > 0)
            {
                var element = dropdowns[index++];

                if (element is TMP_Dropdown dropdown)
                {
                    selecionado = dropdown.captionText.text;
                }
            }

            itemDatabase.metodos.Add(new()
            {
                chave = item.name,
                valor = selecionado
            });
        }
    }

    #endregion
}