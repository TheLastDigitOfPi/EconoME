using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(NavMeshAgent))]
public abstract class Mob : EntityCombatController
{
    [field: SerializeField] public bool IsScared { get; protected set; }
    [field: SerializeField] public bool IsHappy { get; protected set; }
    [field: SerializeField] public bool IsAngry { get; protected set; }
    [field: SerializeField] public bool IsNeutral { get; protected set; } = true;
    public bool IsAlive { get { return Health.CurrentHealth > 0; } protected set { } }

    [field: SerializeField] public float Speed { get; internal set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }

    protected bool CanIdle = true;
    [SerializeField] string _movementAnimationName = "Walk";
    [SerializeField] string _idleAnimationName = "Idle";

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Rigidbody = GetComponent<Rigidbody2D>();
        Renderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    public virtual void OnMobDeath()
    {
        Debug.Log("Mob is dead");
        Destroy(this.gameObject);
    }

    internal virtual void LostPlayer()
    {
        IsAngry = false;
        IsNeutral = true;
    }

    bool _isMoving;
    public virtual bool MoveTowardsPosition(Vector3 position)
    {
        if (Agent == null || !Agent.isOnNavMesh || Health.CurrentHealth <= 0)
            return false;
        if (!Agent.SetDestination(position))
            return false;
        Animator.CrossFade(_movementAnimationName, 0);
        if (!_isMoving)
            CheckFlipPos();
        async void CheckFlipPos()
        {
            _isMoving = true;
            while (Vector2.Distance(Agent.transform.position, Agent.pathEndPosition) > Agent.stoppingDistance + 0.2f)
            {
                FacingDirection = position.x > transform.position.x ? MoveDirection.Right : MoveDirection.Left;
                Renderer.flipX = FacingDirection == MoveDirection.Left;
                await Task.Delay(100);
            }
            _isMoving = false;
            if (CanIdle && Health.CurrentHealth > 0)
                Animator.CrossFade(_idleAnimationName, 0);
        }
        return true;
    }
}


public interface IMobWithAttack
{
    public EntityCombatController Target { get; }
    public event Action OnTargetEnterAggroRange;
    public event Action OnTargetLeaveAggroRange;
    public event Action OnTargetEnterAttackRange;
    public event Action OnTargetLeaveAttackRange;
    public bool TryGetTarget(out EntityCombatController target);
    public bool TryAttack(EntityCombatController target);
    public void TargetEnterAggroRange(EntityCombatController target);
    public void TargetExitAggroRange(EntityCombatController target);
    public void TargetEnterAttackRange();
    public void TargetExitAttackRange();
}