using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;

public class metalLogic : MonoBehaviour
{
    public int maxHealth = 3;
    public int health;
    public Collider2D targetCollider;
    public SpriteRenderer spriteRenderer;
    public Sprite[] metalSprites;

    [Header("Metal Sound Effects")]
    [SerializeField] private EventReference metalPing;
    [SerializeField] private EventReference metalBreak;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;

        targetCollider.enabled = false;

        //Sets starting sprite
        spriteRenderer.sprite = metalSprites[0];
    }


    public void TakeDamage(int damage)
    {
        if (health <= 0 || damage <= 0)
        {
            return;
        }

        health -= damage;

        if (health <= 0)
        {
            health = 0;
            targetCollider.enabled = true;

            RuntimeManager.PlayOneShot(metalBreak, transform.position);

            Destroy(gameObject);
            return;
        }

        RuntimeManager.PlayOneShot(metalPing, transform.position);

        UpdateDamageSprite();


    }

    void UpdateDamageSprite()
    {
        if (health <= 0 || maxHealth <= 0 || metalSprites == null || metalSprites.Length == 0)
        {
            return;
        }

        int spriteIndex = (maxHealth - health) * metalSprites.Length / maxHealth;

        spriteIndex = Mathf.Clamp(spriteIndex, 0, metalSprites.Length - 1);

        spriteRenderer.sprite = metalSprites[spriteIndex];

    }
}

