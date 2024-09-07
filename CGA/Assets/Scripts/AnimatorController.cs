using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController: MonoBehaviour
{
    // var to hold horizontal value		
    float horizontal;
    // var to hold vertical value
    float vertical;
    // Initialize an Animator variable
    Animator animator;
    // booleanvariable to test if facing right
    bool facingRight;

    void Awake()
    {
        // Link the GameObject Animator component to the animator varible
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //check if the user is pressing Horizontal input
        horizontal = Input.GetAxis("Horizontal");
        //check if the user is pressing Vertical input
        vertical = Input.GetAxis("Vertical");

        // Set the Speed parameter in the animator component
        animator.SetFloat("Speed", Mathf.Abs(horizontal != 0 ? horizontal : vertical));
    }

    void FixedUpdate()
    {
        //function for changing the character facing direction
        Flip(horizontal);
    }

    private void Flip(float horizontal)
    {
        //check where the character is currently facing and adjust the graphics direction
        if (horizontal < 0 && !facingRight || horizontal > 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}