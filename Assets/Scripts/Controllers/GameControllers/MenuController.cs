using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button ContinueButton;
    public GameDatabase Database;

    void Start()
    {
        if (ContinueButton != null)
        {
            // tenta carregar o save logo no in�cio
            bool saveOk = SaveSystem.Load(Database);

            ContinueButton.interactable = saveOk;
        }
    }

    public void ContinueGame()
    {
        // se quiser, j� pode salvar antes de trocar de cena
        SaveSystem.Save(Database);
        SceneManager.LoadScene(2);
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
