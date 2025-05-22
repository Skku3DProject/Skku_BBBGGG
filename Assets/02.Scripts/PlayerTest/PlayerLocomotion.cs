using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private ThirdPersonPlayer _player;

    private const float Gravity = -9.8f;
    private float _yVelocity;
    private float _currentSpeed;
    private int _jumpCount;
    private bool _isClimbing;
    private bool _isRunning;
    public float VerticalVelocity => _yVelocity;
    public bool IsRunning => _isRunning;

    private void Awake()
    {
        _player = GetComponent<ThirdPersonPlayer>();
        _currentSpeed = 4f;
        //_currentSpeed = _player.WalkSpeed;
    }

    private void Update()
    {
        HandleInput();
        ApplyMovement();
        UpdateMoveAnimation();

    }
    private void UpdateMoveAnimation_Directional(Vector3 inputDir)
    {
        _player.PlayerAnimator.SetFloat("MoveX", inputDir.x);
        _player.PlayerAnimator.SetFloat("MoveY", inputDir.z);
    }
    private void UpdateMoveAnimation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;
        float moveSpeed = inputDir.magnitude;

        UpdateMoveAnimation_Directional(inputDir);
        _player.PlayerAnimator.SetFloat("MoveSpeed", moveSpeed);

    }
    private void HandleInput()
    {
        Jump();

    }

    private void ApplyMovement()
    {
        Vector3 inputDir = GetInputDirection();
        Vector3 move = inputDir * _currentSpeed;

        // �߷� ����
        if (_player.CharacterController.isGrounded && _yVelocity < 0f)
        {
            _yVelocity = -1f;
            _jumpCount = 0; // ���� ����� �� ���� ī��Ʈ �ʱ�ȭ
        }
        else
        {
            _yVelocity += Gravity * Time.deltaTime;
        }

        move.y = _yVelocity;
        _player.CharacterController.Move(move * Time.deltaTime);
    }

    private Vector3 GetInputDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;
        dir = Camera.main.transform.TransformDirection(dir);
        dir.y = 0; // ���� �̵��� �ݿ�

        return dir;
    }


    private void Jump()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _yVelocity = /*_player.JumpPower;*/2f;
        }
        //bool canJump = _jumpCount < _player.MaxJumpCount && !_isClimbing;

        //if (Input.GetKeyDown(KeyCode.Space) && canJump)
        //{
        //    _yVelocity = _player.JumpPower;
        //    _jumpCount++;
        //}

        //if (_player.CharacterController.isGrounded)
        //{
        //    _jumpCount = 0;
        //    if (_yVelocity < 0f)
        //        _yVelocity = -1f; // ��¦ ������ �ٰ� �ϱ�
        //}
    }

}
