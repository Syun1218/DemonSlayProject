using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤー方向に横移動を行う
/// </summary>
public class EnemyMove : IState
{
    #region 変数  
    //エネミーのステータス変数
    private IEnemyBace _enemyBace = default;
    private EnemyStateController _enemyStateController = default;
    private float _enemySpeed = default;
    private float _moveDirection = default;
    #endregion

    #region メソッド  
    //ステート元オブジェクト取得
    public EnemyMove(IEnemyBace enemy)
    {
        _enemyBace = enemy;
    }

    //状態遷移時の処理
    public void OnStart()
    {
        _enemySpeed = _enemyBace.GetSpeed;
        _enemyStateController = _enemyBace.GetMyState;
    }

    //この状態中の処理
    public void OnUpdate()
    {
        //向きと移動方向を更新する
        _moveDirection = _enemyBace.GetPhysics.ChangeScale(_enemyBace.GetPlayerPos,_enemyBace.GetXScale);

        //移動可能の場合エネミーの物理演算スクリプトから移動メソッドを呼ぶ
        if (!_enemyBace.GetIsLeft && _moveDirection < 0 ||
            !_enemyBace.GetIsRight && _moveDirection > 0)
        {
            _enemyBace.GetPhysics.Move(_enemySpeed * _moveDirection);
        }
        else
        {
            //移動をキャンセルする処理
            _enemyBace.GetPhysics.Move(0);
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
        //移動をキャンセルする処理
        _enemyBace.GetPhysics.Move(0);
    }
    #endregion
}