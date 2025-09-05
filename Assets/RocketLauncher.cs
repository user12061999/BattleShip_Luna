using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

public class RocketLauncher : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public GameObject rocketPrefab;
    public GameObject explosionParticle;
    public Transform spawnPoint;
    public Transform cameraTransform;

    [Header("UI")]
    public Button fireButton;
    public Image fireIndicatorImage; // Ảnh màu đỏ
    public TextMeshProUGUI cooldownText;        // Text thời gian hồi chiêu

    [Header("Settings")]
    public float rocketSpeed = 20f;
    public float cooldownTime = 3f;

    [Header("Camera Settings")]
    public float rotationSpeed = 0.2f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private Vector2 previousPointerPosition;
    private float yaw = 0f;
    private float pitch = 0f;
    private bool isPointerDown = false;
    private float nextFireTime = 0f;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        Vector3 euler = cameraTransform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;

        // Gán sự kiện bắn cho button
        if (fireButton != null)
        {
            fireButton.onClick.AddListener(FireRocket);
        }
    }

    void Update()
    {
        float timeLeft = nextFireTime - Time.time;

        if (timeLeft <= 0f)
        {
            // Được bắn
            if (fireButton != null) fireButton.interactable = true;
            if (fireIndicatorImage != null) fireIndicatorImage.gameObject.SetActive(true);
            if (cooldownText != null) cooldownText.text = "Ready";
        }
        else
        {
            // Đang hồi chiêu
            if (fireButton != null) fireButton.interactable = false;
            if (fireIndicatorImage != null) fireIndicatorImage.gameObject.SetActive(false);
            if (cooldownText != null) cooldownText.text = timeLeft.ToString("F1") + "s";
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        previousPointerPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPointerDown) return;
        isPointerDown = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPointerDown) return;

        Vector2 delta = eventData.delta;

        yaw += delta.x * rotationSpeed;
        pitch -= delta.y * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    public void FireRocket()
    {
        if (Time.time < nextFireTime)
        {
            Debug.Log("Still in cooldown!");
            return;
        }

        LunaManager.ins.CheckClickShowEndCard();
        AudioManager.ins.PlaySoundFire();
        Vector3 direction = cameraTransform.forward;
        direction.y = 0;
        direction.Normalize();

        GameObject rocket = Instantiate(rocketPrefab, spawnPoint.position, Quaternion.LookRotation(direction));

        RocketBehaviour rb = rocket.GetComponent<RocketBehaviour>();
        if (rb != null)
        {
            rb.Setup(explosionParticle, direction, rocketSpeed);
        }
        else
        {
            Debug.LogError("Rocket missing RocketBehaviour!");
            Destroy(rocket);
        }

        nextFireTime = Time.time + cooldownTime;
    }
}
