using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void OnTitleButton()
    {
        //タイトルシーンへ遷移ボタン
        MenuDirector.Instance.PlayMenuSE();
        MenuDirector.Instance.CloseCanvas();
        SceneManager.LoadScene("TitleScene");
    }

    public void OnButtleButton()
    {
        //戦闘シーンへ遷移ボタン
        MenuDirector.Instance.PlayMenuSE();
        SceneManager.LoadScene("NormalScene");
    }

    public void OnCloseMenu()
    {
        //メニュー非表示ボタン
        MenuDirector.Instance.PlayMenuSE();
        MenuDirector.Instance.CloseCanvas();
    }
}