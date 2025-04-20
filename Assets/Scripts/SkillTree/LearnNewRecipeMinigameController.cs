using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using static GameDatabase;

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
    
    private List<Item> Propriedades;
    private List<Item> Metodos;
    private List<string> Palavras;  

    private SkillEnumerator ItemID;
    private int correctWords = 0;
    private TextMeshProUGUI Title;
    private float padding = 20f;

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

        learnButton.gameObject.SetActive(false);
        spawnArea.gameObject.SetActive(false);
    }

    public void LearnSkill(){
        GameController.Instance.AddItemLearned(ItemID);
        SkillTreeController.Instance.HandleSkillTree();
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
        spawnArea.gameObject.SetActive(true);
    }

    public void CraftItem(string title, SkillEnumerator itemID, List<Item> propriedades, List<Item> metodos){
        Title.text = title;
        ItemID = itemID;
        Propriedades = propriedades;
        Metodos = metodos;
        correctWords = 0;
        ClearFields();  

        SpawnCraft(Propriedades, propriedadesArea);
        SpawnCraft(Metodos, metodosArea);

        learnButton.gameObject.SetActive(false);
        spawnArea.gameObject.SetActive(true);
    }

    private void SpawnCraft(List<Item> palavras, RectTransform parentArea)
    {
        foreach (Item palavra in palavras)
        {
            // Instancia o prefab
            GameObject fieldObj = Instantiate(dropdownPrefab, parentArea);

            // Obtém o componente TMP_Dropdown
            TMP_Dropdown dropdown = fieldObj.GetComponent<TMP_Dropdown>();

            if (dropdown == null)
            {
                Debug.LogError("Prefab não possui um componente TMP_Dropdown!");
                continue;
            }

            // Limpa opções anteriores
            dropdown.options.Clear();

            // Adiciona novas opções
            foreach (var option in palavra.options)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }

            // Atualiza o label (texto visível)
            dropdown.captionText.text = palavra.name;
        }
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
