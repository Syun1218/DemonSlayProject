using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour,IHitPointer
{
    #region 変数
    //ステータス変数
    [SerializeField] private float _maxHp;
    [SerializeField] private float _attack;
    [SerializeField] private float _speed;
    private float _hp = default;
    private PhysicsDirector _physics = default;

    //移動変数
    private PlayerInput _playerInput = default;
    private Vector2 _inputValue = default;
    private float _moveValue = default; 
    private float _referenceMoveValue = default;
    private const float MOVE_RIGHT = 1;
    private const float MOVE_LEFT = -1;


    //ジャンプ変数
    [SerializeField] private float _targetJumpHeight;
    [SerializeField] private float _minJumpHeight;
    [SerializeField] private float _jumpForce;
    private bool _isJump = false;
    private float _startYPos = default;
    private float _maxJumpYPos = default;
    private float _minJumpYPos = default;
    private bool _onJumpButton = false;

    //被弾時変数
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private float _maxInvincibleTime;
    private float _nowInvincibleTime = 0;
    private bool _isInvincible = false;

    //コライダー変数
    private bool _isHitUp = false;
    private bool _isHitDown = false;
    private bool _isHitRight = false;
    private bool _isHitLeft = false;

    //クールタイム変数
    private bool _canUseMagic = true;
    private float _targetCoolTime = default;
    private float _nowCoolTime = default;

    //攻撃変数
    Vector3 _magicInstancePos = Vector3.zero;
    [SerializeField] private List<GameObject> _myMagics = new List<GameObject>();
    [SerializeField] private GameObject _poolMaster = default;

    //アニメーション変数
    private Animator _animator = default;
    #endregion

    #region メソッド
    private void Start()
    {
        //初期化
        _hp = _maxHp;
        _hpSlider.maxValue = _maxHp;
        _physics = GetComponent<PhysicsDirector>(); 
        _animator = GetComponent<Animator>();

        //インプットアクションの初期化
        _playerInput = new PlayerInput();
        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Move.canceled += OnMove;
        _playerInput.Player.Jump.performed += OnJump;
        _playerInput.Player.Jump.canceled += OnJump;
        _playerInput.Player.NormalAttack.started += OnNormalAttack;
        _playerInput.Player.AttackTwo.started += OnStrongAttack;
        _playerInput.Player.Menu.started += OnMenu;
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        //インプットアクションの無効化
        _playerInput.Player.Move.performed -= OnMove;
        _playerInput.Player.Move.canceled -= OnMove;
        _playerInput.Player.Jump.performed -= OnJump;
        _playerInput.Player.Jump.canceled -= OnJump;
        _playerInput.Player.NormalAttack.started -= OnNormalAttack;
        _playerInput.Player.AttackTwo.started -= OnStrongAttack;
        _playerInput.Player.Menu.started -= OnMenu;
        _playerInput.Disable();
    }

    private void Update()
    {
        //接地判定
        if(_isHitDown)
        {
            _physics.ResetGravity();
            _animator.SetBool("isFall", false);
        }
        else if(!_isJump)
        {
            //落下処理
            _physics.SetGravity();
            _animator.SetBool("isFall",true);
        }

        //ジャンプボタンの入力内容に応じたリアルタイム処理
        if (!_onJumpButton)
        {
            //最小ジャンプ処理
            if(gameObject.transform.position.y >= _minJumpYPos)
            {
                _isJump = false;
            }
        }
    }

    private void FixedUpdate()
    {
        //HPをスライダーと同期
        _hpSlider.value = _hp;

        //無敵時間の計測処理
        if (_isInvincible)
        {
            _nowInvincibleTime += Time.deltaTime;
            if (_nowInvincibleTime >= _maxInvincibleTime)
            {
                _isInvincible = false;
                _nowInvincibleTime = 0;
            }
        }

        //魔法使用後のクールタイムを測定
        if (_targetCoolTime != default)
        {
            _nowCoolTime += Time.deltaTime;
            if (_nowCoolTime >= _targetCoolTime)
            {
                _nowCoolTime = default;
                _targetCoolTime = default;
                _canUseMagic = true;
            }
        }

        //移動量を補正する
        if (_referenceMoveValue != 0)
        {
            if (_referenceMoveValue < 0)
            {
                if (_isHitLeft)
                {
                    //左側にオブジェクトがある場合移動値を0にする
                    _moveValue = 0;
                }
                else
                {
                    //入力内容を整数に直す
                    _moveValue = MOVE_LEFT;
                }
            }
            else if (_referenceMoveValue > 0)
            {
                if (_isHitRight)
                {
                    //右側にオブジェクトがある場合移動値を0にする
                    _moveValue = 0;
                }
                else
                {
                    //入力内容を整数に直す
                    _moveValue = MOVE_RIGHT;
                }

            }
        }
        else
        {
            _moveValue = 0;
        }

        //移動処理
        _physics.Move(_moveValue * _speed);

        //移動アニメーション再生
        if(_moveValue != 0)
        {
            _animator.SetBool("isWalk",true);
        }
        else
        {
            _animator.SetBool("isWalk", false);
        }

        //移動方向にキャラの向きを変える
        if (_moveValue != 0)
        {
            Vector2 localVec = gameObject.transform.localScale;
            localVec.x = _moveValue;
            gameObject.transform.localScale = localVec;
        }

        
        if (_isJump)
        {
            if(gameObject.transform.position.y >= _maxJumpYPos || _isHitUp)
            {
                //最大高度を超えたらジャンプをやめる処理
                _isJump = false;
                _physics.CancelJump();
            }
            else
            {
                //ジャンプ処理
                _physics.Jump(_jumpForce);
                _animator.SetBool("isJump", true);
            }
        }
        else
        {
            //ジャンプ解除処理
            _animator.SetBool("isJump", false);
            _physics.CancelJump();
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        //入力内容をX軸とY軸の値に変換
        _inputValue = context.ReadValue<Vector2>();
        _referenceMoveValue = _inputValue.x;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        //ジャンプ状態の管理、変更処理
        if(_isHitDown && context.ReadValue<float>() > 0)
        {
            _isJump = true;
            _startYPos = gameObject.transform.position.y;
            _maxJumpYPos = _startYPos + _targetJumpHeight;
            _minJumpYPos = _startYPos + _minJumpHeight;
            _onJumpButton = true;
        }
        else if(context.ReadValue<float>() == 0)
        {
            _onJumpButton = false;
        }
    }

    //攻撃メソッド
    private void OnNormalAttack(InputAction.CallbackContext context)
    {
        if (!_canUseMagic)
        {
            return;
        }

        //攻撃オブジェクトの生成ポジション指定
        _magicInstancePos.Set(gameObject.transform.position.x + gameObject.transform.localScale.x,gameObject.transform.position.y,0);
        
        //装備スロットと同じプレハブを持つプールオブジェクトを取得
        ObjectPoolDirector poolObj = default;
        ObjectPoolDirector[] poolObjs = _poolMaster.GetComponentsInChildren<ObjectPoolDirector>();
        for(int i = 0;i < poolObjs.Length; i++)
        {
            if (_myMagics[0].name == poolObjs[i].GetInstanceObject.name)
            {
                poolObj = poolObjs[i];
                break;
            }
        }

        //プールオブジェクトから魔法オブジェクトを持ってくる
        if(poolObj != default)
        {
            MagicBulletController magicObjCol = poolObj.DequeueObject(_magicInstancePos).GetComponent<MagicBulletController>();
            magicObjCol.SetMagicState(gameObject, gameObject.transform.localScale.x);
            
            //魔法使用後のクールタイムを設定
            _targetCoolTime = magicObjCol.GetCoolTime;
            _canUseMagic = false;
        }
    }

    //貫通攻撃メソッド
    private void OnStrongAttack(InputAction.CallbackContext context)
    {
        if (!_canUseMagic)
        {
            return;
        }

        //攻撃オブジェクトの生成ポジション指定
        _magicInstancePos.Set(gameObject.transform.position.x + gameObject.transform.localScale.x, gameObject.transform.position.y, 0);

        //装備スロットと同じプレハブを持つプールオブジェクトを取得
        ObjectPoolDirector poolObj = default;
        ObjectPoolDirector[] poolObjs = _poolMaster.GetComponentsInChildren<ObjectPoolDirector>();
        for (int i = 0; i < poolObjs.Length; i++)
        {
            if (_myMagics[1].name == poolObjs[i].GetInstanceObject.name)
            {
                poolObj = poolObjs[i];
                break;
            }
        }

        //プールオブジェクトから魔法オブジェクトを持ってくる
        if (poolObj != default)
        {
            MagicBulletController magicObjCol = poolObj.DequeueObject(_magicInstancePos).GetComponent<MagicBulletController>();
            magicObjCol.SetMagicState(gameObject, gameObject.transform.localScale.x);

            //魔法使用後のクールタイムを設定
            _targetCoolTime = magicObjCol.GetCoolTime;
            _canUseMagic = false;
        }
    }

    //メニュー表示処理
    private void OnMenu(InputAction.CallbackContext context)
    {
        MenuDirector.Instance.CanvasEnableDisable();
    }

    //接触位置判定メソッド
    public void HitUp(bool isHitCheck) => _isHitUp = isHitCheck;
    public void HitDown(bool isHitCheck) => _isHitDown = isHitCheck;
    public void HitLeft(bool isHitCheck) => _isHitLeft = isHitCheck;
    public void HitRight(bool isHitCheck) => _isHitRight = isHitCheck;

    //ダメージ処理
    public void PlayerDamage(float damage)
    {
        //無敵状態かの判定
        if (!_isInvincible)
        {
            //無敵状態ではない場合ダメージを与える
            _hp -= damage;
            if(_hp <= 0)
            {
                EnemyCountMaster.Instance.ResetStage();
                SceneManager.LoadScene("DeadScene");
            }
            else
            {
                MenuDirector.Instance.PlayCollisionSE();
            }

            //無敵状態にする
            _isInvincible = true;
        }
    }
    #endregion 
}
