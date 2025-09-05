using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackOnShip
{
    public class WarningLight : MonoBehaviour
    {
        public float Speed = 1;

        // Update is called once per frame
        void Update()
        {
            transform.localEulerAngles += new Vector3(0, Speed * Time.deltaTime, 0);
        }
    }
}
