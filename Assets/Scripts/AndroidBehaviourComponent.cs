using UnityEngine;

namespace Android
{
    public class AndroidBehaviourComponent : MonoBehaviour
    {
        public delegate void AndroidBackButtonActionDelegate();

        private static AndroidBehaviourComponent instance;
        public static AndroidBehaviourComponent Instance
        {
            get
            {
#if UNITY_ANDROID
                if (instance == null)
                {
                    GameObject gameObject = new GameObject(nameof(AndroidBehaviourComponent));
                    instance = gameObject.AddComponent<AndroidBehaviourComponent>();
                }
                return instance;
#else
                return null;
#endif
            }
        }

        public AndroidBackButtonActionDelegate onBack;

        private void Awake()
        {
#if UNITY_ANDROID
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogWarning($"Duplication instance of {nameof(AndroidBehaviourComponent)} has awakened, the latest one will be destroyed.");
                Destroy(gameObject);
                return;
            }
#endif
        }

        private void Update()
        {
#if UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onBack?.Invoke();
            }
#endif
        }

        /// <summary>
        /// Set timeout of the screen device.
        /// Use <see cref="SleepTimeout.NeverSleep"/> for never sleep.
        /// Use <see cref="SleepTimeout.SystemSetting"/> for system settings.
        /// </summary>
        /// <param name="time">Time measured in seconds.</param>
        public void SetScreenSleepTimeout(int time)
        {
#if UNITY_ANDROID
            Screen.sleepTimeout = time;
#endif
        }

        /// <summary>
        /// Set target framerate for this device.
        /// Use -1 for no limits.
        /// </summary>
        /// <param name="framerate">Framerate measured in frame per seconds.</param>
        public void SetTargetFramerate(int framerate)
        {
            Application.targetFrameRate = framerate;
        }
    }
}