using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public Toggle[] CheatToggles;
    private void Start()
    {
        foreach (Toggle toggle in CheatToggles)
        {
            if (PlayerPrefs.HasKey(toggle.gameObject.name))
            {
                if (PlayerPrefs.GetInt(toggle.gameObject.name) == 1)
                {
                    toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
            }
        }
    }
    public void UpdateCheat(bool active)
    {
        string TargetGamerule = gameObject.name;
        ConsoleController.ScriptCommand("/cheat " + TargetGamerule + " | " + active.ToString());
    }
}
