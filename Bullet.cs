using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 direction;
    public bool hasHit = false;
    public float speed = 15f;
    public GameObject owner;
    public string target;

    void FixedUpdate()
    {
        if (!hasHit)
            GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(target))
        {
            if (target == "Player")
            {
                collision.gameObject.GetComponent<CharacterController2D>().GetDamage(1, transform.position);
                Destroy(gameObject);
            }
            else if (target == "Enemy")
            {
                collision.gameObject.SendMessage("GetDamage", Mathf.Sign(direction.x));
                Destroy(gameObject);
            }

        }
        else if (collision.gameObject != owner)
        {
            Destroy(gameObject);
        }
    }
}
