using UnityEngine;
using UnityEngine.UI;

public class UserEntry : MonoBehaviour
{
    [SerializeField] private Text usernameText;
    [SerializeField] private Text coinsText;

    public void SetUserData(User user)
    {
        usernameText.text = user.username;
        coinsText.text = user.coins.ToString();
    }
}
