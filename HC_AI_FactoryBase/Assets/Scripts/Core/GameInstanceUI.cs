using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class GameInstanceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private GameObject _pauseIndicator;

    [Inject] private GameInstance _gameInstance;

    private void Start()
    {
        if (_gameInstance != null)
        {
            _gameInstance.OnScoreChanged += UpdateScoreDisplay;
            _gameInstance.OnLevelChanged += UpdateLevelDisplay;
            _gameInstance.OnPauseStateChanged += UpdatePauseDisplay;
            
            UpdateAllDisplays();
        }
    }

    private void OnDestroy()
    {
        if (_gameInstance != null)
        {
            _gameInstance.OnScoreChanged -= UpdateScoreDisplay;
            _gameInstance.OnLevelChanged -= UpdateLevelDisplay;
            _gameInstance.OnPauseStateChanged -= UpdatePauseDisplay;
        }
    }

    private void Update()
    {
        if (_gameInstance != null && _timeText != null)
        {
            UpdateTimeDisplay();
        }
    }

    private void UpdateScoreDisplay(int score)
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {score:N0}";
        }
    }

    private void UpdateLevelDisplay(int level)
    {
        if (_levelText != null)
        {
            _levelText.text = $"Level: {level}";
        }
    }

    private void UpdateTimeDisplay()
    {
        if (_timeText != null)
        {
            float time = _gameInstance.GameTime;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            _timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    private void UpdatePauseDisplay(bool isPaused)
    {
        if (_pauseIndicator != null)
        {
            _pauseIndicator.SetActive(isPaused);
        }
    }

    private void UpdateAllDisplays()
    {
        UpdateScoreDisplay(_gameInstance.Score);
        UpdateLevelDisplay(_gameInstance.Level);
        UpdatePauseDisplay(_gameInstance.IsPaused);
    }
}