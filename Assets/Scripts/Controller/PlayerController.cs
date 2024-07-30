using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    // Inspector 설정
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float runSpeed = 7.0f;
    [SerializeField] private float jumpForce = 3.0f;
    [SerializeField] private float lookSensitivity = 1.0f;
    [SerializeField] private float maxLookX = 30.0f;
    [SerializeField] private float minLookX = -30.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Vector3 thirdPersonCameraOffset = new Vector3(0, 2, -4);
    [SerializeField] private Vector3 firstPersonCameraOffset = new Vector3(0, 1.5f, 0.5f);

    private float currentSpeed;
    private float rotX;

    // 동작 체크
    private bool isMove;
    private bool isJump;
    private bool isRun;
    private bool isFirstPerson = false;
    private bool isAttack = false;

    // 애니메이터 캐싱
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirZ = Animator.StringToHash("DirZ");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int HasWeapon = Animator.StringToHash("EquipWeapon");

    // HP System
    private HealthSystem _healthSystem;

    // 공격 관련 설정
    [SerializeField] private CapsuleCollider swordCollider;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask enemyLayer;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _healthSystem = GetComponent<HealthSystem>();
        currentSpeed = moveSpeed;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        SetThirdPersonView();

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (_healthSystem.CurrentHp <= 0) return;
        TryWalk(); // 걷기
        TryRun(); // 달리기
        TryJump(); // 점프
        LookAround(); // 마우스 화면 회전
        SwitchView(); // 시점 변환
        TryAttack(); // 공격
        ChangeWeaponIdle(); // 무기 든 상태에서의 Idle로 변경
    }

    // 걷기
    private void TryWalk()
    {
        float _dirX = Input.GetAxis("Horizontal");
        float _dirZ = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(_dirX, 0, _dirZ);
        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }

        // 카메라의 로컬 좌표계를 기준으로 방향을 변환
        Vector3 moveDirection = transform.TransformDirection(direction);
        moveDirection.y = 0; // y축 방향을 0으로 고정하여 캐릭터가 수직으로 움직이지 않도록 함
        moveDirection.Normalize(); // 이동 벡터를 정규화하여 일정한 속도를 유지

        isMove = direction.magnitude > 0;

        if (isMove)
        {
            transform.Translate(moveDirection * (currentSpeed * Time.deltaTime), Space.World);
        }

        _animator.SetBool(Move, isMove);
        _animator.SetFloat(DirX, _dirX);
        _animator.SetFloat(DirZ, _dirZ);
    }

    // 달리기
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
            currentSpeed = runSpeed;
        }
        else
        {
            isRun = false;
            currentSpeed = moveSpeed;
        }

        _animator.SetBool(Run, isRun);
    }

    // 점프
    private void TryJump()
    {
        // 착지 확인
        if (isJump)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(transform.position, -transform.up, out hitInfo, _collider.bounds.extents.y + 0.1f))
            {
                isJump = false;
            }
        }

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            isJump = true;
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z); // 기존의 y축 속도를 0으로 초기화
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _animator.SetTrigger(Jump);
        }
    }

    // 마우스 화면 회전
    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX);

        playerCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + mouseX, 0);
    }

    // 시점 변환
    private void SwitchView()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
            if (isFirstPerson)
            {
                SetFirstPersonView(); // 1인칭
            }
            else
            {
                SetThirdPersonView(); // 3인칭
            }
        }
    }

    // 1인칭
    private void SetFirstPersonView()
    {
        playerCamera.transform.localPosition = firstPersonCameraOffset;
        playerCamera.transform.localRotation = Quaternion.identity;
    }

    // 3인칭
    private void SetThirdPersonView()
    {
        playerCamera.transform.localPosition = thirdPersonCameraOffset;
        playerCamera.transform.localRotation = Quaternion.identity;
    }

    // 공격
    private void TryAttack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttack)
        {
            _animator.SetTrigger(Attack);
            isAttack = true;
        }
    }

    public void AttackFinish()
    {
        isAttack = false;
    }

    public void DealDamage()
    {
        // 공격 범위 안에 적 감지
        Collider[] hitEnemies = Physics.OverlapCapsule(
            swordCollider.bounds.center - swordCollider.transform.up * swordCollider.height / 2,
            swordCollider.bounds.center + swordCollider.transform.up * swordCollider.height / 2,
            swordCollider.radius, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<HealthSystem>()?.TakeDamage(attackDamage);
        }
    }

    // 시야 범위 표시 디버그
    private void OnDrawGizmosSelected()
    {
        if (swordCollider == null) return;

        Gizmos.DrawWireSphere(swordCollider.bounds.center - swordCollider.transform.up * swordCollider.height / 2, swordCollider.radius);
        Gizmos.DrawWireSphere(swordCollider.bounds.center + swordCollider.transform.up * swordCollider.height / 2, swordCollider.radius);
    }

    // 무기 든 상태에서의 Idle로 변경
    private void ChangeWeaponIdle()
    {
        _animator.SetBool(HasWeapon, WeaponManager.Instance.CurrentWeapon != null);
    }
}