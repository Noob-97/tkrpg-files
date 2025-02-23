using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Player Sheet", menuName = "Player Sheet")]
public class PlayerSheet : ScriptableObject
{
    [Header("Main Data")]
    public string BattlerName;
    public int hp;
    public int maxHp;
    public int attackValue;
    public int defenseValue;
    public int minIV;
    public int maxIV;
    [Header("Sprites - Battle")]
    public Sprite NeutralSprite;
    public Sprite LowHealthSprite;
    public Sprite PoopSprite;
    [Header("Weapons")]
    public List<PlayerWeapon> Weapons = new List<PlayerWeapon>();
    [SerializeField] public PlayerWeapon CurrentWeapon;
    [Header("Abilities")]
    public List<PlayerAbility> Abilities = new List<PlayerAbility>();
    [Header("Items")]
    public List<PlayerItem> Items = new List<PlayerItem>();
    [Header("Companions")]
    public List<PlayerCompanion> Companions = new List<PlayerCompanion>();
    public bool HaveACompanion;
    public PlayerCompanion CurrentCompanion;
    [Header("Minigames")]
    public List<BattleMinigame> AttackMinigames = new List<BattleMinigame>();
    public List<BattleMinigame> DefenseMinigames = new List<BattleMinigame>();
    [Header("Temporal Data")]
    public int DamageToEnemy;
    public int DamageToReceive;
}
