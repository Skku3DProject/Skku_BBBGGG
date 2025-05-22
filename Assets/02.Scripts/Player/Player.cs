using UnityEngine;

public class Player : MonoBehaviour, IDamageAble
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;
    private float _currentHealth;

    private float _gravity = -9.81f;
    private Vector3 _velocity;
    private CharacterController _characterController;
    private Animator _playerAnimator;
    public Animator Animator => _playerAnimator;

    [Header("Upper Body Bone")]
    public Transform upperBody; // 상체 회전용 본 (Spine 또는 Chest)

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();
        _currentHealth = PlayerStats.MaxHealth;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(x, 0, z).normalized;

        float speed = inputDir.magnitude;

        // 이동 처리
        if (speed > 0.1f)
        {
            // 이동
            _characterController.Move(inputDir * PlayerStats.MoveSpeed * Time.deltaTime);

            // 하체 회전
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 애니메이션 파라미터 설정
        _playerAnimator.SetFloat("MoveX", inputDir.x);
        _playerAnimator.SetFloat("MoveZ", inputDir.z);
        _playerAnimator.SetFloat("Speed", speed); // 걷기 / 뛰기 전환용

        // 점프 (선택 사항)
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }

        // 중력 처리
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        // 상체 회전 처리
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (inputDir.sqrMagnitude > 0.1f && upperBody != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDir.normalized);
            Quaternion smoothed = Quaternion.Slerp(upperBody.rotation, targetRot, Time.deltaTime * 10f);
            upperBody.rotation = smoothed;
        }
    }

    public void TakeDamage(Damage damage)
    {
        _currentHealth -= damage.Value;
        Debug.Log($"플레이어 {damage.Value}만큼 피해를 입음");
        _playerAnimator.SetTrigger("SwordGetHit");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("사망...");
        // 죽음 애니메이션 등 추가 가능
    }
}

/*using UnityEngine;

public class Player : MonoBehaviour, IDamageAble
{

    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;
    private float _currnetHealth;


    private float _gravity = -9.81f;
    private Vector3 _velocity;
    private bool _isGrounded;
    private CharacterController _characterController;
    private Animator _playerAnimator;
    public Animator Animator => _playerAnimator;

    [Header("Ground Check")]
   // public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;



    private void Start()
    {

        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();

        _currnetHealth = PlayerStats.MaxHealth;
    }

    private void Update()
    {

        // 바닥 체크
        //_isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);


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
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)//_isGrounded)
        {

            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }

        // 중력
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

    }



    //데미지
    public void TakeDamage(Damage damage)
    {
        _currnetHealth -= damage.Value;
        
        Debug.Log($"플레이어 {damage.Value}만큼 피해를 입음");
        _playerAnimator.SetTrigger("SwordGetHit");
        if (_currnetHealth <= 0)
        {
            Die();
        }
    }

    //사망
    private void Die()
    {
       // _playerAnimator.SetTrigger("SwordDie");
        Debug.Log("사망...");
    }
}*/
