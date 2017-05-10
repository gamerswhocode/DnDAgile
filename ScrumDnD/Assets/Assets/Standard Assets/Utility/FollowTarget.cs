using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{


    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 menuOffset = new Vector3(0f, 7.5f, 0f);
        public Vector3 cameraOffset;
        public bool isMenu = false;

        private void LateUpdate()
        {

            if (!isMenu)
                transform.position =
                    new Vector3(target.position.x,  cameraOffset.y, transform.position.z + cameraOffset.z);
            else
            {
                transform.localPosition = target.position + menuOffset;
            }
        }
    }
}
