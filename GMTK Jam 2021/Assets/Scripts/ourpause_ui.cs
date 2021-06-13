using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ourpause_ui : MonoBehaviour
{
    [SerializeField] private int menuscene;
    [SerializeField] private int gameplayScene;
    private static bool gameisPaused = false;
    [SerializeField] public GameObject pausemenu_ui;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameisPaused)
            {
                Resume();
            }

            else
            {
                Paused();
            }
        }
    }

    public void returnMenu()
    {
        SceneManager.LoadScene(menuscene);
        Time.timeScale = 1f;
    }

    public void restartGame()
    {
        SceneManager.LoadScene(gameplayScene);
        Time.timeScale = 1f;
    }

    public void Resume()
    {
        pausemenu_ui.SetActive(false);
        Time.timeScale = 1f;
        gameisPaused = false;
    }

    public void Paused()
    {
        pausemenu_ui.SetActive(true);
        Time.timeScale = 0f;
        gameisPaused = true;
    }
}
