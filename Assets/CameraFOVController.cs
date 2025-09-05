using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraFOVController : MonoBehaviour
{
    public Camera targetCamera; // Camera cần điều chỉnh FOV
    public Slider fovSlider;    // Slider UI
    public TextMeshProUGUI txtFOV;   // Slider UI

    void Start()
    {
        // Gán camera mặc định nếu chưa chọn
        if (targetCamera == null)
            targetCamera = Camera.main;

        // Gán giá trị ban đầu cho slider
        if (fovSlider != null)
        {
            fovSlider.minValue = 20f;
            fovSlider.maxValue = 80f;
            fovSlider.value = targetCamera.fieldOfView;
            txtFOV.text = "x"+(1+(80-fovSlider.value)/15f).ToString("F1");
            // Đăng ký sự kiện thay đổi giá trị
            fovSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    public void OnSliderValueChanged(float newValue)
    {
        if (targetCamera != null)
        {
            
            targetCamera.fieldOfView = newValue;
            txtFOV.text = "x"+(1+(80-newValue)/15f).ToString("F1");
        }
    }

    // Dọn dẹp khi destroy
    private void OnDestroy()
    {
        if (fovSlider != null)
            fovSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }
}