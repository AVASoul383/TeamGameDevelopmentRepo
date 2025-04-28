using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public float attackRange = 2f;
    public int damage = 25;
    public float attackRate = 1f;
    public LayerMask enemyLayer;
    public KeyCode attackKey = KeyCode.M;

    private float nextAttackTime = 0f;
    public Transform attackPoint;

    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetKeyDown(attackKey))
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void Attack()
    {

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<IDamage>()?.takeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
