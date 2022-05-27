namespace QuizApi.Models;

public class User
{
    public int userId { get; set; }

    public string name { get; set; }

    public string email { get; set; }

    public string phone { get; set; }

    public string password { get; set; }
    
    public string IsAdmin { get; set; }
}