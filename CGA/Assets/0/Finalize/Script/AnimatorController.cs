using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    // Variables to hold movement values
    float horizontal;
    float vertical;

    // Reference to the Animator component
    Animator animator;

    // Boolean variable to test if facing right
    bool facingRight = true; // Initialize as true if the character starts facing right

    void Awake()
    {
        // Link the GameObject Animator component to the animator variable
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input for movement
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Calculate movement speed based on both axes
        float speed = new Vector2(horizontal, vertical).magnitude; // Calculate speed based on the magnitude

        // Set the Speed parameter in the animator component
        animator.SetFloat("Speed", speed);

        // Check for attack input
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }

        // Check for flip input (A or D keys)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            Flip(horizontal);
        }
    }

    void FixedUpdate()
    {
        // No need to call Flip here, it’s handled in Update now
    }

    private void Flip(float horizontal)
    {
        // Check if A key is pressed to flip left
        if (horizontal < 0 && facingRight)
        {
            facingRight = false; // Set facing to left
            FlipCharacter();
        }
        // Check if D key is pressed to flip right
        else if (horizontal > 0 && !facingRight)
        {
            facingRight = true; // Set facing to right
            FlipCharacter();
        }
    }

    private void FlipCharacter()
    {
        // Flip the character's scale
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Invert the x scale
        transform.localScale = scale; // Apply the new scale
    }

    private void Attack()
    {
        // Trigger the attack animation
        animator.SetTrigger("Attack");
    }
}
