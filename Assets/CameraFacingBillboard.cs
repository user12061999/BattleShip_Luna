using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackOnShip
{
    public class CameraFacingBillboard : MonoBehaviour
    {
        //Orient the camera after all movement is completed this frame to avoid jittering
        void LateUpdate()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }
}
