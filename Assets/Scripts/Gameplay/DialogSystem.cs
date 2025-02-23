using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Dialogue
{
    public Sprite face;
    public string sentenceES;
    public string sentenceEN;
    public AudioClip voicelineES;
    public AudioClip voicelineEN;
    public bool VozNoCustom;
    public bool AutoEnter;
    public float SecTillNext = 0f;
    public float TextSpeed = 0.02f;
}
[System.Serializable]
public class Conversation
{
    public Dialogue[] Dialogues;
}
public class DialogSystem : MonoBehaviour
{
    public Conversation Conversation;
    public TextMeshProUGUI text;
    public RawImage faceimage;
    public AudioSource audiosource;
    public Animator animator;
    public PlayerMovement PM;
    private bool DialogDone;
    private bool PressedNextSentence;
    private bool PressedNextWhileType;
    public Animator JoyStickAnimator;
    public Animator ButtonEAnimator;
    private bool EnteredTrigger;
    public bool DialogComplete;
    public bool CameFromCutscene;
    public bool ComesFromInteractE;
    private void Start()
    {
        text = GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>();
        faceimage = GameObject.FindGameObjectWithTag("CharImage").GetComponent<RawImage>();
        text = GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>();
        animator = GameObject.FindGameObjectWithTag("TextBox").GetComponent<Animator>();
        PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (Application.platform == RuntimePlatform.Android || PM.MobileControlsOverride)
        {
            JoyStickAnimator = GameObject.FindGameObjectWithTag("JoyStick").GetComponent<Animator>();
            ButtonEAnimator = GameObject.FindGameObjectWithTag("ButtonE").GetComponent<Animator>();
        }
    }
    public void ResetProp()
    {
        text = GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>();
        faceimage = GameObject.FindGameObjectWithTag("CharImage").GetComponent<RawImage>();
        text = GameObject.FindGameObjectWithTag("DialogText").GetComponent<TextMeshProUGUI>();
        animator = GameObject.FindGameObjectWithTag("TextBox").GetComponent<Animator>();
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
    public void RunConversation()
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
        animator.Play("TextBoxFadeIn");
        PM.BlockInput = true;
        for (int i = 0; i < Conversation.Dialogues.Length; i++)
        {
            DialogDone = false;
            PressedNextSentence = false;
            PressedNextWhileType = false;
            // Español
            if (FileSelection.settings.Language == 0)
            {
                StartCoroutine(Type(Conversation.Dialogues[i].sentenceES, Conversation.Dialogues[i].TextSpeed));
                audiosource.clip = Conversation.Dialogues[i].voicelineES;
            }
            // English
            if (FileSelection.settings.Language == 1)
            {
                StartCoroutine(Type(Conversation.Dialogues[i].sentenceEN, Conversation.Dialogues[i].TextSpeed));
                if (Conversation.Dialogues[i].voicelineEN != null)
                {
                    audiosource.clip = Conversation.Dialogues[i].voicelineEN;
                }
                else
                {
                    audiosource.clip = Conversation.Dialogues[i].voicelineES;
                }
            }
            faceimage.texture = Conversation.Dialogues[i].face.texture;
            audiosource.Play();
            bool vozcustom = false;
            bool hasclip = false;
            if (Conversation.Dialogues[i].VozNoCustom)
            {
                vozcustom = false;
                audiosource.loop = true;
            }
            else
            {
                vozcustom = true;
                audiosource.loop = false;
            }
            if (Conversation.Dialogues[i].voicelineES != null)
            {
                hasclip = true;
            }
            else
            {
                hasclip = false;
            }
            // Español
            if (FileSelection.settings.Language == 0)
            {
                yield return new WaitUntil(() => Continue(Conversation.Dialogues[i].AutoEnter) && text.text == Conversation.Dialogues[i].sentenceES && GetAudioFinished(hasclip, vozcustom));
            }
            // English
            if (FileSelection.settings.Language == 1)
            {
                yield return new WaitUntil(() => Continue(Conversation.Dialogues[i].AutoEnter) && text.text == Conversation.Dialogues[i].sentenceEN && GetAudioFinished(hasclip, vozcustom));
            }
            yield return new WaitForSeconds(Conversation.Dialogues[i].SecTillNext);
            text.text = "";
            if (i == Conversation.Dialogues.Length - 1)
            {
                animator.Play("TextBoxFadeOut");
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
    public void GetNextDialogue(InputAction.CallbackContext context)
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
    public bool Continue(bool Auto)
    {
        if (PressedNextSentence)
        {
            return true;
        }
        else if (Auto)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GetAudioFinished(bool HasClip, bool IsClipCustom)
    {
        if (HasClip)
        {
            if (IsClipCustom)
            {
                if (PressedNextWhileType && DialogDone && PressedNextSentence)
                {
                    audiosource.Stop();
                    return true;
                }
                else
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
            }
            else
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
        }
        else
        {
            return true;
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
[CustomEditor(typeof(DialogSystem))]
public class DialogSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.HelpBox("Make sure to subscribe RunConversation() to the Player's Input Component, otherwise not having the Auto Enter option will make the dialog not work.\nRemember that all dialogs must have a Face Sprite, including 'blank', and this script must have an AudioSource even if nothing is playing.", MessageType.Info);
    }
}
#endif
