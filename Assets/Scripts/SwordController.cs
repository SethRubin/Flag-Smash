using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {

    public int startAngle;
    public int duration;
    public int cooldown;
    public int damageCooldown;
    public float speed;
    public PlayerController player;
    public Vector2 knockback;

    private Rigidbody2D rb;
    private float remaining;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
        remaining = 0;
    }

    public void Attack()
    {
        if (remaining == 0)
        {
            gameObject.SetActive(true);
            remaining = duration;
            transform.eulerAngles = new Vector3(0, 0, player.FacingRight() ? -startAngle : startAngle);
        }
    }

    void FixedUpdate()
    {
        if (remaining != 0)
        {
            transform.RotateAround(transform.position, Vector3.forward, player.FacingRight() ? -speed : speed); // change to base of sword
            remaining--;
            if (remaining == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Knock player back
        if (other.gameObject.CompareTag("Player") && other.gameObject != gameObject.GetComponentInParent<PlayerController>().gameObject)
        {
            other.gameObject.GetComponentInParent<PlayerController>().Damage(damageCooldown);
            Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
            otherRb.velocity = new Vector2(0, 0);
            otherRb.AddForce(new Vector2(player.FacingRight() ? knockback.x : -knockback.x, knockback.y), ForceMode2D.Impulse);
        }
    }
}
