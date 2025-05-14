using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private InputField usernameInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Text errorText;
    [SerializeField] private GameObject shopUI;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        errorText.gameObject.SetActive(false);
    }



    private void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Username and password are required");
            return;
        }

        StartCoroutine(APIService.Instance.Login(username, password, (success, message) =>
        {
            if (success)
            {
                SceneManager.LoadScene("StartScene");

            }
            else
            {
                ShowError(message);
            }
        }));
    }

    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }
}