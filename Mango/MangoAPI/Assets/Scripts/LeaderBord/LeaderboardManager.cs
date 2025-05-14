using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject userEntryPrefab; // Prefab for displaying user
    [SerializeField] private Transform leaderboardContainer; // Container for the leaderboard

    private void Start()
    {
        LoadLeaderboard();
    }

    private void LoadLeaderboard()
    {
        StartCoroutine(APIService.Instance.GetAllUsersCoins((users, error) =>
        {
            if (users != null)
            {
                // Sort users by coins in descending order
                users.Sort((x, y) => y.coins.CompareTo(x.coins));
                UpdateLeaderboard(users);
            }
            else
            {
                Debug.LogError($"Failed to load users: {error}");
            }
        }));
    }

    private void UpdateLeaderboard(List<User> users)
    {
        // Clear previous entries
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        // Create entries for each user
        foreach (var user in users)
        {
            GameObject entry = Instantiate(userEntryPrefab, leaderboardContainer);
            entry.GetComponent<UserEntry>().SetUserData(user);
        }
    }
}
