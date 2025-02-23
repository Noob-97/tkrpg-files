using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionData : MonoBehaviour
{
    public BattleData BattleData;
    public int charindex;
    public int companionindex;
    public Image companionimage;
    public bool publicomp;
    private void Start()
    {
        if (publicomp)
        {
            PlayerCompanion target = Resources.LoadAll("GlobalCompanions/Enabled", typeof(PlayerCompanion))[companionindex] as PlayerCompanion;
            GetComponent<TextMeshProUGUI>().text = target.CompanionName + " (Global)";
            GetComponent<TextMeshProUGUI>().color = Color.cyan;
            companionimage.sprite = target.CompanionSprite;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = BattleData.Battlers[charindex].Companions[companionindex].CompanionName;
            companionimage.sprite = BattleData.Battlers[charindex].Companions[companionindex].CompanionSprite;
        }
    }
    public void ButtonSubtitute()
    {
        BattleData.Battlers[charindex].CurrentCompanion = BattleData.Battlers[charindex].Companions[companionindex];
        BattleData.Battlers[charindex].HaveACompanion = true;
        GameObject.Find("BattleHandler").GetComponent<BattleHandler>().ApplyCompanionChanges();
        StartCoroutine(GameObject.Find("BattleHandler").GetComponent<BattleHandler>().StartTextBox("Compañero aplicado!!"));
    }
}
