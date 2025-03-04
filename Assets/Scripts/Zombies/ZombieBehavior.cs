using UnityEngine;
using System.Collections;

public class ZombieBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform target;  
    [Header("Attributes")]
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackDamage = 10f;
    [SerializeField] float attackLength = 0.3f;
    [SerializeField] float attackDelay = 1f;
    [SerializeField] float movementSpeed = 1f;

    bool isAttacking = false;
    float distance = 0f;
    BoxCollider2D attackCollider;

    void Start()
    {
        if (target == null)
        {
            GameObject towerObj = GameObject.FindWithTag("Tower");
            if (towerObj != null)
                target = towerObj.transform;
        }
        
        attackCollider = GetComponent<BoxCollider2D>();
        attackCollider.enabled = false;
    }


    void FixedUpdate()
    {
        if (isAttacking) return;

        if (targetInRange())
            Attack();
        else
            Move();
    }
    
    bool targetInRange()
    {
        if (target == null) return false;
        distance = Vector3.Distance(transform.position, target.position);
        return distance <= attackRange;
    }

    void Attack()
    {
        Debug.Log("Zombie attacked the tower");
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        attackCollider.enabled = true;
        yield return new WaitForSeconds(attackLength);
        attackCollider.enabled = false;
        yield return new WaitForSeconds(attackDelay);

        isAttacking = false;
    }

    void Move()
    {
        Debug.Log("Zombie is moving");
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            movementSpeed * Time.fixedDeltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tower"))
        {
            TowerHealth towerHealth = other.GetComponent<TowerHealth>();
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(attackDamage);
            }
        }
    }
}