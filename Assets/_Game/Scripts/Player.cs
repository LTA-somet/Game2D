using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player :Character
{
    [SerializeField] private Rigidbody2D rb;
  
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 100;
    [SerializeField] private float jumpForce = 350;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    private bool isGrounded=true;
    private bool isJumping=false;
    private bool isAttack=false;
    private bool isDeath=false;

    private float horizontal;
 
    private int coin = 0;
    private Vector3 savePoint;

    private void Awake()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
    }
    void Update()
    {
        if (IsDead)
        {
            return;

        }
        isGrounded =CheckGrounded();
     
      //  horizontal = Input.GetAxisRaw("Horizontal");

      
        if (isAttack) 
        {
            rb.velocity = Vector2.zero;
            return;
        }

      
        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                Jump();
            }
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }
            if (Input.GetKeyDown(KeyCode.C) && isGrounded)
            {
               
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.V) && isGrounded)
            {
                Throw();
            }
            
        }
        if (!isGrounded && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }

        if (Mathf.Abs(horizontal) > 0.1f)
        {       
            rb.velocity=new Vector2(horizontal*speed, rb.velocity.y);
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }
        else if (isGrounded)
        {
            ChangeAnim("idle");
            rb.velocity=Vector2.up*rb.velocity.y;
        }
    }
    public override void OnInit()
    {
        base.OnInit();
        isDeath = false;
        isAttack = false;
        transform.position = savePoint;
        ChangeAnim("idle");
        DeactiveAttack();
        SavePoint();
        UIManager.Instance.SetCoin(coin);
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }
    protected override void OnDeath()
    {
        base.OnDeath();
    }
    private bool CheckGrounded()
    {
       
        Debug.DrawLine(transform.position, transform.position + Vector3.down*1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,1.1f,groundLayer);
        return hit.collider != null;
    }
    public void Attack() 
    {
        rb.velocity = Vector2.zero;
        ChangeAnim("attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
        ActiveAttack(); 
        Invoke(nameof(DeactiveAttack), 0.5f);
    }
    public void Throw() 
    {
       
        ChangeAnim("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
        Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);
    }
    private void ResetAttack()
    {     
        ChangeAnim("idle");
        isAttack = false;   
    }
    public void Jump()
    {
        if (isGrounded)
        {
            isJumping = true;

            ChangeAnim("jump");
            rb.AddForce(jumpForce * Vector2.up);
        }
    }
   private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }
    private void DeactiveAttack()
    {
            attackArea.SetActive(false);
    }
    internal void SavePoint()
    {
        savePoint = transform.position;
    }
    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Coin")
        {        
                coin++;
            PlayerPrefs.SetInt("Coin", coin);
            UIManager.Instance.SetCoin(coin);
                Destroy(collision.gameObject);
        }
        if (collision.tag == "DeathZone")
        {
            isDeath = true;
            ChangeAnim("die");
            Invoke(nameof(OnInit), 1f);
        }
    }

   
}
