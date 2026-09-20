using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private Transform[] wayPoint;

    private Vector3[] wayPointPosition;

    public int wayPointIndex = 1;
    public int moveDirection = 1;

    private bool canMove = true;

    private readonly HashSet<GameObject> objectsBeingDamaged = new HashSet<GameObject>();

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (anim == null) Debug.LogError("[SawTrap] Animator component is missing.", this);
        if (sr == null) Debug.LogError("[SawTrap] SpriteRenderer component is missing.", this);
    }

    private void Start()
    {
        UpdateWaypointsInfo();

        if (wayPointPosition == null || wayPointPosition.Length < 2)
        {
            Debug.LogError("[SawTrap] At least two waypoint children are required.", this);
            enabled = false;
            return;
        }

        wayPointIndex = Mathf.Clamp(wayPointIndex, 0, wayPointPosition.Length - 1);
        transform.position = wayPointPosition[0];
    }

    private void Update()
    {
        if (anim != null) anim.SetBool("active", canMove);

        if (!canMove || wayPointPosition == null || wayPointPosition.Length < 2) return;

        transform.position = Vector2.MoveTowards(transform.position, wayPointPosition[wayPointIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPointPosition[wayPointIndex]) >= .1f) return;

        if (wayPointIndex == wayPointPosition.Length - 1 || wayPointIndex == 0)
        {
            moveDirection *= -1;
            StartCoroutine(StopMovement(cooldown));
        }

        wayPointIndex = Mathf.Clamp(wayPointIndex + moveDirection, 0, wayPointPosition.Length - 1);
    }

    private void UpdateWaypointsInfo()
    {
        List<Transform> wayPointList = new List<Transform>();

        foreach (Transform child in transform) wayPointList.Add(child);

        if (wayPoint == null || wayPointList.Count != wayPoint.Length) wayPoint = wayPointList.ToArray();

        wayPointPosition = new Vector3[wayPoint.Length];

        for (int i = 0; i < wayPoint.Length; i++)
        {
            if (wayPoint[i] == null)
            {
                Debug.LogError($"[SawTrap] Waypoint at index {i} is missing.", this);
                wayPointPosition = null;
                return;
            }

            wayPointPosition[i] = wayPoint[i].position;
        }
    }

    private IEnumerator StopMovement(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(Mathf.Max(0f, delay));

        canMove = true;

        if (sr != null) sr.flipX = !sr.flipX;
    }

    private void OnTriggerEnter2D(Collider2D collision) => ApplyDamage(collision);

    private void OnTriggerStay2D(Collider2D collision) => ApplyDamage(collision);

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null) objectsBeingDamaged.Remove(playerHealth.gameObject);
    }

    private void ApplyDamage(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponentInParent<PlayerController>();
        PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();

        if (playerController != null) playerController.knockback(transform.position.x);

        if (playerHealth == null || objectsBeingDamaged.Contains(playerHealth.gameObject)) return;

        playerHealth.TakeDamage(1);
        objectsBeingDamaged.Add(playerHealth.gameObject);
    }
}