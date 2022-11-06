using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float _startAlpha;            
    [SerializeField] private float _targetAlpha;
    [SerializeField] private float _delay;
    [SerializeField] private float _timeToFade;

    private CanvasGroup _canvasGroup;    
    private float _inc;

    // ----

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    private void Start()
    {
        _inc = (_targetAlpha - _startAlpha) / _timeToFade * Time.deltaTime;        
        StartCoroutine(Fade_Coroutine());
    }

    // ----

    private IEnumerator Fade_Coroutine()
    {
        yield return new WaitForSeconds(_delay);

        while (_canvasGroup.alpha > _targetAlpha)
        {
            yield return new WaitForEndOfFrame();
            _canvasGroup.alpha += _inc;
        }
        _canvasGroup.alpha = 0f;
    }
}
