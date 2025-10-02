using UnityEngine;

public class CompanionController : MonoBehaviour
{
    private Animator animator;
    private Transform jogador;

    [Header("Configurações de Movimento")]
    [Tooltip("A que velocidade o fantasma se move.")]
    public float velocidade = 3.0f;

    [Tooltip("A que distância do jogador o fantasma deve parar.")]
    public float distanciaParaParar = 2.5f;

    [Tooltip("Velocidade com que o fantasma gira para encarar o jogador.")]
    public float velocidadeDeRotacao = 5.0f;

    [Header("Posição Relativa")]
    [Tooltip("Um deslocamento de posição em relação ao jogador (ex: para flutuar ao lado ou acima).")]
    public Vector3 offset = new Vector3(1.5f, 1.0f, 0);

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
        }
    }

    void LateUpdate() 
    {
        if (jogador == null) return; 

       
        Vector3 posicaoAlvo = jogador.position + offset;

      
        float distancia = Vector3.Distance(transform.position, posicaoAlvo);

        if (distancia > distanciaParaParar)
        {
          
            transform.position = Vector3.MoveTowards(transform.position, posicaoAlvo, velocidade * Time.deltaTime);
            animator?.SetBool("IsMoving", true); 
        }
        else
        {
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