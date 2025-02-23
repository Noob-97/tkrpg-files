using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Companion", menuName = "Player Companion")]
public class PlayerCompanion : ScriptableObject
{
    [Header("Companion Data")]
    public string CompanionName;
    public bool GlobalCompanion;
    public Sprite CompanionSprite;
    [Header("Abilities")]
    public List<PlayerAbility> Abilities = new List<PlayerAbility>();
    [Header("Items")]
    public List<PlayerItem> Items = new List<PlayerItem>();
}
