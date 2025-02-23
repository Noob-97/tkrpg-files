using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Acotation
{
    public string acotationES;
    public string acotationEN;
}

public class AcotationSystem : MonoBehaviour
{
    public Acotation[] Acotations;
    public TextMeshProUGUI text;
    public AudioSource audiosource;
    public PlayerMovement PM;
    public float TextSpeed = 0.02f;
    public AudioClip AudioClip;
    private bool DialogDone;
    private bool PressedNextSentence;
    private bool PressedNextWhileType;
    public Animator JoyStickAnimator;
    public Animator ButtonEAnimator;
    private bool EnteredTrigger;
    public bool DialogComplete;
    public bool CameFromCutscene;
    public bool ComesFromInteractE;
    public bool FadeText;
    private void Start()
    {
        PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (Application.platform == RuntimePlatform.Android || PM.MobileControlsOverride)
        {
            JoyStickAnimator = GameObject.FindGameObjectWithTag("JoyStick").GetComponent<Animator>();
            ButtonEAnimator = GameObject.FindGameObjectWithTag("ButtonE").GetComponent<Animator>();
        }
    }
    public void ResetProp()
    {
        PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (Application.platform == RuntimePlatform.Android || PM.MobileControlsOverride)
        {
            JoyStickAnimator = GameObject.FindGameObjectWithTag("JoyStick").GetComponent<Animator>();
            ButtonEAnimator = GameObject.FindGameObjectWithTag("ButtonE").GetComponent<Animator>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnteredTrigger = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        EnteredTrigger = false;
    }
    public void RunAcotations()
    {
        ResetProp();
        if (EnteredTrigger)
        {
            StartCoroutine(WaitForEnter());
            DialogComplete = false;
        }
    }
    public IEnumerator WaitForEnter()
    {
        //Debug.Log(TEST.name);
        text.DOFade(1, 0.5f);
        PM.BlockInput = true;
        for (int i = 0; i < Acotations.Length; i++)
        {
            DialogDone = false;
            PressedNextSentence = false;
            PressedNextWhileType = false;
            // Español
            if (FileSelection.settings.Language == 0)
            {
                StartCoroutine(Type(Acotations[i].acotationES, TextSpeed));
            }
            // English
            if (FileSelection.settings.Language == 1)
            {
                StartCoroutine(Type(Acotations[i].acotationEN, TextSpeed));
            }
            audiosource.clip = AudioClip;
            audiosource.Play();
            audiosource.loop = true;
            // Español
            if (FileSelection.settings.Language == 0)
            {
                yield return new WaitUntil(() => Continue() && text.text == Acotations[i].acotationES && GetAudioFinished());
            }
            // English
            if (FileSelection.settings.Language == 1)
            {
                yield return new WaitUntil(() => Continue() && text.text == Acotations[i].acotationEN && GetAudioFinished());
            }
            text.text = "";
            if (i == Acotations.Length - 1)
            {
                if (FadeText)
                {
                    text.DOFade(0, 0.5f);
                }
                PM.BlockInput = false;
                if (Application.platform == RuntimePlatform.Android || PM.MobileControlsOverride)
                {
                    if (!CameFromCutscene)
                    {
                        JoyStickAnimator.Play("FadeIn");
                        ButtonEAnimator.Play("FadeIn");
                    }
                }
                DialogComplete = true;
                CameFromCutscene = false;
                if (ComesFromInteractE)
                {
                    PM.ReenableInteractE();
                }
            }
        }
    }
    public void GetNextAcotation(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (DialogDone)
            {
                PressedNextSentence = true;
            }
            if (!DialogDone)
            {
                PressedNextWhileType = true;
            }
        }
    }
    public bool Continue()
    {
        if (PressedNextSentence)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GetAudioFinished()
    {
        if (!audiosource.isPlaying)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public IEnumerator Type(string sentence, float WriteSpeed)
    {
        foreach (char letter in sentence.ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSeconds(WriteSpeed);
            if (text.text == sentence)
            {
                DialogDone = true;
                audiosource.loop = false;
            }
            else
            {
                DialogDone = false;
            }
            if (PressedNextWhileType)
            {
                WriteSpeed = 0.0025f;
            }
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(AcotationSystem))]
public class AcotationSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.HelpBox("Make sure to subscribe RunAcotations() to the Player's Input Component, otherwise the dialog won't work.\nIt's not nessesary to assign the PM and Mobile Controls fields.", MessageType.Info);
    }
}
#endif
