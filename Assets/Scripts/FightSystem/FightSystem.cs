﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FightSystem : MonoBehaviour {

    public GameObject AttackButtonToDisableWhenQTE;

    private bool _canIFight = true;
    private bool _sideFlag = false;
    private Animator _anim;

    private float _timeSinceGettingHit = 0f;

    public bool IsUnderAttack = false;
    private bool superFlaga = true;

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
    }

    public IEnumerator CancelQTE( string guy_dog_now )
    {
        if (guy_dog_now == "guy" )
        {
            yield return new WaitForSecondsRealtime(2.0f);
        }
        else if (guy_dog_now == "dog")
        {
            yield return new WaitForSecondsRealtime(1.0f);
        }

        _canIFight = true;
        IsQTE = false;
        IsDogQTE = false;
        GetComponent<PlayerControllerExperimental>().UnsetAttackingState();
        AttackButtonToDisableWhenQTE.SetActive(true);
        superFlaga = true;
    }

    public void ProceedToQTE()
    {
        IsQTE = true;
        GetComponent<PlayerControllerExperimental>().SetAttackingState();
        AttackButtonToDisableWhenQTE.SetActive(false);
    }

    void Update()
    {
            if (IsDogQTE)
            {
            if (ClickedTheCircle)
                {
                    ClickedTheCircle = false;
                    _anim.SetBool("SuperAttack", true);
                    _anim.SetBool("WatchOut", false);
                    CancelQTE( "now" );
            }

            return;
            }

        var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Base Layer.PlayerPunchBare") || stateInfo.IsName("Base Layer.PlayerGotHitAnimation") || stateInfo.IsName("Base Layer.PlayerSuperPunchBare") || stateInfo.IsName("Base Layer.PlayerGotHitAnimation"))
        {
            _sideFlag = true;
        }

        if (_sideFlag == true && !stateInfo.IsName("Base Layer.PlayerPunchBare") && !stateInfo.IsName("Base Layer.PlayerGotHitAnimation") && !stateInfo.IsName("Base Layer.PlayerSuperPunchBare") && !stateInfo.IsName("Base Layer.PlayerGotHitAnimation"))
        {
            _sideFlag = false;
            _canIFight = true;
            FistCollider.enabled = false;
        }

        
        if (_canIFight && superFlaga)
        {
            if ( ClickedAttack || Input.GetKeyDown(KeyCode.F) )
            {
                ShootRay();

                ClickedAttack = false;
                _canIFight = false;
                FistCollider.enabled = true;
                _anim.SetBool("Attack", true);
                GetComponent<PlayerControllerExperimental>().StopMovement();
                                
            }
            else if ( Input.GetKeyDown(KeyCode.G) )
            {
                //_canIFight = false;
                //_isDefending = true;
               // _anim.SetBool("playerdefend", true);
            }
         }


    }

    public void KillTheGuyFinisher()
    {
        superFlaga = false;
        _anim.SetBool("KillTheGuy", true);
        StartCoroutine(CancelQTE("guy"));
    }

    void ShootRay()
    {
        RaycastHit2D hit;

        Vector2 RayDirection;

        if ( transform.localScale.x > 0f )
        {
            RayDirection = new Vector2(transform.position.x + 0.7f, transform.position.y - 0.3f);
        }
        else
        {
            RayDirection = new Vector2(transform.position.x - 0.7f, transform.position.y -0.3f);
        }

        Debug.DrawRay(RayDirection, transform.localScale.x * Vector3.right * 5.0f, Color.yellow, 2.0f);
        int layerMask = LayerMask.GetMask("Enemy");
        hit = Physics2D.Raycast(RayDirection, transform.localScale.x * Vector3.right, 5.0f,layerMask );
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.gameObject.SendMessage("Defend");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if ( col.gameObject.tag == "Enemy" )
        {
            GetComponent<PlayerControllerExperimental>().SetMoveDirection();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if ( col.gameObject.tag == "EnemyFist" && IsUnderAttack && (Time.time - _timeSinceGettingHit >= 1.0f) ) 
        {
            _timeSinceGettingHit = Time.time;

            IsUnderAttack = false;

            _anim.SetBool("GotHit", true);

            GetComponent<PlayerControllerExperimental>().TakeHP(10);

            _canIFight = false;
        }

    }

    public void GetReady( int side )
    {
        _anim.SetBool("Movement", false);
        _anim.SetBool("Ladder", false);
        _anim.SetBool("LadderMovement", false);
        _anim.SetBool("Tube", false);
        _anim.SetBool("Inert Down", false);
        _anim.SetBool("Slope", false);
        _anim.SetBool("Tube Idle", false);
        _anim.SetBool("Czekaning", false);
        _anim.SetBool("WallReflection", false);
        _anim.SetBool("JumpToWall", false);
        _anim.SetBool("Up", false);
        _anim.SetBool("Up1", false);
        _anim.SetBool("Up2", false);
        _anim.SetBool("UpLadder", false);
        _anim.SetBool("UpWall", false);
        _anim.SetBool("JumpIdle", false);
        _anim.SetBool("InertDown2", false);

        GetComponent<PlayerControllerExperimental>().SetAttackingState();

        if ( side == 1  && transform.localScale.x < 0f )
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        else if ( side == -1 && transform.localScale.x > 0f )
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        _anim.SetBool("WatchOut", true);
        _anim.SetBool("Movement", false);
        GetComponent<PlayerControllerExperimental>().StopMovement();
    }

    public void FallDown()
    {
        //GetComponent<PlayerControllerExperimental>().SetAttackingState();
        _anim.SetBool("FallDown", true);
        _anim.SetBool("WatchOut", false);
    }

    public void DogIsDead()
    {
        IsDogQTE = false;
        _anim.SetBool("KillTheDog", true);
        _anim.SetBool("FallDown", false);
        StartCoroutine(CancelQTE("dog"));
        superFlaga = true; 
    }

    public void FinisherFromAir()
    {
        _anim.SetBool("FinisherFromAir", true);
        //Time.timeScale = 0.2f;
    }

    public void KeepFalling()
    {
        _anim.SetBool("KeepFalling", true);
    }

}
