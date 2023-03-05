namespace Riateu;

public class Program 
{
    static void Main() 
    {
        var game = new SimpleGame.SimpleGame(1024, 768, "Simple Game");
        game.Run();
    }
}