using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 10.0f;
    public TextMeshPro levelText;
    public RectTransform levelTextTransform;
    private Quaternion levelTextRotation;

    public int level = 1;
    public Transform enemyPath;
    public NavMeshAgent navMeshAgent;
    public enum EnemyType{
        moving,
        rotating
    }
    public EnemyType type;
    private List<NavMeshPath> paths;
    private int currentPathIndex;
    [HideInInspector] public Animator animator;

    public delegate void EnemyDieHandler(EnemyController enemy);
    public static event EnemyDieHandler OnEnemyDie;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        levelTextRotation = levelTextTransform.rotation;

        if (type == EnemyType.moving)
        {
            GetEnemyPath();
        }
        else if(type == EnemyType.rotating)
        {
            //....
        }
    }

    // Update is called once per frame
    void Update()
    {

        levelText.text = "Lv." + level;

        levelTextTransform.rotation = levelTextRotation;

        if (type == EnemyType.moving)
        {
            if (!navMeshAgent.hasPath)
            {
                navMeshAgent.SetPath(paths[currentPathIndex]);
                currentPathIndex = (currentPathIndex + 1) % paths.Count;
            }
        }
        else if( type == EnemyType.rotating)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    private void GetEnemyPath()
    {
        paths = new List<NavMeshPath>();

        for (int i = 0; i < enemyPath.childCount; i++)
        {
            Vector3 source = enemyPath.GetChild(i).position;
            Vector3 target = enemyPath.GetChild((i + 1) % enemyPath.childCount).position;

            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(source, target, NavMesh.AllAreas, path);

            paths.Add(path);
        }

        if (!navMeshAgent.SetPath(paths[0]))
        {
            print("SetPath failed");
        }
        currentPathIndex = 1;
    }

    public void Die()
    {
        if(OnEnemyDie != null)
        {
            OnEnemyDie(this);
        }
    }
}
