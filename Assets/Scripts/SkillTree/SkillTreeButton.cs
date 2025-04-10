using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTreeButton : MonoBehaviour
{
    public List<string> propriedades;
    public List<string> metodos;
    public List<string> palavras;
    public GameObject PointAndClickCanvas;

    private void Start()
    {
        
    }

    public void Clicked(){
        SkillTreeMinigameSpawner.Instance.ClearFields();

        SkillTreeMinigameSpawner.Instance.palavras = palavras;
        SkillTreeMinigameSpawner.Instance.propriedades = propriedades;
        SkillTreeMinigameSpawner.Instance.metodos = metodos;

        SkillTreeMinigameSpawner.Instance.Spawn();

        PointAndClickCanvas.SetActive(true);
    }
}
