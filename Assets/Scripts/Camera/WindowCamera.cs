using UnityEngine;
using Utils;

public class WindowCamera : SingletonMono<WindowCamera>
{
    public Camera Camera;

    private void Update()
    {
        var pos = CameraController.Instance.transform.position;
        transform.position = new Vector3(pos.x, pos.z, -pos.y);
    }
}
