using System;
using UnityEngine;
using Zenject;

public class GameState : IInitializable, IDisposable
{
    public enum State
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }

    private State _currentState = State.Menu;
    
    public State CurrentState 
    { 
        get => _currentState; 
        private set
        {
            if (_currentState != value)
            {
                var previousState = _currentState;
                _currentState = value;
                OnStateChanged?.Invoke(previousState, value);
            }
        }
    }

    public event Action<State, State> OnStateChanged;

    public void Initialize()
    {
        Debug.Log("GameState initialized");
        SetState(State.Menu);
    }

    public void Dispose()
    {
        Debug.Log("GameState disposed");
    }

    public void SetState(State newState)
    {
        CurrentState = newState;
        Debug.Log($"Game state changed to: {newState}");
    }

    public bool IsInState(State state)
    {
        return CurrentState == state;
    }

    public void StartGame()
    {
        SetState(State.Playing);
    }

    public void PauseGame()
    {
        if (CurrentState == State.Playing)
        {
            SetState(State.Paused);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == State.Paused)
        {
            SetState(State.Playing);
        }
    }

    public void EndGame()
    {
        SetState(State.GameOver);
    }

    public void ReturnToMenu()
    {
        SetState(State.Menu);
    }
}
