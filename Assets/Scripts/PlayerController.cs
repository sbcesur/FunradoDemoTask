using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public Joystick joystick;
    public TextMeshPro levelText;
    public RectTransform levelTextTransform;
    private Quaternion levelTextRotation;

    public objectColor ownedKey = objectColor.none;
    public Animator animator;

    [HideInInspector] public int level = 1;
    private float currentSpeed;
    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        levelText.text = "Lv." + level;
        levelTextTransform = levelText.GetComponent<RectTransform>();
        levelTextRotation = levelTextTransform.rotation;

        ownedKey = objectColor.none;
    }

    private void OnEnable()
    {
        ownedKey = objectColor.none;
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.playing)
        {
            float movementX = joystick.Horizontal;
            float movementZ = joystick.Vertical;

            Vector3 movement = Vector3.forward * movementZ + Vector3.right * movementX;


            Vector3 currentSpeedVector = speed * Time.deltaTime * movement;
            //rigidBody.Move(currentSpeedVector + transform.position, transform.rotation);
            //transform.position += currentSpeedVector;
            transform.Translate(currentSpeedVector, Space.World);

            currentSpeed = currentSpeedVector.sqrMagnitude;

            if (currentSpeedVector != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = rotation;
            }

            levelTextTransform.rotation = levelTextRotation;
            animator.SetFloat("Speed", currentSpeed);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LevelUp"))
        {
            other.gameObject.SetActive(false);          //?????????

            level++;
            levelText.text = "Lv." + level;
        }
        else if(other.CompareTag("Door"))
        {
            objectColor doorColor = other.GetComponent<ColoredObjects>().color;
            if(ownedKey == doorColor)
            {
                //open door
                other.GetComponent<BoxCollider>().enabled = false;
                print("openedDoor");
            }
        }
        else if(other.CompareTag("Key"))
        {
            other.gameObject.SetActive(false);

            ownedKey = other.gameObject.GetComponent<ColoredObjects>().color;

            print(ownedKey);
        }
        else if(other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();

            if(enemy.level < level)
            {
                other.gameObject.SetActive(false);
                enemy.Die();
                if(currentSpeed >= 0.05f)
                {
                    animator.Play("Running Attack", 1);
                }
                else
                {
                    animator.Play("Stationary Attack", 2);
                }

            }
            else if(enemy.level > level)
            {
                //gameObject.SetActive(false);
                enemy.animator.Play("Attack");
            }
        }
    }
}
