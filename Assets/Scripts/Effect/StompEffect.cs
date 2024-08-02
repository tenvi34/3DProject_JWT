using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompEffect : MonoBehaviour
{
    public float damage = 20f;
    public LayerMask playerLayer;
    
    void Start()
    {
        Destroy(gameObject, 2f); // 2초뒤 소멸
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0) // 비트 연산자 -> 특정 레이어 마스크와 비교할 때 사용
        {
            HealthSystem healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
                Debug.Log("충격파 데미지: " + damage);
            }
        }
    }
}
