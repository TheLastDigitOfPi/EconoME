using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;

public class AnimalController : MonoBehaviour
{
    StateMachine _stateMachine;
    [SerializeField] Vector3Variable playerPos;
    public bool Living = true;
    private BoxCollider2D boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        var animator = GetComponent<Animator>();
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var rigidbody = GetComponent<Rigidbody2D>();
        _stateMachine = new StateMachine();

        var Wander = new Wander(this, animator, spriteRenderer, rigidbody);
        var Death = new AnimalDeath(this, animator, spriteRenderer);
        var Scarred = new RunFromPlayer(this, animator, spriteRenderer, playerPos, rigidbody);

        _stateMachine.AddAnyTransition(Wander, IsAlive());
        _stateMachine.AddAnyTransition(Death, IsDead());
        _stateMachine.AddAnyTransition(Scarred, PlayerNearby());

        _stateMachine.SetState(Wander);
        Func<bool> IsAlive() => () => Living && Scarred.HasRun && !PlayerNearby().Invoke();
        Func<bool> IsDead() => () => !Living;
        Func<bool> PlayerNearby() => () => Vector3.Distance(playerPos.Value, transform.position) < 1.5f;
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    private void Start()
    {
        Physics2D.IgnoreCollision(boxCollider, GameObject.FindWithTag("Player").GetComponent<CircleCollider2D>());
    }
}

public class AnimalDeath : IState
{
    private readonly AnimalController _controller;
    private readonly Animator _animator;
    private readonly SpriteRenderer _renderer;
    private static readonly int Die = Animator.StringToHash("Die");

    float TimeDead = 0f;
    public AnimalDeath(AnimalController controller, Animator anim, SpriteRenderer sr)
    {
        _controller = controller;
        _animator = anim;
        _renderer = sr;
    }

    public void OnEnter()
    {
        _animator.CrossFade(Die, 0);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        TimeDead += Time.deltaTime;
        if (TimeDead > 4f)
        {
            UnityEngine.Object.Destroy(_controller.gameObject);
        }
    }
}
public class Wander : IState
{
    private readonly AnimalController _controller;
    private readonly Animator _animator;
    private readonly SpriteRenderer _renderer;
    private readonly Rigidbody2D _rigidBody;

    private Vector2 movePos;
    Vector2 _lastPosition = Vector2.zero;
    public float TimeStuck;
    public float WaitTime = 0f;
    float TimeToWait = UnityEngine.Random.Range(2f, 5f);
    bool Waiting = false;
    bool eating = false;


    private static readonly int Dig = Animator.StringToHash("Dig");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int IdleLook = Animator.StringToHash("Idle Look");
    private static readonly int Nibble = Animator.StringToHash("Nibble");
    private static readonly int Hop = Animator.StringToHash("Hop");

    private static readonly int NoAnim = Animator.StringToHash("NoAnim");

    public Wander(AnimalController controller, Animator anim, SpriteRenderer renderer, Rigidbody2D rb)
    {
        _controller = controller;
        _animator = anim;
        _renderer = renderer;
        _rigidBody = rb;
    }

    public void OnEnter()
    {
        _animator.CrossFade(IdleLook, 0);

        Waiting = true;
        eating = false;
        WaitTime = 0;
    }

    public void OnExit()
    {
        Waiting = true;
        eating = false;
        WaitTime = 0;
    }

    public void Tick()
    {
        if (!Waiting && !eating)
        {
            var moveTo = Vector2.MoveTowards(_rigidBody.position, movePos, Time.deltaTime * 5);
            _rigidBody.MovePosition(moveTo);
            if (_rigidBody.position == movePos)
            {
                Waiting = true;
                _animator.CrossFade(IdleLook, 0);
                return;
            }
            if (Vector2.Distance(_controller.transform.position, _lastPosition) <= 0.02f)
                TimeStuck += Time.deltaTime;
            _lastPosition = _controller.transform.position;

            if (TimeStuck > 3f)
            {
                Waiting = true;
                _animator.CrossFade(Idle, 0);
            }
            return;
        }
        WaitTime += Time.deltaTime;
        if (WaitTime > TimeToWait)
        {
            Waiting = false;
            eating = false;
            WaitTime = 0f;
            TimeToWait = UnityEngine.Random.Range(2f, 5f);
            int num = UnityEngine.Random.Range(0, 10);
            if (num == 0)
            {
                StartNibble();
                return;
            }
            MoveToNewPosition();

        }

    }
    void MoveToNewPosition()
    {
        _animator.CrossFade(Hop, 0);
        TimeStuck = 0f;
        movePos = _rigidBody.position + Extensions.GetRandomDir() * UnityEngine.Random.Range(1f, 2f);

        _renderer.flipX = movePos.x > _controller.transform.position.x ? false : true;

    }
    void StartNibble()
    {
        _animator.CrossFade(Nibble, 0);
        TimeStuck = 0f;
        eating = true;
    }
}
[System.Serializable]
public class RunFromPlayer : IState
{
    private readonly AnimalController _controller;
    private readonly Animator _animator;
    private readonly SpriteRenderer _renderer;
    private readonly Vector3Variable _playerPos;
    private readonly Rigidbody2D _rigidBody;
    private static readonly int Scarred = Animator.StringToHash("Scarred");
    private static readonly int Hop = Animator.StringToHash("Hop");


    Vector2 _lastPosition = Vector2.zero;
    public float TimeStuck;
    bool isRunning = false;
    public bool HasRun = true;
    Vector2 RunawayPos;
    float SupriseTime = 0;
    public RunFromPlayer(AnimalController controller, Animator anim, SpriteRenderer renderer, Vector3Variable playerPos, Rigidbody2D rb)
    {
        _controller = controller;
        _animator = anim;
        _renderer = renderer;
        _playerPos = playerPos;
        _rigidBody = rb;
    }

    public void OnEnter()
    {
        RunToNewPosition();
    }

    private void RunToNewPosition()
    {
        isRunning = false;
        HasRun = false;
        SupriseTime = 0;
        TimeStuck = 0;
        _animator.CrossFade(Scarred, 0);

    }

    public void OnExit()
    {
        isRunning = false;
        HasRun = true;
    }

    public void Tick()
    {
        if (isRunning)
        {
            _rigidBody.MovePosition(Vector2.MoveTowards(_rigidBody.position, RunawayPos, Time.deltaTime * 20));
            if (_rigidBody.position == RunawayPos)
            {
                isRunning = false;
                HasRun = true;
            }

            if (Vector2.Distance(_rigidBody.position, _lastPosition) <= 0.02f)
                TimeStuck += Time.deltaTime;
            _lastPosition = _rigidBody.position;

            if (TimeStuck > 3f)
            {
                isRunning = false;
                HasRun = true;
            }
            return;
        }
        SupriseTime += Time.deltaTime;
        if (SupriseTime > 0.4f && !isRunning)
        {
            isRunning = true;

            var DirFromPlayer = _rigidBody.position - (Vector2)_playerPos.Value;
            DirFromPlayer.Normalize();

            RunawayPos = _rigidBody.position + DirFromPlayer * UnityEngine.Random.Range(3f, 5f);
            _animator.CrossFade(Hop, 0);
            _renderer.flipX = RunawayPos.x > _rigidBody.position.x ? false : true;
        }
    }
}