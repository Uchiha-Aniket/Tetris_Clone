using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    private int _lines;
    private int _level;

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _linesText;
    [SerializeField] private TextMeshProUGUI _levelText;

    private const byte MIN_LINES = 1;
    private const byte MAX_LINES = 4;

    [Space]
    public int linesPerLevel = 5;

    [Space]
    [SerializeField] private ParticleSystem _levelUpFx;

    // ----

    private void Start()
    {
        Reset();   
    }

    private void Reset()
    {
        _level = 1;
        _lines = linesPerLevel * _level;
        UpdateUITexts();
    }

    // ----

    public void ScoreLines(int noOfLines)
    {
        noOfLines = Mathf.Clamp(noOfLines, MIN_LINES, MAX_LINES);

        switch (noOfLines)
        {
            case 1:
                _score += 40 * _level;
                break;

            case 2:
                _score += 100 * _level;
                break;

            case 3:
                _score += 300 * _level;
                break;

            case 4:
                _score += 1200 * _level;
                break;

            default:
                break;
        }

        _lines -= noOfLines;
        if (_lines <= 0) LevelUp();

        UpdateUITexts();
    }

    private void UpdateUITexts()
    {
        _scoreText.text = _score.ToString();
        _linesText.text = _lines.ToString();
        _levelText.text = _level.ToString();
    }

    private void LevelUp()
    {
        _level++;
        _lines = _level * linesPerLevel;

        if (_levelUpFx) _levelUpFx.Play();
    }
}
