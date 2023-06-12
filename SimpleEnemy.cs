using System.Collections;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{

    private Rigidbody2D rb;
    private bool isFloor;
    private bool isObs;
    public LayerMask turnLayerMask;
    private Transform fallCheck;
    private Transform wallCheck;
    private bool facingRight = true;
    public float speed = 5f;

    public int life = 2;
    public bool canHitted = true;
    private bool isHitted = false;

    void Awake()
    {
        fallCheck = transform.Find("FallCheck");
        wallCheck = transform.Find("WallCheck");
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

        if (life <= 0)
        {
            transform.GetComponent<Animator>().SetBool("IsDead", true);
            StartCoroutine(EnemyDead());
        }

        isFloor = Physics2D.OverlapCircle(fallCheck.position, .2f, 1 << LayerMask.NameToLayer("Default"));
        isObs = Physics2D.OverlapCircle(wallCheck.position, .2f, turnLayerMask);

        if (!isHitted && life > 0 && Mathf.Abs(rb.velocity.y) < 0.5f)
        {
            if (isFloor && !isObs && !isHitted)
            {
                if (facingRight)
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
            }
            else
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void GetDamage(int damage)
    {
        if (canHitted)
        {
            float direction = damage / Mathf.Abs(damage);
            damage = Mathf.Abs(damage);
            transform.GetComponent<Animator>().SetBool("Hit", true);
            life -= damage;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(direction * 500f, 100f));
            StartCoroutine(DamageCooldown());
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && life > 0)
        {
            collision.gameObject.GetComponent<CharacterController2D>().GetDamage(1, transform.position);
        }
    }

    IEnumerator DamageCooldown()
    {
        isHitted = true;
        canHitted = false;
        yield return new WaitForSeconds(0.1f);
        isHitted = false;
        canHitted = true;
    }

    IEnumerator EnemyDead()
    {
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        capsule.size = new Vector2(1f, 0.25f);
        capsule.offset = new Vector2(0f, -0.8f);
        capsule.direction = CapsuleDirection2D.Horizontal;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
