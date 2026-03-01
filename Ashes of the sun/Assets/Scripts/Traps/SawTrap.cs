using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : MonoBehaviour
{
    #region Components
    private Animator anim;
    private SpriteRenderer sr;
    #endregion

    #region Inspector Fields
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private Transform[] wayPoint;
    #endregion

    #region Waypoint Data
    private Vector3[] wayPointPosition;
    public int wayPointIndex = 1;
    public int moveDirection = 1;
    #endregion

    #region State
    private bool canMove = true;
    #endregion

    #region Collision Tracking
    private HashSet<GameObject> objectsBeingDamaged = new HashSet<GameObject>(); // tracks players already damaged in this collision
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateWaypointsInfo();
        transform.position = wayPointPosition[0];
    }

    private void Update()
    {
        anim.SetBool("active", canMove);

        if (!canMove) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            wayPointPosition[wayPointIndex],
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, wayPointPosition[wayPointIndex]) < .1f)
        {
            if (wayPointIndex == wayPointPosition.Length - 1 || wayPointIndex == 0)
            {
                moveDirection *= -1;
                StartCoroutine(StopMovement(cooldown));
            }

            wayPointIndex += moveDirection;
        }
    }
    #endregion

    #region Waypoint Logic
    private void UpdateWaypointsInfo()
    {
        List<Transform> wayPointList = new List<Transform>();

        foreach (Transform child in transform)
        {
            wayPointList.Add(child);
        }

        if (wayPointList.Count != wayPoint.Length)
        {
            wayPoint = wayPointList.ToArray();
        }

        wayPointPosition = new Vector3[wayPoint.Length];
        for (int i = 0; i < wayPoint.Length; i++)
        {
            wayPointPosition[i] = wayPoint[i].position;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator StopMovement(float delay)
    {
        canMove = false;
        yield return new WaitForSeconds(delay);
        canMove = true;
        sr.flipX = !sr.flipX;
    }
    #endregion

    #region Collision Damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ApplyDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ApplyDamage(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (objectsBeingDamaged.Contains(collision.gameObject))
            objectsBeingDamaged.Remove(collision.gameObject);
    }

    private void ApplyDamage(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerController != null)
        {
            playerController.knockback(transform.position.x);
        }

        if (playerHealth != null && !objectsBeingDamaged.Contains(collision.gameObject))
        {
            playerHealth.TakeDamage(1);
            objectsBeingDamaged.Add(collision.gameObject);
        }
    }
    #endregion
}