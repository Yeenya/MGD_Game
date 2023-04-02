using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ally : MonoBehaviour
{
    private int damage = 40;
    private bool attacking = false;
    private string currentState = "Idle";
    private GameObject spawner;
    private GameObject target;

    private NavMeshAgent agent;

    private MainManager manager;

    private Animator animator;

    public int health = 150;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MainManager>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target == null || target.GetComponent<Enemy>().health <= 0)
        {
            attacking = false;
            GetClosestEnemy();
        }
        if (target != null)
        {
            if (Vector3.Distance(target.transform.position, transform.position) > 3 && !attacking)
            {
                agent.isStopped = false;
                attacking = false;
                agent.SetDestination(target.transform.position);
            }
            else
            {
                agent.isStopped = true;
                attacking = true;
                ChangeState("Attack");
                StartCoroutine(WaitAnimationOverAndDoThings());
            }
        }

        if (!attacking)
        {
            if (agent.velocity.magnitude > 0)
            {
                ChangeState("Run");
            }
            else
            {
                ChangeState("Idle");
            }
        }
    }

    public void SetSpawnerReference(GameObject spawner)
    {
        this.spawner = spawner;
    }

    private void ChangeState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }

    private void GetClosestEnemy()
    {
        float closestDistance = float.MaxValue;
        foreach(GameObject enemy in manager.enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (target == null || (distance < closestDistance && enemy.GetComponent<Enemy>().health > 0))
            {
                target = enemy;
                closestDistance = distance;
            }
        }
    }

    void DealDamage()
    {
        target.GetComponent<Enemy>().GetDamage(damage);
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            ChangeState("GetHit");
        }
    }

    IEnumerator Die()
    {
        health = 0;
        spawner.GetComponent<AllySpawner>().allySpawned = false;
        ChangeState("Die");
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    private IEnumerator WaitAnimationOverAndDoThings()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        attacking = false;
    }

}
