using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    CameraManager()
    {
        InitInstance(this);
    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        UpdateCameraFollowing();
    }

    public void SetHorizontalSplitScreen(Camera left_camera, Camera right_camera)
    {
        if(left_camera != null && right_camera != null)
        {
            left_camera.rect = new Rect(0, 0, 0.5f, 1);
            right_camera.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
    }

    public void SetVerticalSplitScreen(Camera up_camera, Camera down_camera)
    {
        if (up_camera != null && down_camera != null)
        {
            up_camera.rect = new Rect(0, 0.5f, 1, 0.5f);
            down_camera.rect = new Rect(0, 0, 1, 0.5f);
        }
    }

    public void CameraFollow(Camera camera, GameObject go, float movement_time, Vector3 offset)
    {
        if (go != null && camera != null)
        {
            CameraStopFollow(camera);
            
            CameraFollowItem item = new CameraFollowItem(camera, go, movement_time, offset);
            following_items.Add(item);
        }
    }

    public void CameraUpdateFollow(Camera camera, float movement_time, Vector3 offset)
    {
        for (int i = 0; i < following_items.Count; ++i)
        {
            CameraFollowItem curr_item = following_items[i];

            if (curr_item.GetCamera() == camera)
            {
                curr_item.SetMovementTime(movement_time);
                curr_item.SetOffset(offset);
            }
        }
    }

    public void CameraStopFollow(Camera camera)
    {
        for (int i = 0; i < following_items.Count; ++i)
        {
            CameraFollowItem curr_item = following_items[i];

            if (curr_item.GetCamera() == camera)
            {
                following_items.RemoveAt(i);

                break;
            }
        }
    }

    public void CamerasStopFollow(GameObject go)
    {
        for (int i = 0; i < following_items.Count;)
        {
            CameraFollowItem curr_item = following_items[i];

            if (curr_item.GetFollowingGameObject() == go)
            {
                following_items.RemoveAt(i);
            }
            else
                ++i;
        }
    }

    private void UpdateCameraFollowing()
    {
        for (int i = 0; i < following_items.Count; ++i)
        {
            CameraFollowItem curr_item = following_items[i];

            float movement_time = curr_item.GetMovementTime();
            Vector3 offset = curr_item.GetOffset();
            Camera camera = curr_item.GetCamera();
            Transform following_trans = curr_item.GetFollowingGameObject().transform;

            Vector3 desired_pos = following_trans.position + offset;

            Vector3 smoothed_position = Vector3.SmoothDamp(camera.transform.position, desired_pos, ref curr_item.velocity, movement_time);

            if (!camera.orthographic)
                camera.transform.position = smoothed_position;
            else
                camera.transform.position = new Vector3(smoothed_position.x, smoothed_position.y, camera.transform.position.z);
        }
    }

    public class CameraFollowItem
    {
        public CameraFollowItem(Camera camera, GameObject to_follow, float movement_time, Vector3 offset)
        {
            this.camera = camera;
            this.to_follow = to_follow;
            this.movement_time = movement_time;
            this.offset = offset;
        }

        public GameObject GetFollowingGameObject()
        {
            return to_follow;
        }

        public Camera GetCamera()
        {
            return camera;
        }

        public void SetMovementTime(float movement_time)
        {
            if(movement_time >= 0)
                this.movement_time = movement_time;
        }

        public float GetMovementTime()
        {
            return movement_time;
        }

        public void SetOffset(Vector3 offset)
        {
            this.offset = offset;
        }

        public Vector3 GetOffset()
        {
            return offset;
        }

        private Camera camera = null;
        private GameObject to_follow = null;
        private float movement_time = 0.0f;
        private Vector3 offset = Vector3.zero;

        public Vector3 velocity = Vector3.zero;
    }

    private List<CameraFollowItem> following_items = new List<CameraFollowItem>();
}
