using LuviKunG.Console;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    [RequireComponent(typeof(LuviConsole))]
    [DisallowMultipleComponent]
    public class ConsoleInstance : MonoBehaviour
    {
        [SerializeField]
        private LuviConsole console = default;
        [SerializeField]
        private MobileCameraComponent mobileCameraComp = default;

        private void Awake()
        {
            console ??= LuviConsole.Instance;
            console.AddCommand("/deviceOrientation", OnCommandExecuteDeviceRotation);
            console.AddCommand("/deviceCamera", OnCommandExecuteDeviceCamera);
            console.AddCommand("/useCamera", OnCommandExecuteUseCamera);
            console.AddCommandPreset("/deviceOrientation", "Device Orientation", "Get current device orientation from \'Input.deviceOrientation\'.", "Device", true);
            console.AddCommandPreset("/deviceCamera", "Device Camera", "Get all available device camera from \'WebCamTexture.devices\'.", "Device", true);
            console.AddCommandPreset("/useCamera ", "Use Camera", "Use available camera from \'WebCamTexture.devices\' to \'MobileCameraComponent\'", "Component", false);
        }

        private void OnCommandExecuteDeviceRotation(IReadOnlyList<string> arguments)
        {
            var deviceOrientation = Input.deviceOrientation;
            console.Log($"Device Orientation: {deviceOrientation}", Color.cyan, true, false);
        }

        private void OnCommandExecuteScreenRotation(IReadOnlyList<string> arguments)
        {
            var screenOrientation = Screen.orientation;
            console.Log($"Screen Orientation: {screenOrientation}", Color.cyan, true, false);
        }

        private void OnCommandExecuteDeviceCamera(IReadOnlyList<string> arguments)
        {
            var devices = WebCamTexture.devices;
            if (devices != null && devices.Length > 0)
            {
                console.Log("Available cameras.", Color.green, true, false);
                for (int i = 0; i < devices.Length; i++)
                {
                    var device = devices[i];
                    if (i > 0) console.Log("========", Color.green);
                    console.Log($"index: {i}", Color.green);
                    console.Log($"name: {device.name}", Color.green);
                    console.Log($"depthCameraName: {device.depthCameraName}", Color.green);
                    console.Log($"kind: {device.kind}", Color.green);
                    console.Log($"isFrontFacing: {device.isFrontFacing}", Color.green);
                    console.Log($"isAutoFocusPointSupported: {device.isAutoFocusPointSupported}", Color.green);
                    console.Log($"availableResolutions:", Color.green);
                    for (int j = 0; j < device.availableResolutions.Length; j++)
                    {
                        var resolution = device.availableResolutions[i];
                        console.Log($"  {resolution.width}x{resolution.height} {resolution.refreshRate}Hz", Color.green);
                    }
                    console.Log("========", Color.green);
                }
            }
            else
            {
                console.Log("No camera are available.", Color.red, true, false);
            }
        }

        private void OnCommandExecuteUseCamera(IReadOnlyList<string> arguments)
        {
            if (mobileCameraComp == null)
            {
                console.Log($"No reference of {nameof(MobileCameraComponent)}", Color.red, true, false);
                return;
            }
            var cameraIndex = arguments.Count >= 2 ? (int.TryParse(arguments[1], out int arg1) ? arg1 : -1) : -1;
            var resolutionIndex = arguments.Count >= 3 ? (int.TryParse(arguments[2], out int arg2) ? arg2 : 0) : 0;
            if (cameraIndex < 0)
            {
                console.Log("Wrong camera index. (wrong parse)", Color.red, true, false);
                return;
            }
            var devices = WebCamTexture.devices;
            if (cameraIndex >= devices.Length)
            {
                console.Log("Wrong camera index. (available camera index out of bound)", Color.red, true, false);
                return;
            }
            mobileCameraComp.SelectCameraDevice(devices[cameraIndex], resolutionIndex);
        }
    }
}