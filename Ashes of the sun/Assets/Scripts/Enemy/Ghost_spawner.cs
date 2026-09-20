using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost_spawner : MonoBehaviour
{
    public GameObject Ghost_enemy;

    private float time_passed;

    public Transform[] spawn_positions;

    private float wait_time = 10f;

    public Transform Player;

    private Coroutine spawnRoutine;

    private void Start()
    {
        if (!HasRequiredReferences()) return;

        spawn_ghost();
    }

    private void Update()
    {
        time_passed += Time.deltaTime;

        if (time_passed > 120f) wait_time = 5f;
        else if (time_passed > 60f) wait_time = 7f;
    }

    private IEnumerator wait_spawn_ghost()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, wait_time));

        spawnRoutine = null;

        spawn_ghost();
    }

    public void spawn_ghost()
    {
        if (!HasRequiredReferences()) return;

        List<Transform> valid_points = new List<Transform>();

        foreach (Transform spawnPoint in spawn_positions)
        {
            if (spawnPoint != null &&
                Vector2.Distance(spawnPoint.position, Player.position) > 10f)
            {
                valid_points.Add(spawnPoint);
            }
        }

        if (valid_points.Count > 0)
        {
            Transform selectedpoint =
                valid_points[Random.Range(0, valid_points.Count)];

            GameObject ghost_duplicate =
                Instantiate(Ghost_enemy, selectedpoint.position, Quaternion.identity);

            Enemy_Ghost e_g =
                ghost_duplicate.GetComponent<Enemy_Ghost>();

            if (e_g != null)
            {
                if (time_passed > 120f)
                {
                    e_g.xMinDistance = 4f;
                    e_g.yMinDistance = 2f;
                    e_g.yMaxDistance = 6f;
                }
                else if (time_passed > 60f)
                {
                    e_g.xMinDistance = 6f;
                    e_g.yMinDistance = 4f;
                    e_g.yMaxDistance = 8f;
                }
            }
        }

        ScheduleNextSpawn();
    }

    private void ScheduleNextSpawn()
    {
        if (!isActiveAndEnabled || spawnRoutine != null) return;

        spawnRoutine = StartCoroutine(wait_spawn_ghost());
    }

    private bool HasRequiredReferences()
    {
        if (Ghost_enemy == null)
        {
            Debug.LogError("[Ghost_spawner] Ghost_enemy prefab is missing.", this);
            return false;
        }

        if (Player == null)
        {
            Debug.LogError("[Ghost_spawner] Player Transform reference is missing.", this);
            return false;
        }

        if (spawn_positions == null || spawn_positions.Length == 0)
        {
            Debug.LogError("[Ghost_spawner] No spawn positions are configured.", this);
            return false;
        }

        return true;
    }
}