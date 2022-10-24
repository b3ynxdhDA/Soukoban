using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject g_Controller = null;

    //フィールド操作クラスの定義
    FieldArrayData g_fieldArrayData;

    [Header("プレイヤーの移動回数を表示する")]
    [SerializeField] GameObject u_MoveCount = null;

    private void Awake()
    {
        //コンポーネント取得
        g_fieldArrayData = g_Controller.GetComponent<FieldArrayData>();
    }

    void FixedUpdate()
    {
        u_MoveCount.GetComponent<Text>().text = "" + g_fieldArrayData.GetMoveCount();
    }
}
