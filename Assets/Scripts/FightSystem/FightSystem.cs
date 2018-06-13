﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FightSystem : MonoBehaviour {

    public int HealthPointsMax = 100;
    private int _healthPoints;

    private bool _canIFight = true;
    private bool _sideFlag = false;
    private Animator _anim;

    private bool _isDefending = false;

    [HideInInspector]
    public bool IsQTE;
    [HideInInspector]
    public bool IsDogQTE;
    [HideInInspector]
    public bool ClickedTheCircle;
    [HideInInspector]
    public bool ClickedAttack;

    public BoxCollider2D FistCollider;

    void Start()
    {
        FistCollider.enabled = false;

        IsQTE = false;
        _anim = GetComponentInChildren<Animator>();
        _healthPoints = HealthPointsMax;
    }

    public void CancelQTE()
    {
        _canIFight = true;
        IsQTE = false;
        IsDogQTE = false;
        StartCoroutine(GetComponent<PlayerControllerExperimental>().UnsetAttackingState());
        //GetComponent<PlayerControllerExperimental>().UnsetAttackingState();
    }

    void Update()
    {
        if ( IsQTE )
        {
            if ( ClickedTheCircle )
            {
                Debug.Log("super atak");
                ClickedTheCircle = false;
                _anim.SetBool("SuperAttack", true);
            }

            return;
        }
        else if (IsDogQTE)
            {
            if (ClickedTheCircle)
                {
                    Debug.Log("super atak");
                    ClickedTheCircle = false;
                    _anim.SetBool("SuperAttack", true);
                    _anim.SetBool("WatchOut", false);
                CancelQTE();
            }

            return;
            }

        var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Base Layer.PlayerPunchBare") || stateInfo.IsName("Base Layer.PlayerDefend") || stateInfo.IsName("Base Layer.PlayerSuperPunchBare"))
        {
            _sideFlag = true;
        }

        if (_sideFlag == true && !stateInfo.IsName("Base Layer.PlayerPunchBare") && !stateInfo.IsName("Base Layer.PlayerDefend") && !stateInfo.IsName("Base Layer.PlayerSuperPunchBare"))
        {
            _sideFlag = false;
            _isDefending = false;
            _canIFight = true;
            FistCollider.enabled = false;
        }

        
        if (_canIFight)
        {
            if ( ClickedAttack || Input.GetKey(KeyCode.F) )
            {
                ShootRay();

                ClickedAttack = false;
                _canIFight = false;
                FistCollider.enabled = true;
                _anim.SetTrigger("Attack");
                GetComponent<PlayerControllerExperimental>().StopMovement();
                                
            }
            else if ( Input.GetKeyDown(KeyCode.G) )
            {
                _canIFight = false;
                _isDefending = true;
                _anim.SetBool("playerdefend", true);
            }
         }


    }

    void ShootRay()
    {
        RaycastHit2D hit;

        Vector2 RayDirection = new Vector2(transform.position.x + 0.7f, transform.position.y + 0.5f);
        Debug.DrawRay(RayDirection, transform.localScale.x * Vector3.right * 5.0f, Color.yellow, 2.0f);
        int layerMask = LayerMask.GetMask("Enemy");
        hit = Physics2D.Raycast(RayDirection, transform.localScale.x * Vector3.right, 5.0f,layerMask );
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.gameObject.SendMessage("Defend");
                Debug.Log("wyslalaem");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if ( col.gameObject.tag == "EnemyFist" && !_isDefending )
        {
            //SetHealth(-10);

            if ( _healthPoints <= 0 )
            {
                //Debug.Log("umrales");
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

        if ( _healthPoints > HealthPointsMax)
        {
            _healthPoints = HealthPointsMax;
        }
        else if ( _healthPoints < 0 )
        {
            _healthPoints = 0;
        }
    }

    public void GetReady( int side )
    {
        GetComponent<PlayerControllerExperimental>().SetAttackingState();

        if ( side == 1  && transform.localScale.x < 0f )
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        else if ( side == -1 && transform.localScale.x > 0f )
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        Debug.Log("wywoluje");
        _anim.SetBool("WatchOut", true);
        _anim.SetBool("Movement", false);
        GetComponent<PlayerControllerExperimental>().StopMovement();
    }

    public void FallDown()
    {
        //GetComponent<BoxCollider2D>().isTrigger = true;
        //GetComponent<Rigidbody2D>().isKinematic = true;
        Debug.Log("upadam");
        //GetComponent<PlayerControllerExperimental>().SetAttackingState();
        _anim.SetBool("FallDown", true);
        _anim.SetBool("WatchOut", false);
    }

    public void DogIsDead()
    {
        Debug.Log("dajemy animacje zabijania psa");
        _anim.SetBool("KillTheDog", true);
        _anim.SetBool("FallDown", false);
        CancelQTE();
    }

}
