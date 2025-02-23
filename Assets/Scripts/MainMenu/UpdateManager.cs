using DG.Tweening;
using Newtonsoft.Json;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ContentSection
{
    public string Title;
    public List<string> Points;
}
public struct UpdateInfo
{
    public string Version;
    public string Title;
    public string ReleasedOn;
    public string Size;
    public bool CanAutoInstall;
    public List<ContentSection> Contents;
}

public class UpdateManager : MonoBehaviour
{
    private string InfoURL = "https://drive.google.com/uc?export=download&id=1CKuygHzYWKAd_it289V36eg5wxAPlLRe";
    private string ImageURL = "https://drive.google.com/uc?export=download&id=1am28W38ThF3onlPuek4Fa8ySTQu4Kiot";
    private string WindowsUpdateURL = "https://github.com/Noob-97/tkrpg/archive/refs/heads/Windows.zip";
    private string AndroidUpdateURL = "https://github.com/Noob-97/tkrpg/raw/refs/heads/Android/tkrpg.apk";
    public bool CheckDone;
    public bool LoadingDone;
    public UpdateInfo UpdateInfo;
    public AudioClip updatedclip;
    public AudioClip updatingclip;
    public GameObject DescTitle;
    public GameObject DescPoint;
    public GameObject InstallButton;
    private bool CantQuit;
    [Header("Debug")]
    public bool InstallInEditor;
    private void Start()
    {
        StartCoroutine(GetUpdateInfo());
        if (SceneManager.GetActiveScene().name == "TKRPGUpdater")
        {
            StartCoroutine(WaitTillCheck());
        }
        Application.wantsToQuit += PreventQuit;
    }
    private IEnumerator WaitTillCheck()
    {
        yield return new WaitUntil(() => CheckDone);
        StartCoroutine(GetImage());
        yield return new WaitUntil(() => LoadingDone);
        if (!CompareVersions())
        {
            GameObject.Find("WorkingText").GetComponent<TextMeshProUGUI>().text = "Update Found!";
        }
        else
        {
            GameObject.Find("WorkingText").GetComponent<TextMeshProUGUI>().text = "No Updates Found!";
        }
        GameObject.Find("Music").GetComponent<AudioSource>().clip = updatedclip;
        GameObject.Find("Music").GetComponent<AudioSource>().Play();
        GameObject.Find("linegrid").GetComponent<SpriteRenderer>().DOFade(0.2f, 1f).SetEase(Ease.OutExpo);
    }
    private IEnumerator GetUpdateInfo()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(InfoURL))
            {
                CheckDone = false;
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.LogError("Failed to get UpdateInfo textfile. Result: " + request.result.ToString());
                }
                else if (request.result == UnityWebRequest.Result.Success)
                {
                    UpdateInfo = JsonConvert.DeserializeObject<UpdateInfo>(request.downloadHandler.text);
                }
                CheckDone = true;
            }
        }
        else
        {
            CheckDone = true;
        }
    }
    public bool CompareVersions()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            if (UpdateInfo.Version != Application.version)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    private IEnumerator GetImage()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(ImageURL))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.LogError("Failed to get UpdateImage texture. Result: " + request.result.ToString());
                }
                else if (request.result == UnityWebRequest.Result.Success)
                {
                    RawImage image = GameObject.Find("UpdateImage").GetComponent<RawImage>();
                    image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                }
                LoadUpdateInfo();
            }
        }
    }
    public void LoadUpdateInfo()
    {
        GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>().text = UpdateInfo.Title;
        GameObject.Find("VersionText").GetComponent<TextMeshProUGUI>().text = UpdateInfo.Version;
        GameObject.Find("ReleasedOnText").GetComponent<TextMeshProUGUI>().text = "Released On: " + UpdateInfo.ReleasedOn;
        GameObject.Find("SizeText").GetComponent<TextMeshProUGUI>().text = "Size: " + UpdateInfo.Size;
        GameObject content = GameObject.FindGameObjectWithTag("UpdaterDescContent");
        for (int i = 0; i < UpdateInfo.Contents.Count; i++)
        {
            GameObject title = Instantiate(DescTitle);
            title.transform.SetParent(content.transform);
            title.GetComponentInChildren<TextMeshProUGUI>().text = UpdateInfo.Contents[i].Title;
            title.transform.localScale = Vector3.one;
            for (int x = 0; x < UpdateInfo.Contents[i].Points.Count; x++)
            {
                GameObject point = Instantiate(DescPoint);
                point.transform.SetParent(content.transform);
                point.GetComponent<TextMeshProUGUI>().text = "- " + UpdateInfo.Contents[i].Points[x];
                point.transform.localScale = Vector3.one;
            }
        }
        GameObject button = Instantiate(InstallButton);
        button.transform.SetParent(content.transform);
        button.transform.localScale = Vector3.one;
        LoadingDone = true;
    }
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    public bool PreventQuit()
    {
        if (CantQuit)
        {
            MessageBox(new IntPtr(0), "Can't close game when updates are installing, as it may result to corrupted or broken files.", "TK:RPG Updater", 0);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void EnterDirectory()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            string[] path = StandaloneFileBrowser.OpenFolderPanel("Updated Game Location", "", false);
            if (path.Length == 1 && path[0] != "")
            {
                StartCoroutine(InstallUpdate(path[0]));
                GameObject.Find("InstallButton(Clone)").GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().interactable = false;
                GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().blocksRaycasts = false;
                GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);
            }
        }
        else
        {
            // Update Info: Sustituir tkrpgPP2.0Prev por File Name en Update Info
            string path = Application.persistentDataPath + "/Versions/" + "tkrpgPP2.0Prev.apk";
            StartCoroutine(InstallUpdate(path));
        }
    }

    public IEnumerator InstallUpdate(string path)
    {
        GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(0.5f);
        CanvasGroup updated = GameObject.Find("UpdatedUI").GetComponent<CanvasGroup>();
        updated.interactable = false;
        updated.blocksRaycasts = false;
        updated.DOFade(0, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("Music").GetComponent<AudioSource>().DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        CanvasGroup updating = GameObject.Find("UpdatingUI").GetComponent<CanvasGroup>();
        updating.interactable = true;
        updating.blocksRaycasts = true;
        updating.DOFade(1, 0.5f).SetEase(Ease.OutBack);
        GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
        GameObject.Find("Music").GetComponent<AudioSource>().clip = updatingclip;
        GameObject.Find("Music").GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        CantQuit = true;
        if (Application.platform == RuntimePlatform.WindowsPlayer || InstallInEditor)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(WindowsUpdateURL))
            {
                request.SendWebRequest();
                TextMeshProUGUI percentage = GameObject.Find("UpdatePercentage").GetComponent<TextMeshProUGUI>();
                Image bar = GameObject.Find("UpdateBar").GetComponent<Image>();
                TextMeshProUGUI operations = GameObject.Find("OperationsText").GetComponent<TextMeshProUGUI>();
                CantQuit = true;
                operations.text = "Downloading..";
                while (!request.isDone)
                {
                    percentage.text = Mathf.Floor(request.downloadProgress * 100).ToString() + "%";
                    bar.fillAmount = request.downloadProgress;
                    yield return null;
                }
                bar.fillAmount = 1;
                CantQuit = false;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.LogError("Failed to Update. Result: " + request.result.ToString() + ". Code: " + request.responseCode);
                    operations.text = "Failed to Update. You can safely exit. Code: " + request.responseCode;
                }
                else if (request.result == UnityWebRequest.Result.Success)
                {
                    operations.text = "Writing Files on zip..";
                    File.WriteAllBytes(path + "/TKRPG - " + UpdateInfo.Version + ".zip", request.downloadHandler.data);
                    operations.text = "Extracting zip..";
                    ZipFile.ExtractToDirectory(path + "/TKRPG - " + UpdateInfo.Version + ".zip", path);
                    operations.text = "Closing Internet Connection..";
                    request.Dispose();
                    operations.text = "Done!";
                    GameObject.Find("OpeningGamePopUp").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                    yield return new WaitForSeconds(0.5f);
                    PlayerPrefs.SetString("ComesFromUpdate", "true");
                    Process.Start(path + "/tkrpg-Windows/RPGTrioKeligro.exe");
                    Application.Quit();
                }
            }
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(AndroidUpdateURL))
            {
                request.SendWebRequest();
                TextMeshProUGUI percentage = GameObject.Find("UpdatePercentage").GetComponent<TextMeshProUGUI>();
                Image bar = GameObject.Find("UpdateBar").GetComponent<Image>();
                TextMeshProUGUI operations = GameObject.Find("OperationsText").GetComponent<TextMeshProUGUI>();
                CantQuit = true;
                operations.text = "Downloading..";
                while (!request.isDone)
                {
                    percentage.text = Mathf.Floor(request.downloadProgress * 100).ToString() + "%";
                    bar.fillAmount = request.downloadProgress;
                    yield return null;
                }
                bar.fillAmount = 1;
                CantQuit = false;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.LogError("Failed to Update. Result: " + request.result.ToString() + ". Code: " + request.responseCode);
                    operations.text = "Failed to Update. You can safely exit. Code: " + request.responseCode;
                }
                else if (request.result == UnityWebRequest.Result.Success)
                {
                    operations.text = "Writing APK..";
                    if (!Directory.Exists(Application.persistentDataPath + "/Versions"))
                    {
                        Directory.CreateDirectory(Application.persistentDataPath + "/Versions");
                    }
                    File.WriteAllBytes(path, request.downloadHandler.data);
                    operations.text = "Closing Internet Connection..";
                    request.Dispose();
                    operations.text = "Done! Permission Required to Install.";
                    GameObject.Find("OpeningGamePopUp").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                    yield return new WaitForSeconds(0.5f);
                    PlayerPrefs.SetString("ComesFromUpdate", "true");
                    // Open APK File
                    OpenApk(path);

                    Application.Quit();
                }
            }
        }
    }
    public void OpenApk(string filePath)
    {
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogError("El archivo APK no existe: " + filePath);
            return;
        }

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        intent.Call<AndroidJavaObject>("setAction", "android.intent.action.VIEW");

        // Obtener URI segura con FileProvider yes
        string authority = context.Call<string>("getPackageName") + ".fileprovider";
        AndroidJavaClass fileProvider = new AndroidJavaClass("androidx.core.content.FileProvider");
        AndroidJavaObject file = new AndroidJavaObject("java.io.File", filePath);
        AndroidJavaObject uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", context, authority, file);

        // Configurar Intent con permisos
        intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/vnd.android.package-archive");
        intent.Call<AndroidJavaObject>("addFlags", 1); // FLAG_GRANT_READ_URI_PERMISSION
        intent.Call<AndroidJavaObject>("addFlags", 268435456); // FLAG_ACTIVITY_NEW_TASK

        // Iniciar la instalación del APK
        currentActivity.Call("startActivity", intent);
    }
}

