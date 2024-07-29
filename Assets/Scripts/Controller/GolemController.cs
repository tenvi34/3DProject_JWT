using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GolemController : MonoBehaviour
{
    public float lookRadius = 10f; // 시야 범위
    public float attackRange = 2f; // 공격 범위
    public float attackCoolTime = 1f; // 쿨타임
    public float damage = 10f; // 공격 데미지

    private Transform _target; // 플레이어 위치
    private NavMeshAgent _agent; // NavMeshAgent 컴포넌트
    private Animator _animator;
    private HealthSystem _playerHealthSystem;

    private float _lastAttackTime;
    private static readonly int IsWalk = Animator.StringToHash("Walk");
    private static readonly int IsAttack = Animator.StringToHash("Attack");
    private static readonly int IsDead = Animator.StringToHash("Die");

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _playerHealthSystem = _target.GetComponent<HealthSystem>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        float diastance = Vector3.Distance(_target.position, transform.position);

        // 골렘의 시야 범위 안에 플레이어가 들어오면
        if (diastance <= lookRadius)
        {
            _agent.SetDestination(_target.position); // 플레이어한테로 이동
            // 공격 범위안에 들어오면
            if (diastance <= attackRange)
            {
                if (Time.time - _lastAttackTime >= attackCoolTime)
                {
                    Attack(); // 공격
                    _lastAttackTime = Time.time; // 마지막 공격 시간 초기화
                }
            }
            else
            {
                _animator.SetBool(IsWalk, true);
                _animator.SetBool(IsAttack, false);
            }
        }
        else
        {
            _animator.SetBool(IsWalk, false);
        }
    }

    void Attack()
    {
        _animator.SetBool(IsAttack, true);
        _animator.SetBool(IsWalk, false);
        _agent.isStopped = true;
        
        // 공격 데미지 적용
        if (_playerHealthSystem != null)
        {
            _playerHealthSystem.TakeDamage(damage);
        }
        
        Invoke(nameof(ResumeMove), 1f);
    }

    void ResumeMove()
    {
        _agent.isStopped = false;
    }

    public void Die()
    {
        _animator.SetBool(IsDead, true);
        _agent.isStopped = true; // NavMeshAgent 중지
        
        // 아래에 골렘 소멸 기능 추가
        
    }
    
    // 시야 범위 표시 디버그
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
