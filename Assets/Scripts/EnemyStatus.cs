using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyStatus : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _maxHp;
    [SerializeField] private float _defultAttack;
    [SerializeField] private float _defultSpeed;
    [SerializeField] private bool _isBoss = false;
    [SerializeField] private float _idleTime = default;

    //ステータスを渡す
    public (string,float,float,float) GetEnemyState
    {
        get{ return (_name, _maxHp, _defultAttack, _defultSpeed); }
    }

    public bool GetIsBoss
    {
        get { return _isBoss; }
    }

    public float GetIdleTime
    {
        get { return _idleTime; }
    }
}
