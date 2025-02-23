using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityData : MonoBehaviour
{
    public BattleData BattleData;
    public int charindex;
    public int abilityindex;
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = BattleData.Battlers[charindex].Abilities[abilityindex].AbilityName;
    }
    public void ButtonSubtitute()
    {
        GameObject.Find("Char" + charindex).GetComponent<BattlerData>().UpdateOptionText(BattleOption.Ability, BattleData.Battlers[charindex].Abilities[abilityindex], null);
        GameObject.Find("BattleHandler").GetComponent<BattleHandler>().UpdateTurnList(BattleOption.Ability, BattleData.Battlers[charindex].Abilities[abilityindex], null);
    }
}
