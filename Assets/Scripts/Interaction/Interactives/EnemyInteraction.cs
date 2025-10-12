using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Linq;   

namespace EJETAGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class EnemyInteraction : Interactable
    {
        public enum EstadoIA { Patrulhando, Perseguindo, Atacando, Morto, Parado }

        #region Variáveis de Interação e Vida
        [Header("Combate e Interação")]
        [SerializeField] private double vida = 4;
        [Tooltip("Itens que podem causar dano a este inimigo.")]
        public List<ItemDatabase> fraquezas;
        private InventoryController inventoryController;
        private Animator animator;
        #endregion

        #region Variáveis da IA
        [Header("Referências da IA")]
        private NavMeshAgent agente;
        private Transform jogador;
        [Tooltip("O ponto de onde o inimigo 'olha'. Geralmente um objeto na altura da cabeça.")]
        public Transform pontoDeVisao;

        [Header("Configurações de Patrulha")]
        public Transform[] pontosDePatrulha;
        private int indicePontoAtual = 0;

        [Header("Configurações de Detecção")]
        [Tooltip("A distância máxima que o inimigo pode ver.")]
        [Range(1f, 30f)]
        public float raioDeDeteccao = 12f;
        [Tooltip("A distância em que o inimigo para de perseguir e começa a atacar.")]
        [Range(1f, 10f)]
        public float raioDeAtaque = 2.5f;
        [Tooltip("O ângulo do campo de visão do inimigo (em graus).")]
        [Range(1f, 360f)]
        public float anguloDeVisao = 90f;
        [Tooltip("A altura acima do pivô do jogador que o inimigo tentará mirar (em metros).")]
        public float alturaDoAlvo = 1.0f;

        [Header("Configurações de Ataque da IA")]
        public int danoDoInimigo = 1;
        public float tempoEntreAtaques = 2f;
        private float tempoDesdeUltimoAtaque = 0f;
        #endregion

        #region Estado da IA
        [Header("Estado Atual")]
        [SerializeField]
        private EstadoIA estadoAtual;
        private bool jaMorreu = false;
        #endregion

        #region Variáveis de Movimento
        [Header("Configurações de Movimento")]
        [Tooltip("A velocidade com que o inimigo gira para encarar seu alvo.")]
        public float velocidadeDeRotacao = 10f;
        private float velocidadeOriginalDoAgente;
        #endregion


        public virtual void Start()
        {
            inventoryController = InventoryController.Instance;
            animator = GetComponent<Animator>();
            agente = GetComponent<NavMeshAgent>();

            GameObject jogadorGO = GameObject.FindGameObjectWithTag("Player");
            if (jogadorGO != null)
            {
                jogador = jogadorGO.transform;
            }
            else
            {
                Debug.LogError("Jogador não encontrado! Verifique se o jogador tem a tag 'Player'.");
            }

            agente.updateRotation = false;
            velocidadeOriginalDoAgente = agente.speed;

            MudarParaEstado(EstadoIA.Patrulhando);
        }

        void Update()
        {
            if (jaMorreu || jogador == null || MainCharacterController.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Dead")) return;

            switch (estadoAtual)
            {
                case EstadoIA.Patrulhando: ExecutarEstadoPatrulhando(); break;
                case EstadoIA.Perseguindo: ExecutarEstadoPerseguindo(); break;
                case EstadoIA.Atacando: ExecutarEstadoAtacando(); break;
                case EstadoIA.Morto: ExecutarEstadoMorto(); break;
                case EstadoIA.Parado: ExecutarEstadoParado(); break;
            }

            AtualizarRotacaoEAnimacao();
            tempoDesdeUltimoAtaque += Time.deltaTime;
        }

        private void AtualizarRotacaoEAnimacao()
        {
            if (estadoAtual == EstadoIA.Atacando || estadoAtual == EstadoIA.Morto || estadoAtual == EstadoIA.Parado)
            {
                agente.speed = 0;
                animator.SetBool("Walking", false);
                return;
            }

            if (agente.remainingDistance > agente.stoppingDistance)
            {
                Vector3 direcao = (agente.steeringTarget - transform.position).normalized;
                direcao.y = 0;

                if (direcao.sqrMagnitude > 0.1f)
                {
                    Quaternion rotacaoAlvo = Quaternion.LookRotation(direcao);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, Time.deltaTime * velocidadeDeRotacao);

                    float angulo = Vector3.Angle(transform.forward, direcao);

                    if (angulo < 15.0f)
                    {
                        agente.speed = velocidadeOriginalDoAgente;
                        animator.SetBool("Walking", true);
                    }
                    else
                    {
                        agente.speed = 0f;
                        animator.SetBool("Walking", false);
                    }
                }
            }
            else
            {
                agente.speed = 0f;
                animator.SetBool("Walking", false);
            }
        }

        private void MudarParaEstado(EstadoIA novoEstado)
        {
            if (estadoAtual == novoEstado) return;
            estadoAtual = novoEstado;

            switch (estadoAtual)
            {
                case EstadoIA.Patrulhando: agente.isStopped = false; IrParaProximoPontoDePatrulha(); break;
                case EstadoIA.Perseguindo: agente.isStopped = false; break;
                case EstadoIA.Atacando: agente.isStopped = true; break;
                case EstadoIA.Morto: agente.isStopped = true; break;
                case EstadoIA.Parado: agente.isStopped = true; break;
            }
        }

        private void ExecutarEstadoPatrulhando()
        {
            if (ConsegueVerOJogador())
            {
                MudarParaEstado(EstadoIA.Perseguindo);
                return;
            }
            if (!agente.pathPending && agente.remainingDistance < 0.5f)
            {
                IrParaProximoPontoDePatrulha();
            }
        }

        private void ExecutarEstadoPerseguindo()
        {
            agente.destination = jogador.position;
            float distancia = Vector3.Distance(transform.position, jogador.position);

            if (!ConsegueVerOJogador())
            {
                MudarParaEstado(EstadoIA.Patrulhando);
                return;
            }

            if (distancia <= raioDeAtaque) MudarParaEstado(EstadoIA.Atacando);
        }

        private void ExecutarEstadoAtacando()
        {
            agente.destination = transform.position;

            Vector3 posicaoAlvo = jogador.position + Vector3.up * alturaDoAlvo;
            Vector3 direcaoParaOlhar = (posicaoAlvo - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direcaoParaOlhar.x, 0, direcaoParaOlhar.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * velocidadeDeRotacao);

            if (tempoDesdeUltimoAtaque >= tempoEntreAtaques)
            {
                AtacarJogador();
                tempoDesdeUltimoAtaque = 0f;
            }

            if (Vector3.Distance(transform.position, jogador.position) > raioDeAtaque)
            {
                MudarParaEstado(EstadoIA.Perseguindo);
            }
        }

        private void ExecutarEstadoMorto()
        {
            if (jaMorreu) return;
            jaMorreu = true;

            animator.SetTrigger("Dying");

            if (TryGetComponent<Collider>(out var col))
            {
                col.enabled = false;
            }
            agente.enabled = false;
            Destroy(gameObject, 2.5f);
        }

        private void ExecutarEstadoParado()
        {
            agente.destination = transform.position;
        }

        private void IrParaProximoPontoDePatrulha()
        {
            if (pontosDePatrulha.Length == 0)
            {
                MudarParaEstado(EstadoIA.Parado);
                return;
            }
            agente.destination = pontosDePatrulha[indicePontoAtual].position;
            indicePontoAtual = (indicePontoAtual + 1) % pontosDePatrulha.Length;
        }

        private void AtacarJogador()
        {
            animator.SetTrigger("Atacando");

            if (!MainCharacterController.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking"))
            {
                GameController.Instance.UpdatePlayerLifes(danoDoInimigo);
            }
        }

        public override void Interact()
        {
            if (Input.GetKeyDown(interactionKey) && !MainCharacterController.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Armed_Attack"))
            {
                foreach (var itemFracoContra in fraquezas)
                {
                    if (inventoryController.VerifyItemSelected(itemFracoContra.skillID))
                    {
                        VerifyDamage();
                        break;
                    }
                }
            }
        }

        private void VerifyDamage()
        {
            var actualItem = EquipItemController.Instance.ReturnAcutalItem();

            if (actualItem.skillID == GameDatabase.SkillEnumerator.Crowbar)
            {
                MainCharacterController.Instance.animator.SetFloat("AttackSpeedMultiplier", 1.0f);
                MainCharacterController.Instance.animator.SetTrigger("Attacking");
                ReceberDano(1);
                return;
            }

            var baseDmg = actualItem.skillID == GameDatabase.SkillEnumerator.Sword ? 1.5 : 1.0;
            var actualDmg = baseDmg;
            var propriedadesDict = actualItem.propriedades.ToDictionary(pair => pair.chave, pair => pair.valor);

            if (propriedadesDict.TryGetValue("Tamanho", out string tamanhoValor))
            {
                if (tamanhoValor == "Médio") actualDmg += 0.5;
                else if (tamanhoValor == "Grande") actualDmg += 1.0;
            }

            if (propriedadesDict.TryGetValue("Peso", out string pesoValor))
            {
                if (pesoValor == "Leve") actualDmg += 0.5;
                else if (pesoValor == "Pesado") actualDmg += 1.0;
            }


            float minAttackSpeed = 0.5f; 
            float maxAttackSpeed = 1.5f; 

            float speedMultiplier = 1.0f;

            if (actualDmg > 0.01f)
            {
                speedMultiplier = (float)(baseDmg / actualDmg);
            }

            speedMultiplier = Mathf.Clamp(speedMultiplier, minAttackSpeed, maxAttackSpeed);

            MainCharacterController.Instance.animator.SetFloat("AttackSpeedMultiplier", speedMultiplier);
            MainCharacterController.Instance.animator.SetTrigger("Attacking");

            ReceberDano(actualDmg);
        }

        public void ReceberDano(double quantidade)
        {
            if (estadoAtual == EstadoIA.Morto) return;

            if (jogador != null)
            {
                Vector3 posicaoParaOlhar = new Vector3(jogador.position.x, transform.position.y, jogador.position.z);
                transform.LookAt(posicaoParaOlhar);
            }

            vida -= quantidade;
            if (vida <= 0)
            {
                vida = 0;
                MudarParaEstado(EstadoIA.Morto);
            }
        }

        private bool ConsegueVerOJogador()
        {
            if (jogador == null) return false;

            Vector3 pontoDeOrigem = (pontoDeVisao != null) ? pontoDeVisao.position : transform.position;
            Vector3 posicaoAlvo = jogador.position + Vector3.up * alturaDoAlvo;

            Vector3 direcaoParaJogador = (posicaoAlvo - pontoDeOrigem).normalized;
            float anguloParaJogador = Vector3.Angle(transform.forward, direcaoParaJogador);

            if (anguloParaJogador > anguloDeVisao / 2)
            {
                return false;
            }

            Debug.DrawRay(pontoDeOrigem, direcaoParaJogador * raioDeDeteccao, Color.magenta);

            if (Physics.Raycast(pontoDeOrigem, direcaoParaJogador, out RaycastHit hit, raioDeDeteccao, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 pontoDeOrigem = (pontoDeVisao != null) ? pontoDeVisao.position : transform.position;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, raioDeAtaque);

            Handles.color = new Color(1, 1, 0, 0.2f);
            Vector3 viewAngleA = DirFromAngle(-anguloDeVisao / 2, false);
            Vector3 viewAngleB = DirFromAngle(anguloDeVisao / 2, false);
            Handles.DrawSolidArc(pontoDeOrigem, Vector3.up, viewAngleA, anguloDeVisao, raioDeDeteccao);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pontoDeOrigem, pontoDeOrigem + viewAngleA * raioDeDeteccao);
            Gizmos.DrawLine(pontoDeOrigem, pontoDeOrigem + viewAngleB * raioDeDeteccao);
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
#endif
    }
}