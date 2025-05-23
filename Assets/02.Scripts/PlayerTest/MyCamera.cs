using System;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private Transform _target;
    [SerializeField] private CameraOffset _thirdPersonOffset;
    [SerializeField] private float _rotationSpeed = 300f;
    [SerializeField] private LayerMask cameraCollisionLayer;

    private ThirdPersonCameraMode _thirdPerson;

    private float _rotationX;
    private float _rotationY;


    // 카메라 쉐이크용
    private Vector3 _shakeOffset = Vector3.zero;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _shakeOffset = Vector3.zero;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _thirdPerson = new ThirdPersonCameraMode(_thirdPersonOffset, ref _rotationX, ref _rotationY, _rotationSpeed);//,cameraCollisionLayer);
    }

    private void LateUpdate()
    {
        _thirdPerson.UpdateCamera(transform, _target);

    }
    private void Update()
    {
        // 쉐이크 오프셋 적용
        transform.position += _shakeOffset;
    }

}
