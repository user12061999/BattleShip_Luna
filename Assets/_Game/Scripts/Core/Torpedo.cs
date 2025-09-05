using UnityEngine;

namespace AttackOnShip
{
    public class Torpedo : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private int _damage = 500;
        [SerializeField] private float _lifetime = 7f;
        [SerializeField] private GameObject _explosionFxPf;

        public Type TorpedoType;
        public GameObject FollowCamera;

        public enum Type
        {
            Normal,
            Triple,
            Scatter,
            Homing,
            Guide,
        }

        public Transform Target = null;

        private float _timer;

        public int Damage => _damage;

        public Vector3 PosZeroY
        {
            get
            {
                var value = transform.position;
                value.y = 0f;
                return value;
            }
        }

        private void Awake() => _timer = _lifetime;

        private bool _touch = false;
        private Vector3 _startPos, _endPos, _cameraPos;

        private void Start()
        {
            if (FollowCamera)
            {
                FollowCamera.transform.parent = transform.parent;

                _cameraPos = FollowCamera.transform.position - transform.position;

                
            }

            if ((TorpedoType == Type.Homing && Target != null) || TorpedoType == Type.Guide)
            {
                //TargetLock.Instance.Missile.Add(this);
            }
        }

        private void LateUpdate()
        {
            if (FollowCamera)
            {
                FollowCamera.transform.position = transform.position + _cameraPos;
            }
        }

        private void FixedUpdate()
        {
            if (TorpedoType == Type.Guide && _touch)
            {
                Vector3 movement = (new Vector3(_endPos.x, 0, _endPos.y) - new Vector3(_startPos.x, 0, _startPos.y)).normalized;

                //if (movement.x < 0)
                //    movement.x *= -1;

                if (movement.z < 0)
                    

                if (transform.position.z < 0)
                        movement.z *= -1;
                //movement = (new Vector3(_startPos.x, 0, _startPos.y) - new Vector3(_endPos.x, 0, _endPos.y)).normalized;

                //if (movement.magnitude > 0)
                {
                    //transform.position += transform.right * (_endPos.x - _startPos.x) * Time.deltaTime * 1;

                    //Quaternion targetRot = Quaternion.LookRotation(movement);

                    //targetRot = Quaternion.RotateTowards(transform.rotation, targetRot, Time.fixedDeltaTime * 360);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation + Quaternion.EulerAngles(0,10,0), 0.15f);

                    transform.eulerAngles += new Vector3(0, (_endPos.x - _startPos.x) * Time.deltaTime * 2, 0);

                    //transform.position += (transform.forward + (movement).normalized * 0.025f) * _moveSpeed * Time.deltaTime;
                    //transform.rotation = Quaternion.LookRotation(transform.forward + (movement).normalized * 0.025f);
                }

                _startPos = _endPos;
            }
        }

        private void Update()
        {
            if (TorpedoType == Type.Guide)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _touch = true;
                    _startPos = Input.mousePosition;
                }

                if (Input.GetMouseButton(0) && _touch)
                {
                    _endPos = Input.mousePosition;
                }

                if(Input.GetMouseButtonUp(0))
                {
                    _touch = false;
                }

                //if(TargetLockTrigger.Instance.GetClosestTarget())
                //{
                //    if(!TargetLock.Instance.Missile.Contains(transform))
                //        TargetLock.Instance.Missile.Add(transform);
                //}
                //else
                //{
                //    if (TargetLock.Instance.Missile.Contains(transform))
                //        TargetLock.Instance.Missile.Remove(transform);
                //}
            }

            if (TorpedoType != Type.Triple && TorpedoType != Type.Scatter)
            {
                if (TorpedoType == Type.Homing && Target != null)
                {                    
                    transform.position += (transform.forward + (Target.position - transform.position).normalized*0.025f) * _moveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation(transform.forward + (Target.position - transform.position).normalized * 0.025f);
                }
                else
                {
                    transform.position += transform.forward * _moveSpeed * Time.deltaTime;                    
                }
            }            

            _timer -= Time.deltaTime;

            if (_timer < 0f)
                Deactive();
        }

        public void Deactive()
        {            
            gameObject.SetActive(false);

            if (TorpedoType == Type.Homing || TorpedoType == Type.Guide)
            {
                //TargetLock.Instance.Missile.Remove(this);
            }

            if (TorpedoType == Type.Guide)
            { 
                FollowCamera.SetActive(false);

                
            }
        }

        public void Explode()
        {
            if (TorpedoType == Type.Triple && TorpedoType == Type.Scatter)
                return;

            if (TorpedoType == Type.Homing || TorpedoType == Type.Guide)
            {
                //TargetLock.Instance.Missile.Remove(this);
            }

            if (TorpedoType == Type.Guide)
            {
                FollowCamera.SetActive(false);

                
            }

            Instantiate(_explosionFxPf, PosZeroY, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
