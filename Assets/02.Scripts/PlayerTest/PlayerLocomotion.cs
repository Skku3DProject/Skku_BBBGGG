using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private ThirdPersonPlayer _player;
    private PlayerAttack _playerAttack;

    private const float Gravity = -9.8f;
    private float _yVelocity;
    private float _currentSpeed;
    private int _jumpCount;
    private bool _isRunning;
    public float VerticalVelocity => _yVelocity;
    public bool IsRunning => _isRunning;

    private void Awake()
    {
        _player = GetComponent<ThirdPersonPlayer>();
        _playerAttack = GetComponent<PlayerAttack>();
        _currentSpeed = _player.PlayerStats.MoveSpeed;
        //_currentSpeed = _player.WalkSpeed;
    }

    private void Update()
    {
        HandleInput();
        ApplyMovement();
        UpdateMoveAnimation();
        if (_player.CurrentStamina < _player.PlayerStats.Stamina && !_isRunning && _player.CharacterController.isGrounded)
        {
            _player.RecoverStamina();
        }
        _player.PlayerAnimator.SetBool("OnGround", _player.CharacterController.isGrounded);
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
        Run();
    }

    private void ApplyMovement()
    {
        Vector3 inputDir = GetInputDirection();

        bool isMoveSlow = _playerAttack.IsMoveSlow;
        bool isAttacking = _playerAttack.CurrentWeaponAttack.IsAttacking;

        float speedMultiplier = (isAttacking || isMoveSlow) ? 0.2f : 1f;


        Vector3 move = inputDir * _currentSpeed * speedMultiplier;

        // 중력 적용
        if (_player.CharacterController.isGrounded && _yVelocity < 0f)
        {
            _yVelocity = -1f;
            _jumpCount = 0; // 땅에 닿았을 때 점프 카운트 초기화
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
        dir.y = 0; // 수평 이동만 반영

        return dir;
    }

    private void Run()
    {
        if (_playerAttack.CurrentWeaponAttack.IsAttacking == true) return;


        if (Input.GetKey(KeyCode.LeftShift) && _player.CurrentStamina > 0f)
        {

            _currentSpeed = _player.PlayerStats.MoveSpeed *1.5f;
            _isRunning = true;
            _player.UseStamina(20f);
            _player.PlayerAnimator.SetBool("IsRunning", _isRunning);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentSpeed = _player.PlayerStats.MoveSpeed;
            _isRunning = false;
            _player.PlayerAnimator.SetBool("IsRunning", _isRunning);
        }
    }

    private void Jump()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _yVelocity = _player.PlayerStats.JumpPower;

            if(_playerAttack.IsUsingJumpAnim)
                _player.PlayerAnimator.SetTrigger("Jump");
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
        //        _yVelocity = -1f; // 살짝 눌러서 붙게 하기
        //}
    }

}
