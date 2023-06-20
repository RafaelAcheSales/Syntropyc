using System.Collections;
using UnityEngine;
public class GridMovement : MonoBehaviour
{


    public float moveSpeed = 5f;


    private bool canMove = true;
    
    private Vector2 direction;

    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Movement() {
        bool hasUpInput = Input.GetKey(KeyCode.W);
        bool hasDownInput = Input.GetKey(KeyCode.S);
        bool hasLeftInput = Input.GetKey(KeyCode.A);
        bool hasRightInput = Input.GetKey(KeyCode.D);

        direction = Vector2.zero;
        if (hasUpInput) direction = direction + (Vector2.up);
        if (hasDownInput) direction = direction + (Vector2.down);
        if (hasLeftInput) direction = direction + (Vector2.left);
        if (hasRightInput) direction = direction + (Vector2.right);

        // normalize the vector so that diagonal movement isn't faster
        direction.Normalize();

        // if there is no input, don't move
        if (direction == Vector2.zero) return;

        // if there is input, move in that direction
        Move(direction);

    }
    private void Update() {

        Movement();

    }


//    move the players rigidbody to the new position
    private void Move(Vector2 direction) {
        if (!canMove) return;

        Vector2 newPosition = rb.position + (direction * moveSpeed * Time.fixedDeltaTime);
        Debug.DrawLine(rb.position, newPosition, Color.red);
        rb.MovePosition(newPosition);

        
    }
    
}