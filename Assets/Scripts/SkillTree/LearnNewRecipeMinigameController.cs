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

    private RectTransform propriedadesArea;
    private RectTransform metodosArea;
    private RectTransform spawnArea;
    private Button learnButton;
    
    private List<string> Propriedades;
    private List<string> Metodos;
    private List<string> Palavras;  

    private ItemEnumerator ItemID;
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
        Title = GameObject.Find("Nome").GetComponent<TextMeshProUGUI>();
        spawnArea = GameObject.Find("PointAndClickMinigame").GetComponent<RectTransform>();
        propriedadesArea = GameObject.Find("PropriedadesArea").GetComponent<RectTransform>();
        metodosArea = GameObject.Find("MetodosArea").GetComponent<RectTransform>();
        learnButton = GameObject.Find("AprenderButton").GetComponent<Button>();

        learnButton.gameObject.SetActive(false);
        spawnArea.gameObject.SetActive(false);
    }

    public void LearnSkill(){
        GameController.Instance.AddItemLearned(ItemID);
        SkillTreeController.Instance.HandleSkillTree();
    }

    public void StartNewGame(string title, ItemEnumerator itemID, List<string> palavras, List<string> propriedades, List<string> metodos){
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

    public void SpawnCampos(List<string> palavras, RectTransform parentArea)
    {
        foreach (string palavra in palavras)
        {
            GameObject fieldObj = Instantiate(fieldPrefab, parentArea);
            fieldObj.GetComponent<DropZone>().acceptedWord = palavra;
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
