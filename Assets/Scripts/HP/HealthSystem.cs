using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private ImgsFillDynamic hpUI;
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private Animator animator;
    private float _currentHp;
    private bool isDead = false;
    
    private static readonly int IsDead = Animator.StringToHash("Die");

    public float CurrentHp => _currentHp;
    
    void Start()
    {
        _currentHp = maxHp;
        UpdateHpUI();
    }
    
    private void UpdateHpUI()
    {
        if (hpUI != null)
        {
            float hpPercentage = _currentHp / maxHp;
            hpUI.SetValue(hpPercentage, true);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        _currentHp -= damage;
        _currentHp = Mathf.Clamp(_currentHp, 0, maxHp); // 체력의 범위를 0에서 최대 체력으로 지정
        UpdateHpUI();

        if (_currentHp <= 0)
        {
            PlayerDie();
        }
    }

    public void Heal(float healHp)
    {
        if (isDead) return;
        
        _currentHp += healHp;
        _currentHp = Mathf.Clamp(_currentHp, 0, maxHp); // 체력의 범위를 0에서 최대 체력으로 지정
        UpdateHpUI();
    }

    private void PlayerDie()
    {
        isDead = true;
        if (animator != null)
        {
            animator.SetTrigger(IsDead);
        }
        Debug.Log("플레이어 사망");
        // 아래에 게임 오버 화면 만들기
    }
    
    private void GolemDie()
    {
        isDead = true;
        if (animator != null)
        {
            animator.SetTrigger(IsDead);
        }
        Debug.Log("골렘 처치");
        // 아래에 골렘 소멸 기능 만들기
    }
    
}
