using UnityEngine;
using System.Collections;

public class Enemy_Ghost : Enemy
{
    #region Ghost Behavior
    [Header("Ghost Behavior")]
    [SerializeField] private float activeDuration = 3f;
    private float activeTimer;

    [SerializeField] public float xMinDistance = 8f;
    [SerializeField] public float yMinDistance = 6f;
    [SerializeField] public float yMaxDistance = 10f;

    private bool isChasing;
    private Transform target;
    public AudioSource damage;
    #endregion

    #region Idle Timing
    [Header("Idle Timing")]
    [SerializeField] private float idleDuration = 1.5f;
    private float idleTimer;
    #endregion

    #region Ghost Death
    [Header("Ghost Death")]
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float fadeDuration = 1f;
    #endregion

    #region Drops
    [Header("Drops")]
    [SerializeField] private GameObject motherFlamePrefab;
    #endregion

    #region Unity Callbacks

    //ROYS STUFF
    private bool can_damage = true;
    private float damage_time = 0;
    bool targeting_player;
    public Enemy enemy;


    private void Start()
    {
        int target_int = Random.Range(0, 2);
        if (target_int == 0) 
        {
            targeting_player = true;
        }
        else
        {
            targeting_player = false;
            xMinDistance = 15f;
            yMinDistance = 1f;
            yMaxDistance = 2f;
            enemy.moveSpeed = 1;
        }
    }
    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        activeTimer -= Time.deltaTime;
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f && !isChasing)
            StartChase();
        else if (isChasing && activeTimer <= 0f)
            EndChase();

        if(Vector2.Distance(target.position, transform.position) < 1.5f )
        {
            if (targeting_player)
            {
                PlayerController playerController = target.GetComponent<PlayerController>();
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerController != null)
                {
                    playerController.knockback(transform.position.x);
                }
                if (playerHealth != null && can_damage)
                {
                    //playerHealth.TakeDamage(1);
                    can_damage = false;
                    // objectsBeingDamaged.Add(collision.gameObject);
                }
            }
            else
            {
                Pandorasbox pandorasbox = target.GetComponent<Pandorasbox>();
                if (pandorasbox != null) 
                {
                    pandorasbox.take_damage(10);
                    Destroy(gameObject);
                }
            }

        }

        if (can_damage == false) 
        {
            damage_time += Time.deltaTime;
            if(damage_time > 1)
            {
                can_damage = true;
                damage_time = 0;
            }
        }

        HandleMovement();
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        if (target == null || !isChasing || !canMove) return;

        HandleFlip(target.position.x);
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }
    #endregion

    #region Chase Logic
    private void StartChase()
    {
        if (targeting_player)
        {
            var players = GameObject.FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            if (players.Length == 0) return;

            target = players[Random.Range(0, players.Length)].transform;

            float xOffset = Random.value < 0.5f ? -1 : 1;
            float yOffset = Random.Range(yMinDistance, yMaxDistance);

            transform.position = target.position + new Vector3(xMinDistance * xOffset, yOffset, 0f);

            activeTimer = activeDuration;
            isChasing = true;
            anim.SetTrigger("appear");
        }
        else
        {
            target = GameObject.FindGameObjectWithTag("PANDORAS BOX").transform;
            float xOffset = Random.value < 0.5f ? -1 : 1;
            float yOffset = Random.Range(yMinDistance, yMaxDistance);

            transform.position = target.position + new Vector3(xMinDistance * xOffset, yOffset, 0f);

            activeTimer = activeDuration;
            isChasing = true;
            anim.SetTrigger("appear");
        }
        
    }

    private void EndChase()
    {
        if (targeting_player)
        {
            idleTimer = idleDuration;
            isChasing = false;
            anim.SetTrigger("disappear");
        }
        
    }
    #endregion

    #region Death
    public override void Die()
    {
        if (isDead) return;

        isDead = true;
        canMove = false;
        damage.Play();

        EnableColliders(false);
        anim.SetTrigger("disappear");

        // Spawn Mother Flame immediately at death position
        SpawnMotherFlame();

        StartCoroutine(GhostFallDeath());
    }

    private void SpawnMotherFlame()
    {
        if (motherFlamePrefab == null)
            return;

        Instantiate(motherFlamePrefab, transform.position, Quaternion.identity);
    }

    private IEnumerator GhostFallDeath()
    {
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
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
    #endregion

    #region Visibility Helpers
    public void MakeInvisible() => sr.color = Color.clear;
    public void MakeVisible() => sr.color = Color.white;
    #endregion
}