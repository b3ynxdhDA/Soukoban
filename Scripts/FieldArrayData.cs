using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldArrayData : MonoBehaviour
{
    #region 変数
    //タグリストの名前に紐づく番号
    const int NO_BLOOK = 0;
    const int STATIC_BLOOK = 1;
    const int MOVE_BLOOK = 2;
    const int PLAYER = 3;

    /// <summary>
    /// シーンに配置するオブジェクトのルートをヒエラルキーから設定する
    /// </summary>
    [Header("配置するオブジェクトの親オブジェクトを設定")]
    [SerializeField] GameObject g_fieldRootObject;

    /// <summary>
    /// フィールドのオブジェクトリスト
    /// 0 空欄
    /// 1 動かないブロック
    /// 2 動くブロック
    /// 3 プレイヤー  
    /// </summary>
    string[] g_fieldObjectTagList = { "", "StaticBlock", "MoveBlock", "Player" };

    [Header("動かないオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_staticBlock;
    [Header("動くオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_moveBlock;
    [Header("プレイヤーオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_player;

    /// <summary>
    /// フィールドデータ用の変数を定義
    /// </summary>
    int[,] g_fieldDate =
    {
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
    };

    //縦横の最大数
    int g_horizontalMaxCount = 0;
    int g_verticalMaxCount = 0;

    #endregion

    #region メソッド
    /// <summary>
    /// プレイヤーの位置情報
    /// </summary>
    public Vector2 PlayerPosition { get; set; }

    /// <summary>
    /// fieldRootObjectの配下にあるオブジェクトのタグを読み取り
    /// X座標とZ座標を基にfieldDateへ格納します(fieldDateは上書き削除します)
    /// fieldDateはfieldDate[Z,X]で紐づいている
    /// フィールド初期化に使う
    /// </summary>
    /// <param name="fieldRootObject">フィールドオブジェクトのルートオブジェクトを設定</param>>
    public void ImageToArray()
    {
        //フィールドの縦と横の最大数を取得(フィールドの大きさを取得)
        foreach(Transform fieldObject in g_fieldRootObject.transform)
        {
            //縦は座標との兼ね合いで-をつけて逆転させる
            int col = Mathf.FloorToInt(fieldObject.position.x);
            int row = Mathf.FloorToInt(-fieldObject.position.z);

            if (g_fieldObjectTagList[STATIC_BLOOK].Equals(fieldObject.tag))
            {
                g_fieldDate[row, col] = STATIC_BLOOK;
            }
            else if (g_fieldObjectTagList[MOVE_BLOOK].Equals(fieldObject.tag))
            {
                g_fieldDate[row, col] = MOVE_BLOOK;
            }
            else if (g_fieldObjectTagList[PLAYER].Equals(fieldObject.tag))
            {
                g_fieldDate[row, col] = PLAYER;
            }
        }
    }

    /// <summary>
    /// フィールドのサイズを設定する
    /// フィールドの初期化に使う
    /// </summary>
    public void SetFieldMaxSize()
    {
        //フィールドの縦と横の最大数を取得(フィールドの大きさを取得)
        foreach(Transform fieldObject in g_fieldRootObject.transform)
        {
            //縦は座標との兼ね合いで-をつけて逆転させる
            int positionX = Mathf.FloorToInt(fieldObject.position.x);
            int positionZ = Mathf.FloorToInt(-fieldObject.position.z);

            //横の最大数を設定する
            if(g_horizontalMaxCount < positionX)
            {
                g_horizontalMaxCount = positionX;
            }

            //縦の最大数を設定する
            if(g_verticalMaxCount < positionZ)
            {
                g_verticalMaxCount = positionZ;
            }
        }
        //フィールド配列の初期化
        g_fieldDate = new int[g_verticalMaxCount + 1, g_horizontalMaxCount + 1];
    }




    #endregion
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
