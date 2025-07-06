using System;
using UnityEngine;
using Zenject;

public class GameInstance : IInitializable, IDisposable, ITickable
{
    private float _gameTime;
    private int _score;
    private int _level = 1;
    private bool _isPaused;
    
    public float GameTime => _gameTime;
    public int Score => _score;
    public int Level => _level;
    public bool IsPaused => _isPaused;
    
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLevelChanged;
    public event Action<bool> OnPauseStateChanged;
    public event Action OnGameInstanceReset;

    public void Initialize()
    {
        Debug.Log("GameInstance initialized - persists across scenes");
        
        ResetGameData();
    }

    public void Dispose()
    {
        Debug.Log("GameInstance disposed");
    }

    public void Tick()
    {
    }

    public void AddScore(int points)
    {
        _score += points;
        OnScoreChanged?.Invoke(_score);
        
        CheckLevelProgression();
    }

    public void SetScore(int newScore)
    {
        _score = newScore;
        OnScoreChanged?.Invoke(_score);
    }

    public void NextLevel()
    {
        _level++;
        OnLevelChanged?.Invoke(_level);
        Debug.Log($"Advanced to level {_level}");
    }

    public void SetLevel(int newLevel)
    {
        _level = newLevel;
        OnLevelChanged?.Invoke(_level);
    }

    private void CheckLevelProgression()
    {
        int requiredScore = _level * 1000;
        if (_score >= requiredScore)
        {
            NextLevel();
        }
    }

    private void SetPaused(bool paused)
    {
        if (_isPaused != paused)
        {
            _isPaused = paused;
            OnPauseStateChanged?.Invoke(_isPaused);
        }
    }

    public void ResetGameData()
    {
        _gameTime = 0f;
        _score = 0;
        _level = 1;
        _isPaused = false;
        
        OnScoreChanged?.Invoke(_score);
        OnLevelChanged?.Invoke(_level);
        OnPauseStateChanged?.Invoke(_isPaused);
        OnGameInstanceReset?.Invoke();
        
        Debug.Log("Game data reset");
    }

    public GameInstanceData GetSaveData()
    {
        return new GameInstanceData
        {
            gameTime = _gameTime,
            score = _score,
            level = _level
        };
    }

    public void LoadSaveData(GameInstanceData data)
    {
        _gameTime = data.gameTime;
        _score = data.score;
        _level = data.level;
        
        OnScoreChanged?.Invoke(_score);
        OnLevelChanged?.Invoke(_level);
        
        Debug.Log($"Loaded save data: Level {_level}, Score {_score}, Time {_gameTime:F1}s");
    }
}

[System.Serializable]
public struct GameInstanceData
{
    public float gameTime;
    public int score;
    public int level;
}