using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject openPanel;
    private int starsCount;
    private string CurrentUser => $"Stars_{APIService.Instance.UserId}";

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadStars();
            OpenSecond();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenSecond()
    {
        if(starsCount > 5)
        {
        openPanel.SetActive(false);

        }
    }

    private void LoadStars()
    {
        starsCount = PlayerPrefs.GetInt(CurrentUser, 0); // 0 - значение по умолчанию
    }

}
