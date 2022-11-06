using Unity.VisualScripting;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public delegate void TouchEventHandler(Vector2 swipe);
    
    public static event TouchEventHandler DragEvent;
    public static event TouchEventHandler SwipeEvent;
    public static event TouchEventHandler TapEvent;

    private Vector2 _touchMovement;

    [Range(50, 250)]
    [SerializeField] private short _minDragMovement = 100;
    [Range(50, 250)]
    [SerializeField] private short _minSwipeMovement = 200;

    private float _tapTimeMax;
    private float _tapTimeWindow = .1f;

    // ----

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                _touchMovement = Vector2.zero;
                _tapTimeMax = Time.time + _tapTimeWindow;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                _touchMovement += touch.deltaPosition;
                if (_touchMovement.magnitude > _minDragMovement) OnDrag();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (_touchMovement.magnitude > _minSwipeMovement) OnSwipe();
                else if (Time.time < _tapTimeMax) OnTap();
            }
        }
    }

    // ----

    private void OnDrag()
    {
        if (DragEvent != null) DragEvent(_touchMovement);
    }

    private void OnSwipe()
    {
        if (SwipeEvent != null) SwipeEvent(_touchMovement);
    }

    private void OnTap()
    {
        if (TapEvent != null) TapEvent(_touchMovement);
    }
}
