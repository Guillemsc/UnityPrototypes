using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    UIManager()
    {
        InitInstance(this);
    }

    private void Start()
    {
        UIElement curr_window = SetWorkingElement("test_window");

        if (curr_window != null)
        {
            EffectWait(1);
            PushEffects();

            EffectSetActive(true);
            EffectMove(curr_window.transform.position, new Vector2(curr_window.GetCanvas().transform.position.x, curr_window.transform.position.y), MovementType.MOVEMENT_QUAD_IN_OUT, 1);
            EffectScale(new Vector2(0, 0), curr_window.transform.localScale, MovementType.MOVEMENT_BOUNCE, 1);
            EffectFade(0, 1, 1);
            PushEffects();

            EffectWait(1);
            PushEffects();

            EffectMove(new Vector2(curr_window.GetCanvas().transform.position.x, curr_window.transform.position.y), new Vector2(curr_window.GetCanvas().transform.position.x + 600, curr_window.transform.position.y), MovementType.MOVEMENT_QUAD_IN_OUT, 1);
            EffectFade(1, 0, 1);
            PushEffects();
        }
    }

    void Update ()
    {
        CheckNextEffects();
        UpdateEffects();
    }

    /// <summary>>> Internal, don't use.</summary>
    public void AddElement(UIElement element)
    {
        if (element != null)
        {
            CanvasGroup cg = element.gameObject.AddComponent<CanvasGroup>();
            RectTransform rt = element.gameObject.GetComponent<RectTransform>();
            Canvas c = element.gameObject.GetComponentInParent<Canvas>();

            if (cg != null && rt != null && c != null)
            {
                element.SetInfo(cg, rt, c);

                element.gameObject.SetActive(false);

                elements.Add(element);
            }
        }
    }

    private UIElement GetElement(string name)
    {
        UIElement ret = null;

        for (int i = 0; i < elements.Count; ++i)
        {
            if(elements[i].GetIdentifier() == name)
            {
                ret = elements[i];
            }
        }

        return ret;
    }

    /// <summary>>> Sets the current element in which the next effects will be applied.</su
    public void SetWorkingElement(UIElement el)
    {
        if (el != null)
        {
            working_element = el;
        }
    }

    /// <summary>>> Sets the current element in which the next effects will be applied.</summary>
    /// <param name="name">The name of the element (Needs to be setup at the inspector script UIWindow). </param>
    public UIElement SetWorkingElement(string name)
    {
        UIElement ret = null;

        bool is_current = false;

        if(working_element != null)
        {
            if(working_element.GetIdentifier() == name)
            {
                is_current = true;
            }
        }

        if (!is_current)
        {
            UIElement element = GetElement(name);

            if (element != null)
            {
                working_element = element;
            }
            else
            {
                string error_message = "[UIManager] Element with name " + name + " could not be found";
                Debug.LogError(error_message);
            }
        }

        ret = working_element;

        return ret;
    }

    /// <summary>>> Removes the current working working element from current</summary>
    public void ResetWorkingElement()
    {
        if(working_element != null)
        {
            working_element = null;
        }
    }

    /// <summary>>> Activates or deactivates the game object.</summary>
    /// <param name="set_active"> True for activate, false to deactivate. </param>
    public void EffectSetActive(bool set_active)
    {
        if (working_element != null)
        {
            UIEffectSetActive ef_set_active = new UIEffectSetActive(working_element, set_active);
            curr_cluster.Add(ef_set_active);

            last_effect = ef_set_active;
        }
        else
        {
            EffectError();
        }
    }

    /// <summary>>> Waits x seconds.</summary>
    /// <param name="wait_time"> Time in seconds to wait. </param>
    public void EffectWait(float wait_time)
    {
        if (working_element != null)
        {
            UIEffectWait ef_wait = new UIEffectWait(working_element, wait_time);
            curr_cluster.Add(ef_wait);

            last_effect = ef_wait;
        }
        else
        {
            EffectError();
        }
    }

    /// <summary>>> Changes the oppacity of the game object and childs.</summary>
    /// <param name="start_fade_value"> Starting fade value.</param> 
    /// <param name="final_fade_value"> Ending fade value.</param>
    /// <param name="duration"> Duration of the transition.</param>
    public void EffectFade(float start_fade_value, float final_fade_value, float duration)
    {
        if (working_element != null)
        {
            UIEffectFade ef_fade = new UIEffectFade(working_element, start_fade_value, final_fade_value, duration);
            curr_cluster.Add(ef_fade);

            last_effect = ef_fade;
        }
        else
        {
            EffectError();
        }
    }

    /// <summary>>> Enables or disables the interactability of the object and childs.</summary>
    /// <param name="set_interactable"> True enable interactable, false to disable it. </param>
    public void EffectSetInteractable(bool set_interactable)
    {
        if (working_element != null)
        {
            UIEffectSetInteractable ef_i = new UIEffectSetInteractable(working_element, set_interactable);
            curr_cluster.Add(ef_i);

            last_effect = ef_i;
        }
        else
        {
            EffectError();
        }
    }

    /// <summary>>> Scales a game object to the desired size.</summary>
    /// <param name="start_scale"> Starting scale.</param> 
    /// <param name="final_scale"> Ending scale.</param>
    /// <param name="type"> Type of curve used for the movement.</param>
    /// <param name="duration"> Duration of the transition.</param>
    public void EffectScale(Vector2 start_scale, Vector2 final_scale, MovementType type, float duration)
    {
        if (working_element != null)
        {
            UIEffectScale ef_scale = new UIEffectScale(working_element, type, start_scale, final_scale, duration);

            curr_cluster.Add(ef_scale);

            last_effect = ef_scale;
        }
        else
        {
            EffectError();
        }
    }

    /// <summary>>> Moves a game object to the desired position.</summary>
    /// <param name="start_pos"> Starting position.</param> 
    /// <param name="final_pos"> Ending position.</param>
    /// <param name="type"> Type of curve used for the movement.</param>
    /// <param name="duration"> Duration of the transition.</param>
    public void EffectMove(Vector2 start_pos, Vector2 final_pos, MovementType type, float duration)
    {
        if (working_element != null)
        {
            UIEffectMoveToPos ef_move = new UIEffectMoveToPos(working_element, type, start_pos, final_pos, duration);
            curr_cluster.Add(ef_move);

            last_effect = ef_move;
        }
        else
        {
            EffectError();
        }
    }

    /// <summary>>> Adds a callback for the last effect used, for when the effect has finished.</summary>
    public void LastEffectCallbackOnFinished(UIEffectDel callback)
    {
        if(last_effect != null)
        {
            last_effect.SuscribeFinished(callback);
        }
    }

    /// <summary>>> Adds a callback for the last effect used, for when the effect has started.</summary>
    public void LastEffectCallbackOnStarted(UIEffectDel callback)
    {
        if (last_effect != null)
        {
            last_effect.SuscribeStarted(callback);
        }
    }

    /// <summary>>> Pushes the added effects to the queue, which will execute at the same time.</summary>
    public void PushEffects()
    {
        List<UIEffect> new_cluster = new List<UIEffect>(curr_cluster);

        effects_cluster.Add(new_cluster);

        curr_cluster.Clear();
    }

    private void CheckNextEffects()
    {
        if(effects_cluster.Count > 0)
        {
            List<UIEffect> curr_active_cluster = effects_cluster[0];

            bool finished = true;

            for (int i = 0; i < curr_active_cluster.Count; ++i)
            {
                UIEffect curr_effect = curr_active_cluster[i];

                if (!curr_effect.GetStarted())
                {
                    curr_effect.Start();
                    curr_effect.SetStarted();
                }

                if(!curr_effect.GetFinished())
                {
                    finished = false;
                    break;
                }
            }

            if (finished)
            {
                if (effects_cluster.Count > 0)
                {
                    effects_cluster.RemoveAt(0);

                    if (effects_cluster.Count > 0)
                    {
                        List<UIEffect> new_active_cluster = effects_cluster[0];

                        for (int i = 0; i < new_active_cluster.Count; ++i)
                        {
                            UIEffect curr_effect = new_active_cluster[i];

                            curr_effect.Start();
                            curr_effect.SetStarted();
                        }
                    }
                }
            }
        }
    }

    private void UpdateEffects()
    {
        if (effects_cluster.Count > 0)
        {
            List<UIEffect> curr_active_cluster = effects_cluster[0];

            for (int i = 0; i < curr_active_cluster.Count; ++i)
            {
                curr_active_cluster[i].Update();
            }
        }
    }

    private void EffectError()
    {
        string error_message = "[UIManager] Could not use effect, there is not a selected working window";
        Debug.LogError(error_message);
    }

    private List<UIElement> elements = new List<UIElement>();

    UIElement working_element = null;
    UIEffect last_effect = null;

    private List<UIEffect> curr_cluster = new List<UIEffect>();
    private List<List<UIEffect>> effects_cluster = new List<List<UIEffect>>();
}
