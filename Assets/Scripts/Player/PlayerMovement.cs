using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 200f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator anim;
    private SpriteRenderer sr;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        anim.SetBool("walking", movement.magnitude > 0);

        if (moveX > 0)
            sr.flipX = true;
        else if (moveX < 0)
            sr.flipX = false;
        

        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed * Time.fixedDeltaTime;
    }
}
