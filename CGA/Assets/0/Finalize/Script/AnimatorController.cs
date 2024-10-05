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
    // Reference to PlayerAttack script
    PlayerAttack playerAttack;
    // Boolean variable to test if facing right
    bool facingRight = true;
    // New variable to track if attack is on cooldown
    bool attackOnCooldown = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        // Get input for movement
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Calculate movement speed based on both axes
        float speed = new Vector2(horizontal, vertical).magnitude;
        animator.SetFloat("Speed", speed);

        // Check for attack input only if the player is not currently attacking and not on cooldown
        if (Input.GetButtonDown("Fire1") && !playerAttack.IsAttacking() && !attackOnCooldown)
        {
            Attack();
            StartCoroutine(AttackCooldown());
        }

        // Check for flip input (A or D keys)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            Flip(horizontal);
        }
    }

    private void Flip(float horizontal)
    {
        if (horizontal < 0 && facingRight)
        {
            facingRight = false;
            FlipCharacter();
        }
        else if (horizontal > 0 && !facingRight)
        {
            facingRight = true;
            FlipCharacter();
        }
    }

    private void FlipCharacter()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private IEnumerator AttackCooldown()
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(playerAttack.attackRate);
        attackOnCooldown = false;
    }
}