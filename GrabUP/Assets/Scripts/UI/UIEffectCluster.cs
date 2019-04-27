using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    MOVEMENT_LINEAR,
    MOVEMENT_EXPO_IN,
    MOVEMENT_EXPO_OUT,
    MOVEMENT_EXPO_IN_OUT,
    MOVEMENT_BOUNCE,
    MOVEMENT_QUAD_IN,
    MOVEMENT_QUAD_OUT,
    MOVEMENT_QUAD_IN_OUT,
}

public class UIEffectSetActive : UIEffect
{
    public UIEffectSetActive(UIElement element, bool set_active) : base(element, UIEffectType.UI_SET_ACTIVE)
    {
        active = set_active;
    }

    public override void Start()
    {
        affected_element.gameObject.SetActive(active);

        Finish();
    }

    private bool active = false;
}

public class UIEffectWait : UIEffect
{
    public UIEffectWait(UIElement element, float wait_time) : base(element, UIEffectType.UI_WAIT_TIME)
    {
        this.wait_time = wait_time;
    }

    public override void Start()
    {
        timer.Start();
    }

    public override void Update()
    {
        if(timer.ReadRealTime() >= wait_time)
        {
            Finish();
        }
    }

    private float wait_time = 0.0f;
    private Timer timer = new Timer();
}

public class UIEffectFade : UIEffect
{
    public UIEffectFade(UIElement element, float starting_fade_val, float final_fade_val, float duration) 
        : base(element, UIEffectType.UI_FADE)
    {
        this.starting_fade_val = starting_fade_val;
        this.final_fade_val = final_fade_val;
        this.duration = duration;
    }

    public override void Start()
    {
        affected_element.GetCanvasGroup().alpha = starting_fade_val;

        fade_difference = final_fade_val - starting_fade_val;

        timer.Start();
    }

    public override void Update()
    {
        float alpha = 0;

        alpha = Linear(fade_difference, timer.ReadRealTime(), starting_fade_val, duration);

        affected_element.GetCanvasGroup().alpha = alpha;
         
        if(timer.ReadRealTime() >= duration)
        {
            affected_element.GetCanvasGroup().alpha = final_fade_val;
            Finish();
        }
    }

    private float Linear(float total_distance, float current_time, float starting_pos, float total_duration)
    {
        return total_distance * current_time / total_duration + starting_pos;
    }

    private float starting_fade_val = 0;
    private float final_fade_val = 0;
    private float fade_difference = 0;
    private float duration = 0;

    private Timer timer = new Timer();
}

public class UIEffectSetInteractable : UIEffect
{
    public UIEffectSetInteractable(UIElement element, bool set_interactable) 
        : base(element, UIEffectType.UI_SET_INTERACTABLE)
    {
        this.set_interactable = set_interactable;
    }

    public override void Start()
    {
        affected_element.GetCanvasGroup().interactable = set_interactable;

        Finish();
    }

    private bool set_interactable = false;
}

public class UIEffectScale : UIEffect
{
    public UIEffectScale(UIElement element, MovementType movement_type, Vector2 start_scale, Vector2 final_scale, float duration) 
        : base(element, UIEffectType.UI_SCALE)
    {
        this.starting_scale = start_scale;
        this.movement_type = movement_type;
        this.final_scale = final_scale;
        this.duration = duration;
    }

    public override void Start()
    {
        distance = final_scale - starting_scale;

        timer.Start();
    }

    public override void Update()
    {
        Vector2 new_scale = Vector2.zero;

        switch (movement_type)
        {
            case MovementType.MOVEMENT_LINEAR:
                {
                    new_scale.x = EasingFunctions.Linear(distance.x, timer.ReadRealTime(), starting_scale.x, duration);
                    new_scale.y = EasingFunctions.Linear(distance.y, timer.ReadRealTime(), starting_scale.y, duration);

                    break;
                }

            case MovementType.MOVEMENT_EXPO_IN:
                {
                    new_scale.x = EasingFunctions.ExpoIn(distance.x, timer.ReadRealTime(), starting_scale.x, duration);
                    new_scale.y = EasingFunctions.ExpoIn(distance.y, timer.ReadRealTime(), starting_scale.y, duration);

                    break;
                }

            case MovementType.MOVEMENT_EXPO_OUT:
                {
                    new_scale.x = EasingFunctions.ExpoOut(distance.x, timer.ReadRealTime(), starting_scale.x, duration);
                    new_scale.y = EasingFunctions.ExpoOut(distance.y, timer.ReadRealTime(), starting_scale.y, duration);

                    break;
                }

            case MovementType.MOVEMENT_EXPO_IN_OUT:
                {
                    new_scale.x = EasingFunctions.ExpoInOut(distance.x, timer.ReadRealTime(), starting_scale.x, duration);
                    new_scale.y = EasingFunctions.ExpoInOut(distance.y, timer.ReadRealTime(), starting_scale.y, duration);

                    break;
                }
            case MovementType.MOVEMENT_BOUNCE:
                {
                    new_scale.x = EasingFunctions.Bounce(distance.x, timer.ReadRealTime(), starting_scale.x, duration);
                    new_scale.y = EasingFunctions.Bounce(distance.y, timer.ReadRealTime(), starting_scale.y, duration);

                    break;
                }
            case MovementType.MOVEMENT_QUAD_IN:
                {
                    new_scale.x = EasingFunctions.QuadIn(distance.x, timer.ReadRealTime(), starting_scale.x, duration);
                    new_scale.y = EasingFunctions.QuadIn(distance.y, timer.ReadRealTime(), starting_scale.y, duration);

                    break;
                }
            case MovementType.MOVEMENT_QUAD_OUT:
                {
                    new_scale.x = EasingFunctions.QuadOut(distance.x, timer.ReadRealTime(), starting_scale.x, duration);
                    new_scale.y = EasingFunctions.QuadOut(distance.y, timer.ReadRealTime(), starting_scale.y, duration);

                    break;
                }
            case MovementType.MOVEMENT_QUAD_IN_OUT:
                {
                    new_scale.x = EasingFunctions.QuadInOut(distance.x, timer.ReadRealTime(), starting_scale.x, duration);
                    new_scale.y = EasingFunctions.QuadInOut(distance.y, timer.ReadRealTime(), starting_scale.y, duration);

                    break;
                }
        }

        affected_element.transform.localScale = new_scale;

        if (timer.ReadRealTime() >= duration)
        {
            affected_element.transform.localScale = final_scale;
            Finish();
        }
    }

