using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableItem : MonoBehaviour
{
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateThrow();
    }

    public void Throw(float angle)
    {
        this.angle = angle;

        velocity_vector = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * throw_starting_speed, Mathf.Sin(angle * Mathf.Deg2Rad) * throw_starting_speed);

        float distance = Vector2.Distance(new Vector2(0, 0), velocity_vector);

        throwed = true;
    }

    private void UpdateThrow()
    {
        if(throwed)
        {
            float dt_deceleration_x = Mathf.Abs((throw_deceleration * Mathf.Cos(angle * Mathf.Deg2Rad))) * Time.deltaTime;

            float deceleration_val_x = 0;

            if (velocity_vector.x > 0)
            {
                deceleration_val_x -= dt_deceleration_x;

                if (velocity_vector.x + deceleration_val_x < 0)
                    deceleration_val_x = -velocity_vector.x;
            }

            else if (velocity_vector.x < 0)
            {
                deceleration_val_x += dt_deceleration_x;

                if (velocity_vector.x - deceleration_val_x > 0)
                    deceleration_val_x = -velocity_vector.x;
            }

            velocity_vector.x += deceleration_val_x;

            float dt_deceleration_y = Mathf.Abs((throw_deceleration * Mathf.Sin(angle * Mathf.Deg2Rad))) * Time.deltaTime;

            float deceleration_val_y = 0;

            if (velocity_vector.y > 0)
            {
                deceleration_val_y -= dt_deceleration_y;

                if (velocity_vector.y + deceleration_val_y < 0)
                    deceleration_val_y = -velocity_vector.y;
            }

            else if (velocity_vector.y < 0)
            {
                deceleration_val_y += dt_deceleration_y;

                if (velocity_vector.y - deceleration_val_y > 0)
                    deceleration_val_y = -velocity_vector.y;
            }

            velocity_vector.y += deceleration_val_y;

            rb.velocity = velocity_vector;

            if (velocity_vector.x == 0 && velocity_vector.y == 0)
            {
                throwed = false;

                rb.velocity = Vector2.zero;
            }
        }
    }

    public Vector3 grabbed_offset = Vector3.zero;

    public Quaternion rotation_beffore_grabbing = Quaternion.identity;

    private float angle = 0;

    private bool throwed = false;
    private float throw_starting_speed = 15;
    private float throw_deceleration = 35;

    private Vector2 velocity_vector = Vector2.zero;
    Rigidbody2D rb = null;
}
