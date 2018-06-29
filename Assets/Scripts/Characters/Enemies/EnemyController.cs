using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public int EnemyHealthPointsMax = 100;
    private int _enemyHealthPoints;

    public bool _isUnderAttack = false;

    private float _animatingTime = 0f;

    public float EnemyRunningSpeed = 5.0f;
    public float EnemyWalkingSpeed = 2.5f;
    public float FightingDeadZone = 0.5f;

    private GameObject _playerObject;

    private Rigidbody2D _rb;

    public Transform PatrolDistanceTransformLeft;
    public Transform PatrolDistanceTransformRight;

    private float _patrolDistance;
    private float _patrolRangeA;
    private float _patrolRangeB;
    private float _triggerRange;

    private float _playerEnemyDistance;
    
    private Animator _anim;


    private bool _canHeFight = true;
    private bool _sideFlag = false;

    public Collider2D WeaponCollider;

    private bool _wait = false;
    private float _waitTime = 2.0f;
    private float _waitFlag;

    private bool _dead = false;

    bool _gameStarted = false;

    //public GameObject HitFlag;

    enum Facing
    {
        Right,
        Left
    }

    private Facing _facing;

	enum PlayerState 
	{
		Idle,
        Walking,
		Running,
		Attacking
	}

	private PlayerState _playerState = PlayerState.Walking;

	
	void Start ()
	{
        //Time.timeScale = 0.1f;

        //HitFlag.SetActive(false);

        WeaponCollider.enabled = false;

        _anim = GetComponent<Animator>();
        _enemyHealthPoints = EnemyHealthPointsMax;

        _rb = GetComponent<Rigidbody2D>();
        _playerObject = GameObject.FindGameObjectWithTag("Player");

        _patrolRangeA = PatrolDistanceTransformLeft.position.x;
        _patrolRangeB = PatrolDistanceTransformRight.position.x;

        //Debug.Log("A: " + _patrolRangeA + " B: " + _patrolRangeB);

	}

    public void OnGameStarted()
    {
        _gameStarted = true;
    }

    public void OnPlayerDeath()
    {
        _gameStarted = false;
    }

	void Update () 
	{
        if (_dead || !_gameStarted)
            return;

        var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Base Layer.Attack") || stateInfo.IsName("Base Layer.EnemyDefendAnimation"))
        {
            _sideFlag = true;
        }

        if ( _sideFlag && !stateInfo.IsName("Base Layer.Attack") && !stateInfo.IsName("Base Layer.EnemyDefendAnimation") )
        {
            _canHeFight = true;
            _sideFlag = false;
            WeaponCollider.enabled = false;
        }

        if ( Input.GetKey(KeyCode.Space) )
        {
            //_anim.SetBool("attack", true);
        }
        else
        {
            //_anim.SetBool("attack", false);
        }

        _playerEnemyDistance = _playerObject.transform.position.x - transform.position.x;

        GetState();

        if(  _playerState == PlayerState.Walking && ( transform.position.x <= _patrolRangeA && _facing == Facing.Left ) ||  _playerState == PlayerState.Walking && ( transform.position.x >= _patrolRangeB && _facing == Facing.Right ) )
        {
            _anim.SetBool("stand", true);
            StartCoroutine(Wait());
            _playerState = PlayerState.Idle;
        }
       
	}

    public void DestroyYourself()
    {
        //StartCoroutine(DestroyItself());
        Destroy(gameObject);
    }

    public void TurnOffYourPhysics()
    {
        _rb.isKinematic = true;
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    IEnumerator DestroyItself( float x )
    {
        yield return new WaitForSecondsRealtime(x);
        Destroy(gameObject);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(_waitTime);
        _anim.SetBool("stand", false);

        if( _playerState == PlayerState.Idle )
        {
            ChangeFacingDirection();
            _playerState = PlayerState.Walking;
        }
    }

    void OnCollisionEnter2D( Collision2D col )
    {
        if ( col.gameObject.tag == "Player" )
        {
            _rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            _playerState = PlayerState.Attacking;

            _anim.SetBool("readyToAttack", true);

            if ( ( _playerObject.transform.localScale.x > 0f && transform.localScale.x > 0f && _playerObject.transform.position.x < transform.position.x && _facing == Facing.Right ) || (_playerObject.transform.localScale.x < 0f && transform.localScale.x < 0f && _playerObject.transform.position.x > transform.position.x && _facing == Facing.Left))
            {
                ChangeFacingDirection();
            }
        }
    }

    void OnTriggerEnter2D( Collider2D col )
    {
        if (col.gameObject.tag == "PlayerFist" && _isUnderAttack )
        {
            //HitFlag.SetActive(!HitFlag.active);

            _anim.SetBool("gotHit", true);

            SetHealth(-10);

            Debug.Log("enemy health: " + _enemyHealthPoints);
            _isUnderAttack = false;

            if (_enemyHealthPoints <= 0)
            {
                _playerObject.GetComponent<FightSystem>().KillTheGuyFinisher();
                StartCoroutine(DestroyItself(0.8f));
            }
        }
    }

    void SetHealth(int value)
    {
        _enemyHealthPoints += value;
    }

    void ChangeFacingDirection()
    {
        if (_facing == Facing.Right)
        {
            _facing = Facing.Left;
        }
        else
        {
            _facing = Facing.Right;
        }

        transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void GetState()
    {
        switch( _playerState )
        {
            case PlayerState.Idle:
               // Debug.Log("idldeldleldldl");
                Stay();
                break;

            case PlayerState.Walking:
                //Debug.Log("walking");
                Walk(EnemyWalkingSpeed);
                break;

            case PlayerState.Running:
               // Debug.Log("running");
                Run(EnemyRunningSpeed);

                if (Mathf.Abs(transform.position.y - _playerObject.transform.position.y) >= 3.0f)
                {
                    _playerState = PlayerState.Walking;
                    _anim.SetBool( "run", false );

                    Debug.Log(" z biegu przechodzi do spacerku");

                    if ( _playerObject.transform.position.x > transform.position.x )
                    {
                        _patrolRangeB = transform.position.x;
                        _patrolRangeA = transform.position.x - 2.0f;
                    }
                    else
                    {
                        _patrolRangeA = transform.position.x;
                        _patrolRangeB = transform.position.x + 2.0f;
                    }
                }

                if (Mathf.Abs(_playerEnemyDistance) <= FightingDeadZone)
                {
                    _playerState = PlayerState.Attacking;
                    _anim.SetBool("readyToAttack", true);
                    //Debug.Log("Attacking , distance: " + Mathf.Abs(_playerEnemyDistance));
                }

                break;

            case PlayerState.Attacking:
                //   Debug.Log("attacking");

                if (Mathf.Abs(transform.position.y - _playerObject.transform.position.y) >= 3.0f)
                {
                    _playerState = PlayerState.Walking;
                    _anim.SetBool("run", false);

                    if (_playerObject.transform.position.x > transform.position.x)
                    {
                        _patrolRangeB = transform.position.x;
                        _patrolRangeA = transform.position.x - 2.0f;
                    }
                    else
                    {
                        _patrolRangeA = transform.position.x;
                        _patrolRangeB = transform.position.x + 2.0f;
                    }
                }

                _rb.velocity = new Vector2(0f, _rb.velocity.y);

                if (_canHeFight)
                    StartCoroutine(Attack());

                if (Mathf.Abs(_playerEnemyDistance) > FightingDeadZone)
                {
                    _playerState = PlayerState.Running;
                    _anim.SetBool("readyToAttack", false);
                    _anim.SetBool("run", true);
                    _rb.constraints = RigidbodyConstraints2D.None;
                    _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                break;
        }
    }

    void Stay()
    {
        _rb.velocity = new Vector2(0f, _rb.velocity.y);
    }

    void Walk( float speed )
    {
        int _directionSpecifier = 1;

        if (_facing == Facing.Left)
        {
            _directionSpecifier *= -1;
        }

        _rb.velocity = new Vector2(speed*_directionSpecifier, _rb.velocity.y);
    }

    void Run(float speed)
    {
        if (_playerEnemyDistance>0)
        {
            _rb.velocity = new Vector2(speed, _rb.velocity.y);
            if ( _facing == Facing.Left )
            {
                ChangeFacingDirection();
            }
            //Debug.Log("Running left");
        }
        else if (_playerEnemyDistance<0)
        {
            _rb.velocity = new Vector2(-speed, _rb.velocity.y);
            if ( _facing == Facing.Right )
            {
                ChangeFacingDirection();
            }
            //Debug.Log("Running left");
        }
    }

    IEnumerator Attack()
    {
        float x = Random.Range(0.5f, 2.0f);
        _canHeFight = false;

        yield return new WaitForSecondsRealtime(x);

        WeaponCollider.enabled = true;
        _playerObject.GetComponent<FightSystem>().IsUnderAttack = true;
        _anim.SetBool("attack", true);

    }

    public void Defend()
    {
        if ( _playerState == PlayerState.Attacking )
        {
            _isUnderAttack = true;
        }
    }

    public void CatchPlayer()
    {
        _anim.SetBool("run", true);
        _playerState = PlayerState.Running;
    }

    public void GotShoot()
    {
        Debug.Log("dostal");

        _anim.SetBool("shootDie", true);
        _rb.velocity = new Vector2(0f, 0f);
        StartCoroutine(DestroyItself(2.0f));
        _dead = true;
        // Destroy(gameObject);
    }

}
