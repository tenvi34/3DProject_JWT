using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    private GameObject _currentWeapon; // 현재 장착중인 무기
    private Transform _weaponSlot; // 장착하는 무기 슬롯 (오른손)
    
    public GameObject CurrentWeapon => _currentWeapon;

    private Animator _animator; // 애니메이터
    private static readonly int HasWeapon = Animator.StringToHash("EquipWeapon");

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Scene이 바뀌어도 파괴되지 않게 설정
        }
        else
        {
            Destroy(gameObject); // 이미 있으면 새로운 Instance 삭제
        }
    }

    void Start()
    {
        // 오른손에 장착할 WeaponSlot 찾기
        _weaponSlot = transform.Find("root/root.x/spine_01.x/spine_02.x/spine_03.x/shoulder.r/arm_stretch.r/forearm_stretch.r/hand.r/WeaponSlot");
        _animator = GetComponent<Animator>();
        
        if (_weaponSlot == null)
        {
            Debug.LogError("'WeaponSlot' 오브젝트를 찾을 수 없습니다.");
        }
        else
        {
            Debug.Log("'WeaponSlot' 오브젝트를 찾았습니다.");
        }
    }

    // 무기 장착
    public void EquipWeapon(GameObject weapon)
    {
        if (_currentWeapon != null)
        {
            Debug.Log("이미 장착중인 무기가 있습니다. 기존 무기를 제거합니다.");
            Destroy(_currentWeapon); // 기존 장착된 무기 제거
        }

        if (_weaponSlot != null)
        {
            // 무기를 인스턴스화하여 WeaponSlot 위치에 장착합니다.
            _currentWeapon = Instantiate(weapon, _weaponSlot);

            // 무기 위치 초기화
            _currentWeapon.transform.localPosition = Vector3.zero;
            _currentWeapon.transform.localRotation = Quaternion.identity;

            // 위치와 회전 조정 (필요에 따라 수정 가능)
            _currentWeapon.transform.localPosition = new Vector3(0, 0.1f, 0);
            _currentWeapon.transform.localRotation = Quaternion.Euler(-180, -30, -100);

            // Rigidbody 설정
            Rigidbody weaponRb = _currentWeapon.GetComponent<Rigidbody>();
            if (weaponRb != null)
            {
                weaponRb.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
                weaponRb.detectCollisions = false; // 물리 충돌 비활성화
            }

            // Collider 비활성화
            // Collider weaponCollider = _currentWeapon.GetComponent<Collider>();
            // if (weaponCollider != null)
            // {
            //     weaponCollider.enabled = false;
            // }

            if (_animator != null)
            {
                _animator.SetBool(HasWeapon, true);
            }

            Debug.Log("무기가 성공적으로 장착되었습니다.");
        }
        else
        {
            Debug.LogError("'WeaponSlot' 오브젝트를 찾을 수 없습니다.");
        }
    }

    public void UnequipWeapon()
    {
        if (_currentWeapon != null)
        {
            Destroy(_currentWeapon);
            _currentWeapon = null;

            if (_animator != null)
            {
                _animator.SetBool(HasWeapon, false);
            }
            
            Debug.Log("무기 해제 완료");
        }
        else
        {
            Debug.Log("장착중인 무기가 없습니다.");
        }
    }
}
