using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SummonDirector : MonoBehaviour
{
    #region 変数  
    [SerializeField] private GameObject _enemyPoolMaster;
    private List<ObjectPoolDirector> _summonPools =default;
    private int _rondomCount = 0;
    private List<Transform> _summonPoses = new List<Transform>();
    #endregion

    #region メソッド  
    private void Awake()
    {
        //敵プールを取得
        _summonPools = new List<ObjectPoolDirector>(_enemyPoolMaster.GetComponentsInChildren<ObjectPoolDirector>());
        for(int i = 0; i < transform.childCount; i++)
        {
            _summonPoses.Add(transform.GetChild(i));
        }
    }

    /// <summary>
    /// //ランダムなオブジェクトを生成処理
    /// </summary>
    public void SummonEnemy()
    {
        foreach (Transform obj in _summonPoses)
        {
            _rondomCount = UnityEngine.Random.Range(0, _summonPools.Count);
            _summonPools[_rondomCount].DequeueObject(obj.position);
        }
    }
    #endregion
}