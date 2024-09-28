using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public float speedThreshold = 0.1f; // Speed above which the enemy is considered walking
    private Animator animator;
    private Rigidbody rb; // Use Rigidbody for 3D movement

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check the speed of the enemy using Rigidbody's velocity in 3D
        float speed = rb.velocity.magnitude;

        // If speed is above the threshold, set the enemy to walking
        if (speed > speedThreshold)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
