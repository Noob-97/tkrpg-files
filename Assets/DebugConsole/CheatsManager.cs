using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatsManager : MonoBehaviour
{
    // Bool Cheats
    static public bool DEBUG_STARTUP;
    static public bool DEBUG_TEXT;
    static public bool UPDATER_NOT_INSTALL;
    static public bool MOBILE_OVERRIDE;
    public void Start()
    {
        ReviseCheats();
        if (DEBUG_STARTUP)
        {
            if (SceneManager.GetActiveScene().name != "CheckLoading" && SceneManager.GetActiveScene().name != "FileSelection")
            {
                StartCoroutine(DebugStartup());
            }
        }
    }

    void ReviseCheats()
    {
        // DEBUG_STARTUP
        if (PlayerPrefs.HasKey("DEBUG_STARTUP"))
        {
            int rev = PlayerPrefs.GetInt("DEBUG_STARTUP");
            if (rev == 1)
            {
                DEBUG_STARTUP = true;
            }
            else
            {
                DEBUG_STARTUP = false;
            }
        }
        // DEBUG_TEXT
        if (PlayerPrefs.HasKey("DEBUG_TEXT")) 
        {
            int rev = PlayerPrefs.GetInt("DEBUG_TEXT");
            if (rev == 1)
            {
                DEBUG_TEXT = true;
            }
            else
            {
                DEBUG_TEXT = false;
            }
        }
        // MOBILE_OVERRIDE
        if (PlayerPrefs.HasKey("MOBILE_OVERRIDE"))
        {
            int rev = PlayerPrefs.GetInt("MOBILE_OVERRIDE");
            if (rev == 1)
            {
                MOBILE_OVERRIDE = true;
            }
            else
            {
                MOBILE_OVERRIDE = false;
            }
        }
        // UPDATER_NOT_INSTALL
        if (PlayerPrefs.HasKey("UPDATER_NOT_INSTALL"))
        {
            int rev = PlayerPrefs.GetInt("UPDATER_NOT_INSTALL");
            if (rev == 1)
            {
                UPDATER_NOT_INSTALL = true;
            }
            else
            {
                UPDATER_NOT_INSTALL = false;
            }
        }
    }

    IEnumerator DebugStartup()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("FileSelection");
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "FileSelection");
        SceneManager.LoadScene(currentScene);
    }

    public void UpdateCheats()
    {

    }
}
