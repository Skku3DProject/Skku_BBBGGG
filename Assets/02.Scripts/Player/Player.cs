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
    public Transform upperBody; // ��ü ȸ���� �� (Spine �Ǵ� Chest)

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

        // �̵� ó��
        if (speed > 0.1f)
        {
            // �̵�
            _characterController.Move(inputDir * PlayerStats.MoveSpeed * Time.deltaTime);

            // ��ü ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // �ִϸ��̼� �Ķ���� ����
        _playerAnimator.SetFloat("MoveX", inputDir.x);
        _playerAnimator.SetFloat("MoveZ", inputDir.z);
        _playerAnimator.SetFloat("Speed", speed); // �ȱ� / �ٱ� ��ȯ��

        // ���� (���� ����)
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }

        // �߷� ó��
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        // ��ü ȸ�� ó��
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
        Debug.Log($"�÷��̾� {damage.Value}��ŭ ���ظ� ����");
        _playerAnimator.SetTrigger("SwordGetHit");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("���...");
        // ���� �ִϸ��̼� �� �߰� ����
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

        // �ٴ� üũ
        //_isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);


        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // �̵� �Է�
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        _characterController.Move(move * PlayerStats.MoveSpeed * Time.deltaTime);

        // �ִϸ��̼� ó��
        float moveSpeed = new Vector2(x, z).magnitude;
        _playerAnimator.SetFloat("MoveSpeed", moveSpeed, 0.1f, Time.deltaTime); // �ε巯�� ��ȯ

        // ����
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)//_isGrounded)
        {

            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }

        // �߷�
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

    }



    //������
    public void TakeDamage(Damage damage)
    {
        _currnetHealth -= damage.Value;
        
        Debug.Log($"�÷��̾� {damage.Value}��ŭ ���ظ� ����");
        _playerAnimator.SetTrigger("SwordGetHit");
        if (_currnetHealth <= 0)
        {
            Die();
        }
    }

    //���
    private void Die()
    {
       // _playerAnimator.SetTrigger("SwordDie");
        Debug.Log("���...");
    }
}*/
