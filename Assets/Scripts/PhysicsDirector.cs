using UnityEngine;
using System.Collections;
using System;

public class PhysicsDirector : MonoBehaviour
{
    #region 変数  
    //移動変数
    private const float MOVE_RIGHT = 1;
    private const float MOVE_LEFT = -1;
    private Vector3 _moveVelocity = default;
    private float _gravityScale = default;
    private float _jumpForce = default;
    Vector3 _moveTargetPos = default;
    private float _flySpeed = default;
    private bool _isFly = false;
    private bool _isJump = false;
    private bool _isGoal = false;
    [SerializeField] private float _targetDis = default;

    //加算用変数
    [SerializeField] private float _subtractJumpForce = 0.5f;
    [SerializeField] private float _addGravityForce = 98f;
    #endregion

    #region プロパティ
    public bool GetGoalBool
    {
        get { return _isGoal; }
    }
    #endregion

    #region メソッド  
    private void FixedUpdate()
    {
        if(!_isFly)
        {
            //移動処理
            gameObject.transform.position += _moveVelocity;
        }
        else if(!_isGoal)
        {
            //飛行処理
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position,_moveTargetPos, _flySpeed);

            //目標地点に到着したらBoolを変更する
            if((_targetDis * _targetDis) > (gameObject.transform.position - _moveTargetPos).sqrMagnitude)
            {
                _isGoal = true;
            }
        }
    }

    /// <summary>
    /// 重力適用処理
    /// </summary>
    public void SetGravity(float gravityScale = 0.2f)
    {
        //重力をセットする
        if(_gravityScale == default)
        {
            _gravityScale = gravityScale;
        }
        else
        {
            _gravityScale += _addGravityForce * Time.deltaTime;
        }

        //移動ポジションに重力分を加算する
        _moveVelocity.y = -_gravityScale;
    }

    /// <summary>
    /// 重力解除処理
    /// </summary>
    public void ResetGravity()
    {
        if (!_isJump)
        {
            _moveVelocity.y = default;
        }
        _gravityScale = default;
    }

    /// <summary>
    /// 左右移動メソッド
    /// </summary>
    public void Move(float speed)
    {
        _moveVelocity.x = speed;
    }

    /// <summary>
    /// ジャンプメソッド
    /// </summary>
    public void Jump(float jumpForce)
    {
        _isJump = true;
        if(_jumpForce == default)
        {
            _jumpForce = jumpForce;
        }
        else
        {
            if(_jumpForce - _subtractJumpForce >= 0)
            {
                _jumpForce -= _subtractJumpForce * Time.deltaTime;
            }
        }
        _moveVelocity.y = _jumpForce;
    }

    /// <summary>
    /// ジャンプ終了メソッド
    /// </summary>
    public void CancelJump()
    {
        _isJump =  false;
        _jumpForce = default;
    }

    /// <summary>
    /// 飛行移動処理
    /// </summary>
    public void Fly(float speed,Vector3 targetPos)
    {
        _isFly = true;
        _isGoal = false;
        //移動地点を取得
        _moveTargetPos = targetPos;
        _flySpeed = speed;
    }

    /// <summary>
    /// 飛行解除処理
    /// </summary>
    public void StopFly()
    {
        _isFly = false;
        _isGoal = true;
        _moveTargetPos = transform.position;
    }

    /// <summary>
    /// エネミーの方向転換処理
    /// </summary>
    public float ChangeScale(Vector2 playerPos,float startXScale)
    {
        Vector2 nowScale = transform.localScale;
        float moveDirection = default;

        //プレイヤー位置に応じて向きを変える処理
        Vector2 nowPos = gameObject.transform.position;
        if(nowPos.x < playerPos.x)
        {
            //プレイヤーが右にいる場合
            nowScale.x = startXScale * MOVE_LEFT;
            transform.localScale = nowScale;
            moveDirection = MOVE_RIGHT;
        }
        else if(playerPos.x < nowPos.x)
        {
            //プレイヤーが左にいる場合
            nowScale.x = startXScale;
            transform.localScale = nowScale;
            moveDirection = MOVE_LEFT;
        }
        
        //移動方向を返す
        return moveDirection;
    }
    #endregion
}