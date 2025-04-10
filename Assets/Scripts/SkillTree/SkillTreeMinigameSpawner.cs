using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillTreeMinigameSpawner : MonoBehaviour
{
    public static SkillTreeMinigameSpawner Instance { get; private set; }

    public GameObject fieldPrefab;
    public RectTransform propriedadesArea;
    public RectTransform metodosArea;

    public List<string> propriedades;
    public List<string> metodos;

    public GameObject wordPrefab;
    public RectTransform spawnArea;
    public List<string> palavras;

    public float padding = 20f; // margem mínima nas bordas

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
        
    }

    public void Spawn(){
        SpawnPalavras();
        SpawnCampos(propriedades, propriedadesArea);
        SpawnCampos(metodos, metodosArea);
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
        foreach (string palavra in palavras)
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
