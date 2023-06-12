using System.Collections;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject player;
    private float PlayerDistX;
    private float PlayerDistY;
    public GameObject bullet;
    public int life = 3;

    public float speed = 5f;
    private bool canHitted = true;
    private bool isHitted = false;

    public float DashForce = 25f;
    private bool isDashing = false;

    private Transform attackCheck;
    public float meleeDist = 1.5f;
    public float rangeDist = 5f;
    private bool canAttack = true;
    public float damage = 1;

    private float Decision = 0;
    private bool doDecision = true;
    private bool endDecision = false;

    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        attackCheck = transform.Find("AttackCheck").transform;
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (life <= 0)
        {
            StartCoroutine(AIEnemyDead());
        }
        if (isDashing)
        {
            rb.velocity = new Vector2(transform.localScale.x * DashForce, 0);
        }
        else if (!isHitted)
        {
            PlayerDistX = player.transform.position.x - transform.position.x;
            PlayerDistY = player.transform.position.y - transform.position.y;

            if (Mathf.Abs(PlayerDistX) < 0.25f)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0f, rb.velocity.y);
                animator.SetBool("IsWaiting", true);
            }
            else if (Mathf.Abs(PlayerDistX) > 0.25f && Mathf.Abs(PlayerDistX) < meleeDist && Mathf.Abs(PlayerDistY) < 2f)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0f, rb.velocity.y);
                if ((PlayerDistX > 0f && transform.localScale.x < 0f) || (PlayerDistX < 0f && transform.localScale.x > 0f))
                {
                    Flip();
                }
                if (canAttack)
                {
                    MeleeAttack();
                }
            }
            else if (Mathf.Abs(PlayerDistX) > meleeDist && Mathf.Abs(PlayerDistX) < rangeDist)
            {
                animator.SetBool("IsWaiting", false);
                rb.velocity = new Vector2(PlayerDistX / Mathf.Abs(PlayerDistX) * speed, rb.velocity.y);
            }
            else
            {
                if (!endDecision)
                {
                    if ((PlayerDistX > 0f && transform.localScale.x < 0f) || (PlayerDistX < 0f && transform.localScale.x > 0f))
                        Flip();

                    if (Decision < 0.4f)
                        Run();
                    else if (Decision >= 0.4f && Decision < 0.6f)
                        Jump();
                    else if (Decision >= 0.6f && Decision < 0.8f)
                        StartCoroutine(Dash());
                    else if (Decision >= 0.8f && Decision < 0.95f)
                        RangeAttack();
                    else
                        Idle();
                }
                else
                {
                    endDecision = false;
                }
            }
        }
        else if (isHitted)
        {
            if ((PlayerDistX > 0f && transform.localScale.x > 0f) || (PlayerDistX < 0f && transform.localScale.x < 0f))
            {
                Flip();
                StartCoroutine(Dash());
            }
            else
            {
                StartCoroutine(Dash());
            }
        }
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void GetDamage(int damage)
    {
        if (canHitted)
        {
            life -= damage;
            float direction = damage / Mathf.Abs(damage);
            animator.SetBool("Hit", true);
            transform.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * 300f, 100f));
            StartCoroutine(DamageCooldown());
        }
    }

    public void MeleeAttack()
    {
        transform.GetComponent<Animator>().SetBool("Attack", true);
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.CompareTag("Enemy") && collidersEnemies[i].gameObject != gameObject)
            {
                if (transform.localScale.x < 1)
                {
                    damage = -damage;
                }
                collidersEnemies[i].gameObject.SendMessage("GetDamage", damage);
            }
            else if (collidersEnemies[i].gameObject.CompareTag("Player"))
            {
                collidersEnemies[i].gameObject.GetComponent<CharacterController2D>().GetDamage(1, transform.position);
            }
        }
        StartCoroutine(WaitToAttack(0.5f));
    }

    public void RangeAttack()
    {
        if (doDecision)
        {
            GameObject _bullet = Instantiate(bullet, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity) as GameObject;
            _bullet.GetComponent<Bullet>().owner = gameObject;
            _bullet.GetComponent<Bullet>().target = "Player";
            Vector2 direction = new(transform.localScale.x, 0f);
            _bullet.GetComponent<Bullet>().direction = direction;
            StartCoroutine(NextDecision(0.5f));
        }
    }

    public void Run()
    {
        animator.SetBool("IsWaiting", false);
        rb.velocity = new Vector2(PlayerDistX / Mathf.Abs(PlayerDistX) * speed, rb.velocity.y);
        if (doDecision)
        {
            StartCoroutine(NextDecision(0.5f));
        }
    }
    public void Jump()
    {
        Vector3 targetVelocity = new Vector2(PlayerDistX / Mathf.Abs(PlayerDistX) * speed, rb.velocity.y);
        Vector3 velocity = Vector3.zero;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.05f);
        if (doDecision)
        {
            animator.SetBool("IsWaiting", false);
            rb.AddForce(new Vector2(0f, 850f));
            StartCoroutine(NextDecision(1f));
        }
    }

    public void Idle()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        if (doDecision)
        {
            animator.SetBool("IsWaiting", true);
            StartCoroutine(NextDecision(1f));
        }
    }

    public void EndDecision()
    {
        Decision = Random.Range(0.0f, 1.0f);
        endDecision = true;
    }

    IEnumerator DamageCooldown()
    {
        canHitted = false;
        isHitted = true;
        yield return new WaitForSeconds(0.1f);
        isHitted = false;
        canHitted = true;
    }

    IEnumerator WaitToAttack(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    IEnumerator Dash()
    {
        animator.SetBool("IsDashing", true);
        isDashing = true;
        yield return new WaitForSeconds(0.1f);
        isDashing = false;
        EndDecision();
    }

    IEnumerator NextDecision(float time)
    {
        doDecision = false;
        yield return new WaitForSeconds(time);
        EndDecision();
        doDecision = true;
        animator.SetBool("IsWaiting", false);
    }

    IEnumerator AIEnemyDead()
    {
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        capsule.size = new Vector2(1f, 0.25f);
        capsule.offset = new Vector2(0f, -0.8f);
        capsule.direction = CapsuleDirection2D.Horizontal;
        transform.GetComponent<Animator>().SetBool("IsDead", true);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
