using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ourmenu_ui : MonoBehaviour
{
    [SerializeField] private int gameplayScene;
    [SerializeField] private int controlScene;

    public void startGame()
    {
        SceneManager.LoadScene(gameplayScene);
    }

    public void showControls()
    {
        SceneManager.LoadScene(controlScene);
    }

    public void gameQuit()
    {
        Application.Quit();
    }

}
