using System.Collections.Generic;

namespace DragonAPI;

public class Dragon
{
    public string Name { get; }
    private string Password { get; }

    public Dragon(string name, string password)
    {
        this.Name = name;
        this.Password = password;
    }

    public bool VerifyPassword(string password)
    {
        return this.Password == password;
    }
}
