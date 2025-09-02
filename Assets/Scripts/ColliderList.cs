using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ColliderList : MonoBehaviour
{
    #region 変数  
    private static ColliderList _listInstance;
    private GameObject _colliderParent = default;
    private List<ColliderDirector> _collideres = default;
    private List<ColliderDirector> _canHitMagicCollideres = new List<ColliderDirector>();
    #endregion

    #region プロパティ
    /// <summary>
    /// インスタンス取得プロパティ
    /// </summary>
    public static ColliderList Instance => _listInstance;

    /// <summary>
    /// 全コライダー取得プロパティ
    /// </summary>
    public List<ColliderDirector> Collideres => _collideres;

    /// <summary>
    /// 魔法のターゲットになるコライダー取得プロパティ
    /// </summary>
    public List<ColliderDirector> MagicHitCollideres => _canHitMagicCollideres;
    #endregion

    #region メソッド  
    private void Awake()
    {
        //インスタンス生成
        if(_listInstance == null)
        {
            _listInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    /// <summary>
    /// コライダー持ちオブジェクトをリストに追加
    /// </summary>
    public void AddColliderList(ColliderDirector collider)
    {
        _collideres.Add(collider);
        //敵かプレイヤーオブジェクトの場合魔法のターゲットリストにも追加
        if(collider.transform.tag == "Enemy" || collider.transform.tag == "Player")
        {
            _canHitMagicCollideres.Add(collider);
        }
    }

    /// <summary>
    /// リストから特定要素を削除
    /// </summary>
    public void RemoveColliderList(ColliderDirector collider)
    {
        _collideres.Remove(collider);
        //敵かプレイヤーオブジェクトの場合魔法のターゲットリストからも削除
        if (collider.transform.tag == "Enemy" || collider.transform.tag == "Player")
        {
            _canHitMagicCollideres.Remove(collider);
        }
    }

    /// <summary>
    /// シーン遷移時の処理
    /// </summary>
    void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
    {
        if(SceneManager.GetActiveScene().name == "BossScene" || SceneManager.GetActiveScene().name == "NormalScene")
        {
            //コライダー持ちのオブジェクトをリストに追加
            _colliderParent = GameObject.FindWithTag("Parent");
            _collideres = new List<ColliderDirector>(_colliderParent.GetComponentsInChildren<ColliderDirector>());
            
            //敵かプレイヤーオブジェクトのコライダーをリストに追加
            foreach(ColliderDirector collider in _collideres)
            {
                if(collider.transform.tag == "Enemy" || collider.transform.tag == "Player")
                {
                    _canHitMagicCollideres.Add(collider);
                }
            }
        }
    }

    /// <summary>
    /// シーン終了時にリスト初期化処理
    /// </summary>
    private void OnSceneUnloaded(Scene scene)
    {
        if(_collideres is not null)
        {
            _collideres.Clear();
            _canHitMagicCollideres.Clear();
        }
    }
    #endregion
}