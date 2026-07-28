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
        if (collision.gameObject.CompareTag(EnemiesTag))
        {
            var body = collision.gameObject.GetComponent<Rigidbody2D>();
            if (body)
            {
                body.AddForce(playerTransform.right * 5, ForceMode2D.Impulse);
            }
        }
    }
}
