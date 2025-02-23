using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InstallButtonData : MonoBehaviour
{
    public void ButtonSubtitute()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().blocksRaycasts = true;
            GameObject.Find("EnterDirectoryPopUp").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        }
        else
        {
            GameObject.Find("UpdateManager").GetComponent<UpdateManager>().EnterDirectory();
        }
    }
}
