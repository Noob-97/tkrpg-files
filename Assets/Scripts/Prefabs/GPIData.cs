using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GPIData : MonoBehaviour
{
    GlobalManager GM;
    public void GetInput(InputAction.CallbackContext context)
    {
        GM = FindAnyObjectByType<GlobalManager>();
        if (context.performed && !GM.NotAvailableScenes.Contains(SceneManager.GetActiveScene().name))
        {
            GM.InstantiatePMENU();
        }
    }
}
