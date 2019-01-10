using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    PlayerMovement pm = null;

    struct Trail
    {
        public GameObject trile_go;
        public float distance_from_player;
    }

    void Start()
    {
        weapon_head_script = weapon_head.GetComponent<WeaponHead>();
        weapon_head_script.SetController(this);

        pm = gameObject.GetComponent<PlayerMovement>();

        head_starting_shoot_pos = weapon_head.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float angle = Utils.AngleFromTwoPoints(weapon_pivot.transform.position, mouse);

        if (!recovering)
            weapon_pivot.transform.localEulerAngles = new Vector3(0, 0, angle);

        if (Input.GetMouseButtonDown(0))
        {
            if (!recovering)
                Shoot();
            else
                FinalJump();
        }

        UpdateShoot();
        RecoverUpdate();
        UpdateTrailPos();
    }

    private void Shoot()
    {
        if (!shooting)
        {
            weapon_head_renderer.sprite = closed_head_sprite;
            shooting = false;
            DestroyTrails();

            shooting = true;
            recovering = false;

            weapon_head_renderer.sprite = open_head_sprite;
        }
    }

    private void UpdateShoot()
    {
        if(shooting)
        {
            float shoot_speed_dt = shoot_speed * Time.deltaTime;

            weapon_head.transform.localPosition += new Vector3(shoot_speed_dt, 0, 0);

            float distance = Vector2.Distance(gameObject.transform.position, weapon_head.transform.position);

            if(distance > shoot_max_distance)
            {
                weapon_head.transform.localPosition = head_starting_shoot_pos;
                weapon_head_renderer.sprite = closed_head_sprite;
                shooting = false;
                DestroyTrails();
            }
        }

        SpawnTrail();
    }

    private void Recover()
    {
        if(shooting)
        {
            recovering = true;
            shooting = false;
            recover_waiting_finish = false;

            recover_dir_vec = grabbed_item.transform.position - gameObject.transform.position;
            recover_dir_vec.Normalize();

            weapon_head_renderer.transform.position = grabbed_item.transform.position;

            weapon_head_renderer.sprite = closed_head_sprite;

            recover_dist = Vector2.Distance(weapon_head_renderer.transform.position, gameObject.transform.position);

            pm.DisableGravity();
            pm.ReestartVelocity();
        }
    }

    private void RecoverUpdate()
    {
        if (recovering && !recover_waiting_finish)
        {
            recover_dir_vec = grabbed_item.transform.position - gameObject.transform.position;
            recover_dir_vec.Normalize();

            float recover_speed_dt = shoot_speed * Time.deltaTime * 0.5f;

            weapon_head.transform.position = grabbed_item.transform.position;

            gameObject.transform.position += recover_dir_vec * recover_speed_dt;

            float distance = Vector2.Distance(head_starting_shoot_pos, weapon_head.transform.localPosition);

            if(distance < 0.2f)
            {
                weapon_head_renderer.transform.position = grabbed_item.transform.position;

                weapon_head_renderer.sprite = closed_head_sprite;

                DestroyTrails();

                recover_waiting_finish = true;
            }
        }
    }

    private void FinalJump()
    {
        if (recovering)
        {
            Camera cam = Camera.main;
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float angle = Utils.AngleFromTwoPoints(weapon_pivot.transform.position, mouse);

            Vector2 dir_vec = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            dir_vec.Normalize();

            pm.EnableGravity();
            pm.AddForce(dir_vec, 2000);

            recover_waiting_finish = false;
            recovering = false;

            weapon_head.transform.localPosition = head_starting_shoot_pos;

            weapon_head_renderer.sprite = closed_head_sprite;

            DestroyTrails();
        }
    }

    private void SpawnTrail()
    {
        if (shooting)
        {
            bool spawn = true;

            float distance = 0;
            float distance_from_player = 0;

            if (trails.Count > 0)
            {
                Trail curr_trail = trails[trails.Count - 1];

                distance = Mathf.Abs(Vector2.Distance(weapon_head.transform.position, curr_trail.trile_go.transform.position));
                distance_from_player = Mathf.Abs(Vector2.Distance(gameObject.transform.position, weapon_head.transform.position));

                if (distance > 0.3f)
                {
                    spawn = true;
                }
                else
                    spawn = false;
            }

            if (spawn)
            {
                Trail trail = new Trail();
                trail.trile_go = new GameObject();
                trail.distance_from_player = distance_from_player;

                SpriteRenderer sr = trail.trile_go.AddComponent<SpriteRenderer>();

                sr.sprite = trail_sprite;

                trails.Add(trail);
            }
        }
    }

    private void UpdateTrailPos()
    {
        if (shooting)
        {
            if (trails.Count > 0)
            {
                float angle = Utils.AngleFromTwoPoints(weapon_head.transform.position, gameObject.transform.position);

                Vector3 dir_vec = weapon_head.transform.position - gameObject.transform.position;
                dir_vec.Normalize();

                for (int i = 0; i < trails.Count; ++i)
                {
                    trails[i].trile_go.transform.position = gameObject.transform.position + (dir_vec * trails[i].distance_from_player);
                    trails[i].trile_go.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }
        else if (recovering)
        {
            if (trails.Count > 0)
            {
                for (int i = 0; i < trails.Count; ++i)
                {
                    float distanec = Vector2.Distance(gameObject.transform.position, trails[i].trile_go.transform.position);

                    if(distanec < 0.2f)
                    {
                        Destroy(trails[i].trile_go);
                        trails.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    private void DestroyTrails()
    {
        for(int i = 0; i < trails.Count; ++i)
        {
            Destroy(trails[i].trile_go);
        }

        trails.Clear();
    }

    public void OnHitGrabbable(GrabableItem to_grab)
    {
        if (shooting)
        {
            grabbed_item = to_grab;

            Recover();
        }
    }

    [SerializeField] private float shoot_speed = 0;
    [SerializeField] private float shoot_max_distance = 0;
    float angle = 0.0f;
    private bool shooting = false;
    private bool recovering = false;
    private bool recover_waiting_finish = false;

    private float leave_trail_every = 0.2f;
    private float last_trail_dist = 0;
    List<Trail> trails = new List<Trail>();

    GrabableItem grabbed_item = null;

    Vector2 head_starting_shoot_pos = Vector2.zero;

    Vector3 recover_dir_vec = Vector3.zero;
    float recover_dist = 0;

    [SerializeField] private GameObject weapon_pivot = null;
    [SerializeField] private GameObject weapon_head = null;
    private WeaponHead weapon_head_script = null;
    [SerializeField] private SpriteRenderer weapon_head_renderer = null; 
    [SerializeField] private Sprite open_head_sprite = null;
    [SerializeField] private Sprite closed_head_sprite = null;
    [SerializeField] private Sprite trail_sprite = null;
}
