using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildDogScript : MonoBehaviour
{
    private bool _dead = false;

    public GameObject CanvasObject;

    public PolygonCollider2D PlayerCollider;

    public float EnemyRunningSpeed = 5.0f;
    public float EnemyWalkingSpeed = 2.5f;
    public float FightingDeadZone = 0.5f;

    private GameObject _playerObject;

    private GameObject _circle = null;

    protected Rigidbody2D _rb;

    private float _playerEnemyDistance;
    public BoxCollider2D ViewRangeTrigger;

    private bool _isClutching;
    private float _time;
    private int _keyPressesIterator = 0;

    public GameObject QTECircle;

    private Animator _anim;

    [HideInInspector]
    public bool ClickedTheCircle = false;

    enum Facing
    {
        Right,
        Left
    }

    private Facing _facingD;

    enum PlayerState
    {
        Idle,
        Running,
        Attack
    }

    private PlayerState _dogState = PlayerState.Idle;

    void Start()
    {
        _anim = GetComponent<Animator>();

        _isClutching = false;
        _rb = GetComponent<Rigidbody2D>();

        _playerObject = GameObject.FindGameObjectWithTag("Player");

        if ( transform.localScale.x > 0f )
        {
            _facingD = Facing.Right;
        }
        else
        {
            _facingD = Facing.Left;
        }

    }

    void Update()
    {
        if (_dead)
            return;

        if ( !_isClutching )
        {
            _playerEnemyDistance = _playerObject.transform.position.x - transform.position.x;
            GetState();
        }
        else
        {
            DoQTEFight();
        }

    }

    void DoQTEFight()
    {
        if (_circle == null)
        {
            Vector2 vector = new Vector2(transform.position.x + 1.0f, transform.position.y + 0.5f);

            _circle = Instantiate(QTECircle, CanvasObject.transform);
            _circle.GetComponent<QTECircleScript>().FatherObject = gameObject;
            _circle.GetComponent<QTECircleScript>()._qteType = "Dog";
            _circle.GetComponent<QTECircleScript>().LifeTime = 0f;
        }

        Fight();
    }

    IEnumerator TurnSpriteOffInAWhile()
    {
        yield return new WaitForSecondsRealtime(0.35f);
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void Fight()
    { 
        //_rb.velocity = new Vector2((_playerObject.transform.position.x - transform.position.x) * 7.0f, (_playerObject.transform.position.y + 0.3f - transform.position.y) * 10.0f);

        if ( Time.time - _time >= 5.0f )
        {
            //_playerObject.SetActive(false);
            _playerObject.GetComponent<PlayerControllerExperimental>().TakeHP(_playerObject.GetComponent<PlayerControllerExperimental>().Hp);
            GetComponent<SpriteRenderer>().enabled = true;
            _isClutching = false;
            _dogState = PlayerState.Idle;
            Destroy(_circle);

            return;
        }

        if (ClickedTheCircle && _keyPressesIterator < 10)
        {
            ClickedTheCircle = false;
            _keyPressesIterator++;
        }
        
        if ( _keyPressesIterator >= 10 )
        {
            _playerObject.GetComponent<FightSystem>().DogIsDead();
            Destroy(_circle);
            Destroy(gameObject);

            _dead = true;
            _rb.isKinematic = true;
            GetComponent<BoxCollider2D>().isTrigger = true;
            transform.position = new Vector3(_playerObject.transform.position.x, _playerObject.transform.position.y - 0.5f, _playerObject.transform.position.z);
            //StartCoroutine(TurnSpriteOn());
        }
    }

    IEnumerator TurnSpriteOn()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        GetComponent<SpriteRenderer>().enabled = true;
    }
    /*
    void OnCollisionEnter2D(Collision2D col)
    {
       
    }
    */
    void ChangeFacingDirection()
    {
        if (_facingD == Facing.Right)
        {
            _facingD = Facing.Left;
        }
        else
        {
            _facingD = Facing.Right;
        }

        transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
    }

    void GetState()
    {
        switch (_dogState)
        {
            case PlayerState.Idle:

                break;

            case PlayerState.Running:
                Run(EnemyRunningSpeed);

                if (Mathf.Abs(_playerEnemyDistance) <= FightingDeadZone)
                {
                    _dogState = PlayerState.Attack;
                }

                break;

            case PlayerState.Attack:
                //_anim.SetBool("jump", true);
                _playerObject.GetComponent<FightSystem>().FallDown();
                _playerObject.GetComponent<FightSystem>().IsDogQTE = true;
                GetComponent<SpriteRenderer>().enabled = false;
                _isClutching = true;
                _time = Time.time;
                //StartCoroutine(FreezeTimeForAWhile());
                /*
                Vector2 vector = new Vector2(transform.position.x, transform.position.y + 0.5f);

                _circle = Instantiate(QTECircle, CanvasObject.transform);
                _circle.GetComponent<QTECircleScript>().FatherObject = gameObject;
                _circle.GetComponent<QTECircleScript>()._qteType = "Dog";
                _circle.GetComponent<QTECircleScript>().LifeTime = 0.3f;
                */
                break;
        }
    }

    IEnumerator FreezeTimeForAWhile()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 1.0f;
    }

    void Walk(float speed)
    {
        int _directionSpecifier = 1;

        if (_facingD == Facing.Left)
        {
            _directionSpecifier = -1;
        }

        _rb.velocity = new Vector2(speed * _directionSpecifier, _rb.velocity.y);
    }

    void Run(float speed)
    {
        if (_playerEnemyDistance > 0)
        {
            _rb.velocity = new Vector2(speed, _rb.velocity.y);
        }
        else if (_playerEnemyDistance < 0)
        {
            _rb.velocity = new Vector2(-speed, _rb.velocity.y);
        }
    }

    public void CatchPlayer()
    {
        _dogState = PlayerState.Running;

        if (transform.position.x < _playerObject.transform.position.x)
        {
            _playerObject.GetComponent<FightSystem>().GetReady(-1);
        }
        else
        {
            _playerObject.GetComponent<FightSystem>().GetReady(1);
        }

        _anim.SetBool("run", true);
    }

}
