using DG.Tweening;
using SFB;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class SaveFile
{
    public string SaveFileName = "New Save File";
    public string CurrentChapter = "Ninguno";
    public string CurrentScene = "Ninguna";
    public byte[] CustomFileImage;
    public float[] PlayerPos = new float[3];
    public int PlayTime = 0;
}
[Serializable]
public class GameSettings
{
    public int ResolutionX = 950;
    public int ResolutionY = 800;
    public bool Fullscreen = false;
    public float Volume = 0;
    public int Language = -1;
}

public class FileSelection : MonoBehaviour
{
    public static SaveFile savefile;
    public static GameSettings settings;
    public TextMeshProUGUI savefilename;
    public TextMeshProUGUI chapter;
    public TextMeshProUGUI scene;
    public RawImage savefileimage;
    public Texture2D ChapterNone;
    public Texture2D ChapterPrototype;
    public Button nextbuttonfile;
    public Button custombuttonfile;
    public TMP_Dropdown langdrop;
    public AudioMixer mixer;
    public TMP_InputField custominputfield;
    public Button savebuttoncustom;
    public Button exportbutton;
    public RawImage customsavefileimage;
    public GameObject savefileview;
    public Button backbuttonselect;
    public Button deletebutton;
    public Button HTMLExport;
    public Button HTMLCustomize;
    public Button HTMLNext;
    public BinaryFormatter Formatter = new BinaryFormatter();
    private NativeFilePicker.Permission GeneralPermission;
    [Header("Debug")]
    public bool OverrideHTML;
    public bool OverrideAndroidFileSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // File Generation
        if (Application.platform != RuntimePlatform.WebGLPlayer || !OverrideHTML)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/SaveFiles"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/SaveFiles");
            }
            if (!File.Exists(Application.persistentDataPath + "/TKRPGSettings.tkrpgs"))
            {
                FileStream Settings = new FileStream(Application.persistentDataPath + "/TKRPGSettings.tkrpgs", FileMode.Create);
                GameSettings GS = new GameSettings();
                Formatter.Serialize(Settings, GS);
                Settings.Close();
            }
            if (!PlayerPrefs.HasKey("LastSaveIndex"))
            {
                PlayerPrefs.SetInt("LastSaveIndex", -1);
            }
            FileStream OPEN = new FileStream(Application.persistentDataPath + "/TKRPGSettings.tkrpgs", FileMode.Open);
            settings = Formatter.Deserialize(OPEN) as GameSettings;
            OPEN.Close();
            SetSettings();
            CheckLang();
            LoadSaveFiles();
        }
        else
        {
            CanvasGroup HTML = GameObject.Find("HTMLScreen").GetComponent<CanvasGroup>();
            HTML.alpha = 1;
            HTML.interactable = true;
            HTML.blocksRaycasts = true;
        }
        string path = Application.persistentDataPath + "/SaveFiles/SaveFile" + PlayerPrefs.GetInt("LastSaveIndex").ToString() + ".tkrpg";
        if (!File.Exists(path))
        {
            nextbuttonfile.interactable = false;
            custombuttonfile.interactable = false;
            exportbutton.interactable = false;
            backbuttonselect.interactable = false;
            deletebutton.interactable = false;
            HTMLCustomize.interactable = false;
            HTMLExport.interactable = false;
            HTMLNext.interactable = false;
        }
        else
        {
            FileStream Save = new FileStream(path, FileMode.Open);
            savefile = Formatter.Deserialize(Save) as SaveFile;
            Save.Close();
            ReadGameFile();
        }
    }
    public void CreateBlankSave()
    {
        CreateSaveFile(null);
    }
    public void CreateBlankSaveHTML()
    {
        CreateSaveFileHTML(null);
    }
    public void ImportGameFile()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            string[] path = StandaloneFileBrowser.OpenFilePanel("Importar Archivo de Guardado", "", "tkrpg", false);
            if (path.Length == 1)
            {
                FileStream Stream = new FileStream(path[0], FileMode.Open);
                SaveFile Save = Formatter.Deserialize(Stream) as SaveFile;
                Stream.Close();
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    CreateSaveFileHTML(Save);
                }
                else
                {

                    CreateSaveFile(Save);
                }
                backbuttonselect.interactable = true;
                GameObject.Find("ImportPopup").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.Linear);
                GameObject.Find("ImportPopup").GetComponent<CanvasGroup>().interactable = true;
                GameObject.Find("ImportPopup").GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else
        {
            if (GeneralPermission == NativeFilePicker.Permission.Granted)
            {
                Debug.Log("Permission Granted");
                NativeFilePicker.PickFile((path) =>
                {
                    if (path == null)
                    {
                        Debug.Log("Pick Import File: Canceled");
                    }
                    else
                    {
                        Debug.Log("Pick Import File: Success - " + path);
                        FileStream Stream = new FileStream(path, FileMode.Open);
                        SaveFile Save = Formatter.Deserialize(Stream) as SaveFile;
                        Stream.Close();
                        CreateSaveFile(Save);
                        backbuttonselect.interactable = true;
                        GameObject.Find("ImportPopup").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.Linear);
                        GameObject.Find("ImportPopup").GetComponent<CanvasGroup>().interactable = true;
                        GameObject.Find("ImportPopup").GetComponent<CanvasGroup>().blocksRaycasts = true;
                    }
                }, NativeFilePicker.ConvertExtensionToFileType(".tkrpg"));
            }
            else
            {
                Debug.Log("Permission NOT Granted");
                AskPermission();
            }
        }
    }
    public void ExportGameFile()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            string path = StandaloneFileBrowser.SaveFilePanel("Exportar Archivo de Guardado", "", savefile.SaveFileName, "tkrpg");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, JsonUtility.ToJson(savefile));
                GameObject.Find("ExportPopup").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.Linear);
                GameObject.Find("ExportPopup").GetComponent<CanvasGroup>().interactable = true;
                GameObject.Find("ExportPopup").GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else
        {
            if (GeneralPermission == NativeFilePicker.Permission.Granted)
            {
                Debug.Log("Permission Granted");
                string path = Application.persistentDataPath + "/SaveFiles/SaveFile" + PlayerPrefs.GetInt("LastSaveIndex") + ".tkrpg";
                NativeFilePicker.ExportFile(path, (success) =>
                {
                    Debug.Log("Export Operation: " + success);
                    if (success)
                    {
                        GameObject.Find("ExportPopup").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.Linear);
                        GameObject.Find("ExportPopup").GetComponent<CanvasGroup>().interactable = true;
                        GameObject.Find("ExportPopup").GetComponent<CanvasGroup>().blocksRaycasts = true;
                    }
                }
                );
            }
            else
            {
                Debug.Log("Permission NOT Granted");
                AskPermission();
            }
        }
    }
    public void ReadGameFile()
    {
        nextbuttonfile.interactable = true;
        backbuttonselect.interactable = true;
        HTMLCustomize.interactable = true;
        HTMLExport.interactable = true;
        HTMLNext.interactable = true;
        savefilename.text = savefile.SaveFileName;
        chapter.text = "Capitulo: " + savefile.CurrentChapter;
        scene.text = "Escena: " + savefile.CurrentScene;
        if (savefile.CustomFileImage == null)
        {
            if (savefile.CurrentChapter == "Ninguno")
            {
                savefileimage.texture = ChapterNone;
            }
            else if (savefile.CurrentChapter == "Prototipo")
            {
                savefileimage.texture = ChapterPrototype;
            }
        }
        else
        {
            Texture2D tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, savefile.CustomFileImage);
            savefileimage.texture = tex;
        }
    }
    public void CheckLang()
    {
        langdrop.value = settings.Language;
        SetLang(langdrop.value);
    }
    public void SetLang(int value)
    {
        settings.Language = value;
        UpdateJSON();
    }
    public static void UpdateJSON()
    {
        FileStream Stream = new FileStream(Application.persistentDataPath + "/TKRPGSettings.tkrpgs", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(Stream, settings);
        Stream.Close();
    }
    public void SetSettings()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            Screen.SetResolution(settings.ResolutionX, settings.ResolutionY, settings.Fullscreen);
        }
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, settings.Fullscreen);
        }

        mixer.SetFloat("volume", settings.Volume);
        if (settings.Language == -1)
        {
            GameObject.Find("SetLanguage").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("SetLanguage").GetComponent<CanvasGroup>().blocksRaycasts = true;
            GameObject.Find("SetLanguage").GetComponent<CanvasGroup>().interactable = true;
        }
        else
        {
            SetLang(settings.Language);
            GameObject.Find("LoadGame").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("LoadGame").GetComponent<CanvasGroup>().blocksRaycasts = true;
            GameObject.Find("LoadGame").GetComponent<CanvasGroup>().interactable = true;
        }
    }
    public void RunCoroutine(string CoroutineName)
    {
        StartCoroutine(CoroutineName);
    }
    public IEnumerator CloseLang()
    {
        GameObject.Find("SetLanguage").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("SetLanguage").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("SetLanguage").GetComponent<CanvasGroup>().interactable = false;
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().interactable = true;
    }
    public IEnumerator OpenCustom()
    {
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().interactable = false;
        yield return new WaitForSeconds(0.5f);
        LoadCustom();
        GameObject.Find("CustomizeFile").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("CustomizeFile").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("CustomizeFile").GetComponent<CanvasGroup>().interactable = true;
    }
    public void LoadCustom()
    {
        custominputfield.text = savefile.SaveFileName;
        if (savefile.CustomFileImage == null)
        {
            if (savefile.CurrentChapter == "Ninguno")
            {
                customsavefileimage.texture = ChapterNone;
            }
            else if (savefile.CurrentChapter == "Prototipo")
            {
                customsavefileimage.texture = ChapterPrototype;
            }
        }
        else
        {
            Texture2D tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, savefile.CustomFileImage);
            customsavefileimage.texture = tex;
        }
    }
    public void CheckForNotAllowedName()
    {
        if (custominputfield.text.Contains('"') || custominputfield.text.Contains("'") || custominputfield.text.Contains(char.ConvertFromUtf32(92)) || custominputfield.text == "")
        {
            savebuttoncustom.interactable = false;
        }
        else
        {
            savefile.SaveFileName = custominputfield.text;
            string path = Application.persistentDataPath + "/SaveFiles/SaveFile" + PlayerPrefs.GetInt("LastSaveIndex").ToString() + ".tkrpg";
            FileStream Stream = new FileStream(path, FileMode.Create);
            Formatter.Serialize(Stream, savefile);
            Stream.Close();
            savebuttoncustom.interactable = true;
        }
    }

    public void SelectImage()
    {
        if (Application.platform != RuntimePlatform.Android && OverrideAndroidFileSystem != true)
        {
            var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
            };
            string[] path = StandaloneFileBrowser.OpenFilePanel("Seleccionar Imagen", "", extensions, false);
            if (path.Length == 1)
            {
                savefile.CustomFileImage = File.ReadAllBytes(path[0]);
                Texture2D tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, File.ReadAllBytes(path[0]));
                customsavefileimage.texture = tex;
                string savepath = Application.persistentDataPath + "/SaveFiles/SaveFile" + PlayerPrefs.GetInt("LastSaveIndex").ToString() + ".tkrpg";
                FileStream Stream = new FileStream(savepath, FileMode.Create);
                Formatter.Serialize(Stream, savefile);
                Stream.Close();
            }
        }
        else
        {
            if (GeneralPermission == NativeFilePicker.Permission.Granted)
            {
                Debug.Log("Permission Granted");
                NativeFilePicker.PickFile((path) => 
                {
                    if (path == null)
                    {
                        Debug.Log("Pick Photo: Canceled");
                    }
                    else
                    {
                        Debug.Log("Pick Photo: Success - " + path);
                        savefile.CustomFileImage = File.ReadAllBytes(path);
                        Texture2D tex = new Texture2D(2, 2);
                        ImageConversion.LoadImage(tex, File.ReadAllBytes(path));
                        customsavefileimage.texture = tex;
                        string savepath = Application.persistentDataPath + "/SaveFiles/SaveFile" + PlayerPrefs.GetInt("LastSaveIndex").ToString() + ".tkrpg";
                        FileStream Stream = new FileStream(savepath, FileMode.Create);
                        Formatter.Serialize(Stream, savefile);
                        Stream.Close();
                    }
                }, "image/*" );
            }
            else
            {
                Debug.Log("Permission NOT Granted");
                AskPermission();
            }
        }
    }

    public async void AskPermission()
    {
        NativeFilePicker.Permission permissionResult = await NativeFilePicker.RequestPermissionAsync(false);
        GeneralPermission = permissionResult;
        if (GeneralPermission == NativeFilePicker.Permission.Granted)
        {
            SelectImage();
        }
    }
    public IEnumerator CloseCustom()
    {
        GameObject.Find("CustomizeFile").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("CustomizeFile").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("CustomizeFile").GetComponent<CanvasGroup>().interactable = false;
        yield return new WaitForSeconds(0.5f);
        ReadGameFile();
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().interactable = true;
    }
    public void CreateSaveFile(SaveFile ImportSave)
    {
        var info = new DirectoryInfo(Application.persistentDataPath + "/SaveFiles");
        var fileInfo = info.GetFiles();
        int index = 0;
        if (info.GetFiles().Length == 0)
        {
            index = 0;
        }
        else
        {
            index = info.GetFiles().Length;
        }

        string path = Application.persistentDataPath + "/SaveFiles/SaveFile" + index + ".tkrpg";
        FileStream Stream = new FileStream(path, FileMode.Create);
        if (ImportSave == null)
        {
            SaveFile Save = new SaveFile();
            Formatter.Serialize(Stream, Save);
        }
        else
        {
            Formatter.Serialize(Stream, ImportSave);
        }
        Stream.Close();
        LoadSaveFiles();
    }
    public void CreateSaveFileHTML(SaveFile ImportSave)
    {
        string path = Application.persistentDataPath + "/SaveFiles/SaveFile" + 0 + ".tkrpg";
        FileStream Stream = new FileStream(path, FileMode.Create);
        if (ImportSave == null)
        {
            SaveFile Save = new SaveFile();
            Formatter.Serialize(Stream, Save);
        }
        else
        {
            Formatter.Serialize(Stream, ImportSave);
        }
        Stream.Close();
        FileSelected(0);
        ReadGameFile();
    }
    public void LoadSaveFiles()
    {
        // START UP HERE:
        // Refractor needed for selecting Save Files - Legacy system to be replaced
        for (int i = 0; i < GameObject.FindGameObjectWithTag("SelectFileContent").transform.childCount; i++)
        {
            Destroy(GameObject.FindGameObjectWithTag("SelectFileContent").transform.GetChild(i).gameObject);
        }
        var info = new DirectoryInfo(Application.persistentDataPath + "/SaveFiles");
        var fileInfo = info.GetFiles();

        for (int i = 0; i < info.GetFiles().Length; i++)
        {
            GameObject fileview = Instantiate(savefileview);
            fileview.name = "SaveFile" + i;
            fileview.transform.SetParent(GameObject.FindGameObjectWithTag("SelectFileContent").transform);
            fileview.transform.localScale = Vector3.one;
            fileview.GetComponentInChildren<SaveFileData>().fileindex = i;

            FileStream OPEN = new FileStream(Application.persistentDataPath + "/SaveFiles/SaveFile" + i + ".tkrpg", FileMode.Open);
            SaveFile tempfile = Formatter.Deserialize(OPEN) as SaveFile;
            OPEN.Close();
            string filechapter = tempfile.CurrentChapter;
            if (tempfile.CustomFileImage == null)
            {
                if (filechapter == "Ninguno")
                {
                    fileview.GetComponentInChildren<RawImage>().texture = ChapterNone;
                }
                else if (filechapter == "Prototipo")
                {
                    fileview.GetComponentInChildren<RawImage>().texture = ChapterPrototype;
                }
            }
            else
            {
                Texture2D tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, tempfile.CustomFileImage);
                fileview.GetComponentInChildren<RawImage>().texture = tex;
            }

            for (int textcomp = 0; textcomp < fileview.GetComponentsInChildren<TextMeshProUGUI>().Length; textcomp++)
            {
                if (fileview.GetComponentsInChildren<TextMeshProUGUI>()[textcomp].name == "CreatedTime")
                {
                    fileview.GetComponentsInChildren<TextMeshProUGUI>()[textcomp].text = "Creado: " + info.GetFiles()[i].LastWriteTime;
                }
                else if (fileview.GetComponentsInChildren<TextMeshProUGUI>()[textcomp].name == "SaveFileName")
                {
                    fileview.GetComponentsInChildren<TextMeshProUGUI>()[textcomp].text = tempfile.SaveFileName;
                }
            }
        }
    }
    public IEnumerator OpenSelect()
    {
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().interactable = false;
        yield return new WaitForSeconds(0.5f);
        LoadSaveFiles();
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().interactable = true;
    }
    public IEnumerator CloseSelect()
    {
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("SelectFile").GetComponent<CanvasGroup>().interactable = false;
        yield return new WaitForSeconds(0.5f);
        ReadGameFile();
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("LoadGame").GetComponent<CanvasGroup>().interactable = true;
    }
    public void FileSelected(int index)
    {
        exportbutton.interactable = true;
        custombuttonfile.interactable = true;
        deletebutton.interactable = true;
        PlayerPrefs.SetInt("LastSaveIndex", index);
        string path = Application.persistentDataPath + "/SaveFiles/SaveFile" + PlayerPrefs.GetInt("LastSaveIndex").ToString() + ".tkrpg";
        FileStream Stream = new FileStream(path, FileMode.Open);
        SaveFile Save = Formatter.Deserialize(Stream) as SaveFile;
        Stream.Close();
        savefile = Save;
        StartCoroutine(CloseSelect());
    }
    public void DeleteFile()
    {
        var info = new DirectoryInfo(Application.persistentDataPath + "/SaveFiles");
        string path = Application.persistentDataPath + "/SaveFiles/SaveFile" + PlayerPrefs.GetInt("LastSaveIndex").ToString() + ".tkrpg";
        File.Delete(path);
        int index = PlayerPrefs.GetInt("LastSaveIndex");
        PlayerPrefs.SetInt("LastSaveIndex", -1);
        for (int i = 0; i < info.GetFiles().Length; i++)
        {
            string index1for = info.GetFiles()[i].Name.Replace(Application.persistentDataPath + "/SaveFiles/SaveFile", "");
            string index2for = index1for.Replace(".tkrpg", "");
            string index3for = index2for.Substring(8);
            int indexfor = int.Parse(index3for);
            if (indexfor > index)
            {
                info.GetFiles()[i].MoveTo(Application.persistentDataPath + "/SaveFiles/SaveFile" + (indexfor - 1) + ".tkrpg");
            }
        }
        StartCoroutine(OpenSelect());
        backbuttonselect.interactable = false;
    }
    public IEnumerator LeaveFileSelection()
    {
        GameObject.FindGameObjectWithTag("Panel").GetComponent<Animator>().SetTrigger("IsLeaving");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }
    public void ClosePopup(string PopupName)
    {
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.Linear);
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find(PopupName).GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OpenDelete()
    {
        GameObject.Find("DeletePopup").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.Linear);
        GameObject.Find("DeletePopup").GetComponent<CanvasGroup>().interactable = true;
        GameObject.Find("DeletePopup").GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
