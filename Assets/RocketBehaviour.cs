using System;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    private GameObject explosionParticle;
    private Vector3 direction;
    private float speed;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Tắt gravity nếu cần (tên lửa bay thẳng)
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    // Hàm được gọi từ RocketLauncher để setup tên lửa
    public void Setup(GameObject explosionPrefab, Vector3 dir, float moveSpeed)
    {
        this.explosionParticle = explosionPrefab;
        this.direction = dir;
        this.speed = moveSpeed;

        // Bắt đầu di chuyển
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 contact = other.ClosestPointOnBounds(transform.position);
            Quaternion rot = Quaternion.LookRotation(contact);
            AudioManager.ins.PlaySoundExplor();
            Instantiate(explosionParticle, contact, rot);
        }

        // Hủy tên lửa
        Destroy(gameObject,10);
    }

    // Hủy tên lửa nếu không va chạm trong thời gian dài (phòng trường hợp bay mãi)
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}