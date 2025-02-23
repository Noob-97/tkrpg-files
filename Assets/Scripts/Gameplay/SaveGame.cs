using DG.Tweening;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SaveGame : MonoBehaviour
{
    public AcotationSystem Acotations;
    public Volume Blur;
    public bool OnceGate;
    public bool OKPressed;
    public bool CancelPressed;
    public Texture2D ChapterNone;
    public Texture2D ChapterPrototype;
    public Chapters SaveChapter;
    public BinaryFormatter Formatter = new BinaryFormatter();

    public void OK()
    {
        OKPressed = true;
    }
    public void Cancel()
    {
        CancelPressed = true;
    }
    public void StartSave()
    {
        StartCoroutine(SaveCutscene());
        OnceGate = true;
    }
    public IEnumerator SaveCutscene()
    {
        gameObject.transform.Find("StarCanvas").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        gameObject.transform.Find("StarCanvas").GetComponent<Canvas>().enabled = true;
        gameObject.transform.Find("ParticleSystem").gameObject.SetActive(true);
        gameObject.transform.Find("ParticleCamera").gameObject.SetActive(true);
        Blur.weight = 1;
        Acotations.RunAcotations();
        yield return new WaitUntil(() => Acotations.DialogComplete);
        // Save Prompt
        Acotations.text.text = "";
        if (FileSelection.settings.Language == 0)
        {
            StartCoroutine(Acotations.Type("[¿Quieres guardar tu partida?]", Acotations.TextSpeed));
        }
        else if (FileSelection.settings.Language == 1)
        {
            StartCoroutine(Acotations.Type("[Do you want to save your game?]", Acotations.TextSpeed));   
        }
        Acotations.text.rectTransform.DOLocalMoveY(190, 0.5f).SetEase(Ease.OutExpo);
        gameObject.transform.Find("StarCanvas/FileView").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        gameObject.transform.Find("StarCanvas/FileView/SAVEFILE").DOScaleY(1, 0.5f).SetEase(Ease.OutExpo);
        // File Info
        Texture2D tex = null;
        if (FileSelection.savefile.CustomFileImage == null)
        {
            if (FileSelection.savefile.CurrentChapter == "Ninguno")
            {
                tex = ChapterNone;
            }
            else if (FileSelection.savefile.CurrentChapter == "Prototipo")
            {
                tex = ChapterPrototype;
            }
        }
        else
        {
            tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, FileSelection.savefile.CustomFileImage);
        }
        gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/SaveFileName").GetComponent<TextMeshProUGUI>().text = FileSelection.savefile.SaveFileName;
        gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/SaveFileImage").GetComponent<RawImage>().texture = tex;
        GlobalManager GM = FindAnyObjectByType<GlobalManager>();
        if (FileSelection.savefile.PlayTime / 60 < 10)
        {
            if (FileSelection.savefile.PlayTime % 60 < 10)
            {
                gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().text = "Tiempo Jugado: 0" + (int)(FileSelection.savefile.PlayTime / 60) + ":0" + FileSelection.savefile.PlayTime % 60;
            }
            else
            {
                gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().text = "Tiempo Jugado: 0" + (int)(FileSelection.savefile.PlayTime / 60) + ":" + FileSelection.savefile.PlayTime % 60;
            }
        }
        else
        {
            if (FileSelection.savefile.PlayTime % 60 < 10)
            {
                gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().text = "Tiempo Jugado: " + (int)(FileSelection.savefile.PlayTime / 60) + ":0" + FileSelection.savefile.PlayTime % 60;
            }
            else
            {
                gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().text = "Tiempo Jugado: " + (int)(FileSelection.savefile.PlayTime / 60) + ":" + FileSelection.savefile.PlayTime % 60;
            }
        }
        gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Scene").GetComponent<TextMeshProUGUI>().text = "Escena: " + FileSelection.savefile.CurrentScene;
        yield return new WaitUntil(() => OKPressed || CancelPressed);
        gameObject.transform.Find("StarCanvas/FileView").GetComponent<CanvasGroup>().interactable = false;
        // Update Save
        if (OKPressed)
        {
            Acotations.text.text = "";
            Acotations.text.color = Color.yellow;
            if (FileSelection.settings.Language == 0)
            {
                StartCoroutine(Acotations.Type("[¡Partida Guardada!]", Acotations.TextSpeed));
            }
            else if (FileSelection.settings.Language == 1)
            {
                StartCoroutine(Acotations.Type("[Game has been saved!]", Acotations.TextSpeed));
            }
            GM.SecPlayed();
            int mix = FileSelection.savefile.PlayTime + GlobalManager.SecInMenus;
            if (GlobalManager.SecInMenus / 60 < 10)
            {
                if (FileSelection.savefile.PlayTime % 60 < 10)
                {
                    gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().text = "Tiempo Jugado: 0" + (int)(mix / 60) + ":0" + (mix % 60);
                }
                else
                {
                    gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().text = "Tiempo Jugado: 0" + (int)(mix / 60) + ":" + (mix % 60);
                }
            }
            else
            {
                if (FileSelection.savefile.PlayTime % 60 < 10)
                {
                    gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().text = "Tiempo Jugado: " + (int)(mix / 60) + ":0" + (mix % 60);
                }
                else
                {
                    gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().text = "Tiempo Jugado: " + (int)(mix / 60) + ":" + (mix % 60);
                }
            }
            gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().color = Color.yellow;
            FileSelection.savefile.PlayTime += GlobalManager.SecInMenus;
            gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Scene").GetComponent<TextMeshProUGUI>().text = "Escena: " + SceneManager.GetActiveScene().name;
            gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Scene").GetComponent<TextMeshProUGUI>().color = Color.yellow;
            FileSelection.savefile.CurrentScene = SceneManager.GetActiveScene().name;
            FileSelection.savefile.CurrentChapter = SaveChapter.ToString();
            if (FileSelection.savefile.CustomFileImage == null)
            {
                if (FileSelection.savefile.CurrentChapter == "Ninguno")
                {
                    tex = ChapterNone;
                }
                else if (FileSelection.savefile.CurrentChapter == "Prototipo")
                {
                    tex = ChapterPrototype;
                }
            }
            else
            {
                tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, FileSelection.savefile.CustomFileImage);
            }
            gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/SaveFileImage").GetComponent<RawImage>().texture = tex;
            string path = Application.persistentDataPath + "/SaveFiles/SaveFile" + PlayerPrefs.GetInt("LastSaveIndex").ToString() + ".tkrpg";
            FileStream Stream = new FileStream(path, FileMode.Create);
            Formatter.Serialize(Stream, FileSelection.savefile);
            Stream.Close();
            yield return new WaitForSeconds(3);
        }
        // Quit
        Blur.weight = 0;
        gameObject.transform.Find("StarCanvas").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(0.5f);
        Acotations.text.text = "";
        Acotations.text.color = new Color(0.6830188f, 0.6830188f, 0.6830188f);
        Acotations.text.rectTransform.localPosition = Vector3.zero;
        gameObject.transform.Find("StarCanvas/FileView").GetComponent<CanvasGroup>().alpha = 0;
        gameObject.transform.Find("StarCanvas/FileView/SAVEFILE").localScale = new Vector2(gameObject.transform.Find("StarCanvas/FileView/SAVEFILE").localScale.x, 0);
        gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Playtime").GetComponent<TextMeshProUGUI>().color = new Color(0.5999999f, 0.5999999f, 0.5999999f);
        gameObject.transform.Find("StarCanvas/FileView/SAVEFILE/SaveFile/Scene").GetComponent<TextMeshProUGUI>().color = new Color(0.5999999f, 0.5999999f, 0.5999999f);
        gameObject.transform.Find("StarCanvas").GetComponent<Canvas>().enabled = false;
        gameObject.transform.Find("ParticleSystem").gameObject.SetActive(false);
        gameObject.transform.Find("ParticleCamera").gameObject.SetActive(false);
        gameObject.transform.Find("StarCanvas/FileView").GetComponent<CanvasGroup>().interactable = true;
        OKPressed = false;
        CancelPressed = false;

    }
}
