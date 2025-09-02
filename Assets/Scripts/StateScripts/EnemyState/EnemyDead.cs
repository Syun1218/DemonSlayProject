using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 死亡処理を行う
/// </summary>
public class EnemyDead : IState
{
    #region 変数  
    private IEnemyBace _enemyBace = default;
    private EnemyStateController _enemyStateController = default;
    private AnimatorStateInfo _animInfo = default;
    private const float TARGET_ANIM_TIME = 0.9f;
    #endregion

    #region メソッド  
    //ステート元オブジェクト取得
    public EnemyDead(IEnemyBace enemy)
    {
        _enemyBace = enemy;
    }

    //状態遷移時の処理
    public void OnStart()
    {
        //死亡アニメを再生
        _enemyStateController = _enemyBace.GetMyState;
        _enemyBace.GetAnimator.SetTrigger("isDead");
        ColliderList.Instance.RemoveColliderList(_enemyBace.GetCollider);
    }

    //この状態中の処理
    public void OnUpdate()
    {
        //遷移完了するまでリターンをする
        if (!_enemyBace.GetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            return;
        }

        _animInfo = _enemyBace.GetAnimator.GetCurrentAnimatorStateInfo(0);
        //死亡アニメ再生後、Idleにステートを変更
        if (_animInfo.normalizedTime >= TARGET_ANIM_TIME)
        {
            _enemyStateController.TransitionState(_enemyStateController.GetEnemyIdle);
        }
    }

    //状態終了時の処理
    public void OnEnd()
    {
        
        if (_enemyBace.GetIsBoss)
        {
            //ボスが倒された場合クリアシーンに遷移する
            _enemyBace.GetThisObj.SetActive(false);
            EnemyCountMaster.Instance.ClearScene();
        }
        else
        {
            //ボス以外の敵の場合プールに返却する
            _enemyBace.GetPoolDirector.EnQueueObject(_enemyBace.GetThisObj);
        }
    }
    #endregion
}