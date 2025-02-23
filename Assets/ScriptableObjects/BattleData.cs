using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BattleData", menuName = "Battle Data")]
public class BattleData : ScriptableObject
{
    [Header("Battlers")]
    public List<PlayerSheet> Battlers = new List<PlayerSheet>();
    [Header("Enemy")]
    public PlayerSheet Enemy;
    [Header("Misc")]
    public Sprite Background;
    public AudioClip Music;
}
