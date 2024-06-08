using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyFieldOfView : MonoBehaviour
{
    public EnemyController controller;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public LineRenderer lineRenderer;
    public int numRays = 50;

    Vector3 offsetY = Vector3.up * 0.4f;
    Vector3[] pointPositions;

    bool playerInFOV = false;
    private bool coroutineRunning = false;

    private NavMeshAgent agent;
    private float defaultSpeed;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.loop = false;
        lineRenderer.positionCount = numRays * 2;
        lineRenderer.widthMultiplier = 0.5f;

        pointPositions = new Vector3[numRays * 2];

        agent = GetComponent<NavMeshAgent>();
        defaultSpeed = agent.speed;
    }

    void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        float angle = 90.0f - viewAngle / 2.0f;
        float angleStep = viewAngle / ((float)numRays - 1.0f);

        int pointIndex = 0;
        playerInFOV = false;

        for(int i = 0; i < numRays; i++)
        {
            pointPositions[pointIndex] = transform.position + offsetY;
            pointIndex++;


            Vector3 directionVector = transform.right * Mathf.Cos(angle * Mathf.Deg2Rad) + transform.forward * Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 point;

            RaycastHit hit;
            if (Physics.Raycast(transform.position + offsetY, directionVector, out hit, viewRadius, obstacleMask))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    point = transform.position + offsetY + viewRadius * directionVector;
                    if (!playerInFOV)
                    {
                        if (!coroutineRunning)
                        {
                            StartCoroutine(PlayerInFOV(hit.collider));
                        }
                        playerInFOV = true;
                    }
                }
                else
                {
                    point = hit.point;
                }
            }
            else
            {
                point = transform.position + offsetY + viewRadius * directionVector;
            }

            pointPositions[pointIndex] = point;
            pointIndex++;

            angle += angleStep;
        }

        lineRenderer.SetPositions(pointPositions);

        IEnumerator PlayerInFOV(Collider player)
        {
            coroutineRunning = true;
            int i = 0;
            int loopCount = 20;
            PlayerController playerController = player.GetComponent<PlayerController>();

            for (; i < loopCount; i++)
            {
                yield return new WaitForSeconds(0.005f);
                if(!playerInFOV || playerController.level >= controller.level)
                {
                    break;
                }
                agent.speed /= 5.0f;
            }
            if(i == loopCount)
            {
                player.gameObject.SetActive(false);
                GameManager.Instance.UpdateGameState(GameState.levelFailed);
            }
            agent.speed = defaultSpeed;
            coroutineRunning = false;
        }
    }
}
