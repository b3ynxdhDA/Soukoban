using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScene : MonoBehaviour
{
    //EventSystemのFirstSelectedに
    //呼び出した時に選択状態にするボタンオブジェクトをアタッチして
    [SerializeField] GameObject u_ChangeScreenText = null;

    private bool isScreenMode = true;

    public void OnScreenChange()
    {
        if (isScreenMode)
        {
            isScreenMode = false;
            Screen.SetResolution(Screen.width, Screen.height, false);
            u_ChangeScreenText.GetComponent<Text>().text = "< Full Screen >";
        }
        else
        {
            isScreenMode = true;
            Screen.SetResolution(Screen.width, Screen.height, true);
            u_ChangeScreenText.GetComponent<Text>().text = "< Window >";
        }
    }

    public void OnCheckPoint()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void OnExit()
    {
#if UNITY_EDITOR
        //エディターの時は再生をやめる
        UnityEditor.EditorApplication.isPlaying = false;
#else
            //アプリケーションを終了する
            Application.Quit();
#endif
    }
}
