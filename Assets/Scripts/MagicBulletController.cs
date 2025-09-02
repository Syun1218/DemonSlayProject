using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicBulletController : MonoBehaviour,IPoolSeter
{
    #region 変数
    //魔法ステータス変数
    [SerializeField] private MagicType _type = default;
    [SerializeField] private float _damage = default;
    [SerializeField] private float _speed = default;
    [SerializeField] private float _targetTime = default;
    [SerializeField] private float _myCoolTime = default;
    private float _objDirection = default;
    private float _nowTime = default;

    //ヒット判定変数
    private ColliderDirector _collider = default;
    private GameObject _myParent = default;
    private string _parentTag = default;
    private List<GameObject> _hitObjlist = new List<GameObject>();
    private GameObject _firstHitObj = default;

    //生成用変数
    private GameObject _poolObj = default;
    private bool _isActive = false;

    //移動変数
    private const float MOVE_RIGHT = 1;
    private const float MOVE_LEFT = -1;
    private PhysicsDirector _physicsDirector = default;
    private Vector3 _startScale = default;
    private float _startDirection = default;
    #endregion

    #region プロパティ
    public float GetCoolTime
    {
        get { return _myCoolTime; }
    }
    #endregion

    #region メソッド  
    //魔法の種類
    public enum MagicType
    {
        Normal, //通常の魔法弾
        Penetration, //貫通する魔法弾
    }

    private void Start()
    {
        _startScale = transform.localScale;
        _startDirection = transform.localScale.x;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //生成が完了していない場合処理をさせない
        if (!_isActive)
        {
            return;
        }

        //移動処理
        _physicsDirector.Move(_speed);

        //衝突しているオブジェクトが正しい対象かの判定処理
        for (int i = 0; i < ColliderList.Instance.MagicHitCollideres.Count; i++)
        {
            //接触確認しているオブジェクトが自分なら処理を飛ばす
            if (ColliderList.Instance.MagicHitCollideres[i].gameObject.name == gameObject.name)
            {
                continue;
            }

            //正しい対象と衝突していない場合衝突状況をリセット
            if (ColliderList.Instance.MagicHitCollideres[i].CheckHit(_collider.GetColliderShape, _collider) && _firstHitObj == default)
            {
                _firstHitObj = ColliderList.Instance.MagicHitCollideres[i].gameObject;
                switch (_parentTag)
                {
                    case "Player":
                        if (_firstHitObj.transform.tag != "Enemy")
                        {
                            _firstHitObj = default;
                        }
                    break;

                    case "Enemy":
                        if (_firstHitObj.transform.tag != "Player")
                        {
                            _firstHitObj = default;
                        }
                    break;
                }

                break;
            }
        }

        //すでに当たったオブジェクトの場合変数を初期化する
        if(_hitObjlist.Count > 0 && _firstHitObj != default)
        {
            foreach (GameObject obj in _hitObjlist)
            {
                if(obj.name == _firstHitObj.name)
                {
                    _firstHitObj = default;
                    break;
                }
            }
        }

        //正しい対象に当たっている場合ダメージ処理を呼ぶ
        if(_firstHitObj != default)
        {
            switch (_parentTag)
            {
                case "Player":
                    if (_firstHitObj.transform.tag == "Enemy")
                    {
                        _firstHitObj.GetComponent<IEnemyBace>().EnemyDamage(_damage);
                        Hit();
                    }
                break;

                case "Enemy":
                    if (_firstHitObj.transform.tag == "Player")
                    {
                        _firstHitObj.GetComponent<PlayerController>().PlayerDamage(_damage);
                        Hit();
                    }
                break;
            }
            _firstHitObj = default;
        }

        //消えるまでの時間を計る
        _nowTime += Time.deltaTime;

        //一定時間経過後に弾を回収する処理
        if(_nowTime >= _targetTime)
        {
            RemoveObject();
        }
    }

    //衝突時の処理
    private void Hit()
    {
        MenuDirector.Instance.PlayMagicSE();
        switch (_type)
        {
            case MagicType.Normal:
                RemoveObject();
            break;

            case MagicType.Penetration:
                ResetHitObj();
            break;
        }
    }

    //当たったオブジェクトを配列に入れ初期化する処理
    private void ResetHitObj()
    {
        _hitObjlist.Add(_firstHitObj);
        _firstHitObj = default;
    }

    //生成時処理
    public void SetMagicState(GameObject obj,float direction)
    {
        //変数を初期化
        _collider = GetComponent<ColliderDirector>();
        _physicsDirector = GetComponent<PhysicsDirector>();

        //呼び出したオブジェクトを取得
        _myParent = obj;
        _parentTag = _myParent.transform.tag;

        //オブジェクトの向きをセットする
        if (direction > 0)
        {
            direction = MOVE_LEFT;
            _speed *= MOVE_RIGHT;
        }
        else if (direction < 0)
        {
            direction = MOVE_RIGHT;
            _speed *= MOVE_LEFT;
        }
        _objDirection = direction * _startDirection;

        //オブジェクトの向きを変える処理
        Vector2 localVec = _startScale;
        localVec.x = _objDirection;
        transform.localScale = localVec;
        
        //消滅までの時間を初期化
        _nowTime = 0;

        _isActive = true;
    }

    //プールオブジェクト取得
    public void SetPoolObject(GameObject obj)
    {
        _poolObj = obj;
    }
    
    //このオブジェクトをプールに戻す処理
    private void RemoveObject()
    {
        _hitObjlist.Clear();
        ColliderList.Instance.RemoveColliderList(_collider);
        _isActive = false;
        _firstHitObj = default;
        _speed = Mathf.Abs(_speed);
        _poolObj.GetComponent<ObjectPoolDirector>().EnQueueObject(gameObject);
        
    }
    #endregion
}