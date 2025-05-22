using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float TurnSpeed = 10f; // Slerp 속도

    private void Start()
    {
    }

    private void Update()
    {
        RotateTowardCrosshair();


    }

    private void RotateTowardCrosshair()
    {
        //Vector3 forward = Camera.main.transform.forward;
        //forward.y = 0;
        //forward.Normalize();

        //if (forward != Vector3.zero)
        //{
        //    // 현재 캐릭터 바라보는 방향과 카메라 평면 방향의 각도 계산
        //    float angle = Vector3.Angle(transform.forward, forward);

        //    // 임계각도 이상일 때만 회전 수행
        //    if (angle > 1f) // ← 1도 이상 차이 날 때만 회전
        //    {
        //        Quaternion desiredRot = Quaternion.LookRotation(forward);
        //        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * TurnSpeed);
        //    }
        //}
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }
}
