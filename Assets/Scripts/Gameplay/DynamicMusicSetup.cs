using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class DynamicMusicVariation
{
    public AudioClip Variation;
    public string[] Scenes;
}
[System.Serializable]
public class DynamicMusic
{
    public DynamicMusicVariation[] Variations;
}
public class DynamicMusicSetup : MonoBehaviour
{
    public DynamicMusic DynamicMusic;
    public AudioSource AudioSource;
    private static DynamicMusicSetup instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            AudioSource = GetComponent<AudioSource>();
            SceneManager.activeSceneChanged += SceneUpdated;
        }
        else
        {
            SceneManager.activeSceneChanged -= SceneUpdated;
            Destroy(gameObject);
        }
    }
    public void SceneUpdated(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
    {
        AudioSource.Pause();
        for (int i = 0; i <  DynamicMusic.Variations.Length; i++)
        {
            for (int scene = 0; scene < DynamicMusic.Variations[i].Scenes.Length; scene++)
            {
                if (SceneManager.GetActiveScene().name == "BattleScene")
                {
                    AudioSource.Stop();
                }
                if (DynamicMusic.Variations[i].Scenes[scene] == SceneManager.GetActiveScene().name)
                {
                    StartCoroutine(WaitToGetTime(DynamicMusic.Variations[i].Variation));
                }
            }
        }
    }
    IEnumerator WaitToGetTime(AudioClip AudioToPlay)
    {
        float lasttime = AudioSource.time;
        yield return new WaitForEndOfFrame();
        AudioSource.Stop();
        yield return new WaitForEndOfFrame();
        AudioSource.clip = AudioToPlay;
        AudioSource.time = lasttime;
        AudioSource.Play();
    }
}
