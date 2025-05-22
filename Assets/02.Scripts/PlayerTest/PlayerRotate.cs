using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float TurnSpeed = 10f; // Slerp �ӵ�

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
        //    // ���� ĳ���� �ٶ󺸴� ����� ī�޶� ��� ������ ���� ���
        //    float angle = Vector3.Angle(transform.forward, forward);

        //    // �Ӱ谢�� �̻��� ���� ȸ�� ����
        //    if (angle > 1f) // �� 1�� �̻� ���� �� ���� ȸ��
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
