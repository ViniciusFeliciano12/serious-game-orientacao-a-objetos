using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button ContinueButton;
    public GameDatabase Database;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        // Apenas verifica se o arquivo de save existe para habilitar o botão.
        // Não vamos carregar os dados aqui para manter o GameDatabase "limpo".
        if (ContinueButton != null)
        {
            ContinueButton.interactable = SaveSystem.SaveExists();
        }
    }

    public void ContinueGame()
    {
        if (SaveSystem.Load(Database))
        {
            SceneManager.LoadScene(2); 
        }
        else
        {
            Debug.LogError("Falha ao carregar o save. O arquivo pode estar corrompido ou foi deletado.");
        }
    }

    public void GoToPhase1()
    {
        Database.ResetDatabase();
        SaveSystem.Save(Database);
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}