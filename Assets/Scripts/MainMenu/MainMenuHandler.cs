using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum Chapters
{
    Prototipo
}
public class MainMenuHandler : MonoBehaviour
{
    public AudioClip menunone;
    public AudioClip menustart;
    public AudioClip menuprototype;
    public TMP_Dropdown resolutiondrop;
    public Toggle fullscreen;
    public Slider volume;
    public TMP_Dropdown langdrop;
    List<string> Resolutions = new List<string>();
    public AudioMixer audiomixer;
    private void Start()
    {
        StartCoroutine(StartAnimation());
        if (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android)
        {
            resolutiondrop.ClearOptions();
            List<string> list = new List<string>();
            list.Add("[N/A]");
            resolutiondrop.AddOptions(list);
            resolutiondrop.interactable = false;
        }
        else
        {
            FindResolutions();
        }
        RetrieveJSON();
    }

    public void RetrieveJSON()
    {
        string resolution = FileSelection.settings.ResolutionX + "x" + FileSelection.settings.ResolutionY;
        for (int i = 0; i < resolutiondrop.options.Count; i++)
        {
            if (resolutiondrop.options[i].text == resolution)
            {
                resolutiondrop.value = i;
            }
        }
        fullscreen.isOn = FileSelection.settings.Fullscreen;
        volume.value = FileSelection.settings.Volume;
        langdrop.value = FileSelection.settings.Language;
    }

    public IEnumerator StartAnimation()
    {
        GameObject target = GameObject.Find("Start");
        target.GetComponent<CanvasGroup>().DOFade(0.4f, 1).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1);
        target.GetComponent<CanvasGroup>().DOFade(1, 1).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1);
        StartCoroutine(StartAnimation());
    }
    public void StartPressed()
    {
        StartCoroutine(StartPressedAction());
    }
    public IEnumerator StartPressedAction()
    {
        GameObject splash = GameObject.Find("SplashScreen");
        splash.GetComponent<CanvasGroup>().alpha = 1;
        splash.GetComponent<CanvasGroup>().DOFade(0, 3);
        yield return new WaitForSeconds(3f);
        UpdateManager updatemanager = GameObject.Find("UpdateManager").GetComponent<UpdateManager>();
        yield return new WaitUntil(() => updatemanager.CheckDone);
        if (updatemanager.CompareVersions() == false)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (updatemanager.UpdateInfo.CanAutoInstall == true)
                {
                    GameObject popup0 = GameObject.Find("PopUpScreen0");
                    popup0.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    popup0.GetComponent<CanvasGroup>().interactable = true;
                    popup0.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                    string text = "Hay una nueva actualizacion que es compatible con Update Manager. Quieres decargarla ahora?";
                    popup0.GetComponentInChildren<TextMeshProUGUI>().text = text;
                }
                else
                {
                    GameObject popup0 = GameObject.Find("PopUpScreen0");
                    popup0.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    popup0.GetComponent<CanvasGroup>().interactable = true;
                    popup0.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                    string text = "Hay una nueva actualizacion que NO es compatible con Update Manager. Quieres ir a itch.io para decargarla ahora?";
                    popup0.GetComponentInChildren<TextMeshProUGUI>().text = text;
                }
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                StartGame();
            }
        }
        else
        {
            StartGame();
        }
    }
    public void StartGame()
    {
        GameObject popup0 = GameObject.Find("PopUpScreen0");
        popup0.GetComponent<CanvasGroup>().blocksRaycasts = false;
        popup0.GetComponent<CanvasGroup>().interactable = false;
        popup0.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        GameObject popup1 = GameObject.Find("PopUpScreen1");
        popup1.GetComponent<CanvasGroup>().blocksRaycasts = false;
        popup1.GetComponent<CanvasGroup>().interactable = false;
        popup1.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        AudioSource music = GameObject.Find("MusicManager").GetComponent<AudioSource>();
        float time = music.time;
        music.clip = menustart;
        music.time = time;
        music.Play();
        GameObject background = GameObject.Find("Background");
        GameObject title = GameObject.Find("Title");
        GameObject backmain = GameObject.Find("BackgroundMain");
        GameObject mainmenu = GameObject.Find("MainMenu");
        background.GetComponent<Transform>().DOMoveX(-12, 0.5f);
        title.GetComponent<Transform>().DOMoveX(-11.875f, 0.5f);
        backmain.GetComponent<Transform>().DOMoveX(0, 0.5f);
        mainmenu.GetComponent<Transform>().DOMoveX(0, 0.5f);
    }
    public void RunGoToUpdate()
    {
        StartCoroutine(GoToUpdate());
    }
    public IEnumerator GoToUpdate()
    {
        UpdateManager updatemanager = GameObject.Find("UpdateManager").GetComponent<UpdateManager>();
        if (updatemanager.UpdateInfo.CanAutoInstall == true)
        {
            GameObject popup0 = GameObject.Find("PopUpScreen0");
            popup0.GetComponent<CanvasGroup>().blocksRaycasts = false;
            popup0.GetComponent<CanvasGroup>().interactable = false;
            popup0.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            yield return new WaitForSeconds(0.5f);
            GameObject splash = GameObject.Find("SplashScreen");
            splash.GetComponent<Image>().color = Color.black;
            splash.GetComponent<CanvasGroup>().DOFade(1, 3);
            AudioSource music = GameObject.Find("MusicManager").GetComponent<AudioSource>();
            music.DOFade(0, 3);
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene("TKRPGUpdater");
        }
        else
        {
            Application.OpenURL("https://noob-97.itch.io/tkrpg");
            GameObject popup0 = GameObject.Find("PopUpScreen0");
            popup0.GetComponent<CanvasGroup>().blocksRaycasts = false;
            popup0.GetComponent<CanvasGroup>().interactable = false;
            popup0.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            yield return new WaitForSeconds(0.5f);
            StartGame();
        }
    }
    public void ChangeMusicPrototype()
    {
        AudioSource music = GameObject.Find("MusicManager").GetComponent<AudioSource>();
        float time = music.time;
        music.clip = menuprototype;
        music.time = time;
        music.Play();
    }
    public void ChangeMusicStart()
    {
        AudioSource music = GameObject.Find("MusicManager").GetComponent<AudioSource>();
        float time = music.time;
        music.clip = menustart;
        music.time = time;
        music.Play();
    }
    public void ReturnToStart()
    {
        AudioSource music = GameObject.Find("MusicManager").GetComponent<AudioSource>();
        float time = music.time;
        music.clip = menunone;
        music.time = time;
        music.Play();
        GameObject background = GameObject.Find("Background");
        GameObject title = GameObject.Find("Title");
        GameObject backmain = GameObject.Find("BackgroundMain");
        GameObject mainmenu = GameObject.Find("MainMenu");
        Button starttext = GameObject.Find("StartText").GetComponent<Button>();
        background.GetComponent<Transform>().DOMoveX(0, 0.5f);
        title.GetComponent<Transform>().DOMoveX(0, 0.5f);
        backmain.GetComponent<Transform>().DOMoveX(12, 0.5f);
        mainmenu.GetComponent<Transform>().DOMoveX(11.875f, 0.5f);
        starttext.interactable = true;
    }
    public void StartPrototype()
    {
        StartCoroutine(StartPrototypeCoroutine());
    }
    public IEnumerator StartPrototypeCoroutine()
    {
        GameObject panel = GameObject.FindGameObjectWithTag("Panel");
        panel.GetComponent<Animator>().SetTrigger("IsLeaving");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("PrototypeScene1");
    }
    public void OpenExit()
    {
        GameObject popup0 = GameObject.Find("ExitScreen");
        popup0.GetComponent<CanvasGroup>().blocksRaycasts = true;
        popup0.GetComponent<CanvasGroup>().interactable = true;
        popup0.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }
    public void CloseExit()
    {
        GameObject popup0 = GameObject.Find("ExitScreen");
        popup0.GetComponent<CanvasGroup>().blocksRaycasts = false;
        popup0.GetComponent<CanvasGroup>().interactable = false;
        popup0.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void OpenSettings()
    {
        GameObject popup0 = GameObject.Find("SettingsScreen");
        popup0.GetComponent<CanvasGroup>().blocksRaycasts = true;
        popup0.GetComponent<CanvasGroup>().interactable = true;
        popup0.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }
    public void CloseSettings()
    {
        GameObject popup0 = GameObject.Find("SettingsScreen");
        popup0.GetComponent<CanvasGroup>().blocksRaycasts = false;
        popup0.GetComponent<CanvasGroup>().interactable = false;
        popup0.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
    }
    public void FindResolutions()
    {
        int currentindex = 0;
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Resolutions.Add(Screen.resolutions[i].width + "x" + Screen.resolutions[i].height);
            if (Screen.resolutions[i].width == Screen.currentResolution.width && Screen.resolutions[i].height == Screen.currentResolution.height)
            {
                currentindex = i;
            }
        }
        resolutiondrop.AddOptions(Resolutions);
        resolutiondrop.value = currentindex;
        resolutiondrop.RefreshShownValue();
    }
    public void SetResolution(int index)
    {
        Screen.SetResolution(Screen.resolutions[index].width, Screen.resolutions[index].height, Screen.fullScreen);
        FileSelection.settings.ResolutionX = Screen.resolutions[index].width;
        FileSelection.settings.ResolutionY = Screen.resolutions[index].height;
        FileSelection.UpdateJSON();
    }
    public void SetFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        FileSelection.settings.Fullscreen = fullscreen;
        FileSelection.UpdateJSON();
    }
    public void SetVolume(float value)
    {
        audiomixer.SetFloat("volume", value);
        FileSelection.settings.Volume = value;
        FileSelection.UpdateJSON();
    }
    public void SetLanguague(int value)
    {
        FileSelection.settings.Language = value;
        FileSelection.UpdateJSON();
    }
    public void GotoFileSelection()
    {
        StartCoroutine(FileSelectionCoroutine());
    }
    public IEnumerator FileSelectionCoroutine()
    {
        GameObject panel = GameObject.FindGameObjectWithTag("Panel");
        panel.GetComponent<Animator>().SetTrigger("IsLeaving");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("FileSelection");
    }
}
