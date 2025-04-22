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

    public GameObject fieldPrefab;
    public GameObject wordPrefab;
    public GameObject dropdownPrefab;

    private RectTransform propriedadesArea;
    private RectTransform metodosArea;
    private RectTransform spawnArea;
    private Button learnButton;
    private Button criarButton;
    
    private List<Item> Propriedades;
    private List<Item> Metodos;
    private List<string> Palavras;  

    private Texture Texture;
    private SkillEnumerator ItemID;
    private int correctWords = 0;
    private TextMeshProUGUI Title;
    private float padding = 20f;

    private List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown>();

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

    private void Start()
    {
        Title = FindInactive.FindUIElement("Nome").GetComponent<TextMeshProUGUI>();
        spawnArea = FindInactive.FindUIElement("PointAndClickMinigame").GetComponent<RectTransform>();
        propriedadesArea = FindInactive.FindUIElement("PropriedadesArea").GetComponent<RectTransform>();
        metodosArea = FindInactive.FindUIElement("MetodosArea").GetComponent<RectTransform>();
        learnButton = FindInactive.FindUIElement("AprenderButton").GetComponent<Button>();
        criarButton = FindInactive.FindUIElement("CriarButton").GetComponent<Button>();

        criarButton.onClick.AddListener(CriarJson);

        criarButton.gameObject.SetActive(false);
        learnButton.gameObject.SetActive(false);
        spawnArea.gameObject.SetActive(false);
    }

    public void LearnSkill(){
        GameController.Instance.AddSkillLearned(ItemID);
        SkillTreeController.Instance.HandleSkillTree();
        InteractionText.instance.SetTextTimeout("Habilidade aprendida! Visite sua árvore novamente para criar um novo item.");
    }

    public void StartNewGame(string title, SkillEnumerator itemID, List<string> palavras, List<Item> propriedades, List<Item> metodos){
        correctWords = 0;
        Title.text = title;
        ItemID = itemID;
        Palavras = palavras;
        Propriedades = propriedades;
        Metodos = metodos;

        ClearFields();
        Spawn();

        learnButton.gameObject.SetActive(false);
        criarButton.gameObject.SetActive(false);
        spawnArea.gameObject.SetActive(true);
    }

    public void CraftItem(string title, SkillEnumerator itemID, List<Item> propriedades, List<Item> metodos, Texture texture){
        Title.text = title;
        ItemID = itemID;
        Propriedades = propriedades;
        Metodos = metodos;
        correctWords = 0;
        Texture = texture;
        dropdowns.Clear();

        ClearFields();  

        SpawnCraft(Propriedades, propriedadesArea);
        SpawnCraft(Metodos, metodosArea);

        learnButton.gameObject.SetActive(false);
        criarButton.gameObject.SetActive(false);
        spawnArea.gameObject.SetActive(true);
    }

    private void SpawnCraft(List<Item> palavras, RectTransform parentArea)
    {
        foreach (Item palavra in palavras)
        {
            GameObject fieldObj = Instantiate(dropdownPrefab, parentArea);

            if (!fieldObj.TryGetComponent<TMP_Dropdown>(out var dropdown)) continue;

            dropdown.options.Clear();

            // Adiciona a opção de placeholder (primeira opção)
            dropdown.options.Add(new TMP_Dropdown.OptionData(palavra.name));

            // Adiciona as opções reais
            foreach (var option in palavra.options)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }

            // Seleciona a opção de placeholder no início
            dropdown.value = 0;
            dropdown.RefreshShownValue();

            // Estiliza o placeholder (apenas visual)
            if (dropdown.captionText != null)
            {
                dropdown.captionText.fontStyle = FontStyles.Italic;
                dropdown.captionText.color = new Color32(160, 160, 160, 255); // cinzinha
            }

            // Listener para quando o usuário escolher algo
            dropdown.onValueChanged.AddListener(index =>
            {
                if(dropdown.options[0].text == palavra.name){
                    dropdown.options.RemoveAt(0);
                    correctWords++;
                }

                dropdown.captionText.fontStyle = FontStyles.Normal;
                dropdown.captionText.color = Color.black;
                
                criarButton.gameObject.SetActive(correctWords >= Propriedades.Count + Metodos.Count);
            });

            dropdowns.Add(dropdown);
        }
    }

    private void CriarJson()
    {
        ItemDatabase itemDatabase = new()
        {
            nome = Title.text,
            skillID = ItemID,
            icon = Texture
        };

        int index = 0;

        // Propriedades
        foreach (var item in Propriedades)
        {
            var dropdown = dropdowns[index++];
            string selecionado = dropdown.captionText.text;

            StringPair par = new()
            {
                chave = item.name,
                valor = selecionado
            };

            itemDatabase.propriedades.Add(par);
        }

        // Métodos
        foreach (var item in Metodos)
        {
            var dropdown = dropdowns[index++];
            string selecionado = dropdown.captionText.text;

            StringPair par = new()
            {
                chave = item.name,
                valor = selecionado
            };

            itemDatabase.metodos.Add(par);
        }

        string jsonStr = JsonUtility.ToJson(itemDatabase, true);
        Debug.Log(jsonStr);

        GameController.Instance.AddItemDatabase(itemDatabase);
        
        InteractionText.instance.SetTextTimeout("Item criado!");
    }

    public void VerifyComplete(){
        correctWords++;
        learnButton.gameObject.SetActive(correctWords == Propriedades.Count + Metodos.Count);
    }

    public void Spawn(){
        SpawnPalavras();
        SpawnCampos(Propriedades, propriedadesArea);
        SpawnCampos(Metodos, metodosArea);
    }

    public void ClearFields()
    {
        // Remove todos os filhos da área de propriedades
        foreach (Transform child in propriedadesArea)
        {
            Destroy(child.gameObject);
        }

        // Remove todos os filhos da área de métodos
        foreach (Transform child in metodosArea)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in spawnArea)
        {
            if (child.CompareTag("MinigameWord")){
                Destroy(child.gameObject);
            }
        }

        spawnArea.gameObject.SetActive(false);
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
        foreach (string palavra in Palavras)
        {
            GameObject wordObj = Instantiate(wordPrefab, spawnArea);
            wordObj.GetComponentInChildren<TextMeshProUGUI>().text = palavra;

            RectTransform wordRect = wordObj.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(wordRect); // atualiza tamanho

            // Calcula limites válidos dentro do painel
            float maxX = spawnArea.rect.width / 2f - wordRect.rect.width / 2f - padding;
            float maxY = spawnArea.rect.height / 2f - wordRect.rect.height / 2f - padding;

            float randomX = Random.Range(-maxX, maxX);
            float randomY = Random.Range(-maxY, maxY);

            wordRect.anchoredPosition = new Vector2(randomX, randomY);
        }
    }
}
