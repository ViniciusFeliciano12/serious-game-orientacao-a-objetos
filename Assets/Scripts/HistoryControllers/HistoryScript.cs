using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HistoryScript : MonoBehaviour
{
    private TextMeshProUGUI HistoryText;
    private GameObject ContinueButton;
    private RawImage BackgroundImage;

    public GameObject ControllersPanel;
    public GameObject HistoryPanel;

    public float typingSpeed = 0.000000007f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        if (ControllersPanel != null)
        {
            ControllersPanel.SetActive(true);
        }

        if (HistoryPanel != null)
        {
            ContinueButton = HistoryPanel.transform.Find("Iniciar Jogo").gameObject;
            HistoryText = HistoryPanel.transform.Find("TextHistory").GetComponent<TextMeshProUGUI>();
            BackgroundImage = HistoryPanel.transform.Find("BackgroundImage").GetComponent<RawImage>();
            ContinueButton.SetActive(false);
            HistoryPanel.SetActive(false);
        }
    }

    void Update()
    {

    }

    public void StartHistory()
    {
        ControllersPanel.SetActive(false);
        HistoryPanel.SetActive(true);
        _ = ShowTextAsync();
    }

    async Task ShowTextAsync()
    {
        await MostrarHistoria("Escutem, pois vos conto uma história esquecida pelos homens e observada pelos céus.\r\n\r\nHá eras incontáveis, " +
        "quando o mundo ainda era jovem, um reino se ergueu sobre as ruínas de um antigo conflito. Seu rei, herdeiro de linhagens enfraquecidas, " +
        "temeu a sombra da ruína que se aproximava.\r\n\r\nEm desespero, ele fez uma promessa a um poder sombrio. Ofereceu o que tinha de mais " +
        "precioso: a felicidade de sua família e a confiança de seu povo. Assim, selou um pacto com o Abismo. E em resposta, não recebeu ouro nem glória, " +
        "mas um filhote de besta: um Minotauro.\r\n\r\nA criatura cresceu, movida por uma fúria incontrolável e um poder avassalador. Sob seu comando, o rei venceu " +
        "desafios impossíveis e expandiu seu domínio. O reino prosperou… mas seu líder se tornou um tirano.", "BackgroundImages/minotaur");

        await Task.Delay(1000);

        HistoryText.text = "";

        await MostrarHistoria("Contudo, o poder sombrio sempre cobra seu preço. Entre os soldados havia um homem de bom coração, " +
        "chamado John. Ele via o medo nos olhos do povo e sabia que precisava agir.\r\n\r\nOs céus, vendo a pureza em seu coração, " +
        "o escolheram. Uma luz divina o tocou, concedendo-lhe a força necessária para sua missão: proteger os inocentes e parar o Minotauro. " +
        "John Doe tentou alertar os outros, mas sua nova convicção foi vista como uma ameaça, e ele foi capturado e jogado na prisão junto ao monstro.\r\n\r\n" +
        "Pois é através dos corajosos que os céus agem contra a escuridão. E assim começou a história que agora vos narro...", "BackgroundImages/sitting");

        ContinueButton.SetActive(true);
    }

    public async Task MostrarHistoria(string historia, string image)
    {
        BackgroundImage.texture = Resources.Load<Texture>(image);

        foreach (char caracter in historia)
        {
            HistoryText.text += caracter;

            await Task.Delay((int)(typingSpeed * 500));
        }
    }

    public void GoToPhase2()
    {
        SceneManager.LoadScene(2);
    }
}
