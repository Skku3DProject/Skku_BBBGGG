using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;

    private float _gravity = -9.81f;
    private Vector3 _velocity;
    private bool _isGrounded;
    private CharacterController _characterController;
    private Animator _playerAnimator;

    [Header("Ground Check")]
    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;



    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 바닥 체크
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

  
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // 이동 입력
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        _characterController.Move(move * PlayerStats.MoveSpeed * Time.deltaTime);

        // 애니메이션 처리
        float moveSpeed = new Vector2(x, z).magnitude;
        _playerAnimator.SetFloat("MoveSpeed", moveSpeed, 0.1f, Time.deltaTime); // 부드러운 전환

        // 점프
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {

            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }

        // 중력
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

    }
}
