using UnityEngine;
using Utils;

public class WindowCamera : SingletonMono<WindowCamera>
{
    public Camera Camera;
    private void Update() => transform.position = CameraController.Instance.transform.position;
}
