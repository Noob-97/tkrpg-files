using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    // Global Manager: Pause Menu, PlayerInputs, GameplayMusic, Debug Console, Transition Panel
    public GameObject PauseMenu;
    public GameObject GlobalPlayerInputs;
    public GameObject CheatsManager;
    public static int SecInMenus;
    private int StartTime;
    private int EndTime;
    public List<string> NotAvailableScenes;
    // False: OPEN | True: CLOSE
    public bool PauseGate;

    void Start()
    {
        SceneManager.activeSceneChanged += CheckCounting;

        DontDestroyOnLoad(gameObject);

        GameObject gpi = Instantiate(GlobalPlayerInputs);
        gpi.transform.localScale = Vector3.one;
        gpi.name = "Global Player Inputs";

        DontDestroyOnLoad(gpi);

        GameObject cheats = Instantiate(CheatsManager);
        cheats.transform.localScale = Vector3.one;
        cheats.name = "Cheats Manager";

        DontDestroyOnLoad(cheats);
    }

    public void InstantiatePMENU()
    {
        if (PauseGate)
        {
            Destroy(GameObject.FindGameObjectWithTag("PauseMenu"));
            Time.timeScale = 1;
            PauseGate = false;
        }
        else
        {
            GameObject pmenu = Instantiate(PauseMenu, FindAnyObjectByType<Canvas>().transform);
            pmenu.transform.localScale = Vector3.one;
            pmenu.name = "PauseMenu";
            Time.timeScale = 0;
            PauseGate = true;
        }
    }

    public void CheckCounting(Scene ComingScene, Scene ArrivingScene)
    {   
        if (NotAvailableScenes.Contains(ComingScene.name))
        {
            StartTime = (int)Time.time;
        }
    }
    public void SecPlayed()
    {
        EndTime = (int)Time.time;
        SecInMenus += EndTime - StartTime;
    }
}
