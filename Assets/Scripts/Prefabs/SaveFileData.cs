using UnityEngine;
using UnityEngine.UI;

public class SaveFileData : MonoBehaviour
{
    public int fileindex;
    public void ButtonSubtitute()
    {
        FileSelection script = GameObject.Find("FileSelectionScript").GetComponent<FileSelection>();
        script.FileSelected(fileindex);
    }
}
