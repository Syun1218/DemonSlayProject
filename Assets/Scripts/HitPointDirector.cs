using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitPointDirector : MonoBehaviour
{
    #region 変数  
    //コライダーとの衝突判定変数
    private IHitPointer _hitPointer = default;
    private List<HitState> _hitStates = new List<HitState>();
    private ColliderDirector _colliderDirector = default;
    private bool _isHit = false;
    private GameObject _nowCheckObj = default;
    private bool _isFly = false;
    #endregion

    #region メソッド  
    private void Start()
    {
        //インターフェイス取得
        _hitPointer = GetComponent<IHitPointer>();

        //コライダー変数の初期化
        _colliderDirector = GetComponent<ColliderDirector>();

        if (GetComponent<IEnemyBace>() is not null)
        {
            _isFly = GetComponent<IEnemyBace>().GetIsFly;
        }
    }

    private void Update()
    {
        //コライダーとの衝突判定
        for (int i = 0; i < ColliderList.Instance.Collideres.Count; i++)
        {
            if (ColliderList.Instance.Collideres[i].gameObject.name != gameObject.name)
            {
                _nowCheckObj = ColliderList.Instance.Collideres[i].gameObject;
                _isHit = ColliderList.Instance.Collideres[i].CheckHit(_colliderDirector.GetColliderShape, _colliderDirector);
                
                //プレイヤーへの接触ダメージ処理
                if(_isHit && gameObject.transform.tag == "Enemy" && _nowCheckObj.transform.tag == "Player")
                {
                    gameObject.GetComponent<IEnemyBace>().CollisionDamage();
                }
            }
        }
        
        //足場オブジェクトとの接触状況を確認
        //上方向の接触判定
        _hitPointer.HitUp(_hitStates.Contains(HitState.Up));

        //下方向の接触判定
        _hitPointer.HitDown(_hitStates.Contains(HitState.Dowm));

        //左方向の接触判定
        _hitPointer.HitLeft(_hitStates.Contains(HitState.Left));

        //右方向の接触判定
        _hitPointer.HitRight(_hitStates.Contains(HitState.Right));

        //要素を削除
        _hitStates.Clear();
    }

    //接触位置判定メソッド
    private enum HitState
    {
        Up,
        Dowm,
        Left,
        Right,
    }

    public void IsHitUp()
    {
        _hitStates.Add(HitState.Up);
    }

    public void IsHitDown()
    {
        _hitStates.Add(HitState.Dowm);
    }

    public void IsHitLeft()
    {
        _hitStates.Add(HitState.Left);
    }

    public void IsHitRight()
    {
        _hitStates.Add(HitState.Right);
    }

    public bool GetBoolIsFly()
    {
        return _isFly;
    }
    #endregion
}