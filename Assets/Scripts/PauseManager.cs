using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public TMP_Dropdown resolutiondrop;
    public Toggle fullscreen;
    public Slider volume;
    public TMP_Dropdown langdrop;
    List<string> Resolutions = new List<string>();
    public AudioMixer audiomixer;
    public bool PauseOpen;
    public void Start()
    {
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
    public void GoToMainMenu()
    {
        GameObject popup0 = GameObject.Find("TransitionPanel");
        popup0.GetComponent<CanvasGroup>().alpha = 1;
        GameObject popup1 = GameObject.Find("PauseMenu");
        popup1.GetComponent<CanvasGroup>().blocksRaycasts = false;
        popup1.GetComponent<CanvasGroup>().interactable = false;
        popup1.GetComponent<CanvasGroup>().alpha = 0;
        Time.timeScale = 1;
        GameObject backmusic = GameObject.FindGameObjectWithTag("BackMusic");
        backmusic.GetComponent<AudioReverbFilter>().enabled = false;
        SceneManager.LoadScene("MainMenu");
    }
    public void OpenSettings()
    {
        GameObject popup0 = GameObject.Find("SettingsScreen");
        popup0.GetComponent<CanvasGroup>().blocksRaycasts = true;
        popup0.GetComponent<CanvasGroup>().interactable = true;
        popup0.GetComponent<CanvasGroup>().alpha = 1;
    }
    public void CloseSettings()
    {
        GameObject popup0 = GameObject.Find("SettingsScreen");
        popup0.GetComponent<CanvasGroup>().blocksRaycasts = false;
        popup0.GetComponent<CanvasGroup>().interactable = false;
        popup0.GetComponent<CanvasGroup>().alpha = 0;
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

    public void ClosePause()
    {
        GlobalManager GM = FindAnyObjectByType<GlobalManager>();
        GM.InstantiatePMENU();
    }
}
