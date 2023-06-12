using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject bullet;
    public Transform attackCheck;
    public Animator animator;
    public GameObject ShootButton;

    public float damage = 1;
    public bool canAttack = true;
    public bool canShoot = true;
    public bool isTimeToCheck = false;
    private bool AbleToShoot = false;

    public new GameObject camera;
    public void GettingAbleToShoot()
    {
        AbleToShoot = true;
    }
    void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (Input.GetKeyDown(KeyCode.X) && canAttack)
            {
                MeleeAttack();
            }

            if (Input.GetKeyDown(KeyCode.V) && AbleToShoot && canShoot)
            {
                RangeAttack();
            }

        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            if (AbleToShoot)
            {
                ShootButton.SetActive(true);
            }
        }
    }
    public void OnAttackButtonDown()
    {
        if (canAttack)
        {
            MeleeAttack();
        }
    }
    public void OnShootButtonDown()
    {
        if (canShoot)
        {
            RangeAttack();
        }
    }
    private void MeleeAttack()
    {
        canAttack = false;
        animator.SetBool("IsAttacking", true);
        StartCoroutine(MeleeAttackCooldown());
    }
    private void RangeAttack()
    {
        canShoot = false;
        GameObject _bullet = Instantiate(bullet, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity);
        _bullet.GetComponent<Bullet>().owner = gameObject;
        _bullet.GetComponent<Bullet>().target = "Enemy";
        Vector2 direction = new(transform.localScale.x, 0f);
        _bullet.GetComponent<Bullet>().direction = direction;
        _bullet.name = "PlayerBullet";
        StartCoroutine(ShootingCooldown());
    }
    IEnumerator MeleeAttackCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        canAttack = true;
    }
    IEnumerator ShootingCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        canShoot = true;
    }
    public void DoDamage()
    {
        damage = Mathf.Abs(damage);
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.6f);
        foreach (Collider2D collider in collidersEnemies)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                if (collider.transform.position.x - transform.position.x < 0)
                {
                    damage = -damage;
                }
                collider.gameObject.SendMessage("GetDamage", damage);
                camera.GetComponent<CameraFollow>().ShakeCamera();
            }
        }
    }
}
