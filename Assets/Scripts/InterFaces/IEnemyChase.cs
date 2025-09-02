using UnityEngine;

public interface IEnemyChase
{
    #region プロパティ
    //追跡エネミーの追跡時間取得プロパティ
    public float GetChaseTime { get; }

    //追跡エネミーの追跡ターゲット取得プロパティ
    public Vector2 GetChasePos { get; }
    #endregion
}