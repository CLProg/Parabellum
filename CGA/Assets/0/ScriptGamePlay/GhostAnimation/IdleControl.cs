using UnityEngine;

public class EnemyIdleAnimation : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private EnemyAI enemyAI; // Reference to the EnemyAI script

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        enemyAI = GetComponent<EnemyAI>(); // Get the EnemyAI script
    }

    void Update()
    {
        // Check if the enemy is idle based on the walking state
        if (!enemyAI.IsMoving())
        {
            // Check the direction based on the last movement
            if (enemyAI.IsWalkingLeft())
            {
                FlipSpriteLeft();
            }
            else if (enemyAI.IsWalkingRight())
            {
                FlipSpriteRight();
            }
        }
    }

    private void FlipSpriteLeft()
    {
        transform.localScale = new Vector3(-0.5f, 0.5f, 1); // Flip sprite to left
    }

    private void FlipSpriteRight()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 1); // Flip sprite to right
    }
}
