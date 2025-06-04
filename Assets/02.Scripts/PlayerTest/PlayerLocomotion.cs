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

    [SerializeField] private float walkStepDelay = 0.2f;
    [SerializeField] private float runStepDelay = 0.1f;

    private float _stepTimer = 0f;
    private bool _wasGrounded = true;


    private bool _isExhausted = false; // ��ħ ����
    private float _exhaustedTime = 3f; // ��ħ ���� �ð�
    private float _exhaustedTimer = 0f;
    private void Awake()
    {
        _player = GetComponent<ThirdPersonPlayer>();
        _playerAttack = GetComponent<PlayerAttack>();
        _currentSpeed = _player.PlayerStats.MoveSpeed;
        //_currentSpeed = _player.WalkSpeed;
    }

    private void Update()
    {
        if (!_player.IsAlive || _player.IsReturning) return;

        HandleInput();
        ApplyMovement();
        UpdateMoveAnimation();
        if (_player.CurrentStamina < _player.PlayerStats.Stamina && !_isRunning && _player.CharacterController.isGrounded)
        {
            _player.RecoverStamina();
        }
        _player.PlayerAnimator.SetBool("OnGround", _player.CharacterController.isGrounded);

        HandleFootstepSound();

        bool isGrounded = _player.CharacterController.isGrounded;

        if (isGrounded && !_wasGrounded)
        {
            PlayerSoundController.Instance.PlaySound(PlayerSoundType.FootStep); // �������� �߼Ҹ��� �����ϰų� JumpLanding Ÿ�� �߰�
        }

        _wasGrounded = isGrounded;
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


        float finalSpeed = _currentSpeed + _player.BuffSpeed;
        Vector3 move = inputDir * finalSpeed * speedMultiplier;

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

    private void Run()
    {
        if (_playerAttack.CurrentWeaponAttack.IsAttacking)
            return;

        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift);
        bool isMoving = GetInputDirection().magnitude > 0.1f;
        bool hasStamina = _player.CurrentStamina > 0f;

        // ��ħ ���� ���� ���̸� �޸��� �Ұ�
        if (_isExhausted)
        {
            _exhaustedTimer += Time.deltaTime;
            if (_exhaustedTimer >= _exhaustedTime)
            {
                _isExhausted = false; // ȸ����
                _exhaustedTimer = 0f;
            }

            _isRunning = false;
            _currentSpeed = _player.PlayerStats.MoveSpeed;
        }
        else if (isTryingToRun && isMoving && hasStamina)
        {
            _isRunning = true;
            _currentSpeed = _player.PlayerStats.MoveSpeed * 1.5f;
            _player.UseStamina(10f * Time.deltaTime);

            // ���¹̳ʰ� 0�� �Ǹ� ��ħ ���·� ��ȯ
            if (_player.CurrentStamina <= 0f)
            {
                _isExhausted = true;
                _exhaustedTimer = 0f;
            }
        }
        else
        {
            _isRunning = false;
            _currentSpeed = _player.PlayerStats.MoveSpeed;
        }

        _player.PlayerAnimator.SetBool("IsRunning", _isRunning);
    }

    private void Jump()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _yVelocity = _player.PlayerStats.JumpPower;

            if(_playerAttack.IsUsingJumpAnim)
            {
                _player.PlayerAnimator.SetTrigger("Jump");
                PlayerSoundController.Instance.PlaySound(PlayerSoundType.Jump);
            }
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
    private void HandleFootstepSound()
    {
        // ����: ���� �ְ�, �̵� ���̸�, ���� �� �ƴ�
        if (!_player.CharacterController.isGrounded) return;

        Vector3 inputDir = GetInputDirection();
        if (inputDir.magnitude < 0.1f) return;

        if (_playerAttack.CurrentWeaponAttack.IsAttacking) return;

        float stepDelay = _isRunning ? runStepDelay : walkStepDelay;

        _stepTimer -= Time.deltaTime;
        if (_stepTimer <= 0f)
        {
            PlayerSoundController.Instance.PlaySound(PlayerSoundType.FootStep);
            _stepTimer = stepDelay;
        }
    }
}
