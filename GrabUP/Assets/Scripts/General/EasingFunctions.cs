﻿using System;

// Based on https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBase/Tween/Easing.cs

// t = current_time
// b = starting_value
// c = total_value
// d = total_duration

public class EasingFunctions
{
    public static float Linear(float total_value, float current_time, float starting_value, float total_duration)
    {
        return total_value * current_time / total_duration + starting_value;
    }

    public static float ExpoIn(float total_value, float current_time, float starting_value, float total_duration)
    {
        return (current_time == 0) ? starting_value : total_value * (float)Math.Pow(2, 10 * (current_time / total_duration - 1))
            + starting_value - total_value * 0.001f;
    }

    public static float ExpoOut(float total_value, float current_time, float starting_value, float total_duration)
    {
        return (current_time == total_duration) ? starting_value + total_value :
            total_value * (-(float)Math.Pow(2, -10 * current_time / total_duration) + 1) + starting_value;
    }

    public static float ExpoInOut(float total_value, float current_time, float starting_value, float total_duration)
    {
        if (current_time == 0) return starting_value;
        if (current_time == total_duration) return starting_value + total_value;
        if ((current_time /= total_duration / 2) < 1) return total_value / 2 * (float)Math.Pow(2, 10 * (current_time - 1)) + starting_value;
        return total_value / 2 * (-(float)Math.Pow(2, -10 * --current_time) + 2) + starting_value;
    }

    public static float Bounce(float total_value, float current_time, float starting_value, float total_duration)
    {
        if ((current_time /= total_duration) < (1 / 2.75f))
        {
            return total_value * (7.5625f * current_time * current_time) + starting_value;
        }
        else if (current_time < (2 / 2.75))
        {
            return total_value * (7.5625f * (current_time -= (1.5f / 2.75f)) * current_time + .75f) + starting_value;
        }
        else if (current_time < (2.5f / 2.75f))
        {
            return total_value * (7.5625f * (current_time -= (2.25f / 2.75f)) * current_time + .9375f) + starting_value;
        }
        else
        {
            return total_value * (7.5625f * (current_time -= (2.625f / 2.75f)) * current_time + .984375f) + starting_value;
        }
    }

    public static float QuadIn(float total_value, float current_time, float starting_value, float total_duration)
    {
        return total_value * (current_time /= total_duration) * current_time + starting_value;
    }

    public static float QuadOut(float total_value, float current_time, float starting_value, float total_duration)
    {
        return -total_value * (current_time /= total_duration) * (current_time - 2) + starting_value;
    }

    public static float QuadInOut(float total_value, float current_time, float starting_value, float total_duration)
    {
        if ((current_time /= total_duration / 2) < 1) return total_value / 2 * current_time * current_time + starting_value;
        return -total_value / 2 * ((--current_time) * (current_time - 2) - 1) + starting_value;
    }
}
