using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private bool playerInRange = false; // 플레이어가 트리거 영역 내에 있는지 여부

    void Update()
    {
        WeaponAdd();
    }

    // 트리거 영역에 플레이어가 들어왔을 때 호출
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("플레이어 트리거 진입");
        }
    }

    // 트리거 영역에서 플레이어가 나갔을 때 호출
    void OnTriggerExit(Collider other)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("플레이어 트리거 이탈");
        }
    }

    private void WeaponAdd()
    {
        // 플레이어가 트리거 영역 내에 있고 G 키를 누르면 무기를 획득
        if (playerInRange && Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("G 키 눌림 - 무기 획득 시도");
            // WeaponManager 싱글톤 인스턴스가 존재하는지 확인
            if (WeaponManager.Instance != null)
            {
                Debug.Log("WeaponManager 인스턴스 존재");
                // 무기를 장착하고 현재 무기 오브젝트를 비활성화
                WeaponManager.Instance.EquipWeapon(gameObject);
                gameObject.SetActive(false);
                Debug.Log("무기 장착 완료");
            }
        }
    }
}