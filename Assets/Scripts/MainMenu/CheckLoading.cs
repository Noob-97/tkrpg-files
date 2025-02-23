using DG.Tweening;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckLoading : MonoBehaviour
{
    public TextMeshProUGUI RefText;
    public Animator TransitionPanel;
    public float TextTimes;
    public bool LaunchFromUpdaterChecked;
    public bool PlatformChecked;
    public bool InternetChecked;
    public bool UpdatesChecked;
    public BinaryFormatter Formatter = new BinaryFormatter();
    public NotUpdatedResult UpdateOption;
    public enum NotUpdatedResult
    {
        Undefined,
        Update,
        NotUpdate
    }
    private void Start()
    {
        StartCoroutine(CheckLoad());
    }
    public void CheckPlatform()
    {
        PlatformChecked = true;
    }
    public void CheckInternet()
    {
        InternetChecked = true;
    }
    public void CheckLaunchUpdater()
    {
        LaunchFromUpdaterChecked = true;
    }
    public void CheckUpdates(string Option)
    {
        UpdatesChecked = true;
        UpdateOption = Enum.Parse<NotUpdatedResult>(Option);
    }
    IEnumerator CheckLoad()
    {
        // Check for First Launch from TKRPG Updater
        if (PlayerPrefs.HasKey("ComesFromUpdate"))
        {
            if (PlayerPrefs.GetString("ComesFromUpdate") == "true")
            {
                OpenPopup("SuccessInstallPopup");
                yield return new WaitUntil(() => LaunchFromUpdaterChecked);
                ClosePopup("SuccessInstallPopup");
            }
        }
        else
        {
            PlayerPrefs.SetString("ComesFromUpdate", "false");
        }
        // Check for Save Files
        if (FileSelection.settings == null)
        {
            FileSelection.settings = new GameSettings();
        }
        if (File.Exists(Application.persistentDataPath + "/TKRPGSettings.tkrpgs"))
        {
            FileStream Settings = new FileStream(Application.persistentDataPath + "/TKRPGSettings.tkrpgs", FileMode.Open);
            GameSettings GS = Formatter.Deserialize(Settings) as GameSettings;
            Settings.Close();
            switch (GS.Language)
            {
                case 0:
                    FileSelection.settings.Language = 0;
                    break;
                case 1:
                    FileSelection.settings.Language = 1;
                    break;
            }
        }
        else
        {
            FileSelection.settings = new GameSettings();
            if (Application.systemLanguage == SystemLanguage.Spanish)
            {
                FileSelection.settings.Language = 0;
            }
            else
            {
                FileSelection.settings.Language = 1;
            }
        }

        GenerateText("Idioma Aplicado", "Language Applied");

        yield return new WaitForSeconds(TextTimes);

        // Check For Platform
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            OpenPopup("HTMLPopup");
            yield return new WaitUntil(() => PlatformChecked);
            ClosePopup("HTMLPopup");
        }
        PlatformChecked = true;
        GenerateText("Plataforma Comprobada", "Platform Checked");
        yield return new WaitForSeconds(TextTimes);

        // Check For Internet
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OpenPopup("InternetPopup");
                yield return new WaitUntil(() => InternetChecked);
                ClosePopup("InternetPopup");
            }
            InternetChecked = true;
            GenerateText("Comprabado Internet", "Internet Checked");
            yield return new WaitForSeconds(TextTimes);
        }

        // Check For Updates 
        if (Application.platform != RuntimePlatform.WebGLPlayer && Application.internetReachability != NetworkReachability.NotReachable)
        {
            UpdateManager updatemanager = GameObject.Find("UpdateManager").GetComponent<UpdateManager>();
            GenerateText("Comprobando Actualizaciones...", "Checking For Updates...");
            yield return new WaitForSeconds(TextTimes);
            yield return new WaitUntil(() => updatemanager.CheckDone);
            if (updatemanager.CompareVersions() == false)
            {
                OpenPopup("UpdatePopup");
                yield return new WaitUntil(() => UpdatesChecked);
                ClosePopup("UpdatePopup");
                if (UpdateOption == NotUpdatedResult.Update)
                {
                    TransitionPanel.SetTrigger("IsLeaving");
                    yield return new WaitForSeconds(0.5f);
                    SceneManager.LoadScene("TKRPGUpdater");
                }
            }
            GenerateText("Actualizaciones Comprobadas", "Updates Checked");
            yield return new WaitForSeconds(TextTimes);
        }
        GenerateText("Hecho!", "Done!");
        yield return new WaitForSeconds(TextTimes);
        TransitionPanel.SetTrigger("IsLeaving");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("FileSelection");
    }
    public void GenerateText(string estext,string entext)
    {
        TextMeshProUGUI generated = Instantiate(RefText, GameObject.Find("TextStack").transform);
        generated.gameObject.AddComponent<LocalizeText>();
        generated.text = estext;
        generated.GetComponent<LocalizeText>().SpanishText = estext;
        generated.GetComponent<LocalizeText>().EnglishText = entext;
    }
    public void OpenPopup(string PopupName)
    {
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.Linear);
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().interactable = true;
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    public void ClosePopup(string PopupName)
    {
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.Linear);
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
