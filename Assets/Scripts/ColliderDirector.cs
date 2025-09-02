using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自作コライダーの定義と衝突判定を行うクラス
/// </summary>
public class ColliderDirector : MonoBehaviour
{
    #region 変数
    //接触処理変数
    private ColliderDirector _nowCheckCollider = default;
    private bool _isHit = false;

    //コライダータイプ変数    
    private ColliderShape _colliderShape = default;
    [SerializeField] private ColliderShape _myColliderShape;
    [SerializeField] private ColliderType _myColliderType;
    [SerializeField] private bool _canThrough = false;

    //コライダーサイズ変数
    private Vector2 _circleCenter = default;
    [SerializeField] private float _circleRadius;
    private Rect _boxRect;
    private GameObject _colliderParent;
    [SerializeField] private Vector2 _leftUpPos = default;
    [SerializeField] private Vector2 _rightUpPos = default;
    [SerializeField] private Vector2 _leftDownPos = default;
    [SerializeField] private Vector2 _rightDownPos = default;
    #endregion

    #region プロパティ
    public Vector2 GetCircleCenter => _circleCenter;

    public float GetCircleRadius => _circleRadius;

    public Rect GetBoxRect => _boxRect;

    public ColliderShape GetColliderShape => _myColliderShape;
    #endregion

    #region メソッド
    /// <summary>
    /// コライダーの形
    /// </summary>
    public enum ColliderShape
    {
        Circle, //サークルコライダー
        Box,    //ボックスコライダー
    }

    /// <summary>
    /// コライダーの種類
    /// </summary>
    public enum ColliderType
    {
        Enemy, //敵コライダー
        Damage, //ダメージコライダー
        Player, //プレイヤーコライダー
        Ground, //地面コライダー
    }

    private void Start()
    {
        //コライダー管理オブジェクト取得 
        _colliderParent = transform.root.gameObject;

        UpdateCollider();
    }

    private void Update()
    {
        //コライダー位置を更新する処理
        UpdateCollider();
    }

    /// <summary>
    /// コライダー位置更新処理
    /// </summary>
    private void UpdateCollider()
    {
        switch (_myColliderShape)
        {
            case ColliderShape.Circle:
                //中心点を代入
                _circleCenter = transform.position;
            break;

            case ColliderShape.Box:
                //Rect構造体に要素を代入
                _boxRect.xMin = _leftUpPos.x + transform.position.x;
                _boxRect.xMax = _rightUpPos.x + transform.position.x;
                _boxRect.yMin = _leftUpPos.y + transform.position.y;
                _boxRect.yMax = _rightDownPos.y + transform.position.y;
            break;
        }
    }

    /// <summary>
    /// ほかコライダーと衝突しているかの判定メソッドを呼ぶ処理
    /// </summary>
    public bool CheckHit(ColliderShape shape, ColliderDirector createCollider)
    {
        //接触判定する相手のコライダー情報を代入
        _colliderShape = shape;
        _nowCheckCollider = createCollider;

        //相手のオブジェクトのコライダーの種類に合わせて処理メソッドを呼ぶ
        //(計算式の A * A >= (B - C).sqrMagnitude) は A >= BとCの距離 を求める場合の軽量な表現
        switch (_colliderShape)
        {
            case ColliderShape.Circle:
                CircleCheck();
            break;
                
            case ColliderShape.Box: 
                BoxCheck();
            break;
        }
        return _isHit;
    }

    /// <summary>
    /// 相手がサークルコライダーの場合、自分のコライダーの種類と合う衝突判定を行う
    /// </summary>
    private void CircleCheck()
    {
        //相手コライダーのサイズ変数を取得
        Vector2 center = _nowCheckCollider.GetCircleCenter;
        float radius = _nowCheckCollider.GetCircleRadius;

        //自身のコライダーの種類に基づいて衝突判定
        switch (_myColliderShape)
        {
            //円同士の衝突判定
            case ColliderShape.Circle:
                //二つの円の半径を足したものと中心点の距離を比べて接触判定をする
                float addRadius = _circleRadius + radius;
                _isHit = addRadius * addRadius >= (_circleCenter - center).sqrMagnitude;
            break;

            //円とボックスの衝突判定
            case ColliderShape.Box:
                //円の中心点とボックスの四点の距離を比べて接触判定する
                Vector2 maxPoint = new Vector2(_boxRect.xMax,_boxRect.yMax);
                Vector2 minPoint = new Vector2(_boxRect.xMin,_boxRect.yMin);
                float nearXPoint = Mathf.Clamp(center.x, minPoint.x, maxPoint.x);
                float nearYPoint = Mathf.Clamp(center.y, maxPoint.y, minPoint.y);
                Vector2 nearPoint = new Vector2(nearXPoint, nearYPoint);
                float distance = (center - nearPoint).sqrMagnitude;

                _isHit = (radius * radius >= distance);

                //通り抜け可能な床と魔法オブジェクトの組み合わせの場合ヒット判定を行わない
                if (_canThrough && _nowCheckCollider.gameObject.GetComponent<MagicBulletController>() is not null)
                {
                    _isHit = false;
                }
            break;
        }
    }

    /// <summary>
    /// 相手がボックスコライダーの場合、自分のコライダーの種類と合う衝突判定を行う
    /// </summary>
    private void BoxCheck()
    {
        //相手コライダーのサイズ変数を取得
        Rect otherBoxRect = _nowCheckCollider.GetBoxRect;
        Vector2 otherMaxPoint = new Vector2(otherBoxRect.xMax,otherBoxRect.yMax);
        Vector2 otherMinPoint = new Vector2(otherBoxRect.xMin,otherBoxRect.yMin);

        //自身のコライダーの種類に基づいて衝突判定
        switch (_myColliderShape)
        {
            //円とボックスの衝突判定
            case ColliderShape.Circle:
                //ボックスの四点と円の中心点を比べて接触判定をする
                float nearXPoint = Mathf.Clamp(_circleCenter.x, otherMinPoint.x, otherMaxPoint.x);
                float nearYPoint = Mathf.Clamp(_circleCenter.y, otherMaxPoint.y, otherMinPoint.y);

                float distanceX = _circleCenter.x - nearXPoint;
                float distanceY = _circleCenter.y - nearYPoint;
                float distance = distanceX * distanceX + distanceY * distanceY;

                _isHit = (_circleRadius * _circleRadius >= distance);
            break;

            //ボックス同士の衝突判定
            case ColliderShape.Box:
                //ボックスの四点をそれぞれ比較
                Vector2 maxPoint = new Vector2(_boxRect.xMax, _boxRect.yMax);
                Vector2 minPoint = new Vector2(_boxRect.xMin, _boxRect.yMin);
                _isHit = (_boxRect.xMax > otherBoxRect.xMin && otherBoxRect.xMax > _boxRect.xMin &&
                        _boxRect.yMin > otherBoxRect.yMax && otherBoxRect.yMin > _boxRect.yMax);

                //エネミーオブジェクトじゃない場合ここで処理を止める
                if (_nowCheckCollider.GetComponent<HitPointDirector>() is null)
                {
                    return;
                }

                //飛行エネミーと通り抜け可能な床の組み合わせの判定の場合ヒット判定を行わない
                if (_canThrough && _nowCheckCollider.gameObject.GetComponent<HitPointDirector>().GetBoolIsFly())
                {
                    return;
                }
                //足場オブジェクトと接触している場合、接触している辺を求める
                if (_myColliderType == ColliderType.Ground)
                {
                    if (_isHit)
                    {
                        float rightDis = Mathf.Abs(minPoint.x - otherMaxPoint.x);
                        float leftDis = Mathf.Abs(maxPoint.x - otherMinPoint.x);
                        float upDis = Mathf.Abs(maxPoint.y - otherMinPoint.y);
                        float downDis = Mathf.Abs(minPoint.y - otherMaxPoint.y);
                        float minDistance = Mathf.Min(upDis, downDis, rightDis, leftDis);

                        if (minDistance == rightDis)
                        {
                            _nowCheckCollider.GetComponent<HitPointDirector>().IsHitRight();
                        }
                        else if (minDistance == leftDis)
                        {
                            _nowCheckCollider.GetComponent<HitPointDirector>().IsHitLeft();
                        }
                        else if (minDistance == upDis)
                        {
                            if (!_canThrough)
                            {
                                _nowCheckCollider.GetComponent<HitPointDirector>().IsHitUp();
                            }
                        }
                        else if (minDistance == downDis)
                        {
                            _nowCheckCollider.GetComponent<HitPointDirector>().IsHitDown();
                        }
                    }
                }
                
            break;
        }
    }

    /// <summary>
    /// オブジェクトを後から生成する場合に親オブジェクトを設定する処理
    /// </summary>
    public void SetParentObject(GameObject parentObject)
    {
        _colliderParent = parentObject;
    }

    //コライダー描画メソッド
    private void OnDrawGizmos()
    {
        switch (_myColliderShape)
        {
            //サークルコライダーの描画
            case ColliderShape.Circle:
                Gizmos.DrawSphere(transform.position, _circleRadius);
            return;

            //ボックスコライダーの描画
            case ColliderShape.Box:
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_leftUpPos + (Vector2)transform.position, _leftDownPos + (Vector2)transform.position);
                Gizmos.DrawLine(_leftUpPos + (Vector2)transform.position, _rightUpPos + (Vector2)transform.position);
                Gizmos.DrawLine(_leftDownPos + (Vector2)transform.position, _rightDownPos + (Vector2)transform.position);
                Gizmos.DrawLine(_rightUpPos + (Vector2)transform.position, _rightDownPos + (Vector2)transform.position);
            return;
        }
    }
    #endregion
}
