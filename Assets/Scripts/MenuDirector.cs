using UnityEngine;
using UnityEngine.UI;

public class MenuDirector : MonoBehaviour
{
	#region 変数  
	private static MenuDirector _menuInstance;
    [SerializeField] private Canvas _canvas;
	[SerializeField] private Slider _bgmSlider;
	[SerializeField] private Slider _seSlider;
    [SerializeField] private AudioSource _sourceBGM = default;
    [SerializeField] private AudioSource _sourceSE =  default;
    [SerializeField] private AudioClip _menuClip;
    [SerializeField] private AudioClip _magicClip;
    [SerializeField] private AudioClip _collisionClip;
    private const float PLAY_TIME = 1;
    #endregion

    #region プロパティ
    public static MenuDirector Instance
    {
        get { return _menuInstance; }
    }
    #endregion

    #region メソッド  
    private void Awake()
    {
        //インスタンス生成
        if (_menuInstance == null)
        {
            _menuInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //ロードで破壊されなくする
        DontDestroyOnLoad(gameObject);
        _canvas.enabled = false;

        //初期化
        _seSlider = _seSlider.GetComponent<Slider>();
        _bgmSlider = _bgmSlider.GetComponent<Slider>();
        _sourceSE = GetComponent<AudioSource>();
        _sourceSE.Play();
    }

    private void FixedUpdate()
    {
        //SEの音量を変更
        _sourceSE.volume = _seSlider.value;

        //BGMの音量を変更
        _sourceBGM.volume = _bgmSlider.value;
    }

    /// <summary>
    /// メニュー操作SE再生
    /// </summary>
    public void PlayMenuSE()
    {
        _sourceSE.PlayOneShot(_menuClip);
    }

    /// <summary>
    /// 魔法ヒットSE再生
    /// </summary>
    public void PlayMagicSE()
    {
        _sourceSE.PlayOneShot(_magicClip);
    }

    /// <summary>
    /// 接触SE再生
    /// </summary>
    public void PlayCollisionSE()
    {
        _sourceSE?.PlayOneShot(_collisionClip);
    }

    /// <summary>
    /// カンバス表示・非表示処理
    /// </summary>
    public void CanvasEnableDisable()
    {
        if(_canvas.enabled == true)
        {
            _canvas.enabled = false;
            Time.timeScale = PLAY_TIME;
        }
        else
        {
            _canvas.enabled = true;
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// カンバス非表示処理
    /// </summary>
    public void CloseCanvas()
    {
        _canvas.enabled = false;
        Time.timeScale = PLAY_TIME;
    }
    #endregion
}