using System;
using GlobalSettings;
using UnityEngine;
using Utils;

public class CameraController : SingletonMono<CameraController>
{
    public Camera Camera => _camera;
    private Camera _camera;

    [Header("移动设置")]
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _moveSmoothTime = 0.2f;

    [Header("缩放设置")]
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _minZoom = 0.1f;
    [SerializeField] private float _maxZoom = 5f;
    
    private Vector3 _targetPosition;

    private Vector3 _moveVelocity;
    private float _rotationVelocity;
    private Vector3 _extraMovement;
    private static int moveLock = 0;
    public static void Lock() => moveLock++;
    public static void Unlock() => moveLock = Math.Max(moveLock - 1, 0);
    public void AddExtraMovement(Vector3 movement) { if (moveLock == 0) _extraMovement += movement; }

    void Start()
    {
        _camera = Camera.main;
        _targetPosition = _camera.transform.position;
    }
    void Update()
    {
        if (moveLock == 0) HandleInput();
        CameraMove();
    }

    private void HandleInput()
    {
        // 获取输入方向
        var moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey("up"))
            moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey("down"))
            moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A) || Input.GetKey("left"))
            moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey("right"))
            moveDirection += Vector3.right;
        
        // 处理缩放滚轮
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) 
        {
            var zoomDirection = scroll * _zoomSpeed * _targetPosition.y * _camera.transform.forward;
            if (_targetPosition.y + zoomDirection.y >= _minZoom && _targetPosition.y + zoomDirection.y <= _maxZoom) _targetPosition += zoomDirection;
        }
        
        if (moveDirection.magnitude > 0.1f) _targetPosition += _moveSpeed * Mathf.Sqrt(_targetPosition.y) * Time.deltaTime * (moveDirection.normalized);
        else _targetPosition += _extraMovement;
        
        _extraMovement = Vector3.zero;
    }

    private void CameraMove()
    {
        var bounds = GlobalConsts.CameraBounds;
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, bounds.xMin, bounds.xMax);
        _targetPosition.z = Mathf.Clamp(_targetPosition.z, bounds.zMin, bounds.zMax);
        _camera.transform.position = Vector3.SmoothDamp(
            _camera.transform.position,
            _targetPosition,
            ref _moveVelocity,
            _moveSmoothTime
        );
    }
    
    
}