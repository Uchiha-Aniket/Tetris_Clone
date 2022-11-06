using Core;
using UnityEngine;

public class ShapeHolder : MonoBehaviour
{
    [SerializeField] private Transform _holdForm;
    private Shape _heldShape;
    private float _heldShapeSize = .5f;
    private bool _canRelease;
    public bool CanRelease
    {
        get => _canRelease;
        set => _canRelease = value;
    }

    // ----

    public bool IsShapeHeld() => _heldShape ? true : false;

    public void Hold(Shape shape)
    {
        if (_heldShape) return;

        shape.transform.position = _holdForm.transform.position + shape._queueOffset;
        shape.transform.localScale = new Vector3(_heldShapeSize, _heldShapeSize, _heldShapeSize);
        shape.transform.rotation = Quaternion.identity;
        _heldShape = shape;
    }

    public Shape Release()
    {
        _heldShape.transform.localScale = Vector3.one;
        var shape = _heldShape;
        _heldShape = null;
        _canRelease = false;

        return shape;
    }
}
