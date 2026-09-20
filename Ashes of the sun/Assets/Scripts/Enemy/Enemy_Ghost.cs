using System.Collections;
using UnityEngine;

public class Enemy_Ghost : Enemy
{
    [Header("Ghost Behavior")]
    [SerializeField] private float activeDuration = 3f;
    private float activeTimer;

    public float xMinDistance = 8f;
    public float yMinDistance = 6f;
    public float yMaxDistance = 10f;

    private bool isChasing;
    private Transform target;

    [Header("Idle Timing")]
    [SerializeField] private float idleDuration = 1.5f;
    private float idleTimer;

    [Header("Ghost Death")]
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Drops")]
    [SerializeField] private GameObject motherFlamePrefab;

    private bool can_damage = true;
    private float damage_time;
    private bool targeting_player;

    public Enemy enemy;

    private void Start()
    {
        targeting_player = Random.Range(0, 2) == 0;

        if (targeting_player) return;

        xMinDistance = 15f;
        yMinDistance = 1f;
        yMaxDistance = 2f;

        if (enemy != null) enemy.moveSpeed = 1f;
        else moveSpeed = 1f;
    }

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        activeTimer -= Time.deltaTime;
        idleTimer -= Time.deltaTime;

        if (targeting_player && target != null && !target.gameObject.activeInHierarchy)
        {
            target = null;
            isChasing = false;
            idleTimer = 0f;
        }

        if (!isChasing && idleTimer <= 0f) StartChase();
        else if (isChasing && activeTimer <= 0f) EndChase();

        if (target != null && Vector2.Distance(target.position, transform.position) < 1.5f)
        {
            if (targeting_player)
            {
                if (target.TryGetComponent(out PlayerController playerController)) playerController.knockback(transform.position.x);

                if (can_damage && target.TryGetComponent(out PlayerHealth playerHealth))
                {
                    // Player health damage is intentionally disabled in the existing design.
                    can_damage = false;
                }
            }
            else
            {
                if (target.TryGetComponent(out Pandorasbox pandorasbox))
                {
                    pandorasbox.TakeDamage(10f);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        if (!can_damage)
        {
            damage_time += Time.deltaTime;

            if (damage_time > 1f)
            {
                can_damage = true;
                damage_time = 0f;
            }
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!isChasing || !canMove || target == null) return;

        HandleFlip(target.position.x);

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    private void StartChase()
    {
        if (targeting_player)
        {
            PlayerController[] players = FindObjectsByType<PlayerController>();

            if (players.Length == 0)
            {
                idleTimer = idleDuration;
                return;
            }

            target = players[Random.Range(0, players.Length)].transform;
        }
        else
        {
            GameObject pandorasBox = GameObject.FindGameObjectWithTag("PANDORAS BOX");

            if (pandorasBox == null)
            {
                Debug.LogError("[Enemy_Ghost] No active GameObject with tag 'PANDORAS BOX' was found.", this);
                idleTimer = idleDuration;
                return;
            }

            target = pandorasBox.transform;
        }

        float xOffset = Random.value < .5f ? -1f : 1f;
        float yOffset = Random.Range(yMinDistance, yMaxDistance);

        transform.position = target.position + new Vector3(xMinDistance * xOffset, yOffset, 0f);

        activeTimer = activeDuration;
        isChasing = true;

        if (anim != null) anim.SetTrigger("appear");
    }

    private void EndChase()
    {
        if (!targeting_player) return;

        idleTimer = idleDuration;
        isChasing = false;
        target = null;

        if (anim != null) anim.SetTrigger("disappear");
    }

    public override void Die()
    {
        if (isDead) return;

        isDead = true;
        canMove = false;

        EnableColliders(false);

        if (anim != null) anim.SetTrigger("disappear");

        SpawnMotherFlame();
        StartCoroutine(GhostFallDeath());
    }

    private void SpawnMotherFlame()
    {
        if (motherFlamePrefab != null) Instantiate(motherFlamePrefab, transform.position, Quaternion.identity);
    }

    private IEnumerator GhostFallDeath()
    {
        if (sr == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float elapsed = 0f;
        Color startColor = sr.color;
        float safeFadeDuration = Mathf.Max(.01f, fadeDuration);

        while (elapsed < safeFadeDuration)
        {
            elapsed += Time.deltaTime;
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / safeFadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        while (transform.position.y > -10f)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void MakeInvisible()
    {
        if (sr != null) sr.color = Color.clear;
    }

    public void MakeVisible()
    {
        if (sr != null) sr.color = Color.white;
    }
}