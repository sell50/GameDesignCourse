using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public Animator anim;
    public Transform attackPoint;
    public float attackRange = 1.8f;
    public LayerMask layer;
    public float speed;
    public PlayerMovement player;
    public EnemyStat enemy;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerMovement>();
    }

    void OnTriggerStay(Collider other)
    {
        if(other.name == "Enemy")
        {
            
            player.navMeshAgent.SetDestination(other.transform.position);
            anim.SetBool("isRunning", true);
            if (Vector3.Distance(player.navMeshAgent.transform.position, other.transform.position) <= 10.0f) { Attack(); }
        }
    }

    void Attack()
    {
        anim.SetBool("Attack", true);
        enemy = GameObject.Find("Enemy").GetComponent<EnemyStat>() ;
        enemy.takeDMG();
        if (enemy.HP <= 0) { anim.SetBool("Attack", false); anim.SetBool("isRunning", false); }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
