using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Minigame", menuName = "Battle Minigame")]
public class BattleMinigame : ScriptableObject
{
    [Header("General Info.")]
    public string MinigameName;
    public string MinigameID;
    public enum MinigameType {AttackMinigame, DefenseMinigame};
    public MinigameType TypeOfMinigame;
    [Header("Appearance")]
    public Sprite Background;
    public Color BackgroundColor;
    public Color ColorCorners;
    public int ObjectsToGenerate;
    [SerializeField] public List<ObjectToGenerate> Objects = new List<ObjectToGenerate>();
    [Header("Gameplay Info.")]
    public float InitialTimeLimit;
    public int CurrentLevel;
    public float LevelDifficultyIVPercentage;
}
[Serializable]
public class ObjectToGenerate
{
    [SerializeField] public Sprite ObjectSprite;
    [SerializeField] public Vector2 LocalPosition;
    [SerializeField] public Vector2 Scale;
    [SerializeField] public int OrderInLayer;
    [SerializeField] public bool FreezePositionX;
    [SerializeField] public bool ColliderActive;
    [SerializeField] public Vector2 ColliderSize;
}
#if UNITY_EDITOR
[CustomEditor(typeof(BattleMinigame))]
public class BattleMinigameEditor : Editor
{
    List<bool> ObjectOpen = new List<bool>();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BattleMinigame battleminigame = (BattleMinigame)target;
        serializedObject.Update();
        if (battleminigame.ObjectsToGenerate > 0)
        {
            for (int i = 0; i < battleminigame.ObjectsToGenerate; i++)
            {
                if (ObjectOpen.Count < i + 1)
                {
                    ObjectOpen.Add(false);
                    battleminigame.Objects.Add(new ObjectToGenerate());
                }
                ObjectOpen[i] = EditorGUILayout.BeginFoldoutHeaderGroup(ObjectOpen[i], "Object Data: " + (i + 1));
                if (ObjectOpen[i])
                {
                    EditorGUILayout.LabelField("Transform");
                    battleminigame.Objects[i].LocalPosition = EditorGUILayout.Vector2Field("Local Position", battleminigame.Objects[i].LocalPosition);
                    battleminigame.Objects[i].Scale = EditorGUILayout.Vector2Field("Scale", battleminigame.Objects[i].Scale);
                    EditorGUILayout.LabelField("Sprite Renderer");
                    battleminigame.Objects[i].OrderInLayer = EditorGUILayout.IntField("Order In Layer", battleminigame.Objects[i].OrderInLayer);
                    battleminigame.Objects[i].ObjectSprite = EditorGUILayout.ObjectField("Object Sprite", battleminigame.Objects[i].ObjectSprite, typeof(Sprite), true) as Sprite;
                    EditorGUILayout.LabelField("Rigidbody 2D");
                    battleminigame.Objects[i].FreezePositionX = EditorGUILayout.Toggle("Freeze Position X", battleminigame.Objects[i].FreezePositionX);
                    EditorGUILayout.LabelField("Box Collider 2D");
                    battleminigame.Objects[i].ColliderActive = EditorGUILayout.Toggle("Use Collider", battleminigame.Objects[i].ColliderActive);
                    battleminigame.Objects[i].ColliderSize = EditorGUILayout.Vector2Field("Collider Size", battleminigame.Objects[i].ColliderSize);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.HelpBox("Saved Objects: " + battleminigame.Objects.Count + ".\nIn case of an issue, try setting 'Objects To Generate' to 0, or pressing this button.", MessageType.Info);
            if (GUILayout.Button("Clear Unnecesary Objects"))
            {
                for (int i = 0; i < battleminigame.Objects.Count; i++)
                {
                    if (battleminigame.Objects[i].ObjectSprite == null && battleminigame.Objects[i].LocalPosition == Vector2.zero)
                    {
                        battleminigame.Objects.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        else
        {
            ObjectOpen.Clear();
            battleminigame.Objects.Clear();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif