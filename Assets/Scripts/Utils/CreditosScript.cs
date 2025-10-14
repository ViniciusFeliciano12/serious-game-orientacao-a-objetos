using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditosScript : MonoBehaviour
{
    public float velocidade = 70f;

    public float limiteY = 3000f;

    public string nomeDaCenaDoMenu = "MainMenu";

    private RectTransform rectTransform;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 1f;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        
        rectTransform.Translate(Vector3.up * velocidade * Time.deltaTime);

        if (rectTransform.anchoredPosition.y > limiteY)
        {
            Debug.Log("Créditos finalizados. Voltando para o menu.");
            SceneManager.LoadScene(nomeDaCenaDoMenu);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(nomeDaCenaDoMenu);
        }
    }
}
