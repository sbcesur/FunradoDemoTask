using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject tapToStart;
    [SerializeField] private GameObject nextLevel;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject paused;
    [SerializeField] private GameObject joystick;

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.onGameStateChanged += UpdateUI;
    }
    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= UpdateUI;
    }

    public void UpdateUI(GameState state)
    {
        print("updating UI");

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        tapToStart.SetActive(state == GameState.tapToStart);
        nextLevel.SetActive(state == GameState.levelPassed);
        gameOver.SetActive(state == GameState.levelFailed);
        joystick.SetActive(state == GameState.playing);
    }

    
}
