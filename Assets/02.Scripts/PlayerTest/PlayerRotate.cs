using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float TurnSpeed = 10f; // Slerp ¼Óµµ

    private void Start()
    {
    }

    private void Update()
    {
        RotateTowardCrosshair();


    }

    private void RotateTowardCrosshair()
    {
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(camForward);
            transform.rotation = lookRot;// Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
        
    }
}
