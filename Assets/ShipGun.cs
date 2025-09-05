using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace AttackOnShip
{
    public class ShipGun : MonoBehaviour
    {
        public GameObject Prefab, Fx;

        private float _timer = 0;

        private float _delay = 1;

        // Start is called before the first frame update
        void Start()
        {
            _delay = Random.Range(1f, 4f);
            InvokeRepeating("Stop", 0, Random.Range(3f, 5f));            
        }

        void Stop()
        {
            Fx.SetActive(false);
            _delay = Random.Range(1f, 3f);
        }

        // Update is called once per frame
        void Update()
        {
            _timer -= Time.deltaTime;
            _delay -= Time.deltaTime;

            if (_timer <= 0 && _delay <= 0)
            {
                if(!Fx.activeSelf)
                    Fx.SetActive(true);

                _timer = Random.Range(0.25f, 0.35f);

                var go = Instantiate(Prefab, transform);

                go.transform.localPosition = new Vector3(Random.Range(-2f,2), Random.Range(-2f, 2), 0);

                go.transform.DOMove(Vector3.zero + new Vector3(Random.Range(-10f, 10), Random.Range(0f, 10), 0), 0.5f).OnComplete(() => { Destroy(go);  });

                go.gameObject.SetActive(true);
            }
        }
    }
}
