using UnityEngine;

public interface IEnemyFly
{
    #region プロパティ
    //飛行エネミーの移動先取得プロパティ
    public Vector2 GetFlyPos { get; }
    #endregion
}