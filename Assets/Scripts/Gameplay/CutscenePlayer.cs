using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class MoveCamera
{
    [SerializeField] public Vector2 MoveToPos;
    [SerializeField] public bool UseLocalTransform;
    [SerializeField] public float Time;
}
[System.Serializable]
public class StartDialog
{
    [SerializeField] public DialogSystem Dialog;
}
[Serializable]
public class ChangeOrientation
{
    [SerializeField] public FacingDirection Orientation;
    [SerializeField] public bool UseNPCBrain;
    [SerializeField] public NPCBrain NPCBrain;
}
[Serializable]
public class Wait
{
    [SerializeField] public float Time;
}
[Serializable]
public class Rotate
{
    [SerializeField] public GameObject Target;
    [SerializeField] public float Angle;
    [SerializeField] public float Time;
    [SerializeField] public Ease Ease;
}
[Serializable]
public class RawMove
{
    [SerializeField] public GameObject Target;
    [SerializeField] public bool Local;
    [SerializeField] public Vector3 Position;
    [SerializeField] public float Time;
    [SerializeField] public Ease Ease;
}
[Serializable]
public class StartBattle
{
    [SerializeField] public BattleData BattleData;
}
[Serializable]
public class PlayMovements
{
    [SerializeField] public NPCBrain NPCBrain;
}
[Serializable]
public class CutsceneElement
{
    [SerializeField] public enum CutsceneElementList
    {
        MoveCamera,
        StartDialog,
        ChangeOrientation,
        Wait,
        PlayMovements,
        StartBattle,
        Rotate,
        RawMove
    }
    public CutsceneElementList ElementType;
    public MoveCamera MoveCamera;
    public StartDialog StartDialog;
    public ChangeOrientation ChangeOrientation;
    public Wait Wait;
    public PlayMovements PlayMovements;
    public StartBattle StartBattle;
    public Rotate Rotate;
    public RawMove RawMove;
}
public class CutscenePlayer : MonoBehaviour
{
    // Properties
    [SerializeField] public GameObject CameraFollower;
    public PlayerMovement PlayerMovement;
    public bool FireOnStart;
    public bool Loop;
    [SerializeField] public List<CutsceneElement> Elements = new List<CutsceneElement>();
    public static BattleData CurrentBattleData;
    public static string CameFrom;
    public bool TriggerOn;
    // CustomEditor Properties
    public int listsize;
    public List<int> elementschosen = new List<int>();

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            TriggerOn = true;
            CameFrom = SceneManager.GetActiveScene().name;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            TriggerOn = false;
        }
    }
    public void PlayCutscene()
    {
        if (TriggerOn)
        {
            StartCoroutine(Cutscene());
        }
    }
    private IEnumerator Cutscene()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            PlayerMovement.JoyStickAnimator.Play("FadeOut");
            PlayerMovement.ButtonEAnimator.Play("FadeOut");
        }
        if (PlayerMovement != null)
        {
            PlayerMovement.BlockInput = true;
            if (PlayerMovement.MobileControlsOverride)
            {
                PlayerMovement.JoyStickAnimator.Play("FadeOut");
                PlayerMovement.ButtonEAnimator.Play("FadeOut");
            }
        }
        for (int i = 0; i < Elements.Count; i++)
        {
            if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.ChangeOrientation)
            {
                if (!Elements[i].ChangeOrientation.UseNPCBrain)
                {
                    PlayerMovement.ChangeAnimation(Elements[i].ChangeOrientation.Orientation.ToString());
                }
                if (Elements[i].ChangeOrientation.UseNPCBrain)
                {
                    Elements[i].ChangeOrientation.NPCBrain.ChangeAnimation(Elements[i].ChangeOrientation.Orientation.ToString());
                }
                yield return new WaitForEndOfFrame();
            }
            else if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.StartDialog)
            {
                Elements[i].StartDialog.Dialog.DialogComplete = false;
                Elements[i].StartDialog.Dialog.CameFromCutscene = true;
                StartCoroutine(Elements[i].StartDialog.Dialog.WaitForEnter());
                yield return new WaitUntil(() => Elements[i].StartDialog.Dialog.DialogComplete);
            }
            else if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.MoveCamera)
            {
                if (Elements[i].MoveCamera.Time > 0)
                {
                    if (Elements[i].MoveCamera.UseLocalTransform)
                    {
                        float velx = (Elements[i].MoveCamera.MoveToPos.x - CameraFollower.gameObject.transform.localPosition.x) / Elements[i].MoveCamera.Time;
                        float vely = (Elements[i].MoveCamera.MoveToPos.y - CameraFollower.gameObject.transform.localPosition.y) / Elements[i].MoveCamera.Time;
                        float FPS = 1 / Time.unscaledDeltaTime;
                        float FPSvelx = velx * (1 / FPS);
                        float FPSvely = vely * (1 / FPS);
                        yield return new WaitUntil(() => MoveCameraToVel(i, true, FPSvelx, FPSvely));
                    }
                    else
                    {
                        float velx = (Elements[i].MoveCamera.MoveToPos.x - (CameraFollower.gameObject.transform.position.x + CameraFollower.gameObject.transform.localPosition.x)) / Elements[i].MoveCamera.Time;
                        float vely = (Elements[i].MoveCamera.MoveToPos.y - (CameraFollower.gameObject.transform.position.y + CameraFollower.gameObject.transform.localPosition.y)) / Elements[i].MoveCamera.Time;
                        float FPS = 1 / Time.unscaledDeltaTime;
                        float FPSvelx = velx * (1 / FPS);
                        float FPSvely = vely * (1 / FPS);
                        yield return new WaitUntil(() => MoveCameraToVel(i, false, FPSvelx, FPSvely));
                    }
                }
                else
                {
                    if (Elements[i].MoveCamera.UseLocalTransform)
                    {
                        CameraFollower.gameObject.transform.localPosition = Elements[i].MoveCamera.MoveToPos;
                    }
                    else
                    {
                        CameraFollower.gameObject.transform.position = Elements[i].MoveCamera.MoveToPos;
                    }
                }
            }
            else if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.Wait)
            {
                yield return new WaitForSeconds(Elements[i].Wait.Time);
            }
            else if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.PlayMovements)
            {
                StartCoroutine(Elements[i].PlayMovements.NPCBrain.StartMovement(0));
                yield return new WaitUntil(() => Elements[i].PlayMovements.NPCBrain.MovementsDone);
            }
            else if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.StartBattle)
            {
                GameObject panel = GameObject.FindGameObjectWithTag("Panel");
                panel.GetComponent<Animator>().SetTrigger("IsLeaving");
                yield return new WaitForSeconds(0.5f);
                CurrentBattleData = Elements[i].StartBattle.BattleData;
                SceneManager.LoadScene("BattleScene");
            }
            else if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.Rotate)
            {
                Vector3 TargetAngle = new Vector3(0, 0, Elements[i].Rotate.Angle);
                Elements[i].Rotate.Target.transform.DOLocalRotate(TargetAngle, Elements[i].Rotate.Time).SetEase(Elements[i].Rotate.Ease);
                yield return new WaitForSeconds(Elements[i].Rotate.Time);
            }
            else if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.RawMove)
            {
                if (Elements[i].RawMove.Local)
                {
                    Elements[i].RawMove.Target.transform.DOLocalMove(Elements[i].RawMove.Position, Elements[i].RawMove.Time).SetEase(Elements[i].RawMove.Ease);
                }
                else
                {
                    Elements[i].RawMove.Target.transform.DOMove(Elements[i].RawMove.Position, Elements[i].RawMove.Time).SetEase(Elements[i].RawMove.Ease);
                }
                yield return new WaitForSeconds(Elements[i].RawMove.Time);
            }
        }
        if (Loop)
        {
            StartCoroutine(Cutscene());
        }
        else
        {
            if (PlayerMovement != null)
            {
                if (PlayerMovement.MobileControlsOverride)
                {
                    PlayerMovement.JoyStickAnimator.Play("FadeIn");
                    PlayerMovement.ButtonEAnimator.Play("FadeIn");
                }
                PlayerMovement.BlockInput = false;
            }
            if (Application.platform == RuntimePlatform.Android)
            {
                PlayerMovement.JoyStickAnimator.Play("FadeIn");
                PlayerMovement.ButtonEAnimator.Play("FadeIn");
            }
        }
    }
    public bool MoveCameraToVel(int i, bool local, float FPSvelx, float FPSvely)
    {
        if (local)
        {
            CameraFollower.gameObject.transform.localPosition = new Vector2(CameraFollower.gameObject.transform.localPosition.x + FPSvelx, CameraFollower.gameObject.transform.localPosition.y + FPSvely);
            if (Mathf.Abs(CameraFollower.gameObject.transform.localPosition.x - Elements[i].MoveCamera.MoveToPos.x) <= 0.2 && Mathf.Abs(CameraFollower.gameObject.transform.localPosition.x - Elements[i].MoveCamera.MoveToPos.x) >= 0)
            {
                if (CameraFollower.gameObject.transform.localPosition.y - Elements[i].MoveCamera.MoveToPos.y <= 0.2 && CameraFollower.gameObject.transform.localPosition.y - Elements[i].MoveCamera.MoveToPos.y >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            CameraFollower.gameObject.transform.position = new Vector2(CameraFollower.gameObject.transform.position.x + FPSvelx, CameraFollower.gameObject.transform.position.y + FPSvely);
            if (Mathf.Abs(CameraFollower.gameObject.transform.position.x - Elements[i].MoveCamera.MoveToPos.x) <= 0.2 && Mathf.Abs(CameraFollower.gameObject.transform.position.x - Elements[i].MoveCamera.MoveToPos.x) >= 0)
            {
                if (Mathf.Abs(CameraFollower.gameObject.transform.position.y - Elements[i].MoveCamera.MoveToPos.y) <= 0.2 && Mathf.Abs(CameraFollower.gameObject.transform.position.y - Elements[i].MoveCamera.MoveToPos.y) >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
    public void Start()
    {
        if (FireOnStart)
        {
            StartCoroutine(Cutscene());
        }
        for (int i = 0; i < Elements.Count; i++)
        {
            if (Elements[i].ElementType == CutsceneElement.CutsceneElementList.MoveCamera && Elements[i].MoveCamera.Time == 0 && Elements[i].MoveCamera.MoveToPos == Vector2.zero && Elements[i].MoveCamera.UseLocalTransform == false)
            {
                Elements.RemoveAt(i);
                i--;
            }
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(CutscenePlayer))]
public class CutscenePlayerEditor : Editor
{
    List<bool> elementsopen = new List<bool>();
    List<int> orientationchosen = new List<int>();
    bool movecameras;
    int characterchosen;
    public override void OnInspectorGUI()
    {
        CutscenePlayer cutsceneplayer = (CutscenePlayer)target;
        serializedObject.Update();
        if (movecameras)
        {
            cutsceneplayer.CameraFollower = EditorGUILayout.ObjectField("Camera Follower", cutsceneplayer.CameraFollower, typeof(GameObject), true) as GameObject;
        }
        cutsceneplayer.PlayerMovement = EditorGUILayout.ObjectField("PlayerMovement", cutsceneplayer.PlayerMovement, typeof(PlayerMovement), true) as PlayerMovement;
        cutsceneplayer.FireOnStart = EditorGUILayout.Toggle("Fire On Start", cutsceneplayer.FireOnStart);
        cutsceneplayer.Loop = EditorGUILayout.Toggle("Loop Cutscene", cutsceneplayer.Loop);
        cutsceneplayer.listsize = EditorGUILayout.IntField("Nº of Elements", cutsceneplayer.listsize);
        if (cutsceneplayer.listsize > 0)
        {
            for (int i = 0; i < cutsceneplayer.listsize; ++i)
            {
                // elementsopen.Count = i + 1
                if (elementsopen.Count < i + 1)
                {
                    elementsopen.Add(false);
                    cutsceneplayer.elementschosen.Add(0);
                    cutsceneplayer.Elements.Add(new CutsceneElement());
                    orientationchosen.Add(0);
                }
                elementsopen[i] = EditorGUILayout.BeginFoldoutHeaderGroup(elementsopen[i], "Element " + (i + 1));
                if (elementsopen[i])
                {
                    cutsceneplayer.elementschosen[i] = EditorGUILayout.Popup("Type", cutsceneplayer.elementschosen[i], new string[8] { "MoveCamera", "StartDialog", "ChangeOrientation", "Wait", "PlayMovements", "StartBattle", "Rotate", "Raw Move"});
                    if (cutsceneplayer.elementschosen[i] == 0)
                    {
                        cutsceneplayer.Elements[i].ElementType = CutsceneElement.CutsceneElementList.MoveCamera;
                        cutsceneplayer.Elements[i].MoveCamera.UseLocalTransform = EditorGUILayout.Toggle("Use Local Transfrom", cutsceneplayer.Elements[i].MoveCamera.UseLocalTransform);
                        cutsceneplayer.Elements[i].MoveCamera.MoveToPos = EditorGUILayout.Vector2Field("Move To Pos", cutsceneplayer.Elements[i].MoveCamera.MoveToPos);
                        cutsceneplayer.Elements[i].MoveCamera.Time = EditorGUILayout.FloatField("Time", cutsceneplayer.Elements[i].MoveCamera.Time);
                        movecameras = true;
                    }
                    else if (cutsceneplayer.elementschosen[i] == 1)
                    {
                        cutsceneplayer.Elements[i].ElementType = CutsceneElement.CutsceneElementList.StartDialog;
                        cutsceneplayer.Elements[i].StartDialog.Dialog = EditorGUILayout.ObjectField("Dialog", cutsceneplayer.Elements[i].StartDialog.Dialog, typeof(DialogSystem), true) as DialogSystem;
                    }
                    else if (cutsceneplayer.elementschosen[i] == 2)
                    {
                        cutsceneplayer.Elements[i].ElementType = CutsceneElement.CutsceneElementList.ChangeOrientation;
                        orientationchosen[i] = EditorGUILayout.Popup("Orientation", orientationchosen[i], new string[4] { "UpIdle", "DownIdle", "LeftIdle", "RightIdle" });
                        if (orientationchosen[i] == 0)
                        {
                            cutsceneplayer.Elements[i].ChangeOrientation.Orientation = FacingDirection.UpIdle;
                        }
                        else if (orientationchosen[i] == 1)
                        {
                            cutsceneplayer.Elements[i].ChangeOrientation.Orientation = FacingDirection.DownIdle;
                        }
                        else if (orientationchosen[i] == 2)
                        {
                            cutsceneplayer.Elements[i].ChangeOrientation.Orientation = FacingDirection.LeftIdle;
                        }
                        else if (orientationchosen[i] == 3)
                        {
                            cutsceneplayer.Elements[i].ChangeOrientation.Orientation = FacingDirection.RightIdle;
                        }
                        cutsceneplayer.Elements[i].ChangeOrientation.UseNPCBrain = EditorGUILayout.Toggle("Use NPCBrain", cutsceneplayer.Elements[i].ChangeOrientation.UseNPCBrain);
                        if (cutsceneplayer.Elements[i].ChangeOrientation.UseNPCBrain) {
                            cutsceneplayer.Elements[i].ChangeOrientation.NPCBrain = EditorGUILayout.ObjectField("NPCBrain", cutsceneplayer.Elements[i].ChangeOrientation.NPCBrain, typeof(NPCBrain), true) as NPCBrain;
                        }
                    }
                    else if (cutsceneplayer.elementschosen[i] == 3)
                    {
                        cutsceneplayer.Elements[i].ElementType = CutsceneElement.CutsceneElementList.Wait;
                        cutsceneplayer.Elements[i].Wait.Time = EditorGUILayout.FloatField("Time", cutsceneplayer.Elements[i].Wait.Time);
                    }
                    else if (cutsceneplayer.elementschosen[i] == 4)
                    {
                        cutsceneplayer.Elements[i].ElementType = CutsceneElement.CutsceneElementList.PlayMovements;
                        cutsceneplayer.Elements[i].PlayMovements.NPCBrain = EditorGUILayout.ObjectField("NPCBrain (Movements)", cutsceneplayer.Elements[i].PlayMovements.NPCBrain, typeof(NPCBrain), true) as NPCBrain;
                        EditorGUILayout.Space(2);
                        EditorGUILayout.LabelField("NOTE: This NPCBrain has nothing to do with the NPCBrain that might appear on the\ntop. This element needs a unique NPCBrain that determines the movements the\nsubject has to do, while the top one is used for ChangeOrientation.", GUILayout.MaxHeight(45));
                    }
                    else if (cutsceneplayer.elementschosen[i] == 5)
                    {
                        cutsceneplayer.Elements[i].ElementType = CutsceneElement.CutsceneElementList.StartBattle;
                        cutsceneplayer.Elements[i].StartBattle.BattleData = EditorGUILayout.ObjectField("Battle Data", cutsceneplayer.Elements[i].StartBattle.BattleData, typeof(BattleData), true) as BattleData;
                    }
                    else if (cutsceneplayer.elementschosen[i] == 6)
                    {
                        cutsceneplayer.Elements[i].ElementType = CutsceneElement.CutsceneElementList.Rotate;
                        cutsceneplayer.Elements[i].Rotate.Target = EditorGUILayout.ObjectField("Target Object", cutsceneplayer.Elements[i].Rotate.Target, typeof(GameObject), true) as GameObject;
                        cutsceneplayer.Elements[i].Rotate.Angle = EditorGUILayout.FloatField("Angle", cutsceneplayer.Elements[i].Rotate.Angle);
                        cutsceneplayer.Elements[i].Rotate.Time = EditorGUILayout.FloatField("Time", cutsceneplayer.Elements[i].Rotate.Time);
                        cutsceneplayer.Elements[i].Rotate.Ease = (Ease)EditorGUILayout.EnumPopup("Ease", cutsceneplayer.Elements[i].Rotate.Ease);
                    }
                    else if (cutsceneplayer.elementschosen[i] == 7)
                    {
                        cutsceneplayer.Elements[i].ElementType = CutsceneElement.CutsceneElementList.RawMove;
                        cutsceneplayer.Elements[i].RawMove.Target = EditorGUILayout.ObjectField("Target Object", cutsceneplayer.Elements[i].RawMove.Target, typeof(GameObject), true) as GameObject;
                        cutsceneplayer.Elements[i].RawMove.Local = EditorGUILayout.Toggle("Use Local Transform", cutsceneplayer.Elements[i].RawMove.Local);
                        cutsceneplayer.Elements[i].RawMove.Position = EditorGUILayout.Vector3Field("Position", cutsceneplayer.Elements[i].RawMove.Position);
                        cutsceneplayer.Elements[i].RawMove.Time = EditorGUILayout.FloatField("Time", cutsceneplayer.Elements[i].RawMove.Time);
                        cutsceneplayer.Elements[i].RawMove.Ease = (Ease)EditorGUILayout.EnumPopup("Ease", cutsceneplayer.Elements[i].RawMove.Ease);
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.HelpBox("Saved Elements: " + cutsceneplayer.Elements.Count + ".\nIn case of an issue, try clearing the list by setting 'Elements' to 0 or by pressing this button.", MessageType.Info);
            if (GUILayout.Button("Clear Unnecesary Elements"))
            {
                for (int i = 0; i < cutsceneplayer.Elements.Count; i++)
                {
                    if (cutsceneplayer.Elements[i].ElementType == CutsceneElement.CutsceneElementList.MoveCamera && cutsceneplayer.Elements[i].MoveCamera.Time == 0 && cutsceneplayer.Elements[i].MoveCamera.MoveToPos == Vector2.zero && cutsceneplayer.Elements[i].MoveCamera.UseLocalTransform == false)
                    {
                        cutsceneplayer.Elements.RemoveAt(i);
                        i--;
                    }
                }
            }
            int count1 = 0;
            for (int i = 0; i < cutsceneplayer.Elements.Count; i++)
            {
                if (cutsceneplayer.Elements[i].ElementType == CutsceneElement.CutsceneElementList.MoveCamera)
                {
                    count1++;
                }
                if (i + 1 !< cutsceneplayer.Elements.Count)
                {
                    if (count1 != 0)
                    {
                        movecameras = true;
                    }
                    else
                    {
                        movecameras = false;
                    }
                    count1 = 0;
                }
            }
        }
        else
        {
            movecameras = false;
            elementsopen.Clear();
            cutsceneplayer.elementschosen.Clear();
            cutsceneplayer.Elements.Clear();
            orientationchosen.Clear();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif