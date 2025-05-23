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

    //// ↓ 충돌용 필드 추가
    //private float _collisionRadius = 0.3f;         
    //private float _collisionOffset = 0.1f;
    //private LayerMask _collisionLayerMask;



    public ThirdPersonCameraMode(CameraOffset offset, ref float rotationX, ref float rotationY, float rotationSpeed) //LayerMask layerMask)
    {
        _offset = offset;
        _rotationX = rotationX;
        _rotationY = rotationY;
        _rotationSpeed = rotationSpeed;
        //_collisionLayerMask = layerMask;
    }

    public void UpdateCamera(Transform cameraTransform, Transform target)
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _rotationX += mouseX * _rotationSpeed * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY + mouseY * _rotationSpeed * Time.deltaTime, -50f, 50f);

        // 카메라 위치
        cameraTransform.position = target.position + Quaternion.Euler(-_rotationY, _rotationX, 0f) * _offset.PositionOffset;

        // 회전 쿼터니언으로 적용
        Quaternion rotation = Quaternion.Euler(-_rotationY, _rotationX, 0f);
        cameraTransform.rotation = rotation;

        //// 기본 카메라 회전 및 위치 계산
        //Quaternion rotation = Quaternion.Euler(-_rotationY, _rotationX, 0f);
        //Vector3 desiredPos = target.position + rotation * _offset.PositionOffset;

        //// 충돌 체크용 기준점
        //Vector3 lookTarget = target.position;
        //Vector3 moveVec = desiredPos - lookTarget;
        //float moveDist = moveVec.magnitude;
        //Vector3 moveDir = moveVec.normalized;


        //float collisionRadius = 0.4f;   
        //float collisionOffset = 0.2f;   // 벽과의 최소 거리

        //if (Physics.SphereCast(
        //        lookTarget,
        //        collisionRadius,
        //        moveDir,
        //        out RaycastHit hit,
        //        moveDist,
        //        _collisionLayerMask))
        //{

        //    Vector3 slideDir = Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;


        //    float slideDist = Mathf.Max(hit.distance - collisionOffset, 0.1f);


        //    cameraTransform.position = lookTarget + slideDir * slideDist;
        //}
        //else
        //{
        //    // 충돌 없으면 원래 위치
        //    cameraTransform.position = desiredPos;
        //}

        //// 회전 적용
        //cameraTransform.rotation = rotation;

    }
}
