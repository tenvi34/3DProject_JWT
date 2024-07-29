using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHP : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem; // HealthSystem 참조

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) // J 키를 누르면 체력 감소
        {
            healthSystem.TakeDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.H)) // H 키를 누르면 체력 증가
        {
            healthSystem.Heal(10f);
        }
    }
}
