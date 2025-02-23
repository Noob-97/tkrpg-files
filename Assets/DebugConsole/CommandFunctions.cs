using NoobUtitilies;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandFunctions : MonoBehaviour
{
    // Basic
    public static void AboutTKRPG()
    {
        string text = "TK:RPG hecho por Noob_97, K3L2!!, y FOUNTREX. Version " + Application.version + " (Alpha Cerrado)";
        DebugConsole.DebugPrint(PrintType.Info, text);
    }
    public static void About()
    {
        DebugConsole.DebugPrint(PrintType.Info, "DebugConsole made by Noob_97. Version 0.1");
    }
    public static void Log(object text)
    {
        DebugConsole.DebugPrint(PrintType.Info, (string)text);
    }
    public static void Warn(object text)
    {
        DebugConsole.DebugPrint(PrintType.Warning, (string)text);
    }
    public static void Error(object text)
    {
        DebugConsole.DebugPrint(PrintType.Error, (string)text);
    }
    public static void Cheat(object gamerule, object active)
    {
        switch (gamerule)
        {
            case "DEBUG_STARTUP":
                print(active);
                CheatsManager.DEBUG_STARTUP = bool.Parse((string)active);
                if (bool.Parse((string)active))
                {
                    PlayerPrefs.SetInt("DEBUG_STARTUP", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("DEBUG_STARTUP", 0);
                }
                break;
            case "DEBUG_TEXT":
                CheatsManager.DEBUG_TEXT = bool.Parse((string)active);
                if (bool.Parse((string)active))
                {
                    PlayerPrefs.SetInt("DEBUG_TEXT", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("DEBUG_TEXT", 0);
                }
                break;
            case "UPDATER_NOT_INSTALL":
                CheatsManager.UPDATER_NOT_INSTALL = bool.Parse((string)active);
                if (bool.Parse((string)active))
                {
                    PlayerPrefs.SetInt("UPDATER_NOT_INSTALL", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("UPDATER_NOT_INSTALL", 0);
                }
                break;
            case "MOBILE_OVERRIDE":
                CheatsManager.MOBILE_OVERRIDE = bool.Parse((string)active);
                if (bool.Parse((string)active))
                {
                    PlayerPrefs.SetInt("MOBILE_OVERRIDE", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("MOBILE_OVERRIDE", 0);
                }
                break;
        }
    }
    public static void GoToEnd()
    {
        SceneManager.LoadScene("PrototypeScene4");
    }
}
