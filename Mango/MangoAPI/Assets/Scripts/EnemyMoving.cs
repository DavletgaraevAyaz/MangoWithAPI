using UnityEngine;

public class EnemyMoving : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;
    [SerializeField] private int jumpsToKill = 3;
    private int currentJumps = 0;

    private bool movingRight = false;

    private void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= rightBound.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= leftBound.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                movingRight = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Проверяем, что игрок прыгнул сверху
            if (collision.relativeVelocity.y < 0 && collision.transform.position.y > transform.position.y)
            {
                currentJumps++;
                if (currentJumps >= jumpsToKill)
                {
                    Destroy(gameObject); // Уничтожаем врага
                }
                else
                {
                    // Можно добавить анимацию или звук удара
                    Debug.Log("Враг получил удар: " + currentJumps + "/" + jumpsToKill);
                }
            }
            else
            {
                PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(1);
                }
            }
        }
    }
}
