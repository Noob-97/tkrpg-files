using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Player Item")]
public class PlayerItem : ScriptableObject
{
    [Header("Item Data")]
    public string ItemName;
    public string ItemDesc;
    public Sprite ItemSprite;
    public bool PublicItem;
    public bool CompanionItem;
    [Header("Change In Player/Character")]
    public int sumofHP;
    public int sumofATK;
    public int sumofDEF;
    public int updatedminIV;
    public int updatedmaxIV;
}
