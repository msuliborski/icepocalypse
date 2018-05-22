using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FightSystem : MonoBehaviour {
    [SerializeField]
    public GameObject Enemy;
    private int _animHash = Animator.StringToHash("Base Layer.PlayerPunch");

    //public Text StaminaText;
    public int MaxStaminaPoints = 100;
    private int _staminaPoints;
    private float _staminaRegenerationTime = 2.0f;
    private float _timeStamp;

    //public Text PlayerHealth;
    public int MaxHealthPoints = 100;
    private int _healthPoints;

    private bool _canIFight = true;
    private bool _sideFlag = false;
    private Animator _anim;

    public bool IsFighting = false;
    private bool _isDefending = false;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _staminaPoints = 20;
        //StaminaText.text = "Stamina: " + _staminaPoints;
        _healthPoints = MaxHealthPoints;
        //PlayerHealth.text = "Health: " + _healthPoints;
        _timeStamp = Time.time;
       // Debug.Log("time stamp: " + _timeStamp);
    }

    void Update()
    {
        if ( (Time.time - _timeStamp > _staminaRegenerationTime) )
        {
            SetStamina(5);
            _timeStamp = Time.time;
        }

        if ( IsFighting )
        {
            var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Base Layer.PlayerPunch"))
            {
                _sideFlag = true;
            }
            else if (stateInfo.IsName("Base Layer.PlayerDefend"))
            {
                _sideFlag = true;
            }

            if (_sideFlag == true && !stateInfo.IsName("Base Layer.PlayerPunch") && !stateInfo.IsName("Base Layer.PlayerDefend"))
            {
                //Debug.Log("nie ma animacji");
            }

            if ( _sideFlag == true && !stateInfo.IsName("Base Layer.PlayerPunch") && !stateInfo.IsName("Base Layer.PlayerDefend"))
            {
                _sideFlag = false;
                _isDefending = false;
                _canIFight = true;
                Enemy.GetComponent<EnemyController>()._isUnderAttack = false;
            }

            if (_canIFight)
            {
                if (Input.GetKeyDown(KeyCode.F) && _staminaPoints >= 10 )
                {
                    _canIFight = false;
                    //Debug.Log("animacja uruchomiona");
                    _anim.SetBool("playerattack", true);
                    Enemy.GetComponent<EnemyController>().Defend();
                    //SetStamina(-10);
                }
                else if ( Input.GetKeyDown(KeyCode.G) )
                {
                    _canIFight = false;
                    _isDefending = true;
                    //Debug.Log("animacja uruchomiona");
                    _anim.SetBool("playerdefend", true);
                }
            }
        }
    }

    public void ProceedToFight()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        
        if ( (transform.position.x < Enemy.transform.position.x && transform.localScale.x < 0) || (transform.position.x > Enemy.transform.position.x && transform.localScale.x > 0) )
        {
            transform.localScale = new Vector3(-1.0f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

    _canIFight = true;
    _sideFlag = false;
    _isDefending = false;

}

    void SetStamina( int value )
    {
        _staminaPoints += value;
        //Debug.Log("stamina: " + _staminaPoints);
        if ( _staminaPoints > MaxStaminaPoints )
        {
            _staminaPoints = MaxStaminaPoints;
        }
        else if ( _staminaPoints < 0 )
        {
            _staminaPoints = 0;
        }

        //StaminaText.text = "Stamina: " + _staminaPoints;
    }

    void LetThemFight()
    {
        _canIFight = true;
        Debug.Log("let them");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if ( col.gameObject.tag == "EnemyFist" && !_isDefending )
        {
            //SetHealth(-10);

            if ( _healthPoints <= 0 )
            {
                //Debug.Log("umrales");
                //PlayerHealth.enabled = false;
            }
        }
        else if (col.gameObject.tag == "EnemyFist" && _isDefending)
        {
            //Debug.Log("obrona");
        }

    }
    
    void SetHealth( int value )
    {
        _healthPoints += value;

        if ( _healthPoints > MaxHealthPoints )
        {
            _healthPoints = MaxHealthPoints;
        }
        else if ( _healthPoints < 0 )
        {
            _healthPoints = 0;
        }

        //PlayerHealth.text = "Health: " + _healthPoints;
    }

}
