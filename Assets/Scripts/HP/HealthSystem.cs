using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private ImgsFillDynamic hpUI;
    [SerializeField] private float maxHp = 100f;
    private float _currentHp;
    
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
        _currentHp -= damage;
        _currentHp = Mathf.Clamp(_currentHp, 0, maxHp); // 체력의 범위를 0에서 최대 체력으로 지정
        UpdateHpUI();
    }

    public void Heal(float healHp)
    {
        _currentHp += healHp;
        _currentHp = Mathf.Clamp(_currentHp, 0, maxHp); // 체력의 범위를 0에서 최대 체력으로 지정
        UpdateHpUI();
    }
    
}
