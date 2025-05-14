using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed = 2f; // Скорость движения
    [SerializeField] private float topYPosition = 5f; // Верхняя граница
    [SerializeField] private float bottomYPosition = 0f; // Нижняя граница\

    public int dfs;

    private bool movingUp = true; // Направление движения

    private void Update()
    {
        // Движение вверх
        if (movingUp)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            if (transform.position.y >= topYPosition)
                movingUp = false;
        }
        // Движение вниз
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            if (transform.position.y <= bottomYPosition)
                movingUp = true;
        }
    }
}