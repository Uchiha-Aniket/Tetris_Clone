using Core;
using UnityEngine;

public class GhostHandler : MonoBehaviour
{
    [SerializeField] private Color _color;

    private Shape _ghostShape;
    private bool _hitBottom;

    // ----

    public void Reset()
    {
        _hitBottom = false;
        Destroy(_ghostShape.gameObject);
    }

    // ----

    public void SpawnGhost(Shape originalShape, Board board)
    {
        if (!_ghostShape)
        {
            _ghostShape = Instantiate(originalShape, originalShape.transform.position, originalShape.transform.rotation);
            _ghostShape.gameObject.name = "Ghost Shape";

            var _spriteRenderers = _ghostShape.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.color = _color;
            }
        }
        else
        {
            _ghostShape.transform.SetPositionAndRotation(originalShape.transform.position, originalShape.transform.rotation);
            _ghostShape.transform.localScale = Vector3.one;
        }

        _hitBottom = false;
        while(!_hitBottom)
        {
            _ghostShape.MoveDown();
            if (!board.IsValidPosition(_ghostShape))
            {
                _ghostShape.MoveUp();
                _hitBottom = true;
            }
        }
    }
}
