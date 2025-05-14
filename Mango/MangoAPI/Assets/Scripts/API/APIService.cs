using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIService : MonoBehaviour
{
    public int? UserId => GetUserIdFromToken();
    public static APIService Instance { get; private set; }

    private string _authToken;
    private string _baseUrl = "http://localhost:5185/api/";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadToken();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAuthToken(string token)
    {
        _authToken = token;
        PlayerPrefs.SetString("AuthToken", token); // Сохраняем на устройстве
        PlayerPrefs.Save();
    }

    private void LoadToken()
    {
        _authToken = PlayerPrefs.GetString("AuthToken", "");
    }

    public IEnumerator Login(string username, string password, Action<bool, string> callback)
    {
        string jsonData = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(_baseUrl + "Auth/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Server response: {request.downloadHandler.text}");
                var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                _authToken = response.token;
                callback(true, "Login successful");
                SetAuthToken(response.token);
                Debug.Log($"{_authToken}");
                Debug.Log($"{response.token}");
            }
            else
            {
                callback(false, request.error);
            }
        }
    }

    public IEnumerator GetUserCoins(Action<int, string> callback)
    {
        // Фиксированный ID пользователя
        var userId = GetUserIdFromToken();

        // Формируем URL с ID пользователя
        string url = $"{_baseUrl}User/coins/{userId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Добавляем заголовок Authorization, если требуется
            if (!string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
            }

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Raw server response: {request.downloadHandler.text}");

                try
                {
                    Debug.Log(request.downloadHandler.text + "123");
                    var jsonResponse = JsonUtility.FromJson<CoinsResponse>(request.downloadHandler.text);
                    Debug.Log(jsonResponse.сoins);
                    callback(jsonResponse.сoins, null);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing coins response: {ex.Message}");
                    callback(0, "Ошибка обработки данных.");
                }
            }
            else
            {
                Debug.LogError($"Request error: {request.error}");
                callback(0, request.error);
            }
        }
    }

    // Класс для десериализации payload токена
    [Serializable]
    private class JwtPayload
    {
        public string nameid;
        public string nameidentifier; // Убедитесь, что это имя соответствует claim'у в токене
    }

    // Метод для декодирования Base64
    private string DecodeBase64(string base64)
    {
        try
        {
            // Добавляем недостающие символы '=' для корректного декодирования
            int padding = base64.Length % 4;
            if (padding != 0)
            {
                base64 += new string('=', 4 - padding);
            }

            byte[] data = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error decoding Base64: {ex.Message}");
            throw;
        }
    }

    public IEnumerator AddCoins(Action<int, string> callback)
    {
        var userId = GetUserIdFromToken();
        // Создаем объект с amount = 1 (добавляем ровно одну монету)
        var coinsData = new AddCoinsRequest { amount = 1 };
        string jsonData = JsonUtility.ToJson(coinsData);

        // Формируем URL с ID пользователя (новый эндпоинт /user/add-coins/{id})
        string url = $"{_baseUrl}user/add-coins/{userId}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<CoinsResponse>(request.downloadHandler.text);
                    callback(response.сoins, null);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing response: {ex.Message}");
                    callback(0, "Error parsing server response");
                }
            }
            else
            {
                Debug.LogError($"Request error: {request.error}");
                callback(0, request.error);
            }
        }
    }

    private int? GetUserIdFromToken()
    {
        if (string.IsNullOrEmpty(_authToken))
            return null;

        try
        {
            // JWT токен состоит из 3 частей, разделенных точками
            var tokenParts = _authToken.Split('.');
            if (tokenParts.Length < 2)
                return null;

            // Декодируем payload (вторую часть)
            var payload = DecodeBase64(tokenParts[1]);
            var payloadJson = JsonUtility.FromJson<JwtPayload>(payload);

            if (int.TryParse(payloadJson.nameid, out int userId))
            {
                return userId;
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing token: {ex.Message}");
            return null;
        }
    }

    public IEnumerator GetAllUsersCoins(Action<List<User>, string> callback)
    {
        string url = $"{_baseUrl}user/all-coins"; // Adjust the endpoint as necessary

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            if (!string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
            }

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var jsonResponse = JsonUtility.FromJson<UsersCoinsResponse>(request.downloadHandler.text);
                    callback(jsonResponse.users, null);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing users response: {ex.Message}");
                    callback(null, "Error parsing response.");
                }
            }
            else
            {
                Debug.LogError($"Request error: {request.error}");
                callback(null, request.error);
            }
        }
    }

    [Serializable]
    public class UsersCoinsResponse
    {
        public List<User> users;
    }

    //[Serializable]
    private class LoginResponse
    {
        public string token;
    }

    [Serializable]
    public class CoinsResponse
    {
        public int сoins;
    }

    [Serializable]
    public class AddCoinsRequest
    {
        public int amount;
    }
}
