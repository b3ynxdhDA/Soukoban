using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Playerやブロックの配置、移動などを管理するクラス
/// </summary>
public class FieldArrayData : MonoBehaviour
{
    
    #region 変数
    //タグリストの名前に紐づく番号
    const int NO_BLOOK = 0;
    const int STATIC_BLOOK = 1;
    const int MOVE_BLOOK = 2;
    const int PLAYER = 3;
    const int TARGET = 4;

    /// <summary>
    /// シーンに配置するオブジェクトのルートをヒエラルキーから設定する
    /// </summary>
    [Header("配置するオブジェクトの親オブジェクトを設定")]
    [SerializeField] GameObject g_fieldRootObject = default;

    /// <summary>
    /// フィールドのオブジェクトリスト
    /// 0 空欄
    /// 1 動かないブロック
    /// 2 動くブロック
    /// 3 プレイヤー  
    /// 4 ターゲット
    /// </summary>
    private string[] g_fieldObjectTagList = 
    {
        "", "StaticBlock", "MoveBlock", "Player" ,"TargetPosition"
    };

    [Header("動かないオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_staticBlock = default;
    [Header("動くオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_moveBlock = default;
    [Header("プレイヤーオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_player = default;
    [Header("ターゲットオブジェクトを設定(Tagを識別する)")]
    [SerializeField] GameObject g_target = default;

    /// <summary>
    /// フィールドデータ用の変数を定義
    /// </summary>
    private int[,] g_fieldDate =
    {
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
    };

    //縦横のフィールドの幅
    private int g_horizontalMaxCount = 0;
    private int g_verticalMaxCount = 0;

    ///<summary>
    ///ターゲットデータ用の変数を定義
    ///初期にg_fieldDataを複製する
    ///※フィールドデータは常に変化するが
    ///○ターゲット用データは動かさないことで
    ///○ターゲットにオブジェクトが重なっても動かせるようにする
    ///○クリア判定はこのターゲットデータを使う
    /// </summary>
    private int[,] g_targetData =
    {
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
        {0,0,0,0,0,0, },
    };

    //ブロックがターゲットに入った数
    private int g_targetClearCount = 0;
    //ターゲットの最大数
    private int g_targetMaxCount = 0;

    /// <summary>
    /// 過去の配置を記録する
    /// 第一変数：g_backDataCountに対応
    /// 第二、第三変数：g_fieldData
    /// </summary>
    private int[,,] g_fieldDataHistory = new int[100, 30, 30];

    /// <summary>
    /// ISMOVE:(bool)MoveBlockが動いたか
    /// PRE_MB_ROW:現在のMoveBlockの行位置
    /// PRE_MB_COL:現在のMoveBlockの列位置
    /// NEXT_MB_ROW:移動先のMoveBlockの行位置
    /// NEXT_MB_COL:移動先のMoveBlockの列位置
    /// PRE_PLAYER_ROW:現在のPlayerの行位置
    /// PRE_PLAYER_COL:現在のPlayerの列位置
    /// NEXT_PLAYER_ROW:移動先のPlayerの行位置
    /// NEXT_PLAYER_COL:移動先のPlayerの列位置
    /// </summary>
    const int ISMOVE = 0;
    const int PRE_MB_ROW = 1;
    const int PRE_MB_COL = 2;
    const int NEXT_MB_ROW = 3;
    const int NEXT_MB_COL = 4;
    const int PRE_PLAYER_ROW = 5;
    const int PRE_PLAYER_COL = 6;
    const int NEXT_PLAYER_ROW = 7;
    const int NEXT_PLAYER_COL = 8;
    ArrayList backDatalist = new ArrayList()
    { false, -1, -1, -1, -1, -1, -1, -1, -1};


    //プレイヤーの移動回数
    private int g_playerMoveCount = 0;

    //履歴配列のカウンター
    private int g_backDataCount = 0;

    //undoの折り返しの基準値(Length-2)
    private int g_historyEnd = 0;

    #endregion

    #region MAP情報を配列に反映する
    /// <summary>
    /// プレイヤーの位置情報
    /// </summary>
    public Vector2 PlayerPosition { get; set; }

    /// <summary>
    /// fieldRootObjectの配下にあるオブジェクトのタグを読み取り
    /// X座標とY座標を基にfieldDateへ格納します(fieldDateは上書き削除します)
    /// fieldDateはfieldDate[Y,X]で紐づいている
    /// フィールド初期化に使う
    /// </summary>
    /// <param name="fieldRootObject">フィールドオブジェクトのルートオブジェクトを設定</param>>
    public void ImageToArray()
    {
        //フィールドの縦と横の最大数を取得(フィールドの大きさを取得)
        foreach(Transform fieldObject in g_fieldRootObject.transform)
        {
            //縦は座標との兼ね合いで-をつけて逆転させないでみる
            int col = Mathf.FloorToInt(fieldObject.position.x);
            int row = Mathf.FloorToInt(fieldObject.position.y);

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

                PlayerPosition = new Vector2(row, col);
            }
            else if (g_fieldObjectTagList[TARGET].Equals(fieldObject.tag))
            {
                g_fieldDate[row, col] = TARGET;

                //ターゲットの最大カウント
                g_targetMaxCount++;
            }
            //フィールドデータをターゲット用のデータにコピーする
            g_targetData = (int[,])g_fieldDate.Clone();

            //初期ステージ状態を記録
            for (int y = 0; y <= g_verticalMaxCount; y++)
            {
                for (int x = 0; x <= g_horizontalMaxCount; x++)
                {
                    g_fieldDataHistory[g_backDataCount, y, x] = g_fieldDate[y, x];
                }
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
            //縦は座標との兼ね合いで-をつけて逆転させないでみる
            int positionX = Mathf.FloorToInt(fieldObject.position.x);
            int positionY = Mathf.FloorToInt(fieldObject.position.y);

            //横の最大数を設定する
            if(g_horizontalMaxCount < positionX)
            {
                g_horizontalMaxCount = positionX;
            }

            //縦の最大数を設定する
            if(g_verticalMaxCount < positionY)
            {
                g_verticalMaxCount = positionY;
            }
        }
        //フィールド配列の初期化(フィールドの壁ブロック分 + 1)
        g_fieldDate = new int[g_verticalMaxCount + 1, g_horizontalMaxCount + 1];
    }

    #endregion

    /// <summary>
    /// /初回起動時
    /// シーンに配置されたオブジェクトを元に配列データを生成する
    /// </summary>
    private void Awake()
    {
        SetFieldMaxSize();
        ImageToArray();
        g_historyEnd = g_fieldDataHistory.GetLength(0) - 2;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            
            //配列を出力するテスト
            print("Field---------------------------");
            for (int y = 0; y <= g_verticalMaxCount; y++)
            {
                string outPutString = "";
                for (int x = 0; x <= g_horizontalMaxCount; x++)
                {
                    outPutString += g_fieldDate[y, x];
                    //outPutString += g_fieldDataHistory[g_backDataCount,y,x];
                }
                print(outPutString);
            }
            print("Field---------------------------");
            print("プレイヤーポジション：" + PlayerPosition);
            

            //print(PlayerPosition);
        }
    }

    /// <summary>
    /// g_backDataCountをほかのスクリプトにわたす
    /// </summary>
    public int GetMoveCount()
    {
        return g_playerMoveCount;
    }

    #region ブロックを動かす処理
    ///<summary>
    ///フィールドオブジェクトから指定した座標のオブジェクトを取得する
    ///tagIdが-1の場合すべてのタグを対象に検索する
    ///検索にヒットしない場合Nullを返す
    ///</summary>
    ///<param name="tagId">検索対象のタグを指定</param>
    ///<param name="row">縦位置</param>
    ///<param name="col">横位置</param>
    ///<returuns></returuns>
    public GameObject GetFieldObject(int tagId, int row, int col)
    {
        foreach(Transform fieldObject in g_fieldRootObject.transform)
        {
            if(tagId != -1 && fieldObject.tag != g_fieldObjectTagList[tagId])
            {
                continue;
            }
            //縦は座標との兼ね合いで-をつけて逆転させないでみる
            if (fieldObject.transform.position.x == col &&
               fieldObject.transform.position.y == row)
            {
                return fieldObject.gameObject;
            }
        }
        return null;
    }
    ///<summary>
    ///オブジェクトを移動する
    ///データを上書きするので移動できるかどうかを検査して
    ///移動可能な場合処理を実行する
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public void MoveData(int preRow ,int preCol ,int nextRow ,int nextCol)
    {
        //オブジェクトを移動する
        GameObject moveObject = GetFieldObject(g_fieldDate[preRow, preCol], preRow, preCol);

        if(moveObject != null)
        {
            //縦は座標との兼ね合いで-をつけて逆転させないでみる
            //座標情報なので最初の引数はx
            moveObject.transform.position = new Vector2(nextCol, nextRow);
            //print("MoveData : " + moveObject);
        }
        //g_fieldData上の処理
        //上書きするので要注意
        g_fieldDate[nextRow, nextCol] = g_fieldDate[preRow, preCol];

        //移動したら元のデータは削除する
        g_fieldDate[preRow, preCol] = NO_BLOOK;
        
    }
    /// <summary>
    /// ブロックを移動することが可能かチェックする
    /// trueの場合移動可能　falseの場合移動不可能
    /// </summary>
    /// <param name="y">移動先Y座標</param>
    /// <param name="x">移動先X座標</param>
    /// <returns>ブロック移動の可否</returns>
    public bool BlockMoveCheck(int y,int x)
    {
        //ターゲットブロックだったら
        if(g_targetData[y,x] == TARGET)
        {
            //ターゲットクリアカウントを上げる
            g_targetClearCount++;

            return true;
        }
        return g_fieldDate[y, x] == NO_BLOOK;
    }

    /// <summary>
    /// ブロックを移動する(ブロック移動チェックの実施)
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public bool BlockMove(int preRow,int preCol,int nextRow,int nextCol)
    {
        //print("BlockMove");
        //境界線外エラー
        if(nextRow < 0 || nextCol < 0 || 
           nextRow > g_verticalMaxCount || nextCol > g_horizontalMaxCount)
        {
            return false;
        }

        bool isMove = BlockMoveCheck(nextRow, nextCol);

        //移動可能かチェックする
        if (isMove)
        {
            //移動が可能な場合移動する
            MoveData(preRow, preCol, nextRow, nextCol);
        }
        return isMove;
    }
    #endregion

    #region 一つ戻る処理
    /// <summary>
    /// 配置を一つ前に戻す
    /// </summary>
    public void FieldDataBack()
    {

        if (g_backDataCount == 0)
        {
            return;
        }
        //プレイヤーの移動回数を減らす
        g_playerMoveCount--;
        g_backDataCount--;

        //print(g_backDataCount);

        //ステージをひとつ前の状態と比較するui
        for (int y = 0; y <= g_verticalMaxCount; y++)
        {
            for (int x = 0; x <= g_horizontalMaxCount; x++)
            {
                GetDifferentPosition(y, x);
            }
        }

        //Playerの移動
        MoveData((int)backDatalist[PRE_PLAYER_ROW], (int)backDatalist[PRE_PLAYER_COL], (int)backDatalist[NEXT_PLAYER_ROW], (int)backDatalist[NEXT_PLAYER_COL]);
        //プレイヤーの位置を更新する
        //座標情報なので最初の引数はX
        PlayerPosition = new Vector2((int)backDatalist[NEXT_PLAYER_ROW], (int)backDatalist[NEXT_PLAYER_COL]);

        //動くブロックの移動
        if ((bool)backDatalist[0])
        {
            //print("ブロックも");
            MoveData((int)backDatalist[PRE_MB_ROW], (int)backDatalist[PRE_MB_COL], (int)backDatalist[NEXT_MB_ROW], (int)backDatalist[NEXT_MB_COL]);
            backDatalist[ISMOVE] = false;
        }
    }
    /// <summary>
    /// 現在のフィールドと1つ前のフィールド情報を比較して
    /// 一致しないますの位置情報をbackDatalistに記録
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void GetDifferentPosition(int row, int col)
    {
        //g_fieldDataとg_fieldDataHistory比較
        if (g_fieldDate[row, col] != g_fieldDataHistory[g_backDataCount, row, col])
        {
            //g_fieldDateのMoveBlockの位置を
            //移動前情報として記録
            if (g_fieldDate[row, col] == 2)
            {
                backDatalist[PRE_MB_ROW] = row;
                backDatalist[PRE_MB_COL] = col;
                backDatalist[ISMOVE] = true;
            }
            //g_fieldDateのPlayerの位置を
            //移動前情報として記録
            else if (g_fieldDate[row, col] == 3)
            {
                backDatalist[PRE_PLAYER_ROW] = row;
                backDatalist[PRE_PLAYER_COL] = col;
            }

            //g_fieldDataHistoryのMoveBlockの位置を
            //移動後情報として記録
            if (g_fieldDataHistory[g_backDataCount, row, col] == 2)
            {
                //print("MoveBlock");
                backDatalist[NEXT_MB_ROW] = row;
                backDatalist[NEXT_MB_COL] = col;
                backDatalist[ISMOVE] = true;
            }
            //g_fieldDataHistoryのPlayerの位置を
            //移動後情報として記録
            else if (g_fieldDataHistory[g_backDataCount, row, col] == 3)
            {
                backDatalist[NEXT_PLAYER_ROW] = row;
                backDatalist[NEXT_PLAYER_COL] = col;
            }
        }
    }

    #endregion

    #region キャラクターを動かす処理
    ///<summary>
    ///プレイヤーを移動することが可能かチェックする
    ///trueの場合移動可能　falseの場合移動不可能
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    /// <returns>プレイヤー移動の可否</returns>
    public bool PlayerMoveCheck(int preRow, int preCol, int nextRow, int nextCol)
    {
        //プレイヤーの移動先が動くブロックの時
        //ブロックを移動する処理を実施する
        if(g_fieldDate[nextRow ,nextCol] == MOVE_BLOOK)
        {
            bool blockMoveFlag = BlockMove(nextRow, nextCol,
                                           nextRow + (nextRow - preRow),
                                           nextCol + (nextCol - preCol));
            //ターゲットブロックかつ移動できる移動ブロックだったら
            if(g_targetData[nextRow,nextCol] == TARGET && blockMoveFlag)
            {
                //ターゲットクリアカウントを下げる
                g_targetClearCount--;
            }
            return blockMoveFlag;
        }
        //プレイヤーの移動先が空の時移動する
        //プレイヤーの移動先がターゲットの時移動する
        if(g_fieldDate[nextRow, nextCol] == NO_BLOOK ||
           g_fieldDate[nextRow, nextCol] == TARGET)
        {
            return true;
        }
        return false;
    }
    ///<summary>
    ///プレイヤーを移動する(プレイヤー移動チェックも実施)
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public void PlayerMove(int preRow, int preCol, int nextRow, int nextCol)
    {
        //移動可能かチェックする
        if (PlayerMoveCheck(preRow, preCol, nextRow, nextCol))
        {
            //移動が可能な場合移動する
            MoveData(preRow, preCol, nextRow, nextCol);

            //プレイヤーの位置を更新する
            //座標情報なので最初の引数はX
            PlayerPosition = new Vector2(nextRow, nextCol);

            //プレイヤーの移動数を更新
            g_playerMoveCount++;
            g_backDataCount++;

            //ステージ状態を記録
            for (int y = 0; y <= g_verticalMaxCount; y++)
            {
                for (int x = 0; x <= g_horizontalMaxCount; x++)
                {
                    g_fieldDataHistory[g_backDataCount, y, x] = g_fieldDate[y, x];

                    //配列の末尾になったら
                    if (g_backDataCount >= g_historyEnd)
                    {
                        g_fieldDataHistory[g_backDataCount - g_historyEnd, y, x] = g_fieldDate[y, x];
                    }
                }
            }

            if(g_backDataCount > g_historyEnd)
            {
                g_backDataCount = 1;
            }
        }
    }
    #endregion

    #region クリア判定
    ///<summary>
    ///ゲームクリア判定
    /// </summary>
    /// <returns>ゲームクリアの有無</returns>
    public bool GetGameClearJudgement()
    {
        //ターゲットクリア数とターゲットの最大数が一致したらゲームクリア
        return g_targetClearCount == g_targetMaxCount;
    }
    #endregion
}
