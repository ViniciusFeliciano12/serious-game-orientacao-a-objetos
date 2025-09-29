using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameDatabase;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    public ItemInventoryController[] Inventory;
    private int indexSelected = -1;

    private Dictionary<SkillEnumerator, System.Action> itemUseActions;
    private Dictionary<SkillEnumerator, System.Action> itemUseActionRightButton;

    private ItemDatabase swordSaved;
    private ItemDatabase shieldSaved;

    private bool usingGravel = false;
    private bool usingSword = false;
    private bool usingShield = false;
    private bool usingGravelRightButton = false;
    private bool usingRagRightButton = false;
    private bool usingOilRightButton = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        var menu = GameObject.Find("Bottom itens");
        Inventory = menu.GetComponentsInChildren<ItemInventoryController>();

        InitializeItemActions();

        UpdateInventory();
    }

    private void InitializeItemActions()
    {
        itemUseActions = new Dictionary<SkillEnumerator, System.Action>
        {
             { SkillEnumerator.Torch, UseTorch },
             { SkillEnumerator.Shield, UseShield },
             { SkillEnumerator.Gravel, UseGravel },
             { SkillEnumerator.Crowbar, UseCrowbar },
        };

        itemUseActionRightButton = new Dictionary<SkillEnumerator, System.Action>
        {
             { SkillEnumerator.Set, UseShield },
             { SkillEnumerator.Rag, UseRag },
             { SkillEnumerator.Oil, UseOil },
             { SkillEnumerator.Sword, UseSwordRightButton },
             { SkillEnumerator.Shield, UseShieldRightButton },
             { SkillEnumerator.Gravel, UseGravelRightButton },
        };
    }

    void Update()
    {
        SelectItemInventory();

        TryUseSelectedActiveItem();

        if (Input.GetKeyDown(KeyCode.R) && Inventory[indexSelected].ReturnActualItem() != null && Inventory[indexSelected].ReturnActualItem().icon != null)
        {
            FindInactive.Find("ConfirmErase").SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.E) && Inventory[indexSelected].ReturnActualItem() is ItemDatabase item && item.icon != null && GameController.Instance.VerifyItemFound(SkillEnumerator.IsReusable))
        {
            var gameObj = FindInactive.FindUIElement("SkillTreePanel");

            var buttons = gameObj.GetComponentsInChildren<SkillTreeButton>(true);

            foreach (var button in buttons)
            {
                if (button.title == item.nome)
                {
                    button.StartCraftItemMinigameReusable(item, indexSelected);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryUseSelectedItem();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            TryUseSelectedItemRightButton();
        }

        CreateItens();
    }

    private void CreateItens()
    {
        if (usingShield && usingSword)
        {
            usingShield = false;
            usingSword = false;
            CreateSet();
        }

        if (usingOilRightButton && usingGravelRightButton && usingRagRightButton)
        {
            usingOilRightButton = false;
            usingGravelRightButton = false;
            usingRagRightButton = false;
            CreateIgnitor();
        }
    }

    private void CreateIgnitor()
    {
        Texture2D iconeDaTextura = Resources.Load<Texture2D>("Icon/fogo");

        ItemDatabase itemDatabase = new()
        {
            nome = "Ignitor",
            skillID = SkillEnumerator.Ignitor,
            icon = iconeDaTextura,
        };

        itemDatabase.metodos.Add(new StringPair() { chave = "Acender tocha" });

        string jsonStr = JsonUtility.ToJson(itemDatabase, true);
        Debug.Log(jsonStr);

        GameController.Instance.AddItemDatabase(itemDatabase);
        UIController.Instance.SetTextTimeout("Ignitor criado!");
    }

    private void CreateSet()
    {
        Texture2D iconeDaTextura = Resources.Load<Texture2D>("Icon/espada-escudo");

        ItemDatabase itemDatabase = new()
        {
            nome = "Conjunto",
            skillID = SkillEnumerator.Set,
            icon = iconeDaTextura,
            propriedades = swordSaved.propriedades,
            metodos = swordSaved.metodos
        };

        itemDatabase.metodos.AddRange(shieldSaved.metodos);

        string jsonStr = JsonUtility.ToJson(itemDatabase, true);
        Debug.Log(jsonStr);

        GameController.Instance.AddItemDatabase(itemDatabase);
        UIController.Instance.SetTextTimeout("Conjunto criado!");
    }

    private void TryUseSelectedActiveItem()
    {
        if (indexSelected != -1)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                Inventory[i].VerifyItemActive(i == indexSelected);
            }
        }
    }

    public bool VerifyItemSelected(SkillEnumerator skillID, List<StringPair> propriedades = null, List<StringPair> metodos = null)
    {
        if (indexSelected != -1)
        {
            var actualItem = Inventory[indexSelected].ReturnActualItem();
            if (actualItem != null)
            {
                bool idMatches = actualItem.skillID == skillID;
                bool propriedadesMatch = propriedades == null || List1IsContainedInList2(propriedades, actualItem.propriedades);
                bool metodosMatch = metodos == null || List1IsContainedInList2(metodos, actualItem.metodos);
                if (idMatches && propriedadesMatch && metodosMatch)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool List1IsContainedInList2(List<StringPair> list1, List<StringPair> list2)
    {
        return !list1.Except(list2, new StringPairComparer()).Any();
    }

    public void RemoveItemInventory()
    {
        if (indexSelected != -1)
        {
            var item = Inventory[indexSelected].ReturnActualItem();
            if (item != null)
            {
                GameController.Instance.RemoveItemDatabase(item);
                Inventory[indexSelected].ResetDefaults();
            }

            CloseConfirmRemoveItem();
        }
    }

    public void CloseConfirmRemoveItem()
    {
        FindInactive.Find("ConfirmErase").SetActive(false);
    }

    private void TryUseSelectedItem()
    {
        if (indexSelected != -1)
        {
            var item = Inventory[indexSelected].ReturnActualItem();
            if (item != null)
            {
                if (itemUseActions.TryGetValue(item.skillID, out System.Action useAction))
                {
                    useAction.Invoke();
                }
            }
        }
    }

    private void TryUseSelectedItemRightButton()
    {
        if (indexSelected != -1)
        {
            var item = Inventory[indexSelected].ReturnActualItem();
            if (item != null)
            {
                if (itemUseActionRightButton.TryGetValue(item.skillID, out System.Action useAction))
                {
                    useAction.Invoke();
                }
            }
        }
    }

    public void UpdateInventory()
    {
        var inventoryDatabase = GameController.Instance.GetInventory();
        for (int i = 0; i < inventoryDatabase.Count; i++)
        {
            Inventory[i].InstantiateItem(inventoryDatabase[i]);
        }
    }

    private void SelectItemInventory()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                SelectItemAt(i - 1);
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            SelectItemAt(9);
        }
    }

    public void SelectItemAt(int index)
    {
        if (index >= Inventory.Length) return;

        indexSelected = index;
        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i].ItemSelected(i == index);
        }
    }

    #region Item Use Functions

    private void UseGravel()
    {
        if (!usingGravel)
        {
            usingGravel = true;
            RemoveItemInventory();
            UIController.Instance.SetTextTimeout("Usando pederneira... selecione uma tocha para acender");
        }
        else
        {
            UIController.Instance.SetTextTimeout("Já está utilizando pederneira, acenda uma tocha primeiro");
        }
    }

    private void UseTorch()
    {
        if (!Inventory[indexSelected].ReturnActualItem().itemActive)
        {
            if (usingGravel)
            {
                usingGravel = false;
                Inventory[indexSelected].ReturnActualItem().itemActive = true;
                UIController.Instance.SetTextTimeout("Tocha acesa");
            }
            else
            {
                UIController.Instance.SetTextTimeout("Utilize uma pederneira para acender a tocha");
            }
        }
        else
        {
            UIController.Instance.SetTextTimeout("Tocha já acesa");
        }
    }

    private void UseCrowbar()
    {

    }

    private void UseSwordRightButton()
    {
        if (GameController.Instance.VerifyItemLearned(SkillEnumerator.Set))
        {
            if (!usingSword)
            {
                usingSword = true;
                swordSaved = Inventory[indexSelected].actualItem;
                RemoveItemInventory();
                UIController.Instance.SetTextTimeout("Usando espada... selecione um escudo para ativar o conjunto");
            }
            else
            {
                UIController.Instance.SetTextTimeout("Já está utilizando espada, escolha um escudo primeiro");
            }
        }
    }

    private void UseShieldRightButton()
    {
        if (GameController.Instance.VerifyItemLearned(SkillEnumerator.Set))
        {
            if (!usingShield)
            {
                usingShield = true;
                shieldSaved = Inventory[indexSelected].actualItem;
                RemoveItemInventory();
                UIController.Instance.SetTextTimeout("Usando escudo... selecione uma espada para ativar o conjunto");
            }
            else
            {
                UIController.Instance.SetTextTimeout("Já está utilizando escudo, escolha uma espada primeiro");
            }
        }
    }

    private void UseRag()
    {
        if (GameController.Instance.VerifyItemLearned(SkillEnumerator.Ignitor))
        {
            if (!usingRagRightButton)
            {
                usingRagRightButton = true;
                RemoveItemInventory();
                UIController.Instance.SetTextTimeout("Usando trapo... termine a fórmula para criar o ignitor");
            }
            else
            {
                UIController.Instance.SetTextTimeout("Já está utilizando um trapo, termine de montar o ignitor primeiro");
            }
        }
    }

    private void UseOil()
    {
        if (GameController.Instance.VerifyItemLearned(SkillEnumerator.Ignitor))
        {
            if (!usingOilRightButton)
            {
                usingOilRightButton = true;
                RemoveItemInventory();
                UIController.Instance.SetTextTimeout("Usando trapo... termine a fórmula para criar o ignitor");
            }
            else
            {
                UIController.Instance.SetTextTimeout("Já está utilizando um óleo, termine de montar o ignitor primeiro");
            }
        }
    }

    private void UseGravelRightButton()
    {
        if (GameController.Instance.VerifyItemLearned(SkillEnumerator.Ignitor))
        {
            if (!usingGravelRightButton)
            {
                usingGravelRightButton = true;
                RemoveItemInventory();
                UIController.Instance.SetTextTimeout("Usando trapo... termine a fórmula para criar o ignitor");
            }
            else
            {
                UIController.Instance.SetTextTimeout("Já está utilizando um cascalho, termine de montar o ignitor primeiro");
            }
        }
    }

    private void UseShield()
    {
        CharacterController.Instance.animator.SetBool("Blocking", !CharacterController.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking"));
    }

    #endregion
}