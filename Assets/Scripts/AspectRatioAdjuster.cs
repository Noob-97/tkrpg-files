using UnityEngine;
public class AspectRatioAdjuster : MonoBehaviour
{
    public Camera Camera;
    public float LastRatio;
    private void Start()
    {
        LastRatio = (float)Screen.width / (float)Screen.height;
        if (GameObject.Find("GlobalManager") == null)
        {
            GameObject GM = Instantiate(Resources.Load<GameObject>("GlobalManager"));
            GM.transform.localScale = Vector3.one;
            GM.name = "GlobalManager";

            DontDestroyOnLoad(GM);

        }
    }
    public void Update()
    {
        if (LastRatio != 9.5f / 8f)
        {
            Adjust();
            LastRatio = (float)Screen.width / (float)Screen.height;
        } 
    }
    public void Adjust()
    {
        float targetaspect = 9.5f / 8f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        if (scaleheight < 1.0f)
        {
            Rect rect = Camera.rect;
            rect.width = 1f;
            rect.height = scaleheight;
            rect.x = 0f;
            rect.y = (1f - scaleheight) / 2f;

            Camera.rect = rect;
        }
        else
        {
            float scalewidth = 1f / scaleheight;
            Rect rect = Camera.rect;
            rect.width = scalewidth;
            rect.height = 1f;
            rect.x = (1f - scalewidth) / 2f;
            rect.y = 0;
            
            Camera.rect = rect;
        }
    }
    
}
