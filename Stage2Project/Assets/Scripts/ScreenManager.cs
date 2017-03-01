using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public delegate void GameEvent();
    public static event GameEvent OnNewGame;
    public static event GameEvent OnExitGame;

    public enum Screens { TitleScreen, GameScreen, ResultScreen, Lobby, PauseScreen, NumScreens }

    private Canvas [] mScreens;
    private Screens mCurrentScreen;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        mScreens = new Canvas[(int)Screens.NumScreens];
        Canvas[] screens = GetComponentsInChildren<Canvas>();
        for (int count = 0; count < screens.Length; ++count)
        {
            for (int slot = 0; slot < mScreens.Length; ++slot)
            {
                if (mScreens[slot] == null && ((Screens)slot).ToString() == screens[count].name)
                {
                    mScreens[slot] = screens[count];
                    break;
                }
            }
        }

        for (int screen = 1; screen < mScreens.Length; ++screen)
        {
            mScreens[screen].enabled = false;
        }

        mCurrentScreen = Screens.TitleScreen;
        mScreens[(int)mCurrentScreen].enabled = true;
    }

    public void StartGame()
    {
        if(OnNewGame != null)
        {
            OnNewGame();
        }

        TransitionTo(Screens.Lobby);
    }

    public void GoToLobby()
    {
        TransitionTo(Screens.Lobby);
    }

    public void GoToMain()
    {
        TransitionTo(Screens.TitleScreen);
    }

    public void GoToGame()
    {
        TransitionTo(Screens.GameScreen);
    }

    public void OnClickPause()
    {
        TransitionTo(Screens.PauseScreen);
    }

    public void OnClickExit()
    {
        GameObject.Find("Lobby").GetComponent<GameNetwork>().QuitServer();
        Destroy(gameObject);
        SceneManager.LoadScene("UI");
    }

    public void EndGame()
    {
        if (OnExitGame != null)
        {
            OnExitGame();
        }

        TransitionTo(Screens.ResultScreen);
    }

    private void TransitionTo(Screens screen)
    {
        mScreens[(int)mCurrentScreen].enabled = false;
        mScreens[(int)screen].enabled = true;
        mCurrentScreen = screen;
    }
}
