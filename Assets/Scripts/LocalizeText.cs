using UnityEngine;
using System;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class LocalizeText : MonoBehaviour
{
    [Header("Text Localizer.")]
    public string SpanishText;
    public string EnglishText;
    [Header("Debug")]
    public bool EnglishPreview;
    [NonSerialized] public string Preview;
    void Start()
    {
        EnglishPreview = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LocalizeText))]
public class LocalizeTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LocalizeText script = (LocalizeText)target;

        string currentlang = "N/A";
        if (FileSelection.settings.Language == 0)
        {
            currentlang = "Spanish";
        }
        if (FileSelection.settings.Language == 1)
        {
            currentlang = "English";
        }
        if (script.EnglishPreview)
        {
            currentlang = "English [DEBUG]";
        }

        EditorGUILayout.LabelField("Current Language: " + currentlang);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("PREVIEW: " + script.Preview);
        EditorGUILayout.Space(5);
        if (GUILayout.Button("Get SpanishText from TextMeshProUGUI"))
        {
            if (script.gameObject.GetComponent<TextMeshProUGUI>() != null)
            {
                script.SpanishText = script.gameObject.GetComponent<TextMeshProUGUI>().text;
            }
            else
            {
                Debug.LogWarning("Couldn't set SpanishText's text because TexMeshProUGUI is missing.");
            }
        }

        // Translation
        if (FileSelection.settings.Language == 0)
        {
            script.Preview = script.SpanishText;
        }
        if (FileSelection.settings.Language == 1)
        {
            script.Preview = script.EnglishText;
        }
        if (script.EnglishPreview)
        {
            script.Preview = script.EnglishText;
        }

        // <lorem>
        if (script.Preview.Contains("<lorem>"))
        {
            script.Preview = script.Preview.Replace("<lorem>", "Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
        }

        // End
        if (script.gameObject.GetComponent<TextMeshProUGUI>() != null)
        {
            script.gameObject.GetComponent<TextMeshProUGUI>().text = script.Preview;
        }
        else
        {
            EditorGUILayout.HelpBox("A TextMeshProUGUI compenent is requiered to localize its text.", MessageType.Warning);
        }
    }
}
#endif
