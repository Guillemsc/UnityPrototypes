using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHead : MonoBehaviour
{
    public void SetController(WeaponController con)
    {
        controller = con;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GrabableItem gi = collision.gameObject.GetComponent<GrabableItem>();

        if (gi != null)
            controller.OnHitGrabbable(gi);
    }

    WeaponController controller;
}
