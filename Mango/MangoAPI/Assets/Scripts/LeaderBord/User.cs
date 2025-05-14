[System.Serializable]
public class User
{
    public string username;
    public int coins;

    public User(string username, int coins)
    {
        this.username = username;
        this.coins = coins;
    }
}
