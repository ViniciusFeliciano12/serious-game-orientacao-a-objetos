<div align="center">

# Objetifica
### Serious Game para o Ensino de Orientação a Objetos

![Unity](https://img.shields.io/badge/Unity-2022.3.57f1%20LTS-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![URP](https://img.shields.io/badge/Universal%20Render%20Pipeline-blue?style=for-the-badge&logo=unity)
![License](https://img.shields.io/badge/Licença-Acadêmica-orange?style=for-the-badge)

> Jogo sério em ambiente 3D desenvolvido como Trabalho de Conclusão de Curso na **UTFPR**, com o objetivo de ensinar conceitos de **Programação Orientada a Objetos** de forma imersiva e interativa.

</div>

---

## Sobre o Projeto

**Objetifica** é um serious game de exploração em primeira pessoa com elementos de RPG, onde o jogador aprende conceitos fundamentais de Orientação a Objetos (OO) ao interagir com o mundo do jogo. Os mecanismos de gameplay — inventário, crafting, árvore de habilidades e combate — são projetados como metáforas diretas dos conceitos de OO: classes, propriedades, métodos, composição e instanciação.

O jogo foi desenvolvido como TCC do curso de **Ciência da Computação** da Universidade Tecnológica Federal do Paraná (UTFPR).

---

## Mecânicas de Aprendizagem

O jogo ensina OO em ciclos progressivos:

```
Descoberta → Análise → Fixação → Aplicação → Validação
```

| Etapa | Mecânica in-game | Conceito de OO |
|---|---|---|
| **Descoberta** | Encontrar itens/receitas pelo mundo | Identificar classes e objetos |
| **Análise** | Examinar propriedades e métodos do item | Atributos e métodos de uma classe |
| **Fixação** | Mini-game de drag-and-drop | Relacionar conceitos ao código |
| **Aplicação** | Combinar itens via crafting | Instanciação e composição de objetos |
| **Validação** | Usar itens em combate e puzzles | Testar o comportamento dos objetos |

---

## Gameplay

### Controles

| Tecla | Ação |
|---|---|
| `W A S D` | Movimentação |
| `Mouse` | Câmera |
| `Shift` | Correr |
| `Space` | Pular |
| `1–0` | Selecionar item do inventário |
| `Click Esquerdo` | Ação primária |
| `Click Direito` | Ação secundária / combinar |
| `I` | Abrir Árvore de Habilidades |
| `Esc` | Pausar |

### Sistema de Itens e Crafting

Cada item no jogo representa uma **classe** com propriedades e métodos. O jogador pode combinar itens compatíveis para criar novos objetos (composição):

- **Espada + Escudo** → `Set` (combinação de equipamentos)
- **Óleo + Pedregulho + Trapo** → `Ignitor` (objeto composto com novo comportamento)

### Árvore de Habilidades

Cada habilidade desbloqueada representa um conceito aprendido. Algumas habilidades possuem pré-requisitos (dependências), refletindo a progressão natural do aprendizado em OO.

### Sistema de Combate

Os inimigos possuem **fraquezas** a itens específicos. O dano é calculado com base nas propriedades dos objetos (Tamanho, Peso), reforçando a ideia de que atributos influenciam o comportamento dos objetos em runtime.

---

## Arquitetura do Projeto

```
Assets/
├── Scripts/
│   ├── Controllers/
│   │   ├── GameControllers/      # GameController (singleton), Personagem, Câmera, Pausa
│   │   ├── Inventory/            # Inventário, Equipamento, UI de itens
│   │   └── SkillTree/            # Árvore de habilidades, mini-game de aprendizagem
│   ├── HistoryControllers/       # Sistema de diálogos e narrativa
│   ├── Interaction/
│   │   ├── BaseInteraction/      # Classe base Interactable
│   │   └── Interactives/         # Inimigo, Porta, Tocha, Chefão (Minotauro)...
│   └── Utils/                    # SaveSystem (JSON), utilitários
├── Models/
│   └── ScriptableObject/         # GameDatabase, ItemDatabase, SkillDatabase, Player
├── Scenes/
│   ├── MenuScene.unity
│   ├── GameScene.unity
│   ├── HistoryScene.unity
│   └── CreditsScene.unity
└── Resources/                    # Prefabs, ícones e imagens carregados em runtime
```

### Padrões utilizados

- **Singleton** — `GameController` como ponto central de acesso aos sistemas
- **ScriptableObject** — Dados de jogo persistidos e desacoplados da cena
- **State Machine** — Inimigos com estados Patrol / Chase / Attack / Dead
- **Observer (implícito)** — Eventos de diálogo, desbloqueio de habilidades e puzzle de tochas

---

## Tecnologias e Dependências

| Tecnologia | Uso |
|---|---|
| **Unity 2022.3.57 LTS** | Engine principal |
| **C#** | Linguagem de scripting |
| **Universal Render Pipeline (URP)** | Pipeline gráfico |
| **FeelSpeak (StylishEsper)** | Sistema de diálogos |
| **TextMeshPro 3.0.7** | Renderização de textos na UI |
| **Cinemachine 3.1.2** | Sistema de câmeras |
| **Unity Input System 1.14.2** | Gerenciamento de inputs |
| **NavMesh (AI Navigation)** | Pathfinding dos inimigos |
| **Addressables 2.7.3** | Gerenciamento de assets |
| **SQLite** | Plugin de banco de dados |

---

## Como Executar

### Pré-requisitos

- [Unity Hub](https://unity.com/download)
- Unity **2022.3.57f1 LTS** (versão exata recomendada)

### Passos

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/serious-game-orientacao-a-objetos.git
   ```

2. Abra o **Unity Hub** e adicione o projeto clonado.

3. Certifique-se de usar a versão **2022.3.57f1 LTS** do Unity.

4. Abra a cena `Assets/Scenes/MenuScene.unity` e pressione **Play**.

> O save do jogo é armazenado em `%AppData%/save.json` (build) ou `Assets/EditorSaves/save.json` (editor).

---

## Informações Acadêmicas

| | |
|---|---|
| **Instituição** | Universidade Tecnológica Federal do Paraná — UTFPR |
| **Curso** | Engenharia de Software |
| **Trabalho** | Trabalho de Conclusão de Curso (TCC) |
| **Aluno** | Vinicius Otávio Feliciano |
| **Orientador** | Prof. Dr. Jorge Aikes Junior |

---

<div align="center">

Desenvolvido com dedicação como TCC na **UTFPR** · 2026

</div>
