namespace APIArkanoid.Models
{
    public class UserBallSkin
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int BallSkinId { get; set; }
        public BallSkin BallSkin { get; set; }
        public bool IsEquipped { get; set; }
    }
}
