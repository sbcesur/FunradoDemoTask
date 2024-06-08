using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform playerStartingPosition;
    public List<EnemyController> enemies;
    public List<GameObject> rotatingObjects;

    public float powerUpRotationSpeed = 1f;

    private int enemyNo;

    void Start()
    {
        enemyNo = enemies.Count;
    }

    private void OnEnable()
    {
        EnemyController.OnEnemyDie += HandleEnemyDie;
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyDie -= HandleEnemyDie;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < rotatingObjects.Count; i++)
        {
            rotatingObjects[i].transform.Rotate(0, powerUpRotationSpeed * Time.deltaTime, 0);
        }
    }

    private void HandleEnemyDie(EnemyController enemy)
    {
        enemyNo--;
        //play vfx
        if(enemyNo == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.levelPassed);
        }
    }
}
