using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Player Ability")]
public class PlayerAbility : ScriptableObject
{
    [Header("Ability Data")]
    public string AbilityName;
    public bool SpecialAbility;
    public bool InitiallyAvailable;
    public int AbilityCooldown;
    public AnimationClip AbilityAnimation;
    public bool CompanionAbility;
    [Header("Change In Player/Character")]
    public int sumofHP;
    public int sumofATK;
    public int sumofDEF;
    public int updatedminIV;
    public int updatedmaxIV;
    public int duration;
    [Header("Change in Enemy")]
    public int esumofHP;
    public int esumofATK;
    public int esumofDEF;
    public int eupdatedminIV;
    public int eupdatedmaxIV;
    public int eduration;
}
