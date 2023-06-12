using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Cursor.visible = true;
        }
    }
    public void OnStartClick()
    {
        SceneManager.LoadScene("Game");
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Cursor.visible = false;
        }
    }
    public void OnExitClick()
    {
        Application.Quit();
    }
}
