using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private MainManager manager;
    private string currentState;
    private bool win;
    private bool attacking;

    [Tooltip("Currently nearest target of the enemy, building or ally")]
    public GameObject nearestTarget;

    public int health;
    public int damage;

    private AudioSource audioSource;
    [Tooltip("Audio clips used for enemy sounds")]
    public AudioClip[] clips;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MainManager>();
        currentState = "Idle";
        win = false;
        attacking = false;
        health = 100;
        damage = 20;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(MakeNoise());
    }

    void Update()
    {
        if (win || health <= 0) return;
        GetNearestTarget(); // Always checks for the nearest target to react appropriately
        if (nearestTarget == null) return; // Every building destroyed and every ally killed
        float distance = Vector3.Distance(transform.position, nearestTarget.transform.position);
        if (agent.isStopped)
        {
            if (distance > 3 && !attacking) //Stopped and too far away
            {
                agent.SetDestination(nearestTarget.transform.position);
            }
            else // Stopped and close enough
            {
                ChangeState("Attack01");
                attacking = true;
                StartCoroutine(WaitAnimationOverAndDoThings());
            }
        }
        else
        {
            if (distance < 3) // Navigating and close enough
            {
                agent.isStopped = true;
                ChangeState("Attack01");
                attacking = true;
                StartCoroutine(WaitAnimationOverAndDoThings());
            }
            else // Navigating and still too far away
            {
                agent.SetDestination(nearestTarget.transform.position);
            }
        }
        if (!agent.isStopped)
        {
            if (agent.velocity.magnitude > 0) ChangeState("Walk");
            else ChangeState("Idle");
        }
    }

    private void GetNearestTarget()
    {
        float nearestDistance = float.MaxValue;

        // Loop through all living allies
        foreach (GameObject ally in manager.allies.Where(x => x.GetComponent<Ally>().health > 0))
        {
            float distance = Vector3.Distance(transform.position, ally.transform.position);
            if (distance <= nearestDistance) // <= so that it prefers allies rather than buildings
            {
                if (nearestTarget != null && nearestTarget.GetComponent<HitPoint>() != null)
                {
                    nearestTarget.GetComponent<HitPoint>().aimed = false; //If the previous target was a hit point on building, release it for other enemies
                }
                nearestDistance = distance;
                nearestTarget = ally;
            }
        }

        // Loop through all standing buildings
        foreach (GameObject building in manager.buildings.Where(x => x.GetComponent<Building>().health > 0))
        {
            foreach (Transform hitPoint in building.transform)
            {
                if (hitPoint.GetComponent<HitPoint>().aimed) continue;

                float distance = Vector3.Distance(transform.position, hitPoint.position);
                if (distance < nearestDistance)
                {
                    if (nearestTarget != null && nearestTarget.GetComponent<HitPoint>() != null)
                    {
                        nearestTarget.GetComponent<HitPoint>().aimed = false; //If the previous target was a hit point on building, release it for other enemies
                    }
                    nearestDistance = distance;
                    nearestTarget = hitPoint.gameObject;
                    nearestTarget.GetComponent<HitPoint>().aimed = true;
                }
            }
        }
    }

    private void ChangeState(string newState) // Function used instead of animation transtitions
    {
        if (currentState == newState) return;
        //print(gameObject.name + " " + newState);
        animator.Play(newState);
        currentState = newState;
    }

    public void GetDamage(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            ChangeState("Die");
            audioSource.Stop();
            audioSource.clip = clips[3];
            audioSource.Play();
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().UpdateCash(30);
            StartCoroutine(Die());
            agent.isStopped = true;
        }
        else
        {
            ChangeState("GetHit");
            audioSource.Stop();
            audioSource.clip = clips[2];
            audioSource.Play();
        }
    }

    void DealDamage() // Called from attack animation
    {
        // Need to make difference between a building and an ally
        if (nearestTarget.tag == "HitPoint")
        {
            nearestTarget.GetComponentInParent<Building>().GetDamage(damage);
            if (nearestTarget.GetComponentInParent<Building>().health <= 0)
            {
                nearestTarget = null;
                if (manager.GetComponent<MainManager>().buildings.Count == 0 && manager.GetComponent<MainManager>().allies.Count == 0)
                {
                    agent.SetDestination(transform.position);
                    win = true;
                    agent.isStopped = true;
                }
                ChangeState("Victory");
            }
        }
        else if (nearestTarget.tag == "Ally")
        {
            nearestTarget.GetComponent<Ally>().GetDamage(damage);
            if (nearestTarget.GetComponent<Ally>().health <= 0)
            {
                nearestTarget = null;
                if (manager.GetComponent<MainManager>().buildings.Count == 0 && manager.GetComponent<MainManager>().allies.Count == 0)
                {
                    agent.SetDestination(transform.position);
                    win = true;
                    agent.isStopped = true;
                }
                ChangeState("Victory");
            }
        }
    }

    // Called from animation
    void AfterVictory()
    {
        if (manager.GetComponent<MainManager>().buildings.Count > 0) agent.isStopped = false;
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }

    IEnumerator MakeNoise()
    {
        audioSource.clip = clips[Random.Range(0, 2)]; // First two clips are random noises
        audioSource.Play();
        yield return new WaitForSeconds(Random.Range(4, 8));
        StartCoroutine(MakeNoise());
    }

    private IEnumerator WaitAnimationOverAndDoThings() // Same as in Ally script
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        attacking = false;
    }
}
