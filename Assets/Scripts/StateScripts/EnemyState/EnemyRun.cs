using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyRun : IState
{
    #region 変数  
    private IEnemyBace _enemyBace = default;
    private EnemyStateController _enemyStateController = default;
    private IEnemyRun _enemyRun = default;
    private float _moveDirection = default;
    #endregion

    #region メソッド  
    //ステート元オブジェクト取得
    public EnemyRun(IEnemyBace enemy)
    {
        _enemyBace = enemy;
    }

    //状態遷移時の処理
    public void OnStart()
    {
        _enemyRun = _enemyBace.GetThisObj.GetComponent<IEnemyRun>();
        _enemyStateController = _enemyBace.GetMyState;
        _moveDirection = _enemyBace.GetPhysics.ChangeScale(_enemyBace.GetPlayerPos, _enemyBace.GetXScale);
    }

    //この状態中の処理
    public void OnUpdate()
    {
        //プレイヤーに向かって突進する
        _enemyBace.GetPhysics.Move(_enemyRun.GetRunSpeed *  _moveDirection);

        //壁にぶつかったら待機状態に遷移する処理
        if(_enemyBace.GetIsLeft || _enemyBace.GetIsRight)
        {
            _enemyStateController.TransitionState(_enemyStateController.GetEnemyIdle);
        }
    }

    //状態終了時の処理
    public void OnEnd()
    {
        _enemyBace.GetPhysics.Move(0);
    }
    #endregion
}