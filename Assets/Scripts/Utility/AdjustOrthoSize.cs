using UnityEngine;

public class AdjustOrthoSize : MonoBehaviour
{
    private void Start()
    {
        Camera _cam = GetComponent<Camera>();
        float _orthoSize = _cam.orthographicSize;
        float ratio16x9 = 16f / 9f;

        float height = Screen.height;
        float width = Screen.width;
        float currentRatio = height / width;

        _cam.orthographicSize = currentRatio * _orthoSize / ratio16x9;
    }
}
