using UnityEngine;

public class Sword : MonoBehaviour
{
    private const string EnemiesTag = "Enemy";

    [SerializeField] private float damageAmount = 2.0f;
    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = transform.parent.parent.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var go = collision.gameObject;

        if (go.CompareTag(EnemiesTag))
        {
            var dir = (transform.position - playerTransform.position).normalized;
            var body = go.GetComponent<Rigidbody2D>();

            if (body)
            {
                body.AddForce(dir * 10, ForceMode2D.Impulse);
            }

            var enemy = go.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.Flip((dir.x > 0) ? true : false);
                enemy.TakeDamage(damageAmount);
            }
        }
    }
}
