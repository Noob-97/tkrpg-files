using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public enum ValueType
{
    String,
    Int,
    Bool
}
[Serializable]
public class Parameter
{
    public string Name;
    public ValueType ValueType;
    public bool NoDefault;
    public bool Optional;
    public object DefaultValue;
    public object Value;
}
[Serializable]
public class Command
{
    public string Name;
    public string Executer;
    public List<Parameter> Parameters = new List<Parameter>();
    public UnityEvent ActionVoid;
    public UnityEvent<object> ActionOnePara;
    public UnityEvent<object, object> ActionTwoPara;
    public UnityEvent<object, object, object> ActionThreePara;
    public UnityEvent<object, object, object, object> ActionFourPara;
}

[CreateAssetMenu(fileName = "New Command Set", menuName = "Command Set")]
[Serializable]
public class CommandSet : ScriptableObject
{
    [SerializeField] public List<Command> Commands = new List<Command>();
}
#if UNITY_EDITOR
[CustomEditor(typeof(CommandSet))]
public class CommandSetEditor : Editor
{
    int EraseIndex;
    List<bool> CommandOpen = new List<bool>();
    public override void OnInspectorGUI()
    {
        CommandSet set = (CommandSet)target;
        serializedObject.Update();
        EditorGUILayout.LabelField("Commands");
        if (GUILayout.Button("Create Command"))
        {
            set.Commands.Add(new Command());
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete Command"))
        {
            set.Commands.RemoveAt(EraseIndex);
        }
        if (GUILayout.Button("Clear List"))
        {
            set.Commands.Clear();
        }
        EraseIndex = EditorGUILayout.IntField("Delete Index: ", EraseIndex);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("Saved Commands: " + set.Commands.Count + ".", MessageType.Info);
        if (set.Commands.Count > 0)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < set.Commands.Count; i++)
            {
                if (CommandOpen.Count < i + 1)
                {
                    CommandOpen.Add(false);
                }
                string texttodisplay = set.Commands[i].Name;
                if (string.IsNullOrEmpty(texttodisplay))
                {
                    texttodisplay = "[Command #" + (i + 1)+ "]";
                }
                CommandOpen[i] = EditorGUILayout.BeginFoldoutHeaderGroup(CommandOpen[i], texttodisplay);
                if (CommandOpen[i])
                {
                    int EraseIndexParameter = 0;
                    List<Type> ActionParameters = new List<Type>();

                    EditorGUI.indentLevel++;
                    set.Commands[i].Name = EditorGUILayout.TextField("Name", set.Commands[i].Name);
                    set.Commands[i].Executer = EditorGUILayout.TextField("Executer", set.Commands[i].Executer);

                    EditorGUILayout.LabelField("Parameters");
                    if (GUILayout.Button("Create Parameter"))
                    {
                        set.Commands[i].Parameters.Add(new Parameter());
                    }
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Delete Parameter"))
                    {
                        set.Commands[i].Parameters.RemoveAt(EraseIndexParameter);
                    }
                    if (GUILayout.Button("Clear List"))
                    {
                        set.Commands[i].Parameters.Clear();
                    }
                    EraseIndexParameter = EditorGUILayout.IntField("Delete Index: ", EraseIndexParameter);
                    EditorGUILayout.EndHorizontal();
                    if (set.Commands[i].Parameters.Count > 0)
                    {
                        for (int j = 0; j < set.Commands[i].Parameters.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(set.Commands[i].Parameters[j].Name))
                            {
                                EditorGUILayout.LabelField(set.Commands[i].Parameters[j].Name);
                            }
                            else
                            {
                                EditorGUILayout.LabelField("[Property #" + (j + 1)+ "]");
                            }
                            EditorGUI.indentLevel++;
                            set.Commands[i].Parameters[j].Name = EditorGUILayout.TextField("Name", set.Commands[i].Parameters[j].Name);
                            set.Commands[i].Parameters[j].ValueType = (ValueType)EditorGUILayout.EnumPopup("Value Type", set.Commands[i].Parameters[j].ValueType);

                            if (set.Commands[i].Parameters[j].ValueType == ValueType.String)
                            {
                                set.Commands[i].Parameters[j].DefaultValue = "";
                                set.Commands[i].Parameters[j].DefaultValue = EditorGUILayout.TextField("Default Value", (string)set.Commands[i].Parameters[j].DefaultValue);
                                ActionParameters.Add(typeof(string));
                            }
                            if (set.Commands[i].Parameters[j].ValueType == ValueType.Int)
                            {
                                set.Commands[i].Parameters[j].DefaultValue = 0;
                                set.Commands[i].Parameters[j].DefaultValue = EditorGUILayout.IntField("Default Value", (int)set.Commands[i].Parameters[j].DefaultValue);
                                ActionParameters.Add(typeof(int));
                            }
                            if (set.Commands[i].Parameters[j].ValueType == ValueType.Bool)
                            {
                                set.Commands[i].Parameters[j].DefaultValue = false;
                                set.Commands[i].Parameters[j].DefaultValue = EditorGUILayout.Toggle("Default Value", (bool)set.Commands[i].Parameters[j].DefaultValue);
                                ActionParameters.Add(typeof(bool));
                            }
                            EditorGUI.indentLevel--;
                        }
                        switch (set.Commands[i].Parameters.Count)
                        {
                            case 1:
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("Commands.Array.data[" + i + "].ActionOnePara"));
                                break;
                            case 2:
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("Commands.Array.data[" + i + "].ActionTwoPara"));
                                break;
                            case 3:
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("Commands.Array.data[" + i + "].ActionThreePara"));
                                break;
                            case 4:
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("Commands.Array.data[" + i + "].ActionFourPara"));
                                break;
                            case >= 5:
                                EditorGUILayout.HelpBox("Using more than 4 parameters in UnityEvents is not supported.", MessageType.Warning);
                                break;
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("Commands.Array.data[" + i + "].ActionVoid"));
                    }

                    EditorGUILayout.HelpBox("Saved Parameters: " + set.Commands[i].Parameters.Count + ".", MessageType.Info);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
