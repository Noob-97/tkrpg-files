using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattlerData : MonoBehaviour
{
    public BattleData BattleData;
    public int charindex;

    private TextMeshProUGUI hpchar;
    private TextMeshProUGUI namechar;
    private UnityEngine.UI.Image hpbar;
    private TextMeshProUGUI battleoption;

    public void Start()
    {
        SceneManager.activeSceneChanged += SceneUpdated;
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("HPBarStat").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("HPBarStat")[i].transform.parent.gameObject.name == "Char" + charindex)
            {
                hpchar = GameObject.FindGameObjectsWithTag("HPBarStat")[i].GetComponent<TextMeshProUGUI>();
                hpchar.text = "HP:" + BattleData.Battlers[charindex].hp + "/" + BattleData.Battlers[charindex].maxHp;
            }
            if (GameObject.FindGameObjectsWithTag("HPBarName")[i].transform.parent.gameObject.name == "Char" + charindex)
            {
                namechar = GameObject.FindGameObjectsWithTag("HPBarName")[i].GetComponent<TextMeshProUGUI>();
                namechar.text = BattleData.Battlers[charindex].BattlerName.ToUpper();
            }
            if (GameObject.FindGameObjectsWithTag("HPBar")[i].transform.parent.gameObject.name == "Char" + charindex)
            {
                hpbar = GameObject.FindGameObjectsWithTag("HPBar")[i].GetComponent<UnityEngine.UI.Image>();
                float maxVal = 0f + BattleData.Battlers[charindex].maxHp;
                float clampedhp = 0f + BattleData.Battlers[charindex].hp;
                clampedhp = Mathf.Clamp(clampedhp, 0f, maxVal);
                BattleData.Battlers[charindex].hp = (int)(clampedhp);
                UpdateHPBar();
            }
            if (GameObject.FindGameObjectsWithTag("BattleOption")[i].transform.parent.gameObject.name == "Char" + charindex)
            {
                battleoption = GameObject.FindGameObjectsWithTag("BattleOption")[i].GetComponent<TextMeshProUGUI>();
            }
        }
        SetSprite();
    }
    public void SceneUpdated(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
    {
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("HPBarStat").Length; i++)
            {
                if (GameObject.FindGameObjectsWithTag("HPBarStat")[i].transform.parent.gameObject.name == "Char" + charindex)
                {
                    hpchar = GameObject.FindGameObjectsWithTag("HPBarStat")[i].GetComponent<TextMeshProUGUI>();
                    hpchar.text = "HP:" + BattleData.Battlers[charindex].hp + "/" + BattleData.Battlers[charindex].maxHp;
                }
                if (GameObject.FindGameObjectsWithTag("HPBarName")[i].transform.parent.gameObject.name == "Char" + charindex)
                {
                    namechar = GameObject.FindGameObjectsWithTag("HPBarName")[i].GetComponent<TextMeshProUGUI>();
                    namechar.text = BattleData.Battlers[charindex].BattlerName.ToUpper();
                }
                if (GameObject.FindGameObjectsWithTag("HPBar")[i].transform.parent.gameObject.name == "Char" + charindex)
                {
                    hpbar = GameObject.FindGameObjectsWithTag("HPBar")[i].GetComponent<UnityEngine.UI.Image>();
                    float maxVal = 0f + BattleData.Battlers[charindex].maxHp;
                    float clampedhp = 0f + BattleData.Battlers[charindex].hp;
                    clampedhp = Mathf.Clamp(clampedhp, 0f, maxVal);
                    BattleData.Battlers[charindex].hp = (int)(clampedhp);
                    UpdateHPBar();
                }
                if (GameObject.FindGameObjectsWithTag("BattleOption")[i].transform.parent.gameObject.name == "Char" + charindex)
                {
                    battleoption = GameObject.FindGameObjectsWithTag("BattleOption")[i].GetComponent<TextMeshProUGUI>();
                }
            }
        }
    }
    public void SetSprite()
    {
        int range = Mathf.RoundToInt(BattleData.Battlers[charindex].maxHp * 25 / 100);
        if (BattleData.Battlers[charindex].hp <= 0)
        {
            GameObject.Find("Char" + charindex).GetComponent<UnityEngine.UI.Image>().sprite = BattleData.Battlers[charindex].PoopSprite;
            namechar.text = BattleData.Battlers[charindex].BattlerName.ToUpper() + " [MUERTO]";
            namechar.color = Color.gray;
            hpchar.color = Color.gray;
        }
        else if (BattleData.Battlers[charindex].hp <= range)
        {
            GameObject.Find("Char" + charindex).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = BattleData.Battlers[charindex].LowHealthSprite;
            namechar.text = BattleData.Battlers[charindex].BattlerName.ToUpper();
            namechar.color = Color.white;
            hpchar.color = new Color(1, 0.5f, 0.5f, 1);
        }
        else
        {
            GameObject.Find("Char" + charindex).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = BattleData.Battlers[charindex].NeutralSprite;
            namechar.text = BattleData.Battlers[charindex].BattlerName.ToUpper();
            namechar.color = Color.white;
            hpchar.color = Color.white;
        }
    }
    public void UpdateHPBar()
    {
        hpchar.text = "HP:" + BattleData.Battlers[charindex].hp + "/" + BattleData.Battlers[charindex].maxHp;
        hpbar.DOFillAmount((float)BattleData.Battlers[charindex].hp / (float)BattleData.Battlers[charindex].maxHp, 1f);
    }
    public void ButtonSubtitute()
    {
        GameObject.Find("BattleHandler").GetComponent<BattleHandler>().TurnNewBattlerIndex(charindex);
        GameObject.Find("BattleHandler").GetComponent<BattleHandler>().CurrentIndex = charindex;
    }
    public void UpdateOptionText(BattleOption optionchosen, PlayerAbility ability, PlayerItem item)
    {
        if (optionchosen == BattleOption.None)
        {
            battleoption.text = "NADA ESCODIGO";
            battleoption.color = new Color(0.6f, 0.6f, 0.6f);
        }
        if (optionchosen == BattleOption.RegularFight)
        {
            battleoption.text = "ATACAR: REGULAR";
            battleoption.color = new Color(1f, 0.6f, 0.6f);
        }
        if (optionchosen == BattleOption.Ability)
        {
            battleoption.text = "ATACAR: " + ability.AbilityName.ToUpper();
            battleoption.color = new Color(1f, 0.6f, 0.6f);
        }
        if (optionchosen == BattleOption.Item)
        {
            battleoption.text = "ITEM: " + item.ItemName.ToUpper();
            battleoption.color = new Color(0.6f, 0.6f, 1f);
        }
        if (optionchosen == BattleOption.Pass)
        {
            battleoption.text = "PASAR";
            battleoption.color = new Color(1f, 0.9f, 0.6f);
        }
    }
}
