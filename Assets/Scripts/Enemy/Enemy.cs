using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _renderer;

    private float _health = 100f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        _animator.SetTrigger("Hit");

        gameObject.SetActive(_health <= 0);
    }

    public void Flip(bool state)
    {
        _renderer.flipX = state;
    }
}
