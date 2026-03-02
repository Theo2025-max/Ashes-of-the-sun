using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ghost_spawner : MonoBehaviour
{
    public GameObject Ghost_enemy;
    float time_passed = 0;
    public Transform[] spawn_positions;
    private float wait_time = 10;
    public Transform Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawn_ghost();
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;
        if (time_passed > 120)
        {
            wait_time = 5;
        }
        else if(time_passed > 60)
        {
            wait_time = 7;
        }
    }

    IEnumerator wait_spawn_ghost()
    {
        yield return new WaitForSeconds(wait_time);
        spawn_ghost();
       // GameObject ghost_duplicate = Instantiate(Ghost_enemy, );
    }

    public void spawn_ghost()
    {
        List<Transform> valid_points = new List<Transform>();
        foreach (Transform t in spawn_positions) 
        {
            if(Vector2.Distance(t.position, Player.position) > 10)
            {
                valid_points.Add(t);
            }
        }
        if(valid_points.Count > 0)
        {
            Transform selectedpoint = valid_points[Random.Range(0, valid_points.Count)];
            GameObject ghost_duplicate = Instantiate(Ghost_enemy, selectedpoint.position, Quaternion.identity);
            Enemy_Ghost e_g = ghost_duplicate.GetComponent<Enemy_Ghost>();
            if (time_passed > 120)
            {
                if(e_g != null)
                {
                    print("updated ghst positioning to hard");
                    e_g.xMinDistance = 4;
                    e_g.yMinDistance = 2;
                    e_g.yMaxDistance = 6;

                }
            }
            else if (time_passed > 60)
            {
                if(e_g != null)
                {

                    print("updated ghst positioning to medium");
                    e_g.xMinDistance = 6;
                    e_g.yMinDistance = 4;
                    e_g.yMaxDistance = 8;
                }
                
            }
            StartCoroutine(wait_spawn_ghost());
        }
    }
}
