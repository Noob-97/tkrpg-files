using DG.Tweening;
using TMPro;
using UnityEngine;

namespace NoobUtitilies
{
    public enum PrintType
    {
        Info,
        Warning,
        Error
    }
    public class PrintingMessage
    {
        public PrintType Type;
        public string Text;

        public PrintingMessage(PrintType Type, string Text)
        {
            this.Type = Type;
            this.Text = Text;
        }
    }
    public class DebugConsole : MonoBehaviour
    {
        public static bool PrintGates;
        public static void DebugPrint(PrintType Type, string Text)
        {
            string path = "";
            switch (Type)
            {
                case PrintType.Info:
                    path = "DebugConsole/ConsolePrint";
                    break;
                case PrintType.Warning:
                    path = "DebugConsole/ConsoleWarn";
                    break;
                case PrintType.Error:
                    path = "DebugConsole/ConsoleError";
                    break;
            }
            GameObject ObjToInstantiate = null;
            GameObject ConsolePopup = null;

            // Object Generation
            if (path == null)
            {
                Debug.LogError("An unknown PrintType has been ordered to display. Given: " + Type +". Only availabes are: Info, Warning and Error.");
                return;
            }
            else if (Resources.Load<GameObject>(path) == null)
            {
                Debug.LogError("Couldn't Instantiate DebugConsole because it's not found in Resources/" + path + ".");
                return;
            }
            else
            {
                ObjToInstantiate = Resources.Load<GameObject>(path);
            }

            if (FindFirstObjectByType<Canvas>().transform == null)
            {
                Debug.LogError("Couldn't Instantiate DebugConsole because the script couldn't find a Canvas on the scene.");
                return;
            }
            else
            {
                ConsolePopup = Instantiate(ObjToInstantiate, FindFirstObjectByType<Canvas>().transform);
            }

            ConsolePopup.transform.localPosition = new Vector3(0, 0);
            ConsolePopup.transform.localScale = new Vector3(1, 1);

            if (!PrintGates)
            {
                ConsolePopup.GetComponent<ConsoleController>().DelayDebugPrint(Type, Text);
                return;
            }

            PrintGates = !PrintGates;

            // Text Printing
            TextMeshProUGUI text = ConsolePopup.transform.Find("Mask").Find("Prompt").gameObject.GetComponent<TextMeshProUGUI>();
            text.text = Text;

            // Text Animation
            ConsolePopup.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            if (text.preferredWidth > 785)
            {
                text.rectTransform.DOLocalMoveX(785 - text.preferredWidth, 3).SetDelay(3.5f);
            }
            ConsolePopup.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(8.5f);
        }
        public static void OpenConsole()
        {
            string path = "DebugConsole/ConsoleWrite";
            GameObject ObjToInstantiate = null;
            GameObject ConsolePopup = null;

            // Object Generation
            if (Resources.Load<GameObject>(path) == null)
            {
                Debug.LogError("Couldn't Instantiate DebugConsole because it's not found in Resources/" + path + ".");
                return;
            }
            else
            {
                ObjToInstantiate = Resources.Load<GameObject>(path);
            }

            if (FindFirstObjectByType<Canvas>().transform == null)
            {
                Debug.LogError("Couldn't Instantiate DebugConsole because the script couldn't find a Canvas on the scene.");
                return;
            }
            else
            {
                ConsolePopup = Instantiate(ObjToInstantiate, FindFirstObjectByType<Canvas>().transform);
            }

            ConsolePopup.transform.localPosition = new Vector3(0, 0);
            ConsolePopup.transform.localScale = new Vector3(1, 1);
            ConsolePopup.name = "ConsoleWrite";

            // Text Animation
            ConsolePopup.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }
        public static void CloseConsole(bool CheckForCommand)
        {
            GameObject ConsolePopup = GameObject.Find("ConsoleWrite");

            // Text Animation
            ConsolePopup.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            if (CheckForCommand)
            {
                ConsolePopup.GetComponent<ConsoleController>().CheckForCommand();
            }
            ConsolePopup.GetComponent<ConsoleController>().DelayDestroy();
        }
    }
}
