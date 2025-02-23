using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemData : MonoBehaviour
{
    public BattleData BattleData;
    public int charindex;
    public int itemindex;
    public Image itemimage;
    public Image icon0;
    public Image icon1;
    public Image icon2;
    public Sprite health;
    public Sprite def;
    public Sprite atk;
    public Sprite iv;
    public Sprite all;
    public bool publicitem;
    private bool affectedhealth;
    private bool affecteddef;
    private bool affectedatk;
    private bool affectediv;
    private bool affectedall;
    private void Start()
    {
        if (publicitem)
        {
            PlayerItem target = Resources.LoadAll("GlobalItems/Enabled", typeof(PlayerItem))[itemindex] as PlayerItem;
            GetComponent<TextMeshProUGUI>().text = target.ItemName + " (Global)";
            GetComponent<TextMeshProUGUI>().color = Color.cyan;
            itemimage.sprite = target.ItemSprite;
            if (target.sumofHP > 0)
            {
                affectedhealth = true;
            }
            else
            {
                affectedhealth = false;
            }
            if (target.sumofDEF > 0)
            {
                affecteddef = true;
            }
            else
            {
                affecteddef = false;
            }
            if (target.sumofATK > 0)
            {
                affectedatk = true;
            }
            else
            {
                affectedatk = false;
            }
            if (target.updatedminIV > 0)
            {
                affectediv = true;
            }
            else
            {
                if (target.updatedmaxIV != 0)
                {
                    affectediv = true;
                }
                else
                {
                    affectediv = false;
                }
            }
            if (target.sumofHP > 0 && target.sumofDEF > 0 && target.sumofATK > 0 && target.updatedminIV > 0)
            {
                affectedall = true;
            }
            else if (target.sumofHP > 0 && target.sumofDEF > 0 && target.sumofATK > 0 && target.updatedmaxIV > 0)
            {
                affectedall = true;
            }
            else
            {
                affectedall = false;
            }
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = BattleData.Battlers[charindex].Items[itemindex].ItemName;
            itemimage.sprite = BattleData.Battlers[charindex].Items[itemindex].ItemSprite;
            if (BattleData.Battlers[charindex].Items[itemindex].sumofHP > 0)
            {
                affectedhealth = true;
            }
            else
            {
                affectedhealth = false;
            }
            if (BattleData.Battlers[charindex].Items[itemindex].sumofDEF > 0)
            {
                affecteddef = true;
            }
            else
            {
                affecteddef = false;
            }
            if (BattleData.Battlers[charindex].Items[itemindex].sumofATK > 0)
            {
                affectedatk = true;
            }
            else
            {
                affectedatk = false;
            }
            if (BattleData.Battlers[charindex].Items[itemindex].updatedminIV > 0)
            {
                affectediv = true;
            }
            else
            {
                if (BattleData.Battlers[charindex].Items[itemindex].updatedmaxIV != 0)
                {
                    affectediv = true;
                }
                else
                {
                    affectediv = false;
                }
            }
            if (BattleData.Battlers[charindex].Items[itemindex].sumofHP > 0 && BattleData.Battlers[charindex].Items[itemindex].sumofDEF > 0 && BattleData.Battlers[charindex].Items[itemindex].sumofATK > 0 && BattleData.Battlers[charindex].Items[itemindex].updatedminIV > 0)
            {
                affectedall = true;
            }
            else if (BattleData.Battlers[charindex].Items[itemindex].sumofHP > 0 && BattleData.Battlers[charindex].Items[itemindex].sumofDEF > 0 && BattleData.Battlers[charindex].Items[itemindex].sumofATK > 0 && BattleData.Battlers[charindex].Items[itemindex].updatedmaxIV > 0)
            {
                affectedall = true;
            }
            else
            {
                affectedall = false;
            }
        }

        int i = 0;
        if (affectedall)
        {
            icon0.sprite = all;
            affectedall = false;
            affectedatk = false;
            affecteddef = false;
            affectedhealth = false;
            affectediv = false;
            i++;
        }

        if (affectedhealth)
        {
            icon0.sprite = health;
            affectedhealth = false;
            i++;
        }

        if (affecteddef)
        {
            affecteddef = false;
            if (i == 0)
            {
                icon0.sprite = def;
            }
            else if (i == 1)
            {
                icon1.sprite = def;
            }
            i++;
        }

        if (affectedatk)
        {
            affectedatk = false;
            if (i == 0)
            {
                icon0.sprite = atk;
            }
            else if (i == 1)
            {
                icon1.sprite = atk;
            }
            else if (i == 2)
            {
                icon2.sprite = atk;
            }
            i++;
        }

        if (affectediv)
        {
            affectediv = false;
            if (i == 0)
            {
                icon0.sprite = iv;
            }
            else if (i == 1)
            {
                icon1.sprite = iv;
            }
            else if (i == 2)
            {
                icon2.sprite = iv;
            }
            i++;
        }
    }
    public void ShowInfo()
    {
        bool Global = publicitem;
        GameObject.FindGameObjectWithTag("IIName").GetComponent<TextMeshProUGUI>().text = GetComponent<TextMeshProUGUI>().text;
        GameObject.FindGameObjectWithTag("IIName").GetComponent<TextMeshProUGUI>().color = GetComponent<TextMeshProUGUI>().color;
        GameObject.FindGameObjectWithTag("IIIcon0").GetComponent<Image>().sprite = icon0.sprite;
        GameObject.FindGameObjectWithTag("IIIcon1").GetComponent<Image>().sprite = icon1.sprite;
        GameObject.FindGameObjectWithTag("IIIcon2").GetComponent<Image>().sprite = icon2.sprite;
        if (Global)
        {
            PlayerItem target = Resources.LoadAll("GlobalItems/Enabled", typeof(PlayerItem))[itemindex] as PlayerItem;
            GameObject.FindGameObjectWithTag("IIImage").GetComponent<Image>().sprite = target.ItemSprite;
            GameObject.FindGameObjectWithTag("IIDesc").GetComponent<TextMeshProUGUI>().text = target.ItemDesc;
            GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().text = "HP:" + Mathf.Clamp(BattleData.Battlers[charindex].hp + target.sumofHP, BattleData.Battlers[charindex].hp + target.sumofHP, BattleData.Battlers[charindex].maxHp) + "/" + BattleData.Battlers[charindex].maxHp;
            GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().text = "DEF:" + (BattleData.Battlers[charindex].defenseValue + target.sumofDEF).ToString();
            GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().text = "ATK:" + (BattleData.Battlers[charindex].attackValue + target.sumofATK).ToString();
            int minIV = 0;
            if ((BattleData.Battlers[charindex].maxIV + target.updatedmaxIV) + (BattleData.Battlers[charindex].minIV + target.updatedminIV) > BattleData.Battlers[charindex].maxIV + target.updatedmaxIV)
            {
                minIV = BattleData.Battlers[charindex].maxIV + target.updatedmaxIV;
            }
            else
            {
                minIV = BattleData.Battlers[charindex].minIV + target.updatedminIV;
            }
            GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().text = "IV: " + (BattleData.Battlers[charindex].maxIV + target.updatedmaxIV).ToString() + " / " + minIV;
            if (Mathf.Clamp(BattleData.Battlers[charindex].hp + target.sumofHP, BattleData.Battlers[charindex].hp + target.sumofHP, BattleData.Battlers[charindex].maxHp) > BattleData.Battlers[charindex].hp)
            {
                GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().color = new Color(1, 0.5f, 0.5f);
            }
            else if (Mathf.Clamp(BattleData.Battlers[charindex].hp + target.sumofHP, BattleData.Battlers[charindex].hp + target.sumofHP, BattleData.Battlers[charindex].maxHp) < BattleData.Battlers[charindex].hp)
            {
                GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            }

            if (BattleData.Battlers[charindex].defenseValue + target.sumofDEF > BattleData.Battlers[charindex].defenseValue)
            {
                GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().color = new Color(0.4f, 0.75f, 1);
            }
            else if (BattleData.Battlers[charindex].defenseValue + target.sumofDEF < BattleData.Battlers[charindex].defenseValue)
            {
                GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            }

            if (BattleData.Battlers[charindex].attackValue + target.sumofATK > BattleData.Battlers[charindex].attackValue)
            {
                GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.9f, 0.5f);
            }
            else if (BattleData.Battlers[charindex].attackValue + target.sumofATK < BattleData.Battlers[charindex].attackValue)
            {
                GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            }
            GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            if (target.updatedmaxIV + BattleData.Battlers[charindex].maxIV < BattleData.Battlers[charindex].maxIV)
            {
                GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            if (minIV < BattleData.Battlers[charindex].minIV)
            {
                GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            if (target.updatedmaxIV + BattleData.Battlers[charindex].maxIV > BattleData.Battlers[charindex].maxIV)
            {
                GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.95f, 0.45f);
            }
            if (minIV > BattleData.Battlers[charindex].minIV)
            {
                GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.95f, 0.45f);
            }
        }
        else
        {
            GameObject.FindGameObjectWithTag("IIImage").GetComponent<Image>().sprite = BattleData.Battlers[charindex].Items[itemindex].ItemSprite;
            GameObject.FindGameObjectWithTag("IIDesc").GetComponent<TextMeshProUGUI>().text = BattleData.Battlers[charindex].Items[itemindex].ItemDesc;
            GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().text = "HP:" + Mathf.Clamp(BattleData.Battlers[charindex].hp + BattleData.Battlers[charindex].Items[itemindex].sumofHP, BattleData.Battlers[charindex].hp + BattleData.Battlers[charindex].Items[itemindex].sumofHP, BattleData.Battlers[charindex].maxHp) + "/" + BattleData.Battlers[charindex].maxHp;
            GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().text = "DEF:" + (BattleData.Battlers[charindex].defenseValue + BattleData.Battlers[charindex].Items[itemindex].sumofDEF).ToString();
            GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().text = "ATK:" + (BattleData.Battlers[charindex].attackValue + BattleData.Battlers[charindex].Items[itemindex].sumofATK).ToString();
            int minIV = 0;
            if (BattleData.Battlers[charindex].minIV + BattleData.Battlers[charindex].Items[itemindex].updatedminIV > 0)
            {
                minIV = 0;
            }
            else
            {
                minIV = BattleData.Battlers[charindex].minIV + BattleData.Battlers[charindex].Items[itemindex].updatedminIV;
            }
            GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().text = "IV: " + (BattleData.Battlers[charindex].maxIV + BattleData.Battlers[charindex].Items[itemindex].updatedmaxIV).ToString() + " / " + minIV;
            if (Mathf.Clamp(BattleData.Battlers[charindex].hp + BattleData.Battlers[charindex].Items[itemindex].sumofHP, BattleData.Battlers[charindex].hp + BattleData.Battlers[charindex].Items[itemindex].sumofHP, BattleData.Battlers[charindex].maxHp) > BattleData.Battlers[charindex].hp)
            {
                GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().color = new Color(1, 0.5f, 0.5f);
            }
            else if (Mathf.Clamp(BattleData.Battlers[charindex].hp + BattleData.Battlers[charindex].Items[itemindex].sumofHP, BattleData.Battlers[charindex].hp + BattleData.Battlers[charindex].Items[itemindex].sumofHP, BattleData.Battlers[charindex].maxHp) < BattleData.Battlers[charindex].hp)
            {
                GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                GameObject.FindGameObjectWithTag("ResultHP").GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            }

            if (BattleData.Battlers[charindex].defenseValue + BattleData.Battlers[charindex].Items[itemindex].sumofDEF > BattleData.Battlers[charindex].defenseValue)
            {
                GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().color = new Color(0.4f, 0.75f, 1);
            }
            else if (BattleData.Battlers[charindex].defenseValue + BattleData.Battlers[charindex].Items[itemindex].sumofDEF < BattleData.Battlers[charindex].defenseValue)
            {
                GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                GameObject.FindGameObjectWithTag("ResultDEF").GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            }

            if (BattleData.Battlers[charindex].attackValue + BattleData.Battlers[charindex].Items[itemindex].sumofATK > BattleData.Battlers[charindex].attackValue)
            {
                GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.9f, 0.5f);
            }
            else if (BattleData.Battlers[charindex].attackValue + BattleData.Battlers[charindex].Items[itemindex].sumofATK < BattleData.Battlers[charindex].attackValue)
            {
                GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                GameObject.FindGameObjectWithTag("ResultATK").GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            }
            GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            if (BattleData.Battlers[charindex].Items[itemindex].updatedmaxIV + BattleData.Battlers[charindex].maxIV < BattleData.Battlers[charindex].maxIV)
            {
                GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            if (minIV < BattleData.Battlers[charindex].minIV)
            {
                GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            if (BattleData.Battlers[charindex].Items[itemindex].updatedmaxIV + BattleData.Battlers[charindex].maxIV > BattleData.Battlers[charindex].maxIV)
            {
                GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.95f, 0.45f);
            }
            if (minIV > BattleData.Battlers[charindex].minIV)
            {
                GameObject.FindGameObjectWithTag("ResultIV").GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.95f, 0.45f);
            }
        }
    }
    public void ButtonSubtitute()
    {
        if (publicitem)
        {
            PlayerItem target = Resources.LoadAll("GlobalItems/Enabled", typeof(PlayerItem))[itemindex] as PlayerItem;
            GameObject.Find("Char" + charindex).GetComponent<BattlerData>().UpdateOptionText(BattleOption.Item, null, target);
            GameObject.Find("BattleHandler").GetComponent<BattleHandler>().UpdateTurnList(BattleOption.Item, null, target);
        }
        else
        {
            GameObject.Find("Char" + charindex).GetComponent<BattlerData>().UpdateOptionText(BattleOption.Item, null, BattleData.Battlers[charindex].Items[itemindex]);
            GameObject.Find("BattleHandler").GetComponent<BattleHandler>().UpdateTurnList(BattleOption.Item, null, BattleData.Battlers[charindex].Items[itemindex]);
        }
    }
    public void DisableItem()
    {
        GetComponent<Button>().interactable = false;
        GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
    }
}
