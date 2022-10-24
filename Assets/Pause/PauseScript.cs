using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ポーズ機能を呼び出すスクリプト
//常にアクティブなオブジェクトにアタッチして
public class PauseScript : MonoBehaviour
{
    
    //ポーズしたときに表示するUIプレハブ
    [SerializeField] private GameObject pauseUI = default;

    private void Awake()
    {
        Time.timeScale = 1f;
    }
    //ポーズ中は処理を受け付けない処理を書いてはいけない
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            //ポーズUIのアクティブを切り替え
            pauseUI.SetActive(!pauseUI.activeSelf);

            //ポーズUIが表示されている時は停止
            if (pauseUI.activeSelf)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }

        }
    }

}
