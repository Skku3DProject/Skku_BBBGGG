using System;
using UnityEngine;
[Serializable]
public struct CameraOffset
{
    public Vector3 PositionOffset;
    public Vector3 RotationOffset;
    public float FieldOfView;
}
public class ThirdPersonCameraMode
{
    private CameraOffset _offset;
    private float _rotationSpeed;
    private float _rotationX;
    private float _rotationY;


    public ThirdPersonCameraMode(CameraOffset offset, ref float rotationX, ref float rotationY, float rotationSpeed) //LayerMask layerMask)
    {
        _offset = offset;
        _rotationX = rotationX;
        _rotationY = rotationY;
        _rotationSpeed = rotationSpeed;

    }

    public void UpdateCamera(Transform cameraTransform, Transform target)
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _rotationX += mouseX * _rotationSpeed * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY + mouseY * _rotationSpeed * Time.deltaTime, -80f, 80f);

        // 카메라 위치
        cameraTransform.position = target.position + Quaternion.Euler(-_rotationY, _rotationX, 0f) * _offset.PositionOffset;

        // 회전 쿼터니언으로 적용
        Quaternion rotation = Quaternion.Euler(-_rotationY, _rotationX, 0f);
        cameraTransform.rotation = rotation;


    }
}
