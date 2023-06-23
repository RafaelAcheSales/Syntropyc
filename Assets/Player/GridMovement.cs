using System.Collections;
using UnityEngine;
public class GridMovement : MonoBehaviour
{


    public float moveSpeed = 5f;


    private bool canMove = true;
    
    private Vector2 direction;

    private Rigidbody2D rb;
    private GameObject playerVisual;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerVisual = transform.GetChild(0).gameObject;
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
        GameManager.instance.playerActionAnimator.SetSpeed(direction.magnitude);

        // if there is no input, don't move
        if (direction == Vector2.zero) return;

        // if there is input, move in that direction
        Move(direction);

    }

    void Rotation()
    {
        //ROtates visual
        Vector3 mousePosition = GameManager.instance.GetMouseScreenPos();
        Vector3 lookDirection = mousePosition - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        playerVisual.transform.rotation = Quaternion.Euler(0f, 0f, angle-90f);
    }
    private void Update() {

        Movement();
        Rotation();

    }


//    move the players rigidbody to the new position
    private void Move(Vector2 direction) {
        if (!canMove) return;

        Vector2 newPosition = rb.position + (direction * moveSpeed * Time.fixedDeltaTime);
        Debug.DrawLine(rb.position, newPosition, Color.red);
        rb.MovePosition(newPosition);

        
    }
    
}