namespace FoodOrderingApp.Auth;

public class JWT
{
  public required string Key { get; set; }
  public required string Issuer { get; set; }
  public required string Audience { get; set; }
  public required double DurationInDays { get; set; }
}
