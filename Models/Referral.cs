using MovieMania.Models;

public class Referral
{
    public int Id { get; set; }

    public int ReferrerId { get; set; }
    public User Referrer { get; set; }

    public int ReferredUserId { get; set; }
    public User ReferredUser { get; set; }

    public decimal RewardAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}