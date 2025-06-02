using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private ThirdPersonPlayer _player;
    public float TurnSpeed = 10f; // Slerp ¼Óµµ

    private void Start()
    {
        _player = GetComponent<ThirdPersonPlayer>();
    }

    private void Update()
    {
        if (!_player.IsAlive || _player.IsReturning) return;


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
