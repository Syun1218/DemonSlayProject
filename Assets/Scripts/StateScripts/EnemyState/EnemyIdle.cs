using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 指定秒数待機する
/// </summary>
public class EnemyIdle : IState
{
    #region 変数  
    private IEnemyBace _enemyBace = default;
    private EnemyStateController _enemyStateController = default;
    private float _moveDirection = default;

    //待機時間計測変数
    private float _nowIdleTime = default;
    #endregion

    #region メソッド  
    //ステート元オブジェクト取得
    public EnemyIdle(IEnemyBace enemy)
    {
        _enemyBace = enemy;
    }

    //状態遷移時の処理
    public void OnStart()
    {
        _enemyStateController = _enemyBace.GetMyState;
        _nowIdleTime = default;
    }

    //この状態中の処理
    public void OnUpdate()
    {
        _moveDirection = _enemyBace.GetPhysics.ChangeScale(_enemyBace.GetPlayerPos, _enemyBace.GetXScale);

        //待機時間を計測し終了したら遷移する
        _nowIdleTime += Time.deltaTime;
        if(_nowIdleTime >= _enemyBace.GetIdleTime)
        {
            //持ってるインターフェイスに応じて遷移
            if (_enemyBace.GetThisObj.GetComponent<IEnemyFly>() is not null)
            {
                _enemyStateController.TransitionState(_enemyStateController.GetEnemyFly);
            }
            else if(_enemyBace.GetThisObj.GetComponent<IEnemyChase>() is not null)
            {
                _enemyStateController.TransitionState(_enemyStateController.GetEnemyChase);
            }
            else
            {
                _enemyStateController.TransitionState(_enemyStateController.GetEnemyMove);
            }
        }

        //体力がゼロになったらDeadステートに遷移
        if (_enemyBace.GetHP <= 0)
        {
            _enemyStateController.TransitionState(_enemyStateController.GetEnemyDead);
        }
    }

    //状態終了時の処理
    public void OnEnd()
    {

    }
    #endregion
}