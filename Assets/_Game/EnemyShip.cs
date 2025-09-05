using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyShip : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Orbit Settings")]
    public float orbitRadius = 5f;
    public float orbitSpeed = 50f;        // độ/giây
    public bool clockwise = true;
    public float sideYaw = 90f;           // 90 = sườn phải hướng vào player

    [Header("Combat")]
    public int maxHealth = 100;
    public LayerMask damageLayers;
    public int damageOnHit = 10;

    [Header("Collider Settings")]
    public bool colliderConvex = true;
    public bool colliderIsTrigger = true;

    [Header("Damage VFX (Smoke)")]
    [Tooltip("Kéo thả các GameObject khói (Particle) khác nhau trên thân tàu. Để Inactive sẵn.")]
    public GameObject[] smokeObjects;
    public bool smokeSequential = true;   // true: lần lượt; false: ngẫu nhiên
    public float smokeDuration = 2.5f;    // <=0: không tự tắt

    [Header("State")]
    [Tooltip("Cho phép thuyền di chuyển vòng quanh hay đứng im.")]
    public bool canMove = true;

    [SerializeField, Tooltip("Đã nổ/chết chưa? (read-only)")]
    private bool isDead = false;
    public bool IsDead => isDead;

    private float angle;
    private float initialY;
    private int currentHealth;
    private int smokeIndex;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();

        // MeshCollider & Rigidbody
        var mc = GetComponent<MeshCollider>();
        if (mc != null)
        {
            if (mc.sharedMesh == null)
            {
                var mf = GetComponent<MeshFilter>();
                if (mf == null) mf = GetComponentInChildren<MeshFilter>();
                if (mf != null) mc.sharedMesh = mf.sharedMesh;
            }
            mc.convex = colliderConvex;
            mc.isTrigger = colliderIsTrigger;
        }

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        // tắt khói ban đầu
        if (smokeObjects != null)
        {
            foreach (var go in smokeObjects)
            {
                if (!go) continue;
                go.SetActive(false);
                var ps = go.GetComponentsInChildren<ParticleSystem>(true);
                foreach (var p in ps) p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        //angle = Random.Range(0f, 360f);
        initialY = transform.position.y; // khóa Y
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!player || isDead) // chết rồi thì không làm gì thêm
        {
            if (animator) animator.SetBool("isMoving", false);
            return;
        }

        // di chuyển vòng tròn nếu được phép
        if (canMove)
        {
            angle += (clockwise ? -1f : 1f) * orbitSpeed * Time.deltaTime;
            if (angle >= 360f) angle -= 360f;
            if (angle < 0f) angle += 360f;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 offsetXZ = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius;
            transform.position = new Vector3(player.position.x + offsetXZ.x, initialY, player.position.z + offsetXZ.z);
            
            Vector3 toPlayerFlat = new Vector3(player.position.x - transform.position.x, 0f, player.position.z - transform.position.z);
            if (toPlayerFlat.sqrMagnitude > 0.0001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(toPlayerFlat.normalized, Vector3.up);
                transform.rotation = lookRot * Quaternion.Euler(0f, sideYaw, 0f);
            }

            if (animator) animator.SetBool("isMoving", canMove);
        }

        // quay sườn vào player (yaw-only), kể cả khi đứng im
        
    }

    // === Damage ===
    public void TakeDamage(int amount)
    {
        if (isDead) return; // ✅ không nhận damage khi đã nổ

        currentHealth -= Mathf.Max(0, amount);
        if (animator) animator.SetTrigger("Hit");

        ShowSmoke(); // bật một cụm khói

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return; // tránh gọi nhiều lần
        isDead = true;

        if (animator) animator.SetTrigger("Die");

        // tắt collider để không còn va chạm/damage
        var mc = GetComponent<MeshCollider>();
        if (mc) mc.enabled = false;

        // có thể tắt Rigidbody va chạm
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.detectCollisions = false;

        // TODO: hiệu ứng nổ, âm thanh...
        //Destroy(gameObject, 0.2f);
    }

    // Trigger-based
    void OnTriggerEnter(Collider other)
    {
        if (isDead || !colliderIsTrigger) return;
        if (((1 << other.gameObject.layer) & damageLayers) != 0)
        {
            TakeDamage(damageOnHit);
        }
    }

    // Collision-based
    void OnCollisionEnter(Collision collision)
    {
        if (isDead || colliderIsTrigger) return;
        if (((1 << collision.gameObject.layer) & damageLayers) != 0)
        {
            TakeDamage(damageOnHit);
        }
    }

    // === Smoke helpers ===
    private void ShowSmoke()
    {
        if (smokeObjects == null || smokeObjects.Length == 0) return;

        int idx = smokeSequential ? (smokeIndex++ % smokeObjects.Length)
                                  : Random.Range(0, smokeObjects.Length);

        var go = smokeObjects[idx];
        if (!go) return;

        ActivateSmoke(go);
    }

    private void ActivateSmoke(GameObject go)
    {
        go.SetActive(true);

        var ps = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var p in ps) p.Play(true);

        if (smokeDuration > 0f)
            StartCoroutine(DeactivateAfter(go, smokeDuration));
    }

    private IEnumerator DeactivateAfter(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        if (!go) yield break;

        var ps = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var p in ps) p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        go.SetActive(false);
    }
}