    MovementType movement_type = new MovementType();

    private Vector2 final_scale = Vector2.zero;
    private Vector2 starting_scale = Vector2.zero;
    private Vector2 distance = Vector2.zero;
    private float duration = 0;

    private Timer timer = new Timer();
}

public class UIEffectMoveToPos : UIEffect
{
    public UIEffectMoveToPos(UIElement element, MovementType movement_type, Vector2 start_pos, Vector2 final_pos, float duration) 
        : base(element, UIEffectType.UI_MOVE_TO_POS)
    {
        this.starting_pos = start_pos;
        this.movement_type = movement_type;
        this.final_pos = final_pos;
        this.duration = duration;
    }

    public override void Start()
    {
        distance = final_pos - starting_pos;

        timer.Start();
    }

    public override void Update()
    {
        Vector2 new_pos = Vector2.zero;

        switch(movement_type)
        {
            case MovementType.MOVEMENT_LINEAR:
                {
                    new_pos.x = EasingFunctions.Linear(distance.x, timer.ReadRealTime(), starting_pos.x, duration);
                    new_pos.y = EasingFunctions.Linear(distance.y, timer.ReadRealTime(), starting_pos.y, duration);

                    break;
                }

            case MovementType.MOVEMENT_EXPO_IN:
                {
                    new_pos.x = EasingFunctions.ExpoIn(distance.x, timer.ReadRealTime(), starting_pos.x, duration);
                    new_pos.y = EasingFunctions.ExpoIn(distance.y, timer.ReadRealTime(), starting_pos.y, duration);

                    break;
                }

            case MovementType.MOVEMENT_EXPO_OUT:
                {
                    new_pos.x = EasingFunctions.ExpoOut(distance.x, timer.ReadRealTime(), starting_pos.x, duration);
                    new_pos.y = EasingFunctions.ExpoOut(distance.y, timer.ReadRealTime(), starting_pos.y, duration);

                    break;
                }

            case MovementType.MOVEMENT_EXPO_IN_OUT:
                {
                    new_pos.x = EasingFunctions.ExpoInOut(distance.x, timer.ReadRealTime(), starting_pos.x, duration);
                    new_pos.y = EasingFunctions.ExpoInOut(distance.y, timer.ReadRealTime(), starting_pos.y, duration);

                    break;
                }
            case MovementType.MOVEMENT_BOUNCE:
                {
                    new_pos.x = EasingFunctions.Bounce(distance.x, timer.ReadRealTime(), starting_pos.x, duration);
                    new_pos.y = EasingFunctions.Bounce(distance.y, timer.ReadRealTime(), starting_pos.y, duration);

                    break;
                }
            case MovementType.MOVEMENT_QUAD_IN:
                {
                    new_pos.x = EasingFunctions.QuadIn(distance.x, timer.ReadRealTime(), starting_pos.x, duration);
                    new_pos.y = EasingFunctions.QuadIn(distance.y, timer.ReadRealTime(), starting_pos.y, duration);

                    break;
                }
            case MovementType.MOVEMENT_QUAD_OUT:
                {
                    new_pos.x = EasingFunctions.QuadOut(distance.x, timer.ReadRealTime(), starting_pos.x, duration);
                    new_pos.y = EasingFunctions.QuadOut(distance.y, timer.ReadRealTime(), starting_pos.y, duration);

                    break;
                }
            case MovementType.MOVEMENT_QUAD_IN_OUT:
                {
                    new_pos.x = EasingFunctions.QuadInOut(distance.x, timer.ReadRealTime(), starting_pos.x, duration);
                    new_pos.y = EasingFunctions.QuadInOut(distance.y, timer.ReadRealTime(), starting_pos.y, duration);

                    break;
                }
        }

        affected_element.transform.position = new_pos;

        if(timer.ReadRealTime() >= duration)
        {
            affected_element.transform.position = final_pos;
            Finish();
        }
    }

    private MovementType movement_type = new MovementType();

    private Vector2 final_pos = Vector2.zero;
    private Vector2 starting_pos = Vector2.zero;
    private Vector2 distance = Vector2.zero;
    private float duration = 0;

    private Timer timer = new Timer();
}

