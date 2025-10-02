using static GameDatabase;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

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
    private Button alterarButton;

    // Dados do jogo
    private List<Item> propriedades;
    private List<Item> metodos;
    private List<string> palavras;
    private string texture;
    private SkillEnumerator itemID;
    private int correctWords = 0;
    private TextMeshProUGUI title;
    private ItemDatabase currentItemToEdit; // NOVO: Armazena o item que está sendo editado
    private int indexToEdit = -1;

    // Estado
    private readonly List<object> dropdowns = new();

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
        alterarButton = FindInactive.FindUIElement("AlterarButton").GetComponent<Button>();
        criarButton = FindInactive.FindUIElement("CriarButton").GetComponent<Button>();

        criarButton.gameObject.SetActive(false);
        learnButton.gameObject.SetActive(false);
        alterarButton.gameObject.SetActive(false);
        minigameArea.gameObject.SetActive(false);
    }

    #region Shared

    public void VerifyComplete()
    {
        correctWords++;
        bool learnButtonVisible = correctWords == propriedades.Count + metodos.Count;
        learnButton.gameObject.SetActive(learnButtonVisible);
        if (learnButtonVisible)
        {
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

        bool gameIsAlreadyWon = correctWords == propriedades.Count + metodos.Count;
        learnButton.gameObject.SetActive(isInterface || gameIsAlreadyWon);
        if (gameIsAlreadyWon)
        {
            ClearArea(palavrasArea);
        }

        criarButton.gameObject.SetActive(false);
        minigameArea.gameObject.SetActive(true);
    }

    public void Spawn()
    {
        SpawnPalavras();

        var allItems = propriedades.Concat(metodos).ToList();
        var wordObjects = CreateWordObjects(allItems);

        SpawnCampos(propriedades, propriedadesArea, wordObjects);
        SpawnCampos(metodos, metodosArea, wordObjects);
    }

    private void SpawnCompleted()
    {
        List<GameObject> palavrasObjs = CreateWords();

        CreateFields(propriedades, palavrasObjs, propriedadesArea);
        CreateFields(metodos, palavrasObjs, metodosArea);

        ClearRemainingWords();
    }

    private Dictionary<string, GameObject> CreateWordObjects(List<Item> items)
    {
        var wordDict = new Dictionary<string, GameObject>();
        foreach (var item in items)
        {
            if (item.cameFromInterface && !wordDict.ContainsKey(item.name))
            {
                GameObject wordObj = Instantiate(wordPrefab, minigameArea);
                wordObj.GetComponentInChildren<TextMeshProUGUI>().text = item.name;
                wordObj.SetActive(false);
                wordDict[item.name] = wordObj;
            }
        }
        return wordDict;
    }

    private List<GameObject> CreateWords()
    {
        List<GameObject> palavrasObjs = new();

        foreach (string palavra in palavras)
        {
            GameObject wordObj = Instantiate(wordPrefab, minigameArea);
            wordObj.GetComponentInChildren<TextMeshProUGUI>().text = palavra;
            palavrasObjs.Add(wordObj);
            wordObj.SetActive(false);
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

    public void SpawnCampos(List<Item> items, RectTransform parentArea, Dictionary<string, GameObject> wordObjects)
    {
        foreach (Item item in items)
        {
            GameObject fieldObj = Instantiate(fieldPrefab, parentArea);
            var dropZone = fieldObj.GetComponent<DropZone>();

            if (item.cameFromInterface)
            {
                if (wordObjects.TryGetValue(item.name, out GameObject wordObj))
                {
                    wordObj.SetActive(true);
                    dropZone.SetCorrectWord(wordObj);
                }
                else
                {
                    Debug.LogWarning($"Não foi encontrado o GameObject da palavra '{item.name}' para o campo pré-preenchido.");
                    dropZone.acceptedWord = item.name;
                }
            }
            else
            {
                dropZone.acceptedWord = item.name;
            }
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
        UIController.Instance.SetTextTimeout("Habilidade aprendida! Visite sua árvore novamente para criar um novo item.");
    }

    #endregion

    #region Craft Minigame

    public void CraftItem(string titleText, SkillEnumerator itemID, List<Item> propriedades, List<Item> metodos, string texture)
    {
        // MODIFICADO: Limpa o item de edição ao criar um novo
        currentItemToEdit = null;

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
        minigameArea.gameObject.SetActive(true);
        alterarButton.gameObject.SetActive(false);
    }

    public void CraftItem(string titleText, SkillEnumerator itemID, List<Item> propriedades, List<Item> metodos, string texture, ItemDatabase item, int index)
    {
        currentItemToEdit = item;
        indexToEdit = index;

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
        alterarButton.gameObject.SetActive(true);
    }

    private void SpawnCraft(List<Item> items, RectTransform parentArea)
    {
        var initialValues = new Dictionary<string, string>();
        if (currentItemToEdit != null)
        {
            foreach (var prop in currentItemToEdit.propriedades)
            {
                if (!string.IsNullOrEmpty(prop.valor)) initialValues[prop.chave] = prop.valor;
            }
            foreach (var met in currentItemToEdit.metodos)
            {
                if (!string.IsNullOrEmpty(met.valor)) initialValues[met.chave] = met.valor;
            }
        }

        foreach (Item item in items)
        {
            if (item.options != null && item.options.Any())
            {
                initialValues.TryGetValue(item.name, out string initialValue);
                dropdowns.Add(CreateDropdownComponentFor(item, parentArea, initialValue));
            }
            else
            {
                dropdowns.Add(CreateLabelComponentFor(item, parentArea));
            }
        }

        int totalDropdowns = propriedades.Count(item => item.options != null && item.options.Any()) + metodos.Count(item => item.options != null && item.options.Any());
        criarButton.gameObject.SetActive(correctWords >= totalDropdowns);
    }

    private TMP_Dropdown CreateDropdownComponentFor(Item item, RectTransform parentArea, string initialValue)
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

        if (!string.IsNullOrEmpty(initialValue))
        {
            int selectedIndex = dropdown.options.FindIndex(opt => opt.text == initialValue);
            if (selectedIndex > 0) 
            {
                dropdown.value = selectedIndex;
                SetDropdownAsCorrect(dropdown, item, false); 
            }
            else
            {
                dropdown.value = 0;
                StylePlaceholder(dropdown);
            }
        }
        else
        {
            dropdown.value = 0;
            StylePlaceholder(dropdown);
        }

        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(index => HandleDropdownChange(dropdown, item));

        return dropdown;
    }

    private TextMeshProUGUI CreateLabelComponentFor(Item item, RectTransform parentArea)
    {
        GameObject fieldObj = Instantiate(wordPrefab, parentArea);

        var textComponent = fieldObj.GetComponentInChildren<TextMeshProUGUI>();
        var draggableComponent = fieldObj.GetComponentInChildren<DraggableText>();

        if (textComponent != null) textComponent.text = item.name;
        if (draggableComponent != null) draggableComponent.enabled = false;

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
        SetDropdownAsCorrect(dropdown, item, true);
    }

    // NOVO: Método centralizado para tratar um dropdown como "correto"
    private void SetDropdownAsCorrect(TMP_Dropdown dropdown, Item item, bool removePlaceholder)
    {
        if (removePlaceholder && dropdown.options[0].text == item.name)
        {
            dropdown.options.RemoveAt(0);

            dropdown.value--;
            dropdown.RefreshShownValue();
            correctWords++;
        }
        else if (!removePlaceholder)
        {
            correctWords++;
        }

        dropdown.captionText.fontStyle = FontStyles.Normal;
        dropdown.captionText.color = Color.black;

        int totalDropdowns = propriedades.Count(item => item.options != null && item.options.Any()) + metodos.Count(item => item.options != null && item.options.Any());
        criarButton.gameObject.SetActive(correctWords >= totalDropdowns);
    }

    public void CriarJson()
    {
        if (GameController.Instance.GetInventory().Count > 9 && currentItemToEdit != null)
        {
            UIController.Instance.SetTextTimeout("Inventário lotado, exclua um item para criar novos...");
        }
        else
        {
            ItemDatabase itemDatabase = new()
            {
                nome = title.text,
                skillID = itemID,
                iconPath = "Icon/" + texture
            };

            int index = 0;
            AddPropertiesToDatabase(itemDatabase, ref index);
            AddMethodsToDatabase(itemDatabase, ref index);

            string jsonStr = JsonUtility.ToJson(itemDatabase, true);
            Debug.Log(jsonStr);

            if (currentItemToEdit == null)
            {
                GameController.Instance.AddItemDatabase(itemDatabase);
                UIController.Instance.SetTextTimeout("Item criado!");
            }
            else
            {
                UIController.Instance.SetTextTimeout("Objeto alterado com sucesso!");
                GameController.Instance.EditItemDatabase(itemDatabase, indexToEdit);
            }

            SkillTreeController.Instance.ToggleSkillTree();
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
            itemDatabase.propriedades.Add(new() { chave = item.name, valor = selecionado });
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
            itemDatabase.metodos.Add(new() { chave = item.name, valor = selecionado });
        }
    }

    #endregion
}