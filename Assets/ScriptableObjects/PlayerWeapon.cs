using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName = "New PlayerWeapon", menuName = "Player Weapon")]
public class PlayerWeapon : ScriptableObject
{
    [Header("Weapon Data")]
    public string WeaponName;
    public Sprite WeaponSprite;
    public int WeaponRank;
    [Header("Weapon Ranks")]
    public int Ranks;
    public List<WeaponRank> RanksData = new List<WeaponRank>();
}
[Serializable]
public class WeaponRank
{
    public int AddedATK;
    public int AddedDEF;
    public Animation Animation;
}
#if UNITY_EDITOR
[CustomEditor(typeof(PlayerWeapon))]
public class PlayerWeaponEditor : Editor
{
    List<bool> RankOpen = new List<bool>();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlayerWeapon playerweapon = (PlayerWeapon)target;
        serializedObject.Update();
        if (playerweapon.Ranks > 0)
        {
            for (int i = 0; i < playerweapon.Ranks; i++)
            {
                if (RankOpen.Count < i + 1)
                {
                    RankOpen.Add(false);
                    playerweapon.RanksData.Add(new WeaponRank());
                }
                RankOpen[i] = EditorGUILayout.BeginFoldoutHeaderGroup(RankOpen[i], "Rank Data: " + (i + 1));
                if (RankOpen[i])
                {
                    playerweapon.RanksData[i].AddedDEF = EditorGUILayout.IntField("Added DEF", playerweapon.RanksData[i].AddedDEF);
                    playerweapon.RanksData[i].AddedATK = EditorGUILayout.IntField("Added ATK", playerweapon.RanksData[i].AddedATK);
                    playerweapon.RanksData[i].Animation = EditorGUILayout.ObjectField("Animation", playerweapon.RanksData[i].Animation, typeof(Animation), true) as Animation;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.HelpBox("Saved Ranks: " + playerweapon.RanksData.Count + ".\nIn case of an issue, try setting 'Ranks' to 0, or pressing this button.", MessageType.Info);
            if (GUILayout.Button("Clear Unnecesary Ranks"))
            {
                for (int i = 0; i < playerweapon.RanksData.Count; i++)
                {
                    if (playerweapon.RanksData[i].AddedDEF == 0 && playerweapon.RanksData[i].AddedATK == 0 && playerweapon.RanksData[i].Animation == null)
                    {
                        playerweapon.RanksData.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        else
        {
            RankOpen.Clear();
            playerweapon.RanksData.Clear();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif