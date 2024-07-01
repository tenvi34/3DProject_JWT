using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBackup : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField] private float walkSpeed = 40.0f;
    [SerializeField] private float runSpeed = 70.0f;
    
    private float applySpeed;

    [SerializeField] private float jumpForce = 7.0f;
    
    // 상태 변수
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;
    
    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;
    
    // 카메라 민감도
    [SerializeField] private float lookSensitivity;
    
    // 카메라 리미트
    [SerializeField] 
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    [SerializeField] private Camera theCamera;

    private Rigidbody myRigid;
    
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
    }

    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        Move();
        //CameraRotation();
        //CharacterRotation();
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }
    private void Jump()
    {
        myRigid.velocity = transform.up * jumpForce;
    }
    
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }
    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }
    
    private void Move()
    {
        // float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirX = Input.GetAxis("Horizontal");
        // float _moveDirZ = Input.GetAxisRaw("Vertical");
        float _moveDirZ = Input.GetAxis("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        // (1, 0, 0) + (0, 0, 1) = (1, 0, 1) => 2
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        // 상하 카메라 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharacterRotation()
    {
        // 좌우 카메라 회전
        float _yRotation = Input.GetAxis("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }
}
