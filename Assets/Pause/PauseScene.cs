using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScene : MonoBehaviour
{
    //EventSystemのFirstSelectedに
    //呼び出した時に選択状態にするボタンオブジェクトをアタッチして
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
