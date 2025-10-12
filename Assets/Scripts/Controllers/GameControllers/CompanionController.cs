using UnityEngine;

public class CompanionController : MonoBehaviour
{
    private Animator animator;
    private Transform jogador;

    [Header("Configurações de Movimento")]
    [Tooltip("A velocidade máxima que o companheiro pode atingir.")]
    public float velocidadeMaxima = 5.0f;

    [Tooltip("Quão rápido o companheiro responde ao movimento. Valores menores = mais suave e 'pesado'. Valores maiores = mais rápido e responsivo.")]
    public float tempoDeSuavizacao = 0.25f;

    [Tooltip("A que distância do jogador o companheiro deve parar de se mover.")]
    public float distanciaParaParar = 2.5f;

    [Tooltip("Velocidade com que o companheiro gira para encarar o jogador.")]
    public float velocidadeDeRotacao = 5.0f;

    [Header("Posição Relativa")]
    [Tooltip("O deslocamento de posição em relação ao jogador (X=lados, Y=altura, Z=frente/trás).")]
    public Vector3 offset = new Vector3(1.5f, 1.0f, 0);

    private Vector3 velocidadeAtual = Vector3.zero;
    private float ladoDesejado = 1f; 

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject jogadorGO = GameObject.FindGameObjectWithTag("Player");

        if (jogadorGO != null)
        {
            jogador = jogadorGO.transform;
        }
        else
        {
            Debug.LogError("Jogador não encontrado! Verifique se o jogador tem a tag 'Player'.");
            this.enabled = false; 
        }
    }

    void LateUpdate()
    {
        if (jogador == null) return;

        float inputHorizontal = Input.GetAxis("Horizontal");

        if (Mathf.Abs(inputHorizontal) > 0.1f)
        {
            ladoDesejado = -Mathf.Sign(inputHorizontal);
        }

        Vector3 offsetDinamico = new Vector3(offset.x * ladoDesejado, offset.y, offset.z);

        Vector3 posicaoAlvo = jogador.position
                            + (jogador.right * offsetDinamico.x)
                            + (jogador.up * offsetDinamico.y)
                            + (jogador.forward * offsetDinamico.z);

        float distancia = Vector3.Distance(transform.position, posicaoAlvo);

        if (distancia > distanciaParaParar)
        {
            transform.position = Vector3.SmoothDamp(transform.position, posicaoAlvo, ref velocidadeAtual, tempoDeSuavizacao, velocidadeMaxima);
            animator?.SetBool("IsMoving", true);
        }
        else
        {
            velocidadeAtual = Vector3.zero;
            animator?.SetBool("IsMoving", false);
        }

        Vector3 direcaoParaOlhar = jogador.position - transform.position;
        direcaoParaOlhar.y = 0; 

        if (direcaoParaOlhar != Vector3.zero)
        {
            Quaternion rotacaoAlvo = Quaternion.LookRotation(direcaoParaOlhar);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, velocidadeDeRotacao * Time.deltaTime);
        }
    }
}