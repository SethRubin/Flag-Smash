using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float playerSpeed;
    public float jumpHeight;
    public int jumpCount;
    public bool controlArrows;
    public GameObject flag, myFlag;
    public SwordController sword;
    public Text scoreText;

    private Rigidbody2D rb;
    private int jumps;
    private KeyCode leftKey, rightKey, upKey, downKey, attack1, attack2;
    private int cooldown1, cooldown2;
    private int damageCooldown;
    private bool haveFlag;
    private GameObject capturedFlag;
    private int score;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumps = 0;
        if (controlArrows)
        {
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            upKey = KeyCode.UpArrow;
            downKey = KeyCode.DownArrow;
            attack1 = KeyCode.Period;
            attack2 = KeyCode.Slash;
        }
        else
        {
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            upKey = KeyCode.W;
            downKey = KeyCode.S;
            attack1 = KeyCode.Q;
            attack2 = KeyCode.E;
        }
        cooldown1 = 0;
        cooldown2 = 0;
        damageCooldown = 0;
        haveFlag = false;
        score = 0;
    }

    void Update()
    {
        // Jump
        if (Input.GetKeyDown(upKey) && jumps < jumpCount && damageCooldown == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            jumps++;
        }

        // Attack
        if (Input.GetKeyDown(downKey) && cooldown1 == 0 && damageCooldown == 0)
        {
            sword.Attack();
            cooldown1 = sword.cooldown;
        }

        // Update score
        scoreText.text = score.ToString();
    }

    void FixedUpdate()
    {
        // Horizontal movement
        if (damageCooldown == 0)
        {
            if (Input.GetKey(leftKey) && !Input.GetKey(rightKey))
            {
                rb.velocity = new Vector2(-playerSpeed, rb.velocity.y);
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (Input.GetKey(rightKey) && !Input.GetKey(leftKey))
            {
                rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        // Update cooldowns
        cooldown1 = Math.Max(cooldown1 - 1, 0);
        cooldown2 = Math.Max(cooldown2 - 1, 0);
        damageCooldown = Math.Max(damageCooldown - 1, 0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Update jumps
        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player")) && collision.contacts[0].normal.y > 0)
        {
            jumps = 0;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Update jumps
        jumps = Math.Max(jumps, 1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Flag"))
        {
            // Capture flag
            if (haveFlag && other.gameObject == myFlag)
            {
                capturedFlag.SetActive(true);
                capturedFlag = null;
                flag.SetActive(false);
                haveFlag = false;
                score++;
            }

            // Grab flag
            else if (other.gameObject != myFlag)
            {
                capturedFlag = other.gameObject;
                capturedFlag.SetActive(false);
                flag.SetActive(true);
                haveFlag = true;
            }
        }
    }

    public bool FacingRight()
    {
        return transform.eulerAngles.y == 0;
    }

    public void Damage(int _damageCooldown)
    {
        damageCooldown = _damageCooldown;
        if (haveFlag)
        {
            haveFlag = false;
            flag.SetActive(false);
            capturedFlag.SetActive(true);
        }
    }
}
