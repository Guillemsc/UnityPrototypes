using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	void Start ()
    {
        weapon_head_script = weapon_head.GetComponent<WeaponHead>();
        weapon_head_script.SetController(this);

        weapon_hammer.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Camera cam = Camera.main;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float angle = Utils.AngleFromTwoPoints(weapon_pivot.transform.position, mouse);

        if(!shooting)
            weapon_pivot.transform.localEulerAngles = new Vector3(0, 0, angle);

        if (Input.GetMouseButtonDown(0))
        {
            if (!grabbed)
                Shoot();
            else
                ThrowGrabbedItem();
        }

        if(Input.GetMouseButton(1))
        {
            LeaveItem();
        }

        UpdateShoot();
        UpdateGrabbedPosition();
        UpdateThrowItem();
    }

    public void Shoot()
    {
        if(!shooting && !grabbed && !recovering)
        {
            shooting = true;

            recovering = false;

            shoot_starting_pos = weapon_head.transform.localPosition;

            weapon_head_renderer.sprite = open_head_sprite;

            last_trail_dist = 0;
        }
    }

    private void UpdateShoot()
    {
        if(shooting)
        {
            float dt_speed = shoot_speed * Time.deltaTime;

            float distance = Mathf.Abs(Vector2.Distance(weapon_head.transform.localPosition, shoot_starting_pos));

            if (!recovering)
            {
                weapon_head.transform.localPosition += new Vector3(dt_speed, 0);

                if (distance > last_trail_dist + leave_trail_every)
                {
                    SpawnTrail();

                    last_trail_dist = distance;
                }

                if (distance > shoot_max_distance)
                {
                    EndShootFailure();
                }
            }
            else
            {
                CheckDestroyTrail();

                weapon_head.transform.localPosition -= new Vector3(dt_speed, 0);

                if(distance < 0.1f)
                {
                    weapon_head.transform.localPosition = shoot_starting_pos;

                    shooting = false;

                    DestroyTrails();

                    recovering = false;
                }
            }
        }
    }

    private void EndShootFailure()
    {
        shooting = false;

        weapon_head.transform.localPosition = shoot_starting_pos;

        weapon_head_renderer.sprite = closed_head_sprite;

        DestroyTrails();
    }

    private void EndShootSucces()
    {
        recovering = true;

        weapon_head_renderer.sprite = closed_head_sprite;
    }

    private void SpawnTrail()
    {
        GameObject new_trail = new GameObject();
        SpriteRenderer sr = new_trail.AddComponent<SpriteRenderer>();

        sr.sprite = trail_sprite;

        new_trail.transform.parent = weapon_pivot.transform;

        new_trail.transform.position = weapon_head.transform.position;
        new_trail.transform.rotation = weapon_head.transform.rotation;

        new_trail.transform.localPosition -= new Vector3(0.1f, 0, 0);

        trails.Add(new_trail);
    }

    private void DestroyTrails()
    {
        for(int i = 0; i < trails.Count; ++i)
        {
            Destroy(trails[i]);
        }

        trails.Clear();
    }

    private void CheckDestroyTrail()
    {
        if(trails.Count > 0)
        {
            GameObject curr_trail = trails[trails.Count - 1];

            float distance = Mathf.Abs(Vector2.Distance(weapon_head.transform.localPosition, curr_trail.transform.localPosition));

            if(distance < 0.1f)
            {
                trails.RemoveAt(trails.Count - 1);

                Destroy(curr_trail);
            }
        }
    }

    public void OnHitGrabbable(GrabableItem to_grab)
    {
        if (shooting)
        {
            grabbed = true;
            grabbed_item = to_grab;

            grabbed_item.transform.parent = weapon_head.transform;

            grabbed_item.rotation_beffore_grabbing = grabbed_item.transform.rotation;

            EndShootSucces();
        }
    }

    private void UpdateGrabbedPosition()
    {
        if(grabbed)
        {
            grabbed_item.transform.position = weapon_head.transform.position;
            grabbed_item.transform.localPosition += grabbed_item.grabbed_offset; 
        }
    }

    public void LeaveItem()
    {
        if (grabbed && !recovering)
        {
            grabbed = false;
            throwing = true;

            weapon_head_renderer.sprite = open_head_sprite;

            hammer_start_pos = new Vector2(0.416f, 0);
            weapon_hammer.transform.localPosition = hammer_start_pos;
            weapon_hammer.SetActive(false);

            grabbed_item.transform.parent = null;

            grabbed_item.transform.rotation = grabbed_item.rotation_beffore_grabbing;

            grabbed_item = null;
        }
    }

    public void ThrowGrabbedItem()
    {
        if(grabbed && !recovering)
        {
            grabbed = false;
            throwing = true;

            weapon_head_renderer.sprite = open_head_sprite;

            hammer_start_pos = new Vector2(0.416f, 0);
            weapon_hammer.transform.localPosition = hammer_start_pos;
            weapon_hammer.SetActive(true);

            grabbed_item.transform.parent = null;

            grabbed_item.transform.rotation = grabbed_item.rotation_beffore_grabbing;

            grabbed_item.Throw(weapon_pivot.transform.rotation.eulerAngles.z);

            grabbed_item = null;
        }
    }

    private void UpdateThrowItem()
    {
        if(throwing)
        {
            float distance = Mathf.Abs(Vector2.Distance(hammer_start_pos, weapon_hammer.transform.localPosition));

            float dt_hammer_speed = 2.0f * Time.deltaTime;

            weapon_hammer.transform.localPosition += new Vector3(dt_hammer_speed, 0, 0);

            if (distance > 0.4f)
            {
                throwing = false;

                weapon_head_renderer.sprite = closed_head_sprite;

                weapon_hammer.transform.localPosition = hammer_start_pos;
                weapon_hammer.SetActive(false);
            }
        }
    }

    [SerializeField] private float shoot_speed = 0;
    [SerializeField] private float shoot_max_distance = 0;
    Vector2 shoot_starting_pos = Vector2.zero;
    private bool shooting = false;
    private bool recovering = false;

    private bool throwing = false;
    Vector2 hammer_start_pos = Vector2.zero;

    private float leave_trail_every = 0.2f;
    private float last_trail_dist = 0;
    List<GameObject> trails = new List<GameObject>();

    private bool grabbed = false;
    GrabableItem grabbed_item = null;

    [SerializeField] private GameObject weapon_pivot = null;
    [SerializeField] private GameObject weapon_head = null;
    private WeaponHead weapon_head_script = null;
    [SerializeField] private SpriteRenderer weapon_head_renderer = null;
    [SerializeField] private GameObject weapon_hammer = null;
    [SerializeField] private Sprite open_head_sprite = null;
    [SerializeField] private Sprite closed_head_sprite = null;
    [SerializeField] private Sprite trail_sprite = null;
}
