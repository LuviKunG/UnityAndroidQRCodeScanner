using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

namespace App
{
    public class MobileCameraComponent : MonoBehaviour
    {
        [SerializeField]
        private CanvasScaler canvasScaler = default;
        [SerializeField]
        private RawImage rawImageCameraTexture = default;
        [SerializeField]
        private TextMeshProUGUI textQRScanResult = default;
        [SerializeField]
        private RectTransform rectTransformCameraTexture = default;
        [SerializeField]
        private float readInterval = 1.0f;

        private WebCamDevice? currentCameraDevice;
        private Resolution? currentResolution;
        private WebCamTexture cameraTexture;

        private float currentInterval;

        private IEnumerator Start()
        {
            // Skip initial frame.
            yield return null;
            InitializeMobileCamera();
#if UNITY_ANDROID
            var androidInstance = Android.AndroidBehaviourComponent.Instance;
            androidInstance.onBack = () =>
            {
                Application.Quit();
            };
            androidInstance.SetScreenSleepTimeout(SleepTimeout.NeverSleep);
            androidInstance.SetTargetFramerate(30);
#endif
        }

        private void Update()
        {
            if (cameraTexture == null)
                return;
            currentInterval += Time.unscaledDeltaTime;
            if (currentInterval > readInterval)
            {
                currentInterval = 0;
                ReadCameraTexture();
            }
        }

        private void ReadCameraTexture()
        {
            var texture = cameraTexture.GetPixels32();
            if (texture == null && texture.Length == 0)
                return;
            BarcodeReader reader = new BarcodeReader();
            Result resultData = reader.Decode(texture, cameraTexture.width, cameraTexture.height);
            if (resultData != null)
            {
                textQRScanResult.text = resultData.Text;
            }
        }

        private void InitializeMobileCamera()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices == null || devices.Length == 0)
                return;
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].isFrontFacing == false)
                {
                    SelectCameraDevice(devices[i]);
                    break;
                }
            }
        }

        private void SelectCameraDevice(WebCamDevice device)
        {
            if (currentCameraDevice.HasValue)
                DeselectCameraDevice();
            currentCameraDevice = device;
            currentResolution = device.availableResolutions[0];
            cameraTexture = new WebCamTexture(device.name);
            rawImageCameraTexture.texture = cameraTexture;
            cameraTexture.Play();
            UpdateRectTransformCameraTexture();
        }

        private void DeselectCameraDevice()
        {
            currentCameraDevice = null;
            rawImageCameraTexture.texture = null;
            cameraTexture.Stop();
            cameraTexture = null;
        }

        private void UpdateRectTransformCameraTexture()
        {
            Vector2 referenceResolution = canvasScaler.referenceResolution;
            float referenceAspectRatio = referenceResolution.x / referenceResolution.y;
            Resolution resolution = currentResolution.Value;
            float aspectRatio = (float)resolution.width / resolution.height;
            rectTransformCameraTexture.sizeDelta = aspectRatio > referenceAspectRatio ? new Vector2(referenceResolution.x, referenceResolution.x / aspectRatio) : new Vector2(referenceResolution.y * aspectRatio, referenceResolution.y);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            readInterval = Mathf.Max(readInterval, 0.01f);
        }
#endif
    }
}
