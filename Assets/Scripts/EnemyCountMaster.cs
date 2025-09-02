using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemyCountMaster : MonoBehaviour
{
    #region 変数  
    //シングルトン
    private static EnemyCountMaster _countInstance;

    //ステージ進行変数
    [SerializeField] private int _bossStage = 3;
    private int _nowStageCount = default;

    //敵召喚変数
    private SummonDirector[] _summoneres = default;
    private float _nowSummonCool = 0;
    [SerializeField] private float _targetSummonCool = 3;
    private bool canSummon = false;
    private int _enemyEnemyCount = default;
    private int _repopEnemyCount = default;
    [SerializeField] private int _maxRepopCount = 3;
    private bool _isBoss = false;
    #endregion

    #region プロパティ
    public static EnemyCountMaster Instance => _countInstance;
    #endregion

    #region メソッド  
    private void Awake()
    {
        //インスタンス生成
        if (_countInstance == null)
        {
            _countInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
            //一定秒数がたったら敵を生成する
            _nowSummonCool += Time.deltaTime;
            if (_nowSummonCool >= _targetSummonCool && canSummon)
            {
                canSummon = false;
                foreach (SummonDirector summoner in _summoneres)
                {
                    summoner.SummonEnemy();
                }
            }
    }

    /// <summary>
    /// シーンが変わった場合の処理
    /// </summary>
    private void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
    {
        //変数初期化
        _enemyEnemyCount = 0;
        _repopEnemyCount = 0;
        _nowSummonCool = 0;

        //通常バトルステージの場合の処理
        if (SceneManager.GetActiveScene().name == "NormalScene" )
        {
            //ステージを進行させる
            _nowStageCount++;

            //召喚オブジェクトを取得
            canSummon = true;
            _summoneres = GameObject.FindWithTag("Summoner").GetComponentsInChildren<SummonDirector>();
        }
        //タイトルや死亡、クリアステージの場合の処理
        else if(SceneManager.GetActiveScene().name != "NormalScene" &&
            SceneManager.GetActiveScene().name != "BossScene")
        {
            canSummon = false;
            ResetStage();
        }
    }

    /// <summary>
    /// 敵の総数を増やす処理
    /// </summary>
    public void AddCount()
    {
        _enemyEnemyCount++;
    }

    /// <summary>
    /// 敵の総数を減らす処理
    /// </summary>
    public void RemoveCount()
    {
        _enemyEnemyCount--;
        //敵が全滅したら処理を行う
        if(_enemyEnemyCount == 0)
        {
            _repopEnemyCount++;
            if ( _repopEnemyCount >= _maxRepopCount)
            {
                //シーンを変更する処理
                if( _nowStageCount == _bossStage)
                {
                    //ボスステージを読み込む
                    SceneManager.LoadScene("BossScene");
                    _isBoss = true;
                }
                else
                {
                    //通常ステージを読み込む
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            else if(!_isBoss)
            {
                //敵を再出現させる処理
                _nowSummonCool = 0;
                canSummon = true;
            }
        }
    }

    /// <summary>
    /// ボスが倒された場合の処理
    /// </summary>
    public void ClearScene()
    {
        ResetStage();
        SceneManager.LoadScene("ClearScene");
    }

    /// <summary>
    /// 現在ステージ数をリセット
    /// </summary>
    public void ResetStage()
    {
        _isBoss = false;
        _repopEnemyCount = 0;
        _nowStageCount = 0;
    }
    #endregion
}