using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class GolemController : MonoBehaviour
{
    public float lookRadius = 10f; // 시야 범위
    public float attackRange = 2f; // 공격 범위
    public float attackCoolTime = 1f; // 쿨타임
    public float attackDamage = 10f; // 공격 데미지

    private Transform _target; // 플레이어 위치
    private NavMeshAgent _agent; // NavMeshAgent 컴포넌트
    private Animator _animator;
    //private HealthSystem _playerHealthSystem;

    private float _lastAttackTime;
    private static readonly int IsWalk = Animator.StringToHash("Walk");
    private static readonly int IsAttack = Animator.StringToHash("Attack");
    private static readonly int IsDead = Animator.StringToHash("Die");
    private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");

    // 공격 관련 설정
    [SerializeField] private CapsuleCollider leftArmCollider; // 왼팔
    [SerializeField] private CapsuleCollider rightArmCollider; // 오른팔
    [SerializeField] private LayerMask playerLayer;

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        //_playerHealthSystem = _target.GetComponent<HealthSystem>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_animator.GetBool(IsDead)) return;

        float distance = Vector3.Distance(_target.position, transform.position);

        // 골렘의 시야 범위 안에 플레이어가 들어오면
        if (distance <= lookRadius)
        {
            _agent.SetDestination(_target.position); // 플레이어한테로 이동

            // 공격 범위 안에 들어오면
            if (distance <= attackRange)
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
            }
        }
        else
        {
            _animator.SetBool(IsWalk, false);
        }
    }

    void Attack()
    {
        _animator.SetInteger(AttackIndex, Random.Range(0, 3));
        _animator.SetTrigger(IsAttack); // 트리거로 설정하여 즉시 애니메이션 시작
        _animator.SetBool(IsWalk, false);
        _agent.isStopped = true; // 에이전트를 일시 중지하여 공격 애니메이션이 실행되도록 함
    }
    
    public void AttackFinish()
    {
        _agent.isStopped = false;
        _animator.ResetTrigger(IsAttack);

        // 공격이 끝나면 콜라이더 비활성화
        leftArmCollider.enabled = false;
        rightArmCollider.enabled = false;

        // 플레이어를 계속 추적하고 공격할 수 있도록 업데이트 호출
        Update();
    }

    // 왼팔 공격
    public void LeftArmAttack()
    {
        Debug.Log("왼팔 공격 감지");
        Collider[] hitPlayers = Physics.OverlapCapsule(
            leftArmCollider.bounds.center - leftArmCollider.transform.up * leftArmCollider.height / 2,
            leftArmCollider.bounds.center + leftArmCollider.transform.up * leftArmCollider.height / 2,
            leftArmCollider.radius, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            player.GetComponent<HealthSystem>()?.TakeDamage(attackDamage);
        }
    }

    // 오른팔 공격
    public void RightArmAttack()
    {
        Debug.Log("오른팔 공격 감지");
        Collider[] hitPlayers = Physics.OverlapCapsule(
            rightArmCollider.bounds.center - rightArmCollider.transform.up * rightArmCollider.height / 2,
            rightArmCollider.bounds.center + rightArmCollider.transform.up * rightArmCollider.height / 2,
            rightArmCollider.radius, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            player.GetComponent<HealthSystem>()?.TakeDamage(attackDamage);
        }
    }

    // 양팔 공격
    public void BothArmAttack()
    {
        Debug.Log("양팔 공격 감지");
        LeftArmAttack();
        RightArmAttack();
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