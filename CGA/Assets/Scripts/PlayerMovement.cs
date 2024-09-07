using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Variable used to set the movement speed of the player
    public int runSpeed = 1;
    //variable used to hold the horizontal value
    float horizontal;
    //variable used to hold the vertical value
    float vertical;

    void Update()
    {
        //check if the user is pressing Horizontal inputs
        horizontal = Input.GetAxis("Horizontal");
        //checkif the user is pressing Vertical inputs
        vertical = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        //Set the value vector movement of the player depending on the user input
        Vector3 movement = new Vector3(horizontal * runSpeed, 0.0f, vertical * runSpeed);
        //move the player to the set vector location according the movement value
        transform.position = transform.position + movement * Time.deltaTime;
    }
}