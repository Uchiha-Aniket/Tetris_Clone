using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour
{
    [SerializeField] private Sprite _toggleOn;
    [SerializeField] private Sprite _toggleOff;
    private Image _image;

    // ----

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
     
    // ----

    public void Toggle(bool value)
    {
        _image.sprite = (value) ? _toggleOn : _toggleOff;
    }
}
