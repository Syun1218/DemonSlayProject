using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    #region 変数  
    [SerializeField] private float _zPos = -50;
    [SerializeField] private float _yPosoffset = default;
    [SerializeField] private GameObject _camera = default;
    [SerializeField] private GameObject _player = default;
    private Vector3 _playerPos = Vector3.zero;
    #endregion

    #region メソッド  
    private void FixedUpdate()
    {
        //カメラを追従させる処理
        _playerPos = _player.transform.position;
        _camera.transform.position = _playerPos + Vector3.up * _yPosoffset;
        _camera.transform.position += Vector3.forward * _zPos;
    }
    #endregion
}