using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<GameState> onGameStateChanged;
    public GameState? state = null;

    public Transform playerTransform;
    public List<GameObject> Levels = new List<GameObject>();

    private GameObject Level;
    private int currentLevel = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerPrefs.SetInt("Level", currentLevel);
        UpdateGameState(GameState.tapToStart);
        Level = Instantiate(Levels[currentLevel], new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void Update()
    {
        if (state == GameState.tapToStart)
        {
            if(Input.GetMouseButtonDown(0))
            {
                UpdateGameState(GameState.playing);
            }
        }

        print(state.ToString());
    }

    public void UpdateGameState(GameState newState)
    {
        if(newState != state)
        {
            state = newState;

            switch(state)
            {
                case GameState.tapToStart:
                    HandlerTapToStart();
                    break;
                case GameState.playing:
                    HandlerPlaying();
                    break;
                case GameState.paused:
                    HandlerPaused();
                    break;
                case GameState.levelPassed:
                    HandlerLevelPassed();
                    break;
                case GameState.levelFailed:
                    HandlerLevelFailed();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, "invalid game state.");
            }

            onGameStateChanged?.Invoke(newState);
        }
    }

    private void HandlerTapToStart()    //enter state 
    {
        Time.timeScale = 0.0f;
        playerTransform.position = Levels[currentLevel].GetComponent<LevelManager>().playerStartingPosition.position;
    }
    private void HandlerPlaying()
    {
        Time.timeScale = 1.0f;
        playerTransform.position = Levels[currentLevel].GetComponent<LevelManager>().playerStartingPosition.position;
    }
    private void HandlerPaused()
    {

    }
    private void HandlerLevelPassed()
    {
        currentLevel = PlayerPrefs.GetInt("Level");
        currentLevel = (currentLevel + 1) % Levels.Count;
        PlayerPrefs.SetInt("Level", currentLevel);
        Time.timeScale = 0.0f;
    }

    private void HandlerLevelFailed()
    {
        currentLevel = PlayerPrefs.GetInt("Level");

    }

    public void LoadLevel()
    {
        if (Level != null)
        {
            Destroy(Level);
            UpdateGameState(GameState.tapToStart);
            playerTransform.gameObject.SetActive(true);
            Level = Instantiate(Levels[currentLevel], new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}

public enum GameState
{
    tapToStart,
    playing,
    paused,
    levelFailed,
    levelPassed,
}
