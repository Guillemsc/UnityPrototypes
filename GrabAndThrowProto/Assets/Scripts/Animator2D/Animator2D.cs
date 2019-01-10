using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Animator2D : MonoBehaviour
{
    private void Awake()
    {
        CreateSpriteRenderer();
    }

    private void Update()
    {
        if (!Application.isPlaying)
            CreateSpriteRenderer();
        else
            UpdateAnimation();
    }

    public void AddAnimation()
    {
        Animation2D new_animation = new Animation2D();

        animations.Add(new_animation);
    }

    public void RemoveAnimation(int index)
    {
        if(animations.Count < index)
            animations.RemoveAt(index);
    }

    public List<Animation2D> GetAnimations()
    {
        return animations;
    }

    public void PlayAnimation(string animation_name, float speed, bool loop = true)
    {
        bool already_playing = false;

        if(curr_animation != null)
        {
            if (curr_animation.GetName() == animation_name)
                already_playing = true;
        }

        if (!already_playing)
        {
            Animation2D anim = GetAnimation(animation_name);

            if (anim != null)
            {
                curr_animation = anim;

                this.speed = speed;
                this.loop = loop;

                timer.Start();
                curr_animation_sprite = 0;
            }
        }
    }

    public void SetFilpX(bool set)
    {
        sprite_renderer.flipX = set;
    }

    private void CreateSpriteRenderer()
    {
        sprite_renderer = gameObject.GetComponent<SpriteRenderer>();

        if (sprite_renderer == null)
            sprite_renderer = gameObject.AddComponent<SpriteRenderer>();
    }

    private Animation2D GetAnimation(string animation_name)
    {
        Animation2D ret = null;

        for (int i = 0; i < animations.Count; ++i)
        {
            Animation2D curr_animation = animations[i];

            if (curr_animation.GetName() == animation_name)
            {
                ret = curr_animation;
                break;
            }
        }

        return ret;
    }

    private void UpdateAnimation()
    {
        if(curr_animation != null)
        {
            Sprite curr_sprite = curr_animation.GetSprite(curr_animation_sprite);

            sprite_renderer.sprite = curr_sprite;

            if (timer.ReadTime() >= speed)
            {
                ++curr_animation_sprite;

                int sprites_count = curr_animation.GetSpritesCount();

                if (curr_animation_sprite > sprites_count - 1)
                {
                    if(loop)
                    {
                        curr_animation_sprite = 0;
                    }
                    else
                    {
                        curr_animation_sprite = sprites_count - 1;
                    }
                }

                timer.Start();
            }
        }
    }

    [SerializeField] [HideInInspector]
    private List<Animation2D> animations = new List<Animation2D>();

    private Timer timer = new Timer();

    private Animation2D curr_animation = null;
    private int curr_animation_sprite = 0;
    private float speed = 0.0f;
    private bool loop = false;

    private SpriteRenderer sprite_renderer = null;
}
