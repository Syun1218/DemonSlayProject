using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolDirector : MonoBehaviour
{
    #region 変数  
    //生成時変数
    private GameObject _instanceParent = default;
    [SerializeField] private GameObject _instanceObject = default;
    [SerializeField] private Vector3 _instancePos = new Vector3(-500,-500,0);
    [SerializeField] private int _instanceCount = 50;
    private Queue<GameObject> _objQueue = new Queue<GameObject>();

    //貸出リスト変数
    private string _objName = default;
    private List<string> _dequeueList = new List<string>();
    #endregion

    #region プロパティ
    public GameObject GetInstanceObject
    {
        get { return _instanceObject; }
    }
    #endregion

    #region メソッド  
    private void Start()
    {
        //オブジェクトの名前を取得
        _objName = _instanceObject.name;
        _instanceParent = GameObject.FindWithTag("Parent");
        
        //指定個数オブジェクトを生成
        for(int i = 0;i < _instanceCount; i++)
        {
            GameObject obj = Instantiate(_instanceObject, _instancePos, Quaternion.identity, _instanceParent.transform);
            obj.GetComponent<ColliderDirector>().SetParentObject(_instanceParent);
            obj.name = _objName + i;
            obj.GetComponent<IPoolSeter>().SetPoolObject(gameObject);
            _objQueue.Enqueue(obj);
        }
    }

    /// <summary>
    /// プールから取り出す処理
    /// </summary>
    public GameObject DequeueObject(Vector3 pos)
    {
        if (_objQueue.Count > 0)
        {
            //オブジェクトを取り出す
            GameObject obj = _objQueue.Dequeue();
            
            if(obj.GetComponent<IEnemyBace>() is not null)
            {
                //敵オブジェクトを取り出した場合敵の総数を増やす処理
                EnemyCountMaster.Instance.AddCount();
                obj.GetComponent<IEnemyBace>().InitializationEnemyState();
            }

            //オブジェクトを指定された位置に渡し、有効化する
            obj.transform.position = pos;
            obj.SetActive(true);
            _dequeueList.Add(obj.name);
            ColliderList.Instance.AddColliderList(obj.GetComponent<ColliderDirector>());
            return obj;
        }
        else
        {
            //取り出せるオブジェクトがない場合の処理
            return default;
        }
    }

    /// <summary>
    /// プールに返す処理
    /// </summary>
    public void EnQueueObject(GameObject obj)
    {
        for(int i = 0;i < _dequeueList.Count;i++)
        {
            if (obj.name == _dequeueList[i])
            {
                obj.SetActive(false);
                _objQueue.Enqueue(obj);
                
                //敵を返却した場合総数を減らす処理
                if(obj.GetComponent<IEnemyBace>() is not null)
                {
                    EnemyCountMaster.Instance.RemoveCount();
                }
            }
        }
        //貸出リストから要素を削除
        _dequeueList.Remove(obj.name);
    }
    #endregion
}