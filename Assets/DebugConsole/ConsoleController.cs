using NoobUtitilies;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class ConsoleController : MonoBehaviour
{
    // Debug Print
    private static int CallNumber;
    public static bool ConsoleOpened;
    public void DelayDebugPrint(PrintType Type, string Text)
    {
        StartCoroutine(DelayDebugPrintFunc(Type, Text));
    }
    private IEnumerator DelayDebugPrintFunc(PrintType Type, string Text)
    {
        CallNumber++;
        int current = CallNumber;
        yield return new WaitForSeconds(9 * (CallNumber - 1));
        DebugConsole.PrintGates = !DebugConsole.PrintGates;
        DebugConsole.DebugPrint(Type, Text);
        if (current == CallNumber)
        {
            // Last Print: Reset
            CallNumber = 0;
            DebugConsole.PrintGates = true;
        }
        Destroy(gameObject);
    }
    public void DelayDestroy()
    {
        StartCoroutine(DelayDestroyFunc());
    }
    private IEnumerator DelayDestroyFunc()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    // Command Console
    public TMP_InputField InputField;
    public List<string> AutocompleteList = new List<string>();
    public static void OpenConsole(CallbackContext context)
    {
        if (context.performed && ConsoleOpened == false)
        {
            ConsoleOpened = true;
            DebugConsole.OpenConsole();
        }
    }
    public static void RunCommand(CallbackContext context)
    {
        if (context.performed && ConsoleOpened == true)
        {
            ConsoleOpened = false;
            DebugConsole.CloseConsole(true);
        }
    }
    public void CloseConsole()
    {
        if (ConsoleOpened == true)
        {
            ConsoleOpened = false;
            DebugConsole.CloseConsole(false);
        }
    }
    public void CheckForCommand()
    {
        string text = InputField.text;
        if (text == "")
        {
            return;
        }
        else if (!text.StartsWith("/"))
        {
            // Logging
            if (text.StartsWith("(e) "))
            {
                Debug.LogError(text.Substring(5));
                DebugConsole.DebugPrint(PrintType.Info, "Successfully logged message as error in console. If a command was meant, make sure to use '/' at the beginning.");
            }
            else if (text.StartsWith("(w) "))
            {
                Debug.LogWarning(text.Substring(5));
                DebugConsole.DebugPrint(PrintType.Info, "Successfully logged message as warning in console. If a command was meant, make sure to use '/' at the beginning.");
            }
            else if (text.StartsWith("(i) "))
            {
                Debug.Log(text.Substring(5));
                DebugConsole.DebugPrint(PrintType.Info, "Successfully logged message as info in console. If a command was meant, make sure to use '/' at the beginning.");
            }
            else if (text.StartsWith("(i)") || text.StartsWith("(w)") || text.StartsWith("(e)"))
            {
                DebugConsole.DebugPrint(PrintType.Error, "No space was found between '(i)', '(w)' or '(e)' and the message. Use  a '(#) [message]' format instead of '(#)[message]' for logging.");
                return;
            }
            else
            {
                Debug.Log(text);
                DebugConsole.DebugPrint(PrintType.Info, "Successfully logged message as info in console. If a command was meant, make sure to use '/' at the beginning.");
            }
        }
        else
        {
            // Commands
            CommandSet[] CommandsSets = Resources.LoadAll<CommandSet>("DebugConsole/Command Sets");
            int CommandsExecuted = 0;
            foreach (CommandSet set in CommandsSets)
            {
                foreach (Command command in  set.Commands)
                {
                    if (text.StartsWith("/" + command.Executer + " ") || text == "/" + command.Executer)
                    {
                        string textnocommand = text.Substring(("/" + command.Executer).Length);
                        if (textnocommand.Contains("/" + command.Executer))
                        {
                            DebugConsole.DebugPrint(PrintType.Error, "The command to be ran was written twice or more.");
                            return;
                        }
                        string[] parameters = null;
                        if (text != "/" + command.Executer && textnocommand.Length > 0)
                        {
                            parameters = textnocommand.Substring(1).Split(" | ");
                        }

                        if (parameters == null && command.Parameters.Count > 0)
                        {
                            DebugConsole.DebugPrint(PrintType.Error, "The number of paramenters given doesn't match the amount the command requires.");
                            return;
                        }

                        if (parameters != null)
                        {
                            if (parameters.Length != command.Parameters.Count)
                            {
                                DebugConsole.DebugPrint(PrintType.Error, "The number of paramenters given doesn't match the amount the command requires.");
                                return;
                            }
                        }

                        // Conversion Test
                        for (int i = 0; i < command.Parameters.Count; i++)
                        {
                            // String are omitted.
                            if (command.Parameters[i].ValueType == ValueType.Int && int.TryParse(parameters[i], out int result) == false)
                            {
                                DebugConsole.DebugPrint(PrintType.Error, "Parameter '" + command.Parameters[i].Name + "' couldn't convert the prompt '" + parameters[i] + "' into it's requested type: int.");
                            }
                            if (command.Parameters[i].ValueType == ValueType.Bool && bool.TryParse(parameters[i], out bool result2) == false)
                            {
                                DebugConsole.DebugPrint(PrintType.Error, "Parameter '" + command.Parameters[i].Name + "' couldn't convert the prompt '" + parameters[i] + "' into it's requested type: bool.");
                            }
                        }

                        switch (command.Parameters.Count)
                        {
                            case 0:
                                command.ActionVoid.Invoke();
                                CommandsExecuted++;
                                break;
                            case 1:
                                object arg0 = parameters[0];
                                command.ActionOnePara.Invoke(arg0);
                                CommandsExecuted++;
                                break;
                            case 2:
                                arg0 = parameters[0];
                                object arg1 = parameters[1];
                                command.ActionTwoPara.Invoke(arg0, arg1);
                                CommandsExecuted++;
                                break;
                            case 3:
                                arg0 = parameters[0];
                                arg1 = parameters[1];
                                object arg2 = parameters[2];
                                command.ActionThreePara.Invoke(arg0, arg1, arg2);
                                CommandsExecuted++;
                                break;
                            case 4:
                                arg0 = parameters[0];
                                arg1 = parameters[1];
                                arg2 = parameters[2];
                                object arg3 = parameters[3];
                                command.ActionFourPara.Invoke(arg0, arg1, arg2, arg3);
                                CommandsExecuted++;
                                break;
                            case >= 5:
                                DebugConsole.DebugPrint(PrintType.Warning, "Using 5 or more parameters in commands is not supported. The command action will be omitted.");
                                return;
                        }
                    }
                }
            }
            if (CommandsSets.Length == 0)
            {
                DebugConsole.DebugPrint(PrintType.Warning, "No Command Sets, and therefore, commands were found on Resources/DebugConsole/Command Sets to be ran.");
                return;
            }
            else if (CommandsExecuted == 0)
            {
                DebugConsole.DebugPrint(PrintType.Error, "The command '" + text + "' wasn't recognized or found on Resources/DebugConsole/Command Sets");
                return;
            }
            else if (CommandsExecuted > 1)
            {
                DebugConsole.DebugPrint(PrintType.Warning, "There were two commands found with the executer '" + text + "', so both were ran. To prevent ambiguity, rename one of the command's executer.");
            }
        }
    }
    static public void ScriptCommand(string text)
    {
        // Copy of "CheckForCommand" but static & text field added to run Commands in other scripts with this.
        if (text == "")
        {
            return;
        }
        else if (!text.StartsWith("/"))
        {
            // Logging
            if (text.StartsWith("(e) "))
            {
                Debug.LogError(text.Substring(5));
                DebugConsole.DebugPrint(PrintType.Info, "Successfully logged message as error in console. If a command was meant, make sure to use '/' at the beginning.");
            }
            else if (text.StartsWith("(w) "))
            {
                Debug.LogWarning(text.Substring(5));
                DebugConsole.DebugPrint(PrintType.Info, "Successfully logged message as warning in console. If a command was meant, make sure to use '/' at the beginning.");
            }
            else if (text.StartsWith("(i) "))
            {
                Debug.Log(text.Substring(5));
                DebugConsole.DebugPrint(PrintType.Info, "Successfully logged message as info in console. If a command was meant, make sure to use '/' at the beginning.");
            }
            else if (text.StartsWith("(i)") || text.StartsWith("(w)") || text.StartsWith("(e)"))
            {
                DebugConsole.DebugPrint(PrintType.Error, "No space was found between '(i)', '(w)' or '(e)' and the message. Use  a '(#) [message]' format instead of '(#)[message]' for logging.");
                return;
            }
            else
            {
                Debug.Log(text);
                DebugConsole.DebugPrint(PrintType.Info, "Successfully logged message as info in console. If a command was meant, make sure to use '/' at the beginning.");
            }
        }
        else
        {
            // Commands
            CommandSet[] CommandsSets = Resources.LoadAll<CommandSet>("DebugConsole/Command Sets");
            int CommandsExecuted = 0;
            foreach (CommandSet set in CommandsSets)
            {
                foreach (Command command in set.Commands)
                {
                    if (text.StartsWith("/" + command.Executer + " ") || text == "/" + command.Executer)
                    {
                        string textnocommand = text.Substring(("/" + command.Executer).Length);
                        if (textnocommand.Contains("/" + command.Executer))
                        {
                            DebugConsole.DebugPrint(PrintType.Error, "The command to be ran was written twice or more.");
                            return;
                        }
                        string[] parameters = null;
                        if (text != "/" + command.Executer && textnocommand.Length > 0)
                        {
                            parameters = textnocommand.Substring(1).Split(" | ");
                        }

                        if (parameters == null && command.Parameters.Count > 0)
                        {
                            DebugConsole.DebugPrint(PrintType.Error, "The number of paramenters given doesn't match the amount the command requires.");
                            return;
                        }

                        if (parameters != null)
                        {
                            if (parameters.Length != command.Parameters.Count)
                            {
                                DebugConsole.DebugPrint(PrintType.Error, "The number of paramenters given doesn't match the amount the command requires.");
                                return;
                            }
                        }

                        // Conversion Test
                        for (int i = 0; i < command.Parameters.Count; i++)
                        {
                            // String are omitted.
                            if (command.Parameters[i].ValueType == ValueType.Int && int.TryParse(parameters[i], out int result) == false)
                            {
                                DebugConsole.DebugPrint(PrintType.Error, "Parameter '" + command.Parameters[i].Name + "' couldn't convert the prompt '" + parameters[i] + "' into it's requested type: int.");
                            }
                            if (command.Parameters[i].ValueType == ValueType.Bool && bool.TryParse(parameters[i], out bool result2) == false)
                            {
                                DebugConsole.DebugPrint(PrintType.Error, "Parameter '" + command.Parameters[i].Name + "' couldn't convert the prompt '" + parameters[i] + "' into it's requested type: bool.");
                            }
                        }

                        switch (command.Parameters.Count)
                        {
                            case 0:
                                command.ActionVoid.Invoke();
                                CommandsExecuted++;
                                break;
                            case 1:
                                object arg0 = parameters[0];
                                command.ActionOnePara.Invoke(arg0);
                                CommandsExecuted++;
                                break;
                            case 2:
                                arg0 = parameters[0];
                                object arg1 = parameters[1];
                                command.ActionTwoPara.Invoke(arg0, arg1);
                                CommandsExecuted++;
                                break;
                            case 3:
                                arg0 = parameters[0];
                                arg1 = parameters[1];
                                object arg2 = parameters[2];
                                command.ActionThreePara.Invoke(arg0, arg1, arg2);
                                CommandsExecuted++;
                                break;
                            case 4:
                                arg0 = parameters[0];
                                arg1 = parameters[1];
                                arg2 = parameters[2];
                                object arg3 = parameters[3];
                                command.ActionFourPara.Invoke(arg0, arg1, arg2, arg3);
                                CommandsExecuted++;
                                break;
                            case >= 5:
                                DebugConsole.DebugPrint(PrintType.Warning, "Using 5 or more parameters in commands is not supported. The command action will be omitted.");
                                return;
                        }
                    }
                }
            }
            if (CommandsSets.Length == 0)
            {
                DebugConsole.DebugPrint(PrintType.Warning, "No Command Sets, and therefore, commands were found on Resources/DebugConsole/Command Sets to be ran.");
                return;
            }
            else if (CommandsExecuted == 0)
            {
                DebugConsole.DebugPrint(PrintType.Error, "The command '" + text + "' wasn't recognized or found on Resources/DebugConsole/Command Sets");
                return;
            }
            else if (CommandsExecuted > 1)
            {
                DebugConsole.DebugPrint(PrintType.Warning, "There were two commands found with the executer '" + text + "', so both were ran. To prevent ambiguity, rename one of the command's executer.");
            }
        }
    }
    public void AutoComplete(string text)
    {
        // Clear Previous
        for (int i = 0; i < gameObject.transform.Find("Suggestions").childCount; i++)
        {
            Destroy(gameObject.transform.Find("Suggestions").GetChild(i).gameObject);
        }
        // STEP 1: Locate Command
        if (text.StartsWith("/"))
        {
            CommandSet[] CommandsSets = Resources.LoadAll<CommandSet>("DebugConsole/Command Sets");
            foreach (CommandSet set in CommandsSets)
            {
                foreach (Command command in set.Commands)
                {
                    if (("/" + command.Executer).StartsWith(text))
                    {
                        AutocompleteList.Add("/" + command.Executer);
                    }
                }
            }
        }
        // Generation
        for (int i = 0; i < AutocompleteList.Count; i++)
        {
            string path = "DebugConsole/AutoCompleteElement";
            GameObject ObjToInstantiate = null;
            GameObject Element = null;

            if (Resources.Load<GameObject>(path) == null)
            {
                Debug.LogError("Couldn't Instantiate AutoComplete Suggestions because it's prefab is not found in Resources/" + path + ".");
                return;
            }
            else
            {
                ObjToInstantiate = Resources.Load<GameObject>(path);
            }

            Element = Instantiate(ObjToInstantiate, gameObject.transform.Find("Suggestions"));

            Element.transform.localScale = new Vector3(1, 1);
            Element.name = "AutoCompleteElement";

            Element.GetComponentInChildren<TextMeshProUGUI>().text = AutocompleteList[i];
        }

        AutocompleteList.Clear();

    }
}
