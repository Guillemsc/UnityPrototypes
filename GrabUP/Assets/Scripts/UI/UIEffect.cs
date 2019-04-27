using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIEffectType
{
    UI_SET_ACTIVE,
    UI_WAIT_TIME,
    UI_FADE,
    UI_SET_INTERACTABLE,
    UI_SCALE,
    UI_MOVE_TO_POS,
}

public delegate void UIEffectDel(UIEffect efect);

public class UIEffect
{
    protected UIEffect(UIElement affected_element, UIEffectType type)
    {
        this.affected_element = affected_element;
        this.type = type;
    }

    protected void Finish()
    {
        finished = true;

        if (on_effect_finished != null)
            on_effect_finished(this);
    }

    public bool GetFinished()
    {
        return finished;
    }

    public void SetStarted()
    {
        started = true;

        if (on_effect_started != null)
            on_effect_started(this);
    }

    public bool GetStarted()
    {
        return started;
    }

    public void SuscribeFinished(UIEffectDel sus)
    {
        on_effect_finished += sus;
    }

    public void SuscribeStarted(UIEffectDel sus)
    {
        on_effect_started += sus;
    }

    public virtual void Start() { }
    public virtual void Update() { }

    protected UIElement affected_element = null;
    private UIEffectType type = new UIEffectType();

    private bool finished = false;
    private bool started = false;

    private UIEffectDel on_effect_finished = null;
    private UIEffectDel on_effect_started = null;
}
