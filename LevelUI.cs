using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject Victory;
    public GameObject GameOver;
    public GameObject Hint;
    public GameObject PCHint;
    public GameObject PauseButton;
    private bool isAbleToPause = true;
    private bool isPaused = false;
    private void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            PauseButton.SetActive(true);
        }
        else if(Application.platform == RuntimePlatform.WindowsPlayer)
        {
            PCHint.SetActive(true);
        }
    }
    void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer && Input.GetKeyDown(KeyCode.Escape) && isAbleToPause)
        {
            if (!isPaused)
            {
                isPaused = true;
                OnPauseClick();
            }
            else
            {
                isPaused = false;
                OnResumeClick();
            }
        }
    }
    public void OnPauseClick()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Cursor.visible = true;
        }
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnResumeClick()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Cursor.visible = false;
        }
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void OnExitClick()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
    public void OnRestartClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("Game");
    }
    public void OnFinishEnter() 
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Cursor.visible = true;
        }
        Victory.SetActive(true);
        Time.timeScale = 0;
        isAbleToPause = false;
    }
    public void OnGameOver()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Cursor.visible = true;
        }
        GameOver.SetActive(true);
        Time.timeScale = 0;
        isAbleToPause = false;
    }
    public void GetHint(string hint)
    {
        StartCoroutine(ShowText(hint));
    }
    IEnumerator ShowText(string text)
    {
        Hint.GetComponent<Text>().text = text;
        yield return new WaitForSeconds(5.0f);
        Hint.GetComponent<Text>().text = null;
    }
}
