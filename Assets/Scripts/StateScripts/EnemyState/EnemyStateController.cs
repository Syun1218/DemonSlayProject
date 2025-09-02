using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStateController
{
    #region 変数  
    //現在のステート
    private IState _currentState = default;

    //ステート変数
    private IState _idle = default;
    private IState _move = default;
    private IState _fly = default;
    private IState _chase = default;
    private IState _attack = default;
    private IState _run = default;
    private IState _dead = default;
    #endregion

    #region プロパティ
    //ステート取得プロパティ群
    /// <summary>
    /// 待機状態へ遷移
    /// </summary>
    public IState GetEnemyIdle => _idle;
    /// <summary>
    /// 移動状態へ遷移
    /// </summary>
    public IState GetEnemyMove => _move;
    /// <summary>
    /// 飛行状態へ遷移
    /// </summary>
    public IState GetEnemyFly => _fly;
    /// <summary>
    /// 追跡状態へ遷移
    /// </summary>
    public IState GetEnemyChase => _chase;
    /// <summary>
    /// 攻撃状態へ遷移
    /// </summary>
    public IState GetEnemyAttack => _attack;
    /// <summary>
    /// 突進状態へ遷移
    /// </summary>
    public IState GetEnemyRun => _run;
    /// <summary>
    /// 死亡状態へ遷移
    /// </summary>
    public IState GetEnemyDead => _dead;
    #endregion

    #region メソッド  
    /// <summary>
    /// スクリプト起動時に各アクションに起動元を渡す
    /// </summary>
    public EnemyStateController(IEnemyBace enemy)
    {
        _idle = new EnemyIdle(enemy);
        _move = new EnemyMove(enemy);
        _fly = new EnemyFly(enemy);
        _chase = new EnemyChase(enemy);
        _attack = new EnemyAttack(enemy);
        _run = new EnemyRun(enemy);
        _dead = new EnemyDead(enemy);
    }

	/// <summary>
	/// 渡されたステートで初期化
	/// </summary>
	public void Initialization(IState firstState)
    {
        _currentState = firstState;
        _currentState.OnStart();
    }

	/// <summary>
	/// 渡されたステートに遷移
	/// </summary>
	public void TransitionState(IState nextState)
    {
        _currentState.OnEnd();
        _currentState = nextState;
        _currentState.OnStart();
    }

	/// <summary>
	/// 現在のステートの更新処理を起動
	/// </summary>
	public void OnUpdateState()
    {
        if(_currentState is not null)
        {
            _currentState.OnUpdate();
        }
    }
	#endregion
}