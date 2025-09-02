using UnityEngine;

//エネミーインターフェイス
public interface IEnemyBace
{
    #region プロパティ
    //ステートスクリプト取得プロパティ
    public EnemyStateController GetMyState { get; }

    //アニメーター取得プロパティ
    public Animator GetAnimator { get; }

    //物理演算スクリプト取得プロパティ
    public PhysicsDirector GetPhysics { get; }

    //エネミーステータスプロパティ群
    public GameObject GetThisObj { get; }
    public float GetHP { get; }
    public float GetSpeed { get; }
    public float GetAttack { get; }
    public bool GetIsBoss { get; }
    public bool GetIsFly { get; }
    public float GetIdleTime { get; }

    //プール取得プロパティ
    public ObjectPoolDirector GetPoolDirector { get; }

    //コライダー取得プロパティ
    public ColliderDirector GetCollider { get; }

    //壁、地面との接触判定取得プロパティ群
    public bool GetIsUp { get; }
    public bool GetIsDown { get; }
    public bool GetIsRight { get; }
    public bool GetIsLeft { get; }

    //初期Xスケール取得プロパティ
    public float GetXScale {  get; }

    //プレイヤー位置取得プロパティ
    public Vector2 GetPlayerPos { get; }

    //行動変化体力取得プロパティ
    public float GetChangeHP { get; }
    #endregion

    #region メソッド
    //ダメージ計算処理メソッド
    public void EnemyDamage(float damage);

    //プレイヤーへの接触ダメージ処理
    public void CollisionDamage();

    //生成時初期化処理
    public void InitializationEnemyState();
    #endregion
}
