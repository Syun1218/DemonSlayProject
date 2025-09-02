using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 絶え間なくプレイヤーを飛行移動で追跡する
/// </summary>
public class EnemyChase : IState
{
    #region 変数  
    //スクリプト取得変数
    private IEnemyBace _enemyBase = default;
    private IEnemyChase _chaseInterFace = default;
    private EnemyStateController _enemyStateController = default;

    //追跡場所指定変数
    private float _moveDirection = default;
    private Vector2 _targetPos = default;
    private Vector2 _nowPos = default;

    //時間計測変数
    private float _nowChaseTime = default;
    #endregion

    #region メソッド  
    //ステート元オブジェクト取得
    public EnemyChase(IEnemyBace enemy)
    {
        _enemyBase = enemy;
    }

    //状態遷移時の処理
    public void OnStart()
    {
        _enemyStateController = _enemyBase.GetMyState;
        _chaseInterFace = _enemyBase.GetThisObj.GetComponent<IEnemyChase>();

        _nowChaseTime = default;
    }

    //この状態中の処理
    public void OnUpdate()
    {
        //向きを変更する
        _moveDirection = _enemyBase.GetPhysics.ChangeScale(_enemyBase.GetPlayerPos, _enemyBase.GetXScale);

        //壁や地面に接触している場合、移動位置に補正をかける
        _targetPos = _chaseInterFace.GetChasePos;
        _nowPos = _enemyBase.GetThisObj.transform.position;

        if (_enemyBase.GetIsUp && _targetPos.y > _nowPos.y || _enemyBase.GetIsDown && _nowPos.y > _targetPos.y)
        {
            _targetPos.y = _nowPos.y;
        }
        if (_enemyBase.GetIsLeft && _nowPos.x > _targetPos.x || _enemyBase.GetIsRight && _targetPos.x > _nowPos.x)
        {
            _targetPos.x = _nowPos.x;
        }

        //ターゲットを追跡する
        _enemyBase.GetPhysics.Fly(_enemyBase.GetSpeed, _targetPos);

        //体力がゼロになったらDeadステートに遷移
        if(_enemyBase.GetHP <= 0)
        {
            _enemyStateController.TransitionState(_enemyStateController.GetEnemyDead);
        }

        //追跡時間が一定を過ぎ、体力が一定以下なら遷移
        _nowChaseTime += Time.deltaTime;
        if(_nowChaseTime >= _chaseInterFace.GetChaseTime)
        {
            if(_enemyBase.GetChangeHP >= _enemyBase.GetHP)
            {
                _enemyStateController.TransitionState(_enemyStateController.GetEnemyRun);
            }
            else
            {
                _nowChaseTime = 0;
            }
        }
    }

    //状態終了時の処理
    public void OnEnd()
    {
        _enemyBase.GetPhysics.StopFly();
    }
    #endregion
}