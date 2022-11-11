using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    const int width = 1280;
    const int height = 720;
    private void Awake()
    {
        // width:横解像度, height:縦解像度, bool:フルスクリーンにするかどうか
        Screen.SetResolution(width, height, false);
    }
    public void OnStart()
    {
        SceneManager.LoadScene("MaineScene");
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
