using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHearts = 3;
    public int currentHearts;

    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private float upwardForce = 5f;  
    [SerializeField] private List<Image> healthUI; 
    [SerializeField] private string scenePath; 
    private Rigidbody2D rb;

    private void Start()
    {
        currentHearts = maxHearts;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        currentHearts -= damage;
        Destroy(healthUI[0]);
        healthUI.RemoveAt(0);
        Debug.Log("Здоровье: " + currentHearts);
        if (currentHearts <= 0)
        {
            Debug.Log("Игрок умер!");
            SceneManager.LoadScene(scenePath);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 bounceDirection = CalculateBounceDirection(collision);

            rb.velocity = Vector2.zero;
            rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }
    }

    private Vector2 CalculateBounceDirection(Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0];

        float playerX = transform.position.x;
        float enemyX = collision.transform.position.x;
        float horizontalDirection = (playerX > enemyX) ? 1f : -1f;

        return new Vector2(horizontalDirection, 1f).normalized;
    }
}