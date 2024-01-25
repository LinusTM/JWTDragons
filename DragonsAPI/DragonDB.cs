namespace DragonAPI;

public class DragonDB
{
    private static Dictionary<string, Dragon> Dragons = new Dictionary<string, Dragon>();

    public static void Register(Dragon dragon)
    {
        Dragons.Add(dragon.Name, dragon);
    }

    public static Dragon? Fetch(string Name)
    {
        // If dragon does not exist in DB, return null
        Dragons.TryGetValue(Name, out Dragon? dragon);

        return dragon;

    }
}
