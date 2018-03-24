using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;







public class PlayerController : MonoBehaviour
{

    public float JumpForce;
    public float MoveSpeed;
    private int _moveDirection = 0;
    private bool _facingRight = true;
    public enum Sides {LEFT, RIGHT};
    public Sides Facing = Sides.LEFT;
    private Rigidbody2D _rigidbody;
    public bool Grounded = false;
    public LayerMask WhatIsGround;
    private Collider2D _collider;



    // Use this for initialization
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<Collider2D>();
    }




    // Update is called once per frame
    void Update()
    {

        Grounded = Physics2D.IsTouchingLayers(_collider, WhatIsGround);

        if (Grounded)
        {

            _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) _moveDirection++;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) _moveDirection--;
        if (Input.GetKeyUp(KeyCode.RightArrow)) _moveDirection--;
        if (Input.GetKeyUp(KeyCode.LeftArrow)) _moveDirection++;
        
        if (_moveDirection == 1)
        {
            if (Facing == Sides.RIGHT)
            {
                Facing = Sides.LEFT;
                Flip();
            }
        }
        else if (_moveDirection == -1)
        {
            if (Facing == Sides.LEFT)
            {
                Facing = Sides.RIGHT;
                Flip();
            }
        }
    }
    void Flip()
    {
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        gameObject.transform.localScale = localScale;
    }
}

