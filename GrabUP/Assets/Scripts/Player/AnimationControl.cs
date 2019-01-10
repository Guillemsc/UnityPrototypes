using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    private void Awake()
    {
        CameraManager.Instance.CameraFollow(Camera.main, gameObject, 0.4f, new Vector3(0, 1, 0));

        animator = gameObject.GetComponentInChildren<Animator2D>();
        player_movement = gameObject.GetComponent<PlayerMovement>();

        last_player_pos = gameObject.transform.position;
    }

    // Use this for initialization
    void Start ()
    {
        animator.PlayAnimation("idle", 0.1f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        Vector2Int input_vector = player_movement.GetInputVector();
        Vector2 velocity_vector = player_movement.GetVelocityVector();

        if(last_player_pos.x < gameObject.transform.position.x)
        {
            animator.SetFilpX(false);
        }
        else if(last_player_pos.x > gameObject.transform.position.x)
        {
            animator.SetFilpX(true);
        }

        if(velocity_vector.x > 0.1f || velocity_vector.x < -0.1f || velocity_vector.y > 0.1f || velocity_vector.y < -0.1f)
            animator.PlayAnimation("run", 0.07f);
        else
            animator.PlayAnimation("idle", 0.1f);

        last_player_pos = gameObject.transform.position;
    }

    private Animator2D animator = null;
    private PlayerMovement player_movement = null;

    Vector3 last_player_pos = Vector3.zero;
}
