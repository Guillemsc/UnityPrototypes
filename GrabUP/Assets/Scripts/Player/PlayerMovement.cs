using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateMovement();

        velocity_vector = rb.velocity;

        if (rb.gravityScale > 0)
        {
            if (rb.velocity.y > 6)
                rb.velocity = new Vector2(rb.velocity.x, 6);

            if (rb.velocity.y < -4)
                rb.velocity = new Vector2(rb.velocity.x, -4);
        }
    }

    public Vector2 GetVelocityVector()
    {
        return velocity_vector;
    }

    public Vector2Int GetInputVector()
    {
        return input_vector;
    }

    private Vector2Int UserInput()
    {
        Vector2Int ret = Vector2Int.zero;

        if (Input.GetKey("a"))
        {
            ret += new Vector2Int(-1, 0);
        }

        if (Input.GetKey("d"))
        {
            ret += new Vector2Int(1, 0);
        }

        return ret;
    }

    public void AddForce(Vector2 dir, float foce)
    {
        rb.AddForce(dir * foce);
    }

    public void DisableGravity()
    {
        rb.gravityScale = 0.0f;
    }

    public void EnableGravity()
    {
        rb.gravityScale = 1.0f;
    }

    public void ReestartVelocity()
    {
        rb.velocity = Vector2.zero;
    }

    private void UpdateMovement()
    {
        input_vector = UserInput();

        float movement_angle = 0;

        if (input_vector.x > 0 && input_vector.y == 0)
            movement_angle = 0;

        else if (input_vector.x < 0 && input_vector.y == 0)
            movement_angle = 180;

        movement_angle *= Mathf.Deg2Rad;

        bool accelerating_x = false;

        if (input_vector.x != 0)
            accelerating_x = true;


        if (accelerating_x)
        {
            float dt_acceleration = acceleration * Time.deltaTime;

            velocity_vector.x += dt_acceleration * Mathf.Cos(movement_angle);

            if (velocity_vector.x > max_speed)
                velocity_vector.x = max_speed;

            if (velocity_vector.x < -max_speed)
                velocity_vector.x = -max_speed;
        }
        else
        {
            float dt_deceleration = deceleration * Time.deltaTime;

            float deceleration_val = 0;

            if (velocity_vector.x > 0)
            {
                deceleration_val -= dt_deceleration;

                if (velocity_vector.x + deceleration_val < 0)
                    deceleration_val = -velocity_vector.x;
            }

            else if (velocity_vector.x < 0)
            {
                deceleration_val += dt_deceleration;

                if (velocity_vector.x - deceleration_val > 0)
                    deceleration_val = -velocity_vector.x;
            }

            velocity_vector.x += deceleration_val;


        }
    }

    [SerializeField] private float acceleration = 0;
    [SerializeField] private float deceleration = 0;
    [SerializeField] private float max_speed = 0;

    private Rigidbody2D rb = null;

    private Vector2Int input_vector = Vector2Int.zero;
    private Vector2 velocity_vector = Vector2.zero;
}
