﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateMachine : MonoBehaviour
{
    private static GameStateMachine _instance;
    private StateMachine _stateMachine;
    public static event Action<IState> OnGameStateChanged;
    public Type CurrentStateType => _stateMachine.CurrentState.GetType();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        _stateMachine = new StateMachine();
        _stateMachine.OnStateChanged += state => OnGameStateChanged?.Invoke(state);
        
        var menu = new Menu();
        var loading = new LoadLevel();
        var play = new Play();
        var pause = new Pause();
        var inventoryScreen = new InventoryPause();
        
        _stateMachine.SetState(menu);
        
        _stateMachine.AddTransition(
            menu, loading, () => PlayButton.LevelToLoad != null);
        
        _stateMachine.AddTransition(
            loading, play, loading.Finished);

        _stateMachine.AddTransition(
            pause, menu, () => RestartButton.Pressed);

        _stateMachine.AddTransitionViceVerca(
            play, pause, () => PlayerInput.Instance.PausePress);

        _stateMachine.AddTransitionViceVerca(
            play, inventoryScreen, () => PlayerInput.Instance.InventoryButtonPress);

    }

    private void Update()
    {
        _stateMachine.UpdateStates();
    }
}

public class Menu : IState
{
    public void Tick()
    {
        
    }

    public void OnEnter()
    {
        PlayButton.LevelToLoad = null;
        SceneManager.LoadSceneAsync("Menu");
    }

    public void OnExit()
    {
        
    }
}
public class Play : IState
{
    public void Tick()
    {
        
    }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }
}

public class LoadLevel : IState
{
    private List<AsyncOperation> _operations = new List<AsyncOperation>();
    public bool Finished() => _operations.TrueForAll(t => t.isDone);

    public void Tick()
    {
        
    }

    public void OnEnter()
    {
        _operations.Add(SceneManager.LoadSceneAsync(PlayButton.LevelToLoad));
        _operations.Add(SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive));
    }

    public void OnExit()
    {
        _operations.Clear();
    }
}
public class Pause : IState
{
    public static bool Active { get; set; }
    
    public void Tick()
    {
        
    }

    public void OnEnter()
    {
        Active = true;
        Time.timeScale = 0;
    }

    public void OnExit()
    {
        Active = false;
        Time.timeScale = 1;
    }
}
public class InventoryPause : IState
{
    public void Tick(){}

    public void OnEnter()
    {
        Pause.Active = true;
        Time.timeScale = 0;
    }

    public void OnExit()
    {
        Pause.Active = false;
        Time.timeScale = 1;
    }
}