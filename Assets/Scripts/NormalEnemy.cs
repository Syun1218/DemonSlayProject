using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class NormalEnemy : MonoBehaviour, IEnemyBace,IHitPointer,IPoolSeter
{
    #region 変数
    //ステータス変数
    [SerializeField] private bool _isFly = false;
    [SerializeField] private EnemyStatus _status;
    private string _name;
    private float _maxHp = default;
    private float _hp;
    private float _speed;
    private float _attack;
    private PhysicsDirector _physics = default;
    private EnemyStateController _stateController = default;

    //プレイヤー探知変数
    private GameObject _player = default;
    private Vector2 _playerPos = default;
    private PlayerController _playerController = default;

    //振り向き変数
    private float _startXScale = default;

    //被弾時変数
    private Animator _anim;

    //コライダー変数
    private ColliderDirector _collider = default;
    private bool _isHitUp = false;
    private bool _isHitDown = false;
    private bool _isHitRight = false;
    private bool _isHitLeft = false;

    //オブジェクトプール変数
    private GameObject _poolObj = default;
    #endregion

    #region プロパティ
    //ステートスクリプト取得プロパティ
    public EnemyStateController GetMyState => _stateController;

    //エネミーステータスプロパティ群
    public GameObject GetThisObj => gameObject;
    public float GetHP => _hp;
    public float GetSpeed => _speed;
    public float GetAttack => _attack;
    public bool GetIsBoss => _status.GetIsBoss;
    public bool GetIsFly => _isFly;
    public float GetIdleTime => _status.GetIdleTime;

    //プール取得プロパティ
    public ObjectPoolDirector GetPoolDirector => _poolObj.GetComponent<ObjectPoolDirector>();

    //物理演算スクリプト取得プロパティ
    public PhysicsDirector GetPhysics => _physics;

    //アニメーター取得プロパティ
    public Animator GetAnimator => _anim;

    //コライダー取得プロパティ
    public ColliderDirector GetCollider => _collider;

    //壁、地面との接触判定取得プロパティ群
    public bool GetIsUp => _isHitUp;
    public bool GetIsDown => _isHitDown;
    public bool GetIsRight => _isHitRight;
    public bool GetIsLeft => _isHitLeft;

    //初期Xスケール取得プロパティ
    public float GetXScale => _startXScale;

    //プレイヤー位置取得プロパティ
    public Vector2 GetPlayerPos => _playerPos;

    //行動変化体力取得プロパティ
    public float GetChangeHP { get; }
    #endregion

    #region メソッド
    private void Awake()
    {
        //ステート管理スクリプトインスタンス生成
        _stateController = new EnemyStateController(this);
    }

    private void Start()
    {
        //初期化
        _player = GameObject.FindWithTag("Player");
        _startXScale = transform.localScale.x;
        _playerController = _player.GetComponent<PlayerController>();
        (_name,_hp, _attack, _speed) = _status.GetEnemyState;
        _maxHp = _hp;
        _anim = GetComponent<Animator>();
        _physics = GetComponent<PhysicsDirector>();
        _collider = GetComponent<ColliderDirector>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        //ステート更新処理
        _stateController.OnUpdateState();

        //変数更新
        _playerPos = _player.transform.position;

        //接地判定
        if (_isHitDown)
        {
            _physics.ResetGravity();
        }
        else if (_hp > 0)
        {
            _physics.SetGravity();
        }
    }

    public void EnemyDamage(float damage)
    {
        //体力を減らす
        _hp -= damage;
        if (_hp > 0)
        {
            //被弾モーションを再生
            _anim.SetTrigger("isDamage");
        }
    }

    //プレイヤーへの接触ダメージ処理
    public void CollisionDamage()
    {
        if(_hp > 0)
        {
            _playerController.PlayerDamage(_attack);
        }
    }

    //オブジェクト初期化処理
    public void InitializationEnemyState()
    {
        _stateController.Initialization(_stateController.GetEnemyIdle);
        _hp = _maxHp;
    }

    //接触位置判定メソッド
    public void HitUp(bool isHitCheck) => _isHitUp = isHitCheck;
    public void HitDown(bool isHitCheck) => _isHitDown = isHitCheck;
    public void HitLeft(bool isHitCheck) => _isHitLeft = isHitCheck;
    public void HitRight(bool isHitCheck) => _isHitRight = isHitCheck;

    //プール元のオブジェクトを取得
    public void SetPoolObject(GameObject obj)
    {
        _poolObj = obj;
    }
    #endregion
}
