using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float runSpeed = 7.0f;
    [SerializeField] private float jumpForce = 3.0f;
    [SerializeField] private float lookSensitivity = 2.0f;
    [SerializeField] private float maxLookX = 60.0f;
    [SerializeField] private float minLookX = -60.0f;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Vector3 thirdPersonCameraOffset = new Vector3(0, 2, -4);
    [SerializeField] private Vector3 firstPersonCameraOffset = new Vector3(0.5f, 1.5f, 0);

    private float currentSpeed;
    private float rotX;

    private bool isMove;
    private bool isJump;
    private bool isRun;
    private bool isFirstPerson = false;

    // 애니메이터 캐싱
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirZ = Animator.StringToHash("DirZ");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Run = Animator.StringToHash("Run");

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        currentSpeed = moveSpeed;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        SetThirdPersonView();
    }

    void Update()
    {
        TryWalk();
        TryRun();
        TryJump();
        LookAround();
        SwitchView();
    }

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
        Vector3 moveDirection = playerCamera.transform.TransformDirection(direction);
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

        // 플레이어의 회전을 이동 방향으로만 설정
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 0.15f);
        }
    }

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

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX);

        playerCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + mouseX, 0);

        if (isFirstPerson)
        {
            // 1인칭 시점에서는 캐릭터 회전을 카메라 회전에 맞추지 않음
            playerCamera.transform.localRotation = Quaternion.Euler(rotX, playerCamera.transform.localRotation.eulerAngles.y + mouseX, 0);
        }
    }

    private void SwitchView()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
            if (isFirstPerson)
            {
                SetFirstPersonView();
            }
            else
            {
                SetThirdPersonView();
            }
        }
    }

    private void SetFirstPersonView()
    {
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = firstPersonCameraOffset;
        playerCamera.transform.localRotation = Quaternion.identity;
    }

    private void SetThirdPersonView()
    {
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = thirdPersonCameraOffset;
        playerCamera.transform.localRotation = Quaternion.identity;
    }
}
