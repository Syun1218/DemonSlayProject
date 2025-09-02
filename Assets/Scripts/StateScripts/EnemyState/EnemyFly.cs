using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 一定間隔でランダム性を持つ飛行を行う
/// </summary>
public class EnemyFly : IState
{
    #region 変数  
    private IEnemyBace _enemyBace = default;
    private IEnemyFly _flyInterFace = default;
    private EnemyStateController _enemyStateContorller = default;
    private Vector2 _flyTarget = default;
    private Vector2 _objPos = default;
    private float _moveDirection = default;
    #endregion

    //ステート元オブジェクト取得
    public EnemyFly(IEnemyBace enemy)
    {
        _enemyBace = enemy;
    }

    //状態遷移時の処理
    public void OnStart()
    {
        _enemyStateContorller = _enemyBace.GetMyState;
        _flyInterFace = _enemyBace.GetThisObj.GetComponent<IEnemyFly>();
        //飛行処理を行う
        _enemyBace.GetAnimator.SetBool("isFly", true);
        _flyTarget = _flyInterFace.GetFlyPos;
        _enemyBace.GetPhysics.Fly(_enemyBace.GetSpeed,_flyTarget);
    }

    //この状態中の処理
    public void OnUpdate()
    {
        //向きを変更する
        _moveDirection = _enemyBace.GetPhysics.ChangeScale(_enemyBace.GetPlayerPos, _enemyBace.GetXScale);

        //このステートを持つオブジェクトの位置を取得
        _objPos = _enemyBace.GetThisObj.transform.position;

        //壁や地面に衝突したら目標地点を修正
        if (_enemyBace.GetIsUp && _flyTarget.y > _objPos.y ||
                _enemyBace.GetIsDown && _flyTarget.y < _objPos.y ||
                    _enemyBace.GetIsRight && _flyTarget.x > _objPos.x ||
                        _enemyBace.GetIsLeft && _flyTarget.x < _objPos.x)
        {
            _enemyBace.GetPhysics.StopFly();
        }

        //目的地につくか飛行をストップしたら待機状態に遷移する
        if (_enemyBace.GetPhysics.GetGoalBool)
        {
            _enemyStateContorller.TransitionState(_enemyStateContorller.GetEnemyIdle);
        }

        //体力がゼロになったらDeadステートに遷移
        if(_enemyBace.GetHP <= 0)
        {
            _enemyStateContorller.TransitionState(_enemyStateContorller.GetEnemyDead);
        }
    }

    //状態終了時の処理
    public void OnEnd()
    {
        _enemyBace.GetPhysics.StopFly();
        _enemyBace.GetAnimator.SetBool("isFly", false);
    }
}