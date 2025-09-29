using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HistoryScript : MonoBehaviour
{
    public TextMeshProUGUI HistoryText;
    public GameObject ContinueButton;

    public float typingSpeed = 0.000000007f;

    async void Start()
    {
        if (ContinueButton != null)
        {
            ContinueButton.SetActive(false);
        }

        if (HistoryText != null)
        {
            await ShowTextAsync();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    async Task ShowTextAsync()
    {
        string historia = "Escutem, filhos da noite e do dia, pois vos conto uma história esquecida pelos homens e temida pelos deuses.\r\n\r\nHá eras incontáveis, quando deuses e demônios caminhavam entre nós, um reino se ergueu sobre cinzas e restos. Seu rei, herdeiro de linhagens decadentes, temeu a ruína que se aproximava como um lobo faminto.\r\n\r\nNo desespero, ofereceu o sangue mais puro de sua casa: a esposa que lhe dera amor e o filho que lhe daria futuro. Assim, selou um pacto sombrio com o Abismo. E em resposta, não recebeu ouro nem glória, mas um filhote de besta: um Minotauro.\r\n\r\nA criatura cresceu, moldada na sede por carne humana e destruição. Sob sua fúria, o rei venceu batalhas impossíveis e tomou coroas que não lhe pertenciam. Prosperou… mas não como homem, e sim como tirano.";

        HistoryText.text = "";

        await MostrarHistoria(historia);

        await Task.Delay(1000);

        HistoryText.text = "";

        string historia2 = "Contudo, até o poder mais negro se volta contra quem o empunha. Entre seus soldados havia um homem simples, chamado John Doe. Os deuses encontraram-no a oportunidade perfeita de aplicar a punição contra o rei profano, utilizando seu soldado para isso.\r\n\r\nComo uma centelha divina, a primeira missão foi data; destruir o Minotauro para acabar com seu reinado. John Doe, sob instrução divina, iniciou uma revolta e não tardou para ser capturado e jogado na prisão junto com o monstro.\r\n\r\nPois é através dos pequenos e rejeitados pela justiça humana que os céus se erguem contra os ímpios. E assim começou a história que agora vos narro...";
        await MostrarHistoria(historia2);

        ContinueButton.SetActive(true);
    }

    public async Task MostrarHistoria(string historia)
    {
        foreach (char caracter in historia)
        {
            HistoryText.text += caracter;

            // Pausa a execução. Task.Delay espera um valor em milissegundos.
            await Task.Delay((int)(typingSpeed * 500));
        }
    }

    public void GoToPhase2()
    {
        SceneManager.LoadScene(2);
    }
}
