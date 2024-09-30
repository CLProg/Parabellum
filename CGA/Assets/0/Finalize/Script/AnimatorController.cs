using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    // Var to hold horizontal value		
    float horizontal;
    // Var to hold vertical value
    float vertical;
    // Initialize an Animator variable
    Animator animator;
    // Boolean variable to test if facing right
    bool facingRight;

    void Awake()
    {
        // Link the GameObject Animator component to the animator variable
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if the user is pressing Horizontal input
        horizontal = Input.GetAxis("Horizontal");
        // Check if the user is pressing Vertical input
        vertical = Input.GetAxis("Vertical");

        // Set the Speed parameter in the animator component
        animator.SetFloat("Speed", Mathf.Abs(horizontal != 0 ? horizontal : vertical));

        // Check for attack input
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        // Function for changing the character facing direction
        Flip(horizontal);
    }

    private void Flip(float horizontal)
    {
        // Check where the character is currently facing and adjust the graphics direction
        if (horizontal < 0 && !facingRight || horizontal > 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void Attack()
    {
        // Trigger the attack animation
        animator.SetTrigger("Attack");
    }
}
