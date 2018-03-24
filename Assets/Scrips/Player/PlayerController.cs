using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;







public class PlayerController : MonoBehaviour
{

    public float JumpForce;
    public float WallReflectionForce;
    public float MoveSpeed;

    public bool FacingRight = true;
    float _moveDirection = 1;
    
    public bool Grounded = false;
    public bool Walled = false;

    public LayerMask Ground;
    public LayerMask Wall;
    
    private Rigidbody2D _rigidbody;
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
        Grounded = Physics2D.IsTouchingLayers(_collider, Ground);
        Walled = Physics2D.IsTouchingLayers(_collider, Wall);

        if (Input.GetKeyDown(KeyCode.RightArrow)) {setFacingRight(true);}
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {setFacingRight(false);} 
        if (Input.GetKeyDown(KeyCode.DownArrow)) _moveDirection = 0;  

            
        
         if (Grounded)
        {
            _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);   

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            }
        }


        if (Walled)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - _moveDirection*WallReflectionForce, JumpForce);
                    
                if (FacingRight) 
                    setFacingRight(false);
                else 
                    setFacingRight(true);          
               
            }
        }

    }

    void setFacingRight(bool _facingRight)
    {
        if (_facingRight)
        {
            FacingRight = true;
            if (transform.localScale.x > 0) transform.localScale *= 1;
            else transform.localScale *= -1;
            _moveDirection = 1;
        }
        else 
        {
            FacingRight = false;
            if (transform.localScale.x > 0) transform.localScale *= -1;
            else transform.localScale *= 1;
            _moveDirection = -1;
        }
        

    }
}

/*
        Grounded = Physics2D.IsTouchingLayers(_collider, Ground);
        Walled = Physics2D.IsTouchingLayers(_collider, Wall);

        if (Grounded)
        {
            _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);           
            
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            }
        }

        if (Walled && _rigidbody.velocity.y < 0)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                          
                if (Facing == Sides.RIGHT)
                {
                     _rigidbody.velocity = new Vector2(_rigidbody.velocity.x - BounceFromWallForce, JumpForce);
                    Facing = Sides.LEFT;
                    Flip();
                }
                else 
                {
                     _rigidbody.velocity = new Vector2(_rigidbody.velocity.x + BounceFromWallForce, JumpForce);
                    Facing = Sides.RIGHT;
                    Flip();
                }
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

     */


