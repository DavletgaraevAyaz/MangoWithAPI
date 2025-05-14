using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdate : MonoBehaviour
{
    [SerializeField] private Text starText;

    private int coinCount;

    private void Start()
    {
        LoadUserData();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            coinCount++;
            starText.text = coinCount.ToString();
            Destroy(collision.gameObject);
            StartCoroutine(APIService.Instance.AddCoins((coins, error) => {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Ошибка загрузки данных: {error}");
                    starText.text = "Ошибка загрузки";
                    return;
                }
            }));
        }
    }

    public void LoadUserData()
    {
        StartCoroutine(APIService.Instance.GetUserCoins((coins, error) =>
        {
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Ошибка загрузки данных: {error}");
                starText.text = "Ошибка загрузки";
                return;
            }

            coinCount = coins;
            starText.text = coinCount.ToString();
        }));
    }
}
