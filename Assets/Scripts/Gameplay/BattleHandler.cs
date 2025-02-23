using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum BattleOption
{
    RegularFight,
    Ability,
    Item,
    Pass,
    None
}
public enum MinigameResult
{
    Poop,
    Bad,
    Meh,
    Ok,
    Good,
    Perfect
}
public enum BattleMenu
{
    Main,
    Fighting,
    Weapons,
    Item,
    Pooped
}
public class BattleTurn
{
    public BattleOption BattleOption;
    public PlayerAbility Ability;
    public PlayerItem Item;
}
public class BattleHandler : MonoBehaviour
{
    public BattleData BattleData;
    public GameObject CharPrefab;
    public GameObject AbilityPrefab;
    public GameObject WeaponPrefab;
    public GameObject CompanionPrefab;
    public GameObject ItemPrefab;
    public GameObject MinigameObjectPrefab;
    private AudioSource AudioSource;
    private Sprite Background;
    private SpriteRenderer BKRenderer;
    private TextMeshProUGUI ehpchar;
    private TextMeshProUGUI enamechar;
    private UnityEngine.UI.Image ehpbar;
    private bool GetEnterBool;
    private bool GetTime;
    private bool MinigameFinished;
    private Vector3 GetMovementVector;
    private int PongState;
    public Sprite NoCompanionSprite;
    public Sprite FinishTurnSprite;
    public Sprite FinishTurnDisabled;
    public BattleMinigame CurrentMinigame;
    private bool OnMinigames;
    private bool GotResult;
    [Header("Turns System")]
    public int CurrentIndex;
    public List<BattleMenu> LastMenuVisited;
    public List<BattleTurn> BattleTurns;
    [Header("Debug")]
    public bool MobileControlsOverride;

    public void Awake()
    {
        SceneManager.activeSceneChanged += SceneUpdated;
        BattleData = CutscenePlayer.CurrentBattleData;
        AudioSource = GetComponent<AudioSource>();
        Background = BattleData.Background;
        BKRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
        ehpchar = GameObject.FindGameObjectWithTag("EHPBarStat").GetComponent<TextMeshProUGUI>();
        ehpchar.text = "HP:" + BattleData.Enemy.hp + "/" + BattleData.Enemy.maxHp;
        ehpbar = GameObject.FindGameObjectWithTag("EHPBar").GetComponent<Image>();
        float maxVal = 0f + BattleData.Enemy.maxHp;
        float clampedhp = 0f + BattleData.Enemy.hp;
        clampedhp = Mathf.Clamp(clampedhp, 0f, maxVal);
        BattleData.Enemy.hp = (int)(clampedhp);
        enamechar = GameObject.FindGameObjectWithTag("EHPBarName").GetComponent<TextMeshProUGUI>();
        enamechar.text = BattleData.Enemy.BattlerName.ToUpper();
        UpdateEnemyBar();
        EnemySetSprite();
        LastMenuVisited = new List<BattleMenu>(BattleData.Battlers.Count);
        BattleTurns = new List<BattleTurn>(BattleData.Battlers.Count);
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            if (BattleData.Battlers[i].hp <= 0)
            {
                LastMenuVisited.Add(BattleMenu.Pooped);
            }
            else
            {
                LastMenuVisited.Add(BattleMenu.Main);
            }
            BattleTurns.Add(new BattleTurn());
            BattleTurns[i].BattleOption = BattleOption.None;
        }

        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            GameObject newchar = Instantiate(CharPrefab);
            newchar.transform.SetParent(GameObject.Find("CharacterDisplays").transform, false);
            newchar.name = "Char" + i;
            BattlerData battlerdata = newchar.GetComponent<BattlerData>();
            battlerdata.BattleData = BattleData;
            battlerdata.charindex = i;
            RectTransform battlerrect = newchar.GetComponent<RectTransform>();
            if (i == 0)
            {
                battlerrect.anchoredPosition = new Vector2(-165, -50);
            }
            if (i == 1)
            {
                battlerrect.anchoredPosition = new Vector2(85, -50);
            }
            if (i == 2)
            {
                battlerrect.anchoredPosition = new Vector2(335, -50);
            }
        }
        DOTween.SetTweensCapacity(500, 50);
    }
    public void UpdateEnemyBar()
    {
        ehpchar.text = "HP:" + BattleData.Enemy.hp + "/" + BattleData.Enemy.maxHp;
        ehpbar.DOFillAmount((float)BattleData.Enemy.hp / (float)BattleData.Enemy.maxHp, 1f);
    }
    public void EnemySetSprite()
    {
        int range = Mathf.RoundToInt(BattleData.Enemy.maxHp * 25 / 100);
        if (BattleData.Enemy.hp <= 0)
        {
            GameObject.FindGameObjectWithTag("Enemy").GetComponent<SpriteRenderer>().sprite = BattleData.Enemy.PoopSprite;
            enamechar.text = BattleData.Enemy.BattlerName.ToUpper() + " [MUERTO]";
            enamechar.color = Color.gray;
            ehpchar.color = Color.gray;
        }
        else if (BattleData.Enemy.hp <= range)
        {
            GameObject.FindGameObjectWithTag("Enemy").GetComponent<SpriteRenderer>().sprite = BattleData.Enemy.LowHealthSprite;
            enamechar.text = BattleData.Enemy.BattlerName.ToUpper();
            enamechar.color = Color.white;
            ehpchar.color = new Color (1, 0.5f, 0.5f, 1);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Enemy").GetComponent<SpriteRenderer>().sprite = BattleData.Enemy.NeutralSprite;
            enamechar.text = BattleData.Enemy.BattlerName.ToUpper();
            enamechar.color = Color.white;
            ehpchar.color = Color.white;
        }
    }
    public void Start()
    {
        TurnNewBattlerIndex(0);
        CurrentIndex = 0;
        if (Application.platform != RuntimePlatform.Android || !MobileControlsOverride)
        {
            GameObject.Find("PauseButton").SetActive(false);
        }
    }
    public void UpdateStats()
    {
        PlayerSheet TargetBattler = BattleData.Battlers[CurrentIndex];
        GameObject.FindGameObjectWithTag("NameStat").GetComponent<TextMeshProUGUI>().text = TargetBattler.BattlerName.ToUpper();
        GameObject.FindGameObjectWithTag("HPStat").GetComponent<TextMeshProUGUI>().text = "HP:" + TargetBattler.hp + "/" + TargetBattler.maxHp;
        GameObject.FindGameObjectWithTag("ATKStat").GetComponent<TextMeshProUGUI>().text = "ATK:" + TargetBattler.attackValue;
        GameObject.FindGameObjectWithTag("DEFStat").GetComponent<TextMeshProUGUI>().text = "DEF:" + TargetBattler.defenseValue;
        GameObject.FindGameObjectWithTag("CompHPStat").GetComponent<TextMeshProUGUI>().text = "HP:" + TargetBattler.hp + "/" + TargetBattler.maxHp;
        GameObject.FindGameObjectWithTag("CompATKStat").GetComponent<TextMeshProUGUI>().text = "ATK:" + TargetBattler.attackValue;
        GameObject.FindGameObjectWithTag("CompDEFStat").GetComponent<TextMeshProUGUI>().text = "DEF:" + TargetBattler.defenseValue;
        GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().text = "HP:" + TargetBattler.hp + "/" + TargetBattler.maxHp;
        GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().text = "ATK:" + TargetBattler.attackValue;
        GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().text = "DEF:" + TargetBattler.defenseValue;
        GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().text = "IV: " + TargetBattler.maxIV + " / " + TargetBattler.minIV;
        GameObject.Find("Char" + CurrentIndex).GetComponent<BattlerData>().UpdateHPBar();
    }
    public void TurnNewBattlerIndex(int target)
    {
        PlayerSheet TargetBattler = BattleData.Battlers[target];
        GameObject.FindGameObjectWithTag("NameStat").GetComponent<TextMeshProUGUI>().text = TargetBattler.BattlerName.ToUpper();
        GameObject.FindGameObjectWithTag("HPStat").GetComponent<TextMeshProUGUI>().text = "HP:" + TargetBattler.hp + "/" + TargetBattler.maxHp;
        GameObject.FindGameObjectWithTag("ATKStat").GetComponent<TextMeshProUGUI>().text = "ATK:" + TargetBattler.attackValue;
        GameObject.FindGameObjectWithTag("DEFStat").GetComponent<TextMeshProUGUI>().text = "DEF:" + TargetBattler.defenseValue;
        GameObject.FindGameObjectWithTag("CompHPStat").GetComponent<TextMeshProUGUI>().text = "HP:" + TargetBattler.hp + "/" + TargetBattler.maxHp;
        GameObject.FindGameObjectWithTag("CompATKStat").GetComponent<TextMeshProUGUI>().text = "ATK:" + TargetBattler.attackValue;
        GameObject.FindGameObjectWithTag("CompDEFStat").GetComponent<TextMeshProUGUI>().text = "DEF:" + TargetBattler.defenseValue;
        GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().text = "HP:" + TargetBattler.hp + "/" + TargetBattler.maxHp;
        GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().text = "ATK:" + TargetBattler.attackValue;
        GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().text = "DEF:" + TargetBattler.defenseValue;
        GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().text = "IV: " + TargetBattler.maxIV + " / " + TargetBattler.minIV;
        GameObject.FindGameObjectWithTag("AbilityScroll").GetComponent<Scrollbar>().value = 1;
        GameObject.FindGameObjectWithTag("WeaponsScroll").GetComponent<Scrollbar>().value = 1;
        GameObject.FindGameObjectWithTag("CompanionsScroll").GetComponent<Scrollbar>().value = 1;
        GameObject.FindGameObjectWithTag("ItemsScroll").GetComponent<Scrollbar>().value = 1;


        if (TargetBattler.HaveACompanion == false)
        {
            GameObject.FindGameObjectWithTag("COMPStat").GetComponent<Image>().sprite = NoCompanionSprite;
        }
        else
        {
            GameObject.FindGameObjectWithTag("COMPStat").GetComponent<Image>().sprite = TargetBattler.CurrentCompanion.CompanionSprite;
        }

        // Ability Generation

        if (GameObject.FindGameObjectWithTag("AbilityContent").transform.childCount != 0)
        {
            int childs = GameObject.FindGameObjectWithTag("AbilityContent").transform.childCount;
            while (childs != 0)
            {
                Destroy(GameObject.FindGameObjectWithTag("AbilityContent").transform.GetChild(childs - 1).gameObject);
                childs--;
            }
        }

        for (int i = 0; i < BattleData.Battlers[target].Abilities.Count; i++)
        {
            GameObject newability = Instantiate(AbilityPrefab);
            newability.transform.SetParent(GameObject.FindGameObjectWithTag("AbilityContent").transform, false);
            newability.name = "Ability" + i;
            AbilityData abilitydata = newability.GetComponent<AbilityData>();
            abilitydata.BattleData = BattleData;
            abilitydata.charindex = target;
            abilitydata.abilityindex = i;
        }

        // Weapon Generation

        if (GameObject.FindGameObjectWithTag("WeaponsContent").transform.childCount != 0)
        {
            int childs = GameObject.FindGameObjectWithTag("WeaponsContent").transform.childCount;
            while (childs != 0)
            {
                Destroy(GameObject.FindGameObjectWithTag("WeaponsContent").transform.GetChild(childs - 1).gameObject);
                childs--;
            }
        }

        for (int i = 0; i < BattleData.Battlers[target].Weapons.Count; i++)
        {
            GameObject newweapon = Instantiate(WeaponPrefab);
            newweapon.transform.SetParent(GameObject.FindGameObjectWithTag("WeaponsContent").transform, false);
            newweapon.name = "Weapon" + i;
            WeaponData weapondata = newweapon.GetComponent<WeaponData>();
            weapondata.BattleData = BattleData;
            weapondata.charindex = target;
            weapondata.weaponindex = i;
        }

        // Companion Generation

        if (GameObject.FindGameObjectWithTag("CompanionsContent").transform.childCount != 0)
        {
            int childs = GameObject.FindGameObjectWithTag("CompanionsContent").transform.childCount;
            while (childs != 0)
            {
                Destroy(GameObject.FindGameObjectWithTag("CompanionsContent").transform.GetChild(childs - 1).gameObject);
                childs--;
            }
        }

        for (int i = 0; i < BattleData.Battlers[target].Companions.Count; i++)
        {
            GameObject newcompanion = Instantiate(CompanionPrefab);
            newcompanion.transform.SetParent(GameObject.FindGameObjectWithTag("CompanionsContent").transform, false);
            newcompanion.name = "Companion" + i;
            CompanionData companiondata = newcompanion.GetComponent<CompanionData>();
            companiondata.BattleData = BattleData;
            companiondata.charindex = target;
            companiondata.companionindex = i;
            companiondata.publicomp = false;
        }
        for (int i = 0; i < Resources.LoadAll("GlobalCompanions/Enabled", typeof(ScriptableObject)).Length; i++)
        {
            GameObject newcompanion = Instantiate(CompanionPrefab);
            newcompanion.transform.SetParent(GameObject.FindGameObjectWithTag("CompanionsContent").transform, false);
            newcompanion.name = "GlobalCompanion" + i;
            CompanionData companiondata = newcompanion.GetComponent<CompanionData>();
            companiondata.BattleData = BattleData;
            companiondata.charindex = target;
            companiondata.companionindex = i;
            companiondata.publicomp = true;
        }

        // Item Generation

        if (GameObject.FindGameObjectWithTag("ItemsContent").transform.childCount != 0)
        {
            int childs = GameObject.FindGameObjectWithTag("ItemsContent").transform.childCount;
            while (childs != 0)
            {
                Destroy(GameObject.FindGameObjectWithTag("ItemsContent").transform.GetChild(childs - 1).gameObject);
                childs--;
            }
        }

        for (int i = 0; i < BattleData.Battlers[target].Items.Count; i++)
        {
            GameObject newitem = Instantiate(ItemPrefab);
            newitem.transform.SetParent(GameObject.FindGameObjectWithTag("ItemsContent").transform, false);
            newitem.name = "Item" + i;
            ItemData itemdata = newitem.GetComponent<ItemData>();
            itemdata.BattleData = BattleData;
            itemdata.charindex = target;
            itemdata.itemindex = i;
            itemdata.publicitem = false;
        }
        for (int i = 0; i < Resources.LoadAll("GlobalItems/Enabled", typeof(ScriptableObject)).Length; i++)
        {
            GameObject newitem = Instantiate(ItemPrefab);
            newitem.transform.SetParent(GameObject.FindGameObjectWithTag("ItemsContent").transform, false);
            newitem.name = "GlobalItem" + i;
            ItemData itemdata = newitem.GetComponent<ItemData>();
            itemdata.BattleData = BattleData;
            itemdata.charindex = target;
            itemdata.itemindex = i;
            itemdata.publicitem = true;
        }
        LoadMenu(LastMenuVisited[target].ToString());
    }
    public void UpdateLastMenu(BattleMenu BattleMenu)
    {
        LastMenuVisited[CurrentIndex] = BattleMenu;
    }
    public void UpdateTurnList(BattleOption BattleOption, PlayerAbility abilitychosen, PlayerItem itemchosen)
    {
        // Navegacion: HECHA
        // Generacion: HECHA
        // Turn List: HECHA
        // Da�o y efectos a Enemigo: HECHO
        // Da�o y efectos a Battlers: HECHO YAAAAAAAAAAAAAAAAAA-
        if (BattleOption == BattleOption.Pass)
        {
            BattleTurns[CurrentIndex].BattleOption = BattleOption.Pass;
        }
        if (BattleOption == BattleOption.RegularFight)
        {
            BattleTurns[CurrentIndex].BattleOption = BattleOption.RegularFight;
        }
        if (BattleOption == BattleOption.Ability)
        {
            BattleTurns[CurrentIndex].BattleOption = BattleOption.Ability;
        }
        if (BattleOption == BattleOption.Item)
        {
            BattleTurns[CurrentIndex].BattleOption = BattleOption.Item;
        }
        if (BattleOption == BattleOption.None)
        {
            BattleTurns[CurrentIndex].BattleOption = BattleOption.None;
        }
        if (abilitychosen != null)
        {
            BattleTurns[CurrentIndex].Ability = abilitychosen;
        }
        if (itemchosen != null)
        {
            BattleTurns[CurrentIndex].Item = itemchosen;
        }

        int turnsfilled = 0;
        for (int i = 0; i < BattleTurns.Count; i++)
        {
            if (BattleTurns[i].BattleOption != BattleOption.None)
            {
                turnsfilled++;
            }
        } 
        if (turnsfilled == BattleTurns.Count)
        {
            GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Image>().sprite = FinishTurnSprite;
            GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Button>().interactable = true;
            GameObject.FindGameObjectWithTag("FinishTurn").GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Image>().sprite = FinishTurnDisabled;
            GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Button>().interactable = false;
            GameObject.FindGameObjectWithTag("FinishTurn").GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.35f, 0.35f, 0.35f);
        }

        if (CurrentIndex + 1 == BattleData.Battlers.Count)
        {
            TurnNewBattlerIndex(0);
            CurrentIndex = 0;
        }
        else
        {
            TurnNewBattlerIndex(CurrentIndex + 1);
            CurrentIndex++;
        }
    }
    public void TurnListPass()
    {
        GameObject.Find("Char" + CurrentIndex).GetComponent<BattlerData>().UpdateOptionText(BattleOption.Pass, null, null);
        UpdateTurnList(BattleOption.Pass, null, null);
    }
    public void TurnListRegFight()
    {
        GameObject.Find("Char" + CurrentIndex).GetComponent<BattlerData>().UpdateOptionText(BattleOption.RegularFight, null, null);
        UpdateTurnList(BattleOption.RegularFight, null, null);
    }
    public void LoadMenu(string Menu)
    {
        BattleMenu BattleMenu = BattleMenu.Main;
        if (Menu == "Main")
        {
            BattleMenu = BattleMenu.Main;
        }
        else if (Menu == "Weapons")
        {
            BattleMenu = BattleMenu.Weapons;
        }
        else if (Menu == "Item")
        {
            BattleMenu = BattleMenu.Item;
        }
        else if (Menu == "Fighting")
        {
            BattleMenu = BattleMenu.Fighting;
        }
        else if (Menu == "Pooped")
        {
            BattleMenu = BattleMenu.Pooped;
        }

        if (LastMenuVisited[CurrentIndex] == BattleMenu.Main)
        {
            GameObject.Find("MainMenu").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            GameObject.Find("MainMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("MainMenu").transform.SetParent(GameObject.Find("Menus").transform);
            if (BattleMenu != BattleMenu.Main || BattleMenu != BattleMenu.Pooped)
            {
                GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                GameObject.FindGameObjectWithTag("Stats").transform.SetParent(GameObject.Find("Menus").transform);
            }
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Fighting)
        {
            GameObject.Find("FightingMenu").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            GameObject.Find("FightingMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("FightingMenu").transform.SetParent(GameObject.Find("Menus").transform);
            if (BattleMenu == BattleMenu.Main || BattleMenu == BattleMenu.Pooped)
            {
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = false;
                GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.Find("Menus").transform);

            }
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Weapons)
        {
            GameObject.Find("Weapons&CompMenu").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            GameObject.Find("Weapons&CompMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("Weapons&CompMenu").transform.SetParent(GameObject.Find("Menus").transform);
            if (BattleMenu == BattleMenu.Main || BattleMenu == BattleMenu.Pooped)
            {
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = false;
                GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.Find("Menus").transform);
            }
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Item)
        {
            GameObject.Find("ItemMenu").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            GameObject.Find("ItemMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ItemMenu").transform.SetParent(GameObject.Find("Menus").transform);
            if (BattleMenu == BattleMenu.Main || BattleMenu == BattleMenu.Pooped)
            {
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = false;
                GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.Find("Menus").transform);
            }
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Pooped)
        {
            GameObject.Find("PoopedMenu").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            GameObject.Find("PoopedMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("PoopedMenu").transform.SetParent(GameObject.Find("Menus").transform);
            if (BattleMenu != BattleMenu.Main || BattleMenu != BattleMenu.Pooped)
            {
                GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                GameObject.FindGameObjectWithTag("Stats").transform.SetParent(GameObject.Find("Menus").transform);
            }

        }
        StartCoroutine(AfterMenuFade(BattleMenu));
    }
    public IEnumerator AfterMenuFade(BattleMenu BattleMenu)
    {
        yield return new WaitForSeconds(0.5f);
        if (BattleMenu == BattleMenu.Main && OnMinigames == false)
        {
            GameObject.Find("MainMenu").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            GameObject.Find("MainMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("MainMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            if (GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().alpha == 0)
            {
                GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().interactable = true;
                GameObject.FindGameObjectWithTag("Stats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            }
        }
        else if (BattleMenu == BattleMenu.Fighting)
        {
            GameObject.Find("FightingMenu").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            GameObject.Find("FightingMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("FightingMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            if (GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().alpha == 0)
            {
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = true;
                GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            }
        }
        else if (BattleMenu == BattleMenu.Weapons)
        {
            GameObject.Find("Weapons&CompMenu").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            GameObject.Find("Weapons&CompMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("Weapons&CompMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            if (GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().alpha == 0)
            {
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = true;
                GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            }
        }
        else if (BattleMenu == BattleMenu.Item)
        {
            GameObject.Find("ItemMenu").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            GameObject.Find("ItemMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("ItemMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            if (GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().alpha == 0)
            {
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = true;
                GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            }
        }
        else if (BattleMenu == BattleMenu.Pooped)
        {
            GameObject.Find("PoopedMenu").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            GameObject.Find("PoopedMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("PoopedMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            if (GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().alpha == 0)
            {
                GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().interactable = true;
                GameObject.FindGameObjectWithTag("Stats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            }
        }
        UpdateLastMenu(BattleMenu);
    }
    public void SceneUpdated(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
    {
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            Background = BattleData.Background;
            BKRenderer.sprite = Background;
            AudioSource.clip = BattleData.Music;
            AudioSource.Play();
        }
    }
    public void DisplayEnemyInfo()
    {
        string TextToDisplay = BattleData.Enemy.BattlerName.ToUpper() + " - HP: " + BattleData.Enemy.hp + "/" + BattleData.Enemy.maxHp + "\nATK: " + BattleData.Enemy.attackValue + " | DEF: " + BattleData.Enemy.defenseValue + "\nIV: " + BattleData.Enemy.maxIV + " / " + BattleData.Enemy.minIV;
        StartCoroutine(StartTextBox(TextToDisplay));
    }
    public IEnumerator StartTextBox(string TextToDisplay)
    {
        GetEnterBool = false;
        GameObject.FindGameObjectWithTag("TextBox").GetComponent<Animator>().Play("TextBoxFadeIn");
        GameObject.FindGameObjectWithTag("EnemyStats").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        GameObject.FindGameObjectWithTag("EnemyStats").GetComponent<CanvasGroup>().interactable = false;
        StartCoroutine(Type(TextToDisplay, 0.08f));
        AudioSource audiosource = GameObject.FindGameObjectWithTag("TextBox").GetComponent<AudioSource>();
        audiosource.Play();
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>().text == TextToDisplay);
        audiosource.Stop();
        yield return new WaitUntil(() => GetEnterBool);
        GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>().text = "";
        GameObject.FindGameObjectWithTag("TextBox").GetComponent<Animator>().Play("TextBoxFadeOut");
        GameObject.FindGameObjectWithTag("EnemyStats").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        GameObject.FindGameObjectWithTag("EnemyStats").GetComponent<CanvasGroup>().interactable = true;
    }
    public IEnumerator Type(string sentence, float WriteSpeed)
    {
        //foreach (char letter in sentence.ToCharArray())
        //{
        //   GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>().text += letter;
        //    yield return new WaitForSeconds(WriteSpeed);
        //}
        for (int i = 0; i < sentence.ToCharArray().Length; i++)
        {
            string result = GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>().text += sentence.ToCharArray()[i];
            yield return new WaitForSeconds(WriteSpeed);
            GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>().text = result;
        }
    }
    public void GetEnter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GetEnterBool = true;
        }
        else
        {
            GetEnterBool = false;
        }
    }
    public void HideMenus()
    {
        OnMinigames = true;
        #region HidingMenus
        if (LastMenuVisited[CurrentIndex] == BattleMenu.Main)
        {
            GameObject.Find("MainMenu").GetComponent<CanvasGroup>().DOFade(0, 0);
            GameObject.Find("MainMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("MainMenu").transform.SetParent(GameObject.Find("Menus").transform);
            GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            GameObject.FindGameObjectWithTag("Stats").transform.SetParent(GameObject.Find("Menus").transform);
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Fighting)
        {
            GameObject.Find("FightingMenu").GetComponent<CanvasGroup>().DOFade(0, 0);
            GameObject.Find("FightingMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("FightingMenu").transform.SetParent(GameObject.Find("Menus").transform);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(0, 0);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = false;
            GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.Find("Menus").transform);
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Weapons)
        {
            GameObject.Find("Weapons&CompMenu").GetComponent<CanvasGroup>().DOFade(0, 0);
            GameObject.Find("Weapons&CompMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("Weapons&CompMenu").transform.SetParent(GameObject.Find("Menus").transform);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(0, 0);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = false;
            GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.Find("Menus").transform);
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Item)
        {
            GameObject.Find("ItemMenu").GetComponent<CanvasGroup>().DOFade(0, 0);
            GameObject.Find("ItemMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ItemMenu").transform.SetParent(GameObject.Find("Menus").transform);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(0, 0);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = false;
            GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.Find("Menus").transform);
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Pooped)
        {
            GameObject.Find("PoopedMenu").GetComponent<CanvasGroup>().DOFade(0, 0);
            GameObject.Find("PoopedMenu").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("PoopedMenu").transform.SetParent(GameObject.Find("Menus").transform);
            GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            GameObject.FindGameObjectWithTag("Stats").transform.SetParent(GameObject.Find("Menus").transform);
        }

        GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Image>().sprite = FinishTurnDisabled;
        GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Button>().interactable = false;
        GameObject.FindGameObjectWithTag("FinishTurn").GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.35f, 0.35f, 0.35f);

        #endregion
        GameObject.Find("MenuBackground").GetComponent<RectTransform>().DOLocalMoveY(-500f, 0.5f).SetEase(Ease.OutExpo);
        GameObject.FindGameObjectWithTag("NameStat").GetComponent<Transform>().DOLocalMoveY(-3.5f, 0.5f).SetEase(Ease.OutExpo);
        GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Transform>().DOLocalMoveY(-285f, 0.5f).SetEase(Ease.OutExpo);
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            GameObject battler = GameObject.Find("Char" + i);
            battler.GetComponent<Transform>().DOLocalMoveY(-250f, 0.5f).SetEase(Ease.OutExpo);
        }
    }
    public void RevealMenus()
    {
        OnMinigames = false;
        #region RevealingMenus
        if (LastMenuVisited[CurrentIndex] == BattleMenu.Main)
        {
            GameObject.Find("MainMenu").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.Find("MainMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("MainMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.FindGameObjectWithTag("Stats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Fighting)
        {
            GameObject.Find("FightingMenu").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.Find("FightingMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("FightingMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = true;
            GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Weapons)
        {
            GameObject.Find("Weapons&CompMenu").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.Find("Weapons&CompMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("Weapons&CompMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = true;
            GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Item)
        {
            GameObject.Find("ItemMenu").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.Find("ItemMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("ItemMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.FindGameObjectWithTag("CompStats").GetComponent<CanvasGroup>().interactable = true;
            GameObject.FindGameObjectWithTag("CompStats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
        }
        else if (LastMenuVisited[CurrentIndex] == BattleMenu.Pooped)
        {
            GameObject.Find("PoopedMenu").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.Find("PoopedMenu").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("PoopedMenu").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
            GameObject.FindGameObjectWithTag("Stats").GetComponent<CanvasGroup>().DOFade(1, 1);
            GameObject.FindGameObjectWithTag("Stats").transform.SetParent(GameObject.FindGameObjectWithTag("CurrentDisp").transform);
        }

        GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Image>().sprite = FinishTurnDisabled;
        GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Button>().interactable = false;
        GameObject.FindGameObjectWithTag("FinishTurn").GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.35f, 0.35f, 0.35f);

        #endregion
        GameObject.Find("MenuBackground").GetComponent<RectTransform>().DOLocalMoveY(-300f, 1).SetEase(Ease.OutExpo);
        GameObject.FindGameObjectWithTag("NameStat").GetComponent<Transform>().DOLocalMoveY(-1.02f, 1).SetEase(Ease.OutExpo);
        GameObject.FindGameObjectWithTag("FinishTurn").GetComponent<Transform>().DOLocalMoveY(-87.25f, 1).SetEase(Ease.OutExpo);
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            GameObject battler = GameObject.Find("Char" + i);
            battler.GetComponent<Transform>().DOLocalMoveY(-50f, 1).SetEase(Ease.OutExpo);
            UpdateTurnList(BattleOption.None, null, null);
            battler.GetComponent<BattlerData>().UpdateOptionText(BattleOption.None, null, null);
        }
    }
    public void RunAttackMinigame()
    {
        StartCoroutine(StartAttackMinigame());
    }
    public IEnumerator StartAttackMinigame()
    {
        CheckForItem();
        CheckForAbilities();
        int battlerindex = UnityEngine.Random.Range(0, BattleData.Battlers.Count - 1);
        int minigameindex = UnityEngine.Random.Range(0, BattleData.Battlers[battlerindex].AttackMinigames.Count - 1);
        BattleMinigame minigame = BattleData.Battlers[battlerindex].AttackMinigames[minigameindex];
        CurrentMinigame = minigame;
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("MinigameSetUp").GetComponent<SpriteRenderer>().sprite = minigame.Background;
        GameObject.Find("MinigameSetUp").GetComponent<SpriteRenderer>().color = minigame.BackgroundColor;
        GameObject.Find("MinigameCorners").GetComponent<SpriteRenderer>().color = minigame.ColorCorners;
        GameObject.Find("MinigameBorders").GetComponent<SpriteRenderer>().color = new Color(minigame.ColorCorners.r, minigame.ColorCorners.g, minigame.ColorCorners.b, 0.5f);
        // Object Generation
        for (int i = 0; i < minigame.ObjectsToGenerate; i++)
        {
            GameObject minigameobject = Instantiate(MinigameObjectPrefab);
            minigameobject.transform.SetParent(GameObject.Find("MinigameSetUp").transform);
            minigameobject.name = "MinigameObject" + i;
            minigameobject.tag = "MinigameObject" + i;
            minigameobject.GetComponent<Transform>().localPosition = minigame.Objects[i].LocalPosition;
            minigameobject.GetComponent<Transform>().localScale = minigame.Objects[i].Scale;
            minigameobject.GetComponent<SpriteRenderer>().sprite = minigame.Objects[i].ObjectSprite;
            minigameobject.GetComponent<SpriteRenderer>().sortingOrder = minigame.Objects[i].OrderInLayer;
            if (minigame.Objects[i].FreezePositionX)
            {
                minigameobject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                minigameobject.GetComponent<Rigidbody2D>().freezeRotation = true;
            }
            minigameobject.GetComponent<BoxCollider2D>().enabled = minigame.Objects[i].ColliderActive;
            minigameobject.GetComponent<BoxCollider2D>().size = minigame.Objects[i].Scale;
        }
        GameObject.Find("MinigameSetUp").GetComponent<Transform>().DOScaleY(1.5f, 0.4f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(0.4f);
        Mathf.Clamp(minigame.CurrentLevel, 1, 99);
        if (minigame.MinigameID == "PrecisionPos")
        {
            // PRECISION, POSITION
            float percentage = minigame.LevelDifficultyIVPercentage;
            for (int i = 0; i < minigame.CurrentLevel; i++)
            {
                percentage = percentage + (percentage * minigame.LevelDifficultyIVPercentage / 100);
            }
            float TimeLimit = minigame.InitialTimeLimit - (minigame.InitialTimeLimit * percentage / 100);
            GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().DOLocalMoveX(2f, TimeLimit).SetEase(Ease.Linear);
            yield return new WaitForSeconds(2f);
            if (GameObject.FindGameObjectWithTag("MinigameObject1") == null)
            {
                StartCoroutine(DetermineResult(MinigameResult.Poop, true));
            }
        }
    }
    public IEnumerator StartDefenseMinigame()
    {
        MinigameFinished = false;
        int battlerindex = UnityEngine.Random.Range(0, BattleData.Battlers.Count - 1);
        int minigameindex = UnityEngine.Random.Range(0, BattleData.Battlers[battlerindex].DefenseMinigames.Count - 1);
        BattleMinigame minigame = BattleData.Battlers[battlerindex].DefenseMinigames[minigameindex];
        CurrentMinigame = minigame;
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("MinigameSetUp").GetComponent<SpriteRenderer>().sprite = minigame.Background;
        GameObject.Find("MinigameSetUp").GetComponent<SpriteRenderer>().color = minigame.BackgroundColor;
        GameObject.Find("MinigameCorners").GetComponent<SpriteRenderer>().color = minigame.ColorCorners;
        GameObject.Find("MinigameBorders").GetComponent<SpriteRenderer>().color = new Color(minigame.ColorCorners.r, minigame.ColorCorners.g, minigame.ColorCorners.b, 0.5f);
        // Object Generation
        for (int i = 0; i < minigame.ObjectsToGenerate; i++)
        {
            GameObject minigameobject = Instantiate(MinigameObjectPrefab);
            minigameobject.transform.SetParent(GameObject.Find("MinigameSetUp").transform);
            minigameobject.name = "MinigameObject" + i;
            minigameobject.tag = "MinigameObject" + i;
            minigameobject.GetComponent<Transform>().localPosition = minigame.Objects[i].LocalPosition;
            minigameobject.GetComponent<Transform>().localScale = minigame.Objects[i].Scale;
            minigameobject.GetComponent<SpriteRenderer>().sprite = minigame.Objects[i].ObjectSprite;
            minigameobject.GetComponent<SpriteRenderer>().sortingOrder = minigame.Objects[i].OrderInLayer;
            if (minigame.Objects[i].FreezePositionX)
            {
                minigameobject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                minigameobject.GetComponent<Rigidbody2D>().freezeRotation = true;
            }
            minigameobject.GetComponent<BoxCollider2D>().enabled = minigame.Objects[i].ColliderActive;
            minigameobject.GetComponent<BoxCollider2D>().size = minigame.Objects[i].ColliderSize;
        }
        GameObject.Find("MinigameSetUp").GetComponent<Transform>().DOScaleY(1.5f, 0.4f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(0.4f);
        Mathf.Clamp(minigame.CurrentLevel, 1, 99);
        if (minigame.MinigameID == "Pong")
        {
            // PONG
            float percentage = minigame.LevelDifficultyIVPercentage;
            for (int i = 0; i < minigame.CurrentLevel; i++)
            {
                percentage = percentage + (percentage * minigame.LevelDifficultyIVPercentage / 100);
            }
            StartCoroutine(WaitForTime(minigame.InitialTimeLimit));
            float vel = 1 + (1 * percentage / 100);
            GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-0.5f, -0.5f) * vel;
            if (Application.platform == RuntimePlatform.Android || MobileControlsOverride)
            {
                GameObject.FindGameObjectWithTag("Up&Down").GetComponent<CanvasGroup>().DOFade(1, 0.15f);
            }
            yield return new WaitUntil(() => PongLogic(vel) && GetTime && GotResult);
            MinigameFinished = true;
            GetTime = false;
            if (Application.platform == RuntimePlatform.Android || MobileControlsOverride)
            {
                GameObject.FindGameObjectWithTag("Up&Down").GetComponent<CanvasGroup>().DOFade(0, 0.15f);
            }
        }
    }
    #region PONG
    private IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
        GetTime = true;
    }
    private bool PongLogic(float vel)
    {
        if (GetMovementVector.y != 1 && GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<Transform>().localPosition.y > -0.69f)
        {
            GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<Transform>().localPosition = new Vector3(1.98f, GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<Transform>().localPosition.y + (GetMovementVector.y * 0.1f));
        }
        if (GetMovementVector.y != -1 && GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<Transform>().localPosition.y! < 0.69f)
        {
            GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<Transform>().localPosition = new Vector3(1.98f, GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<Transform>().localPosition.y + (GetMovementVector.y * 0.1f));
        }
        if (GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localPosition.y > -0.69f)
        {
            GameObject.FindGameObjectWithTag("MinigameObject1").GetComponent<Transform>().localPosition = new Vector3(-1.98f, GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localPosition.y);
        }
        if (GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localPosition.y ! < 0.69f)
        {
            GameObject.FindGameObjectWithTag("MinigameObject1").GetComponent<Transform>().localPosition = new Vector3(-1.98f, GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localPosition.y);
        }
        if (GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<BoxCollider2D>().IsTouching(GameObject.FindGameObjectWithTag("MinigameObject1").GetComponent<BoxCollider2D>())) 
        {
            StartCoroutine(FlickerCollider(GameObject.FindGameObjectWithTag("MinigameObject1").GetComponent<BoxCollider2D>()));
            //PongState++;
        }
        if (GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<BoxCollider2D>().IsTouching(GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<BoxCollider2D>()))
        {
            StartCoroutine(FlickerCollider(GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<BoxCollider2D>()));
            float result = Mathf.Abs(GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localPosition.y - GameObject.FindGameObjectWithTag("MinigameObject2").GetComponent<Transform>().localPosition.y);
            if (result <= 0.05)
            {
                StartCoroutine(DetermineResult(MinigameResult.Perfect, false));
            }
            else if (result <= 0.1)
            {
                StartCoroutine(DetermineResult(MinigameResult.Good, false));
            }
            else if (result <= 0.2)
            {
                StartCoroutine(DetermineResult(MinigameResult.Ok, false));
            }
            else if (result <= 0.3)
            {
                StartCoroutine(DetermineResult(MinigameResult.Meh, false));
            }
            else if (result <= 0.1)
            {
                StartCoroutine(DetermineResult(MinigameResult.Bad, false));
            }
            //PongState++;
        }
        if (GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<BoxCollider2D>().IsTouching(GameObject.Find("MinigameSetUp").GetComponent<PolygonCollider2D>()))
        {
            if (GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localPosition.x >= 1.9f && GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localPosition.x <= 2f)
            {
                StartCoroutine(DetermineResult(MinigameResult.Poop, false));
            }
            StartCoroutine(FlickerCollider(GameObject.Find("MinigameSetUp").GetComponent<PolygonCollider2D>()));
            //PongState++;
        }
        if (PongState == 0)
        {
            GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-0.5f, -0.5f) * vel;
        }
        if (PongState == 1)
        {
            GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-0.5f, 0.5f) * vel;
        }
        if (PongState == 2)
        {
            GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0.5f, 0.5f) * vel;
        }
        if (PongState == 3)
        {
            GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0.5f, -0.5f) * vel;
        }
        if (PongState >= 4)
        {
            PongState = 0;
        }
        return true;
    }
    private IEnumerator FlickerCollider(Collider2D collider)
    {
        collider.enabled = false;
        PongState++;
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<BoxCollider2D>().IsTouching(collider));
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<BoxCollider2D>().IsTouching(collider));
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<BoxCollider2D>().IsTouching(collider));
        collider.enabled = true;
    }
    #endregion 
    public void NextDialogueDone(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CurrentMinigame != null)
            {
                if (CurrentMinigame.MinigameID == "PrecisionPos")
                {
                    // PRECISION, POSITION
                    GameObject copy = Instantiate(GameObject.FindGameObjectWithTag("MinigameObject0"));
                    copy.transform.SetParent(GameObject.Find("MinigameSetUp").transform);
                    copy.name = "MinigameObject1";
                    copy.tag = "MinigameObject1";
                    copy.GetComponent<Transform>().localPosition = GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localPosition;
                    copy.GetComponent<Transform>().localScale = GameObject.FindGameObjectWithTag("MinigameObject0").GetComponent<Transform>().localScale;
                    copy.GetComponent<SpriteRenderer>().color = new Color(0.35f, 0.35f, 0.35f);
                    if (copy.GetComponent<Transform>().localPosition.x > -0.02f && copy.GetComponent<Transform>().localPosition.x < 0.13f)
                    {
                        StartCoroutine(DetermineResult(MinigameResult.Perfect, true));
                        CurrentMinigame.CurrentLevel++;
                    }
                    else if (copy.GetComponent<Transform>().localPosition.x > -0.1f && copy.GetComponent<Transform>().localPosition.x < 0.2f)
                    {
                        StartCoroutine(DetermineResult(MinigameResult.Good, true));
                        CurrentMinigame.CurrentLevel++;
                    }
                    else if (copy.GetComponent<Transform>().localPosition.x > -0.35f && copy.GetComponent<Transform>().localPosition.x < 0.45f)
                    {
                        StartCoroutine(DetermineResult(MinigameResult.Ok, true));
                        CurrentMinigame.CurrentLevel++;
                    }
                    else if (copy.GetComponent<Transform>().localPosition.x > -0.75f && copy.GetComponent<Transform>().localPosition.x < 0.85f)
                    {
                        StartCoroutine(DetermineResult(MinigameResult.Meh, true));
                        CurrentMinigame.CurrentLevel--;
                    }
                    else if (copy.GetComponent<Transform>().localPosition.x > -1.25f && copy.GetComponent<Transform>().localPosition.x < 1.35f)
                    {
                        StartCoroutine(DetermineResult(MinigameResult.Bad, true));
                        CurrentMinigame.CurrentLevel--;
                    }
                    else
                    {
                        StartCoroutine(DetermineResult(MinigameResult.Poop, true));
                        CurrentMinigame.CurrentLevel--;
                    }
                }
            }
        }
    }
    public void GetMovement(InputAction.CallbackContext context)
    {
        if (context.performed == true)
        {
            GetMovementVector = context.ReadValue<Vector2>();
        }
        if (context.canceled == true)
        {
            GetMovementVector = Vector2.zero;
        }
    }
    public IEnumerator DetermineResult(MinigameResult minigameresult, bool isAttack)
    {
        GotResult = true;
        GameObject.Find("MinigameResult").GetComponent<TextMeshPro>().color = Color.white;
        GameObject.FindGameObjectWithTag("MinigameBlackText").GetComponent<TextMeshPro>().color = new Color(0, 0, 0, 0.65f);
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            Mathf.Clamp(BattleData.Battlers[i].DamageToEnemy, 0, 999);
        }
        if (minigameresult == MinigameResult.Perfect)
        {
            // Info Update
            GameObject.FindGameObjectWithTag("MinigameResult").GetComponent<TextMeshPro>().text = "PERFECTO";
            GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<SpriteRenderer>().color = new Color (0, 1, 0, 0.85f);
            GameObject.FindGameObjectWithTag("MinigameBlackText").GetComponent<TextMeshPro>().text = "PERFECTO";
            for (int i = 0; i < BattleData.Battlers.Count; i++)
            {
                BattleData.Battlers[i].DamageToEnemy = Mathf.RoundToInt(BattleData.Battlers[i].attackValue + (100 * BattleData.Battlers[i].maxIV / 100) - BattleData.Enemy.defenseValue);
                BattleData.Battlers[i].DamageToReceive = Mathf.RoundToInt(BattleData.Enemy.attackValue + (100 * BattleData.Battlers[i].maxIV / 100) - BattleData.Battlers[i].defenseValue);
            }
        }
        else if (minigameresult == MinigameResult.Good)
        {
            // Info Update
            GameObject.FindGameObjectWithTag("MinigameResult").GetComponent<TextMeshPro>().text = "BIEN";
            GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<SpriteRenderer>().color = new Color(0.5f, 1, 0, 0.85f);
            GameObject.FindGameObjectWithTag("MinigameBlackText").GetComponent<TextMeshPro>().text = "BIEN";
            for (int i = 0; i < BattleData.Battlers.Count; i++)
            {
                BattleData.Battlers[i].DamageToEnemy = Mathf.RoundToInt(BattleData.Battlers[i].attackValue + (50 * BattleData.Battlers[i].maxIV / 100) - BattleData.Enemy.defenseValue);
                BattleData.Battlers[i].DamageToReceive = Mathf.RoundToInt(BattleData.Enemy.attackValue + (50 * BattleData.Battlers[i].maxIV / 100) - BattleData.Battlers[i].defenseValue);
            }
        }
        else if (minigameresult == MinigameResult.Ok)
        {
            // Info Update
            GameObject.FindGameObjectWithTag("MinigameResult").GetComponent<TextMeshPro>().text = "OK";
            GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<SpriteRenderer>().color = new Color(0, 1, 0.5f, 0.85f);
            GameObject.FindGameObjectWithTag("MinigameBlackText").GetComponent<TextMeshPro>().text = "OK";
            for (int i = 0; i < BattleData.Battlers.Count; i++)
            {
                BattleData.Battlers[i].DamageToEnemy = Mathf.RoundToInt(BattleData.Battlers[i].attackValue + (25 * BattleData.Battlers[i].maxIV / 100) - BattleData.Enemy.defenseValue);
                BattleData.Battlers[i].DamageToReceive = Mathf.RoundToInt(BattleData.Enemy.attackValue + (25 * BattleData.Battlers[i].maxIV / 100) - BattleData.Battlers[i].defenseValue);
            }
        }
        else if (minigameresult == MinigameResult.Meh)
        {
            // Info Update
            GameObject.FindGameObjectWithTag("MinigameResult").GetComponent<TextMeshPro>().text = "MEH";
            GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 0.85f);
            GameObject.FindGameObjectWithTag("MinigameBlackText").GetComponent<TextMeshPro>().text = "MEH";
            for (int i = 0; i < BattleData.Battlers.Count; i++)
            {
                BattleData.Battlers[i].DamageToEnemy = Mathf.RoundToInt(BattleData.Battlers[i].attackValue + (25 * BattleData.Battlers[i].minIV / 100) - BattleData.Enemy.defenseValue);
                BattleData.Battlers[i].DamageToReceive = Mathf.RoundToInt(BattleData.Enemy.attackValue + (25 * BattleData.Battlers[i].minIV / 100) - BattleData.Battlers[i].defenseValue);
            }
        }
        else if (minigameresult == MinigameResult.Bad)
        {
            // Info Update
            GameObject.FindGameObjectWithTag("MinigameResult").GetComponent<TextMeshPro>().text = "MAL";
            GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<SpriteRenderer>().color = new Color(1, 0, 0.5f, 0.85f);
            GameObject.FindGameObjectWithTag("MinigameBlackText").GetComponent<TextMeshPro>().text = "MAL";
            for (int i = 0; i < BattleData.Battlers.Count; i++)
            {
                BattleData.Battlers[i].DamageToEnemy = Mathf.RoundToInt(BattleData.Battlers[i].attackValue + (50 * BattleData.Battlers[i].minIV / 100) - BattleData.Enemy.defenseValue);
                BattleData.Battlers[i].DamageToReceive = Mathf.RoundToInt(BattleData.Enemy.attackValue + (50 * BattleData.Battlers[i].minIV / 100) - BattleData.Battlers[i].defenseValue);
            }
        }
        else if (minigameresult == MinigameResult.Poop)
        {
            // Info Update
            GameObject.FindGameObjectWithTag("MinigameResult").GetComponent<TextMeshPro>().text = "POPOXD";
            GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.85f);
            GameObject.FindGameObjectWithTag("MinigameBlackText").GetComponent<TextMeshPro>().text = "POPOXD";
            for (int i = 0; i < BattleData.Battlers.Count; i++)
            {
                BattleData.Battlers[i].DamageToEnemy = Mathf.RoundToInt(BattleData.Battlers[i].attackValue + (100 * BattleData.Battlers[i].minIV / 100) - BattleData.Enemy.defenseValue);
                BattleData.Battlers[i].DamageToReceive = Mathf.RoundToInt(BattleData.Enemy.attackValue + (100 * BattleData.Battlers[i].minIV / 100) - BattleData.Battlers[i].defenseValue);
            }
        }
        // Animation
        GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<Transform>().DOScaleY(1.5f, 0.05f);
        GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<Transform>().DOLocalMoveY(0.85f, 0.05f);
        yield return new WaitForSeconds(0.05f);
        GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<Transform>().DOScaleY(0.99f, 0.5f);
        GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<Transform>().DOLocalMoveY(0.33f, 0.5f);
        yield return new WaitForSeconds(2f);
        if (CurrentMinigame.MinigameID == "Pong")
        {
            int level = CurrentMinigame.CurrentLevel;
            yield return new WaitUntil(() => MinigameFinished);
            if (minigameresult == MinigameResult.Perfect || minigameresult == MinigameResult.Good || minigameresult == MinigameResult.Ok)
            {
                level++;
            }
            else
            {
                level--;
            }
            CurrentMinigame.CurrentLevel = level;
        }
        // Reset
        GameObject.Find("MinigameSetUp").GetComponent<Transform>().DOScaleY(0, 0.4f);
        yield return new WaitForSeconds(0.4f);
        GameObject.FindGameObjectWithTag("MinigameResult").GetComponent<TextMeshPro>().color = Color.clear;
        GameObject.FindGameObjectWithTag("MinigameBlackText").GetComponent<TextMeshPro>().color = Color.clear;
        GameObject.FindGameObjectWithTag("MinigameGradient").GetComponent<SpriteRenderer>().color = Color.clear;
        for (int i = 0; i < 5; i++)
        {
            if (GameObject.FindGameObjectWithTag("MinigameObject" + i) != null)
            {
                Destroy(GameObject.FindGameObjectWithTag("MinigameObject" + i));
            }
        }
        if (isAttack)
        {
            StartCoroutine(DamageEnemy());
            yield return new WaitForSeconds(4f);
            if (BattleData.Enemy.hp <= 0)
            {

            }
            else
            {
                StartCoroutine(StartDefenseMinigame());
            }
        }
        else
        {
            StartCoroutine(DamageBattlers());
            yield return new WaitForSeconds(7f);
            DisableAbilities();
            RevealMenus();
        }
        GotResult = false;
    }
    public IEnumerator DamageEnemy()
    {
        yield return new WaitForSeconds(1f);
        int totaldamage = 0;
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        { 
            if (BattleData.Battlers[i].hp > 0 && BattleTurns[i].BattleOption == BattleOption.RegularFight)
            {
                totaldamage = totaldamage + BattleData.Battlers[i].DamageToEnemy;
            }
            else if (BattleData.Battlers[i].hp > 0 && BattleTurns[i].BattleOption == BattleOption.Ability)
            {
                totaldamage = totaldamage + BattleData.Battlers[i].DamageToEnemy;
            }
        }
        if (BattleData.Enemy.hp - totaldamage !< BattleData.Enemy.hp)
        {
            BattleData.Enemy.hp = BattleData.Enemy.hp - totaldamage;
            GameObject.FindGameObjectWithTag("EDamageBlack").GetComponent<TextMeshProUGUI>().color = new Color(0,0,0,1);
            GameObject.FindGameObjectWithTag("EDamage").GetComponent<TextMeshProUGUI>().color = new Color(1, 0.5f, 0.5f, 1);
            GameObject.FindGameObjectWithTag("EDamageBlack").GetComponent<TextMeshProUGUI>().text = totaldamage.ToString();
            GameObject.FindGameObjectWithTag("EDamage").GetComponent<TextMeshProUGUI>().text = totaldamage.ToString();
            StartCoroutine(DoRectShakeX(GameObject.FindGameObjectWithTag("EDamageBlack").GetComponent<Transform>()));
            StartCoroutine(DoRectShakeX(GameObject.FindGameObjectWithTag("EDamage").GetComponent<Transform>()));
            //StartCoroutine(DoShakeX(GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>()));
            UpdateEnemyBar();
            EnemySetSprite();
            yield return new WaitForSeconds(2f);
            GameObject.FindGameObjectWithTag("EDamageBlack").GetComponent<TextMeshProUGUI>().DOFade(0, 0.5f);
            GameObject.FindGameObjectWithTag("EDamage").GetComponent<TextMeshProUGUI>().DOFade(0, 0.5f);
        }
        if (BattleData.Enemy.hp <= 0)
        {
            StartCoroutine(StartTextBox("YOU WIN!!"));
            GameObject panel = GameObject.FindGameObjectWithTag("Panel");
            panel.GetComponent<Animator>().SetTrigger("IsLeaving");
            yield return new WaitForSeconds(0.5f);
            //SceneManager.LoadScene(CutscenePlayer.CameFrom);
            SceneManager.LoadScene("PrototypeScene4");
        }
    }
    public IEnumerator DamageBattlers()
    {
        int pooped = 0;
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            if (BattleData.Battlers[i].hp + BattleData.Battlers[i].DamageToReceive < BattleData.Battlers[i].hp && BattleData.Battlers[i].DamageToReceive < 0 && BattleData.Battlers[i].hp > 0)
            {
                int hp = BattleData.Battlers[i].hp;
                yield return new WaitForSeconds(0.1f);
                hp = hp + BattleData.Battlers[i].DamageToReceive;
                TextMeshProUGUI Damage = null;
                TextMeshProUGUI DamageBlack = null;
                for (int i2 = 0; i2 < GameObject.FindGameObjectsWithTag("Damage").Length; i2++)
                {
                    if (GameObject.FindGameObjectsWithTag("Damage")[i2].transform.parent.gameObject.name == "Char" + i)
                    {
                        Damage = GameObject.FindGameObjectsWithTag("Damage")[i2].GetComponent<TextMeshProUGUI>();
                    }
                    if (GameObject.FindGameObjectsWithTag("DamageBlack")[i2].transform.parent.gameObject.name == "Char" + i)
                    {
                        DamageBlack = GameObject.FindGameObjectsWithTag("DamageBlack")[i2].GetComponent<TextMeshProUGUI>();
                    }
                }
                DamageBlack.color = new Color(0, 0, 0, 1);
                Damage.color = new Color(1, 0.5f, 0.5f, 1);
                DamageBlack.text = Mathf.Abs(BattleData.Battlers[i].DamageToReceive).ToString();
                Damage.text = Mathf.Abs(BattleData.Battlers[i].DamageToReceive).ToString();
                StartCoroutine(DoShakeX(DamageBlack.gameObject.GetComponent<Transform>()));
                StartCoroutine(DoShakeX(Damage.gameObject.GetComponent<Transform>()));
                //StartCoroutine(DoShakeX(GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>()));
                BattleData.Battlers[i].hp = hp;
                GameObject.Find("Char" + i).GetComponent<BattlerData>().UpdateHPBar();
                GameObject.Find("Char" + i).GetComponent<BattlerData>().SetSprite();
                if (BattleData.Battlers[i].hp <= 0)
                {
                    LastMenuVisited[i] = BattleMenu.Pooped;
                }
                yield return new WaitForSeconds(2f);
                DamageBlack.DOFade(0, 0.5f);
                Damage.DOFade(0, 0.5f);
            }
            if (BattleData.Battlers[i].hp <= 0)
            {
                pooped++;
            }
        }
        if (pooped == BattleData.Battlers.Count)
        {
            StartCoroutine(StartTextBox("GAME OVER"));
            GameObject panel = GameObject.FindGameObjectWithTag("Panel");
            panel.GetComponent<Animator>().SetTrigger("IsLeaving");
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(CutscenePlayer.CameFrom);
        }
    }
    public IEnumerator DoRectShakeX(Transform transform)
    {
        transform.DOLocalMoveX(-150f, 0.05f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.05f);
        transform.DOLocalMoveX(150f, 0.05f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.05f);
        transform.DOLocalMoveX(-100f, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveX(100f, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveX(-50, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveX(50, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveX(0, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
    }
    public IEnumerator DoShakeX(Transform transform)
    {
        transform.DOLocalMoveX(-3f, 0.05f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.05f);
        transform.DOLocalMoveX(3f, 0.05f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.05f);
        transform.DOLocalMoveX(-2f, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveX(2f, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveX(-1, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveX(1, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveX(0, 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
    }
    public void CheckForItem()
    {
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            if (BattleTurns[i].BattleOption == BattleOption.Item)
            {
                BattleData.Battlers[i].hp = BattleData.Battlers[i].hp + BattleTurns[i].Item.sumofHP;
                BattleData.Battlers[i].attackValue = BattleData.Battlers[i].attackValue + BattleTurns[i].Item.sumofATK;
                BattleData.Battlers[i].defenseValue = BattleData.Battlers[i].defenseValue + BattleTurns[i].Item.sumofDEF;
                BattleData.Battlers[i].minIV = BattleTurns[i].Item.updatedminIV;
                BattleData.Battlers[i].maxIV = BattleTurns[i].Item.updatedmaxIV;
                int itemindex = 0;
                for (int i2 = 0; i2 < BattleData.Battlers[i].Weapons.Count; i2++)
                {
                    if (BattleData.Battlers[i].Items[i2] == BattleTurns[i].Item)
                    {
                        itemindex = i2;
                        break;
                    }
                }
                GameObject.Find("Char" + i).GetComponent<BattlerData>().UpdateHPBar();
                StartCoroutine(WaitForIndex(i, itemindex));
            }
        }
    }
    public void ApplyWeapons()
    {
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            if (BattleData.Battlers[i].CurrentWeapon != null)
            {
                int rank = BattleData.Battlers[i].CurrentWeapon.WeaponRank - 1;
                print(rank);
                int ATKToApply = BattleData.Battlers[i].CurrentWeapon.RanksData[rank].AddedATK;
                int DEFToApply = BattleData.Battlers[i].CurrentWeapon.RanksData[rank].AddedDEF;
                BattleData.Battlers[i].attackValue = BattleData.Battlers[i].attackValue + ATKToApply;
                BattleData.Battlers[i].defenseValue = BattleData.Battlers[i].defenseValue + DEFToApply;
            }
        }
    }
    public void DeactivateWeapon()
    {
        if (BattleData.Battlers[CurrentIndex].CurrentWeapon != null)
        {
            int rank = BattleData.Battlers[CurrentIndex].CurrentWeapon.WeaponRank - 1;
            print(rank);
            int ATKToApply = BattleData.Battlers[CurrentIndex].CurrentWeapon.RanksData[rank].AddedATK;
            int DEFToApply = BattleData.Battlers[CurrentIndex].CurrentWeapon.RanksData[rank].AddedDEF;
            BattleData.Battlers[CurrentIndex].attackValue = BattleData.Battlers[CurrentIndex].attackValue - ATKToApply;
            BattleData.Battlers[CurrentIndex].defenseValue = BattleData.Battlers[CurrentIndex].defenseValue - DEFToApply;
            BattleData.Battlers[CurrentIndex].CurrentWeapon = null;
        }
    }
    public void CheckForAbilities()
    {
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            if (BattleTurns[i].BattleOption == BattleOption.Ability)
            {
                BattleData.Battlers[i].hp = BattleData.Battlers[i].hp + BattleTurns[i].Ability.sumofHP;
                BattleData.Battlers[i].attackValue = BattleData.Battlers[i].attackValue + BattleTurns[i].Ability.sumofATK;
                BattleData.Battlers[i].defenseValue = BattleData.Battlers[i].defenseValue + BattleTurns[i].Ability.sumofDEF;
                BattleData.Battlers[i].minIV = BattleTurns[i].Ability.updatedminIV;
                BattleData.Battlers[i].maxIV = BattleTurns[i].Ability.updatedmaxIV;
                GameObject.Find("Char" + i).GetComponent<BattlerData>().UpdateHPBar();

                BattleData.Enemy.hp = BattleData.Enemy.hp + BattleTurns[i].Ability.esumofHP;
                BattleData.Enemy.attackValue = BattleData.Enemy.attackValue + BattleTurns[i].Ability.esumofATK;
                BattleData.Enemy.defenseValue = BattleData.Enemy.defenseValue + BattleTurns[i].Ability.esumofDEF;
                BattleData.Enemy.maxIV = BattleTurns[i].Ability.eupdatedmaxIV;
                BattleData.Enemy.minIV = BattleTurns[i].Ability.eupdatedminIV;
                UpdateEnemyBar();
            }
        }
    }
    public void DisableAbilities()
    {
        for (int i = 0; i < BattleData.Battlers.Count; i++)
        {
            if (BattleTurns[i].BattleOption == BattleOption.Ability)
            {
                BattleData.Battlers[i].hp = BattleData.Battlers[i].hp - BattleTurns[i].Ability.sumofHP;
                BattleData.Battlers[i].attackValue = BattleData.Battlers[i].attackValue - BattleTurns[i].Ability.sumofATK;
                BattleData.Battlers[i].defenseValue = BattleData.Battlers[i].defenseValue - BattleTurns[i].Ability.sumofDEF;
                BattleData.Battlers[i].minIV = -20;
                BattleData.Battlers[i].maxIV = 20;
                GameObject.Find("Char" + i).GetComponent<BattlerData>().UpdateHPBar();

                BattleData.Enemy.hp = BattleData.Enemy.hp - BattleTurns[i].Ability.esumofHP;
                BattleData.Enemy.attackValue = BattleData.Enemy.attackValue - BattleTurns[i].Ability.esumofATK;
                BattleData.Enemy.defenseValue = BattleData.Enemy.defenseValue - BattleTurns[i].Ability.esumofDEF;
                BattleData.Enemy.maxIV = 20;
                BattleData.Enemy.minIV = -20;
                UpdateEnemyBar();
            }
        }
    }
    public void ApplyCompanionChanges()
    {
        ClearCompanionAbilities();
        for (int i = 0; i < BattleData.Battlers[CurrentIndex].CurrentCompanion.Abilities.Count; i++)
        {
            BattleData.Battlers[CurrentIndex].Abilities.Add(BattleData.Battlers[CurrentIndex].CurrentCompanion.Abilities[i]);
        }

        // Ability Generation

        if (GameObject.FindGameObjectWithTag("AbilityContent").transform.childCount != 0)
        {
            int childs = GameObject.FindGameObjectWithTag("AbilityContent").transform.childCount;
            while (childs != 0)
            {
                Destroy(GameObject.FindGameObjectWithTag("AbilityContent").transform.GetChild(childs - 1).gameObject);
                childs--;
            }
        }

        for (int i = 0; i < BattleData.Battlers[CurrentIndex].Abilities.Count; i++)
        {
            GameObject newability = Instantiate(AbilityPrefab);
            newability.transform.SetParent(GameObject.FindGameObjectWithTag("AbilityContent").transform, false);
            newability.name = "Ability" + i;
            AbilityData abilitydata = newability.GetComponent<AbilityData>();
            abilitydata.BattleData = BattleData;
            abilitydata.charindex = CurrentIndex;
            abilitydata.abilityindex = i;
        }

        ClearCompanionItems();
        for (int i = 0; i < BattleData.Battlers[CurrentIndex].CurrentCompanion.Items.Count; i++)
        {
            BattleData.Battlers[CurrentIndex].Items.Add(BattleData.Battlers[CurrentIndex].CurrentCompanion.Items[i]);
        }

        // Item Generation

        if (GameObject.FindGameObjectWithTag("ItemsContent").transform.childCount != 0)
        {
            int childs = GameObject.FindGameObjectWithTag("ItemsContent").transform.childCount;
            while (childs != 0)
            {
                Destroy(GameObject.FindGameObjectWithTag("ItemsContent").transform.GetChild(childs - 1).gameObject);
                childs--;
            }
        }

        for (int i = 0; i < BattleData.Battlers[CurrentIndex].Items.Count; i++)
        {
            GameObject newitem = Instantiate(ItemPrefab);
            newitem.transform.SetParent(GameObject.FindGameObjectWithTag("ItemsContent").transform, false);
            newitem.name = "Item" + i;
            ItemData itemdata = newitem.GetComponent<ItemData>();
            itemdata.BattleData = BattleData;
            itemdata.charindex = CurrentIndex;
            itemdata.itemindex = i;
            itemdata.publicitem = false;
        }
        for (int i = 0; i < Resources.LoadAll("GlobalItems/Enabled", typeof(ScriptableObject)).Length; i++)
        {
            GameObject newitem = Instantiate(ItemPrefab);
            newitem.transform.SetParent(GameObject.FindGameObjectWithTag("ItemsContent").transform, false);
            newitem.name = "GlobalItem" + i;
            ItemData itemdata = newitem.GetComponent<ItemData>();
            itemdata.BattleData = BattleData;
            itemdata.charindex = CurrentIndex;
            itemdata.itemindex = i;
            itemdata.publicitem = true;
        }
        GameObject.FindGameObjectWithTag("COMPStat").GetComponent<Image>().sprite = BattleData.Battlers[CurrentIndex].CurrentCompanion.CompanionSprite;
    }
    public void ClearCompanionAbilities()
    {
        int abilities = BattleData.Battlers[CurrentIndex].Abilities.Count;
        for (int i = 0; i < abilities; i++)
        {
            if (BattleData.Battlers[CurrentIndex].Abilities[i].CompanionAbility)
            {
                BattleData.Battlers[CurrentIndex].Abilities.RemoveAt(i);
                abilities--;
                i--;
            }
        }
    }
    public void ClearCompanionItems()
    {
        int items = BattleData.Battlers[CurrentIndex].Items.Count;
        for (int i = 0; i < items; i++)
        {
            if (BattleData.Battlers[CurrentIndex].Items[i].CompanionItem)
            {
                BattleData.Battlers[CurrentIndex].Items.RemoveAt(i);
                items--;
                i--;
            }
        }
    }
    public IEnumerator WaitForIndex(int i, int itemindex)
    {
        yield return new WaitUntil(() => CurrentIndex == i);
        GameObject.Find("Item" + itemindex).GetComponent<ItemData>().DisableItem();
    }
}

