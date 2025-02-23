using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponData : MonoBehaviour
{
    public BattleData BattleData;
    public int charindex;
    public int weaponindex;
    public Image weaponimage;
    public TextMeshProUGUI ranktext;
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = BattleData.Battlers[charindex].Weapons[weaponindex].WeaponName;
        weaponimage.sprite = BattleData.Battlers[charindex].Weapons[weaponindex].WeaponSprite;
        ranktext.text = BattleData.Battlers[charindex].Weapons[weaponindex].WeaponRank.ToString();
    }
    public void ButtonSubtituteWeapon()
    {
        GameObject.Find("BattleHandler").GetComponent<BattleHandler>().DeactivateWeapon();
        BattleData.Battlers[charindex].CurrentWeapon = BattleData.Battlers[charindex].Weapons[weaponindex];
        GameObject.Find("BattleHandler").GetComponent<BattleHandler>().ApplyWeapons();
        GameObject.Find("BattleHandler").GetComponent<BattleHandler>().UpdateStats();
        StartCoroutine(GameObject.Find("BattleHandler").GetComponent<BattleHandler>().StartTextBox("Arma aplicada!"));
    }
}
