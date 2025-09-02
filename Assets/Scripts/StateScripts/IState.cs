using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IState
{
    //状態遷移時の処理
    public void OnStart();

    //この状態中の処理
    public void OnUpdate();

    //状態終了時の処理
    public void OnEnd();
}