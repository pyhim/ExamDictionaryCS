namespace ExamDictionary;

public sealed class Menu
{
    private delegate void CurrentMenu();

    private CurrentMenu? _currentMenu;

    public Menu()
    {
        _currentMenu = DictOptionMenu;
    }

    public void Start()
    {
        Console.WriteLine("Welcome to dictionary app!");
        Thread.Sleep(1000);
        
        while (_currentMenu != null)
        {
            Console.Clear();
            _currentMenu();
        }

        Console.Clear();
        Console.WriteLine("Bye!");
    }

    private void DictOptionMenu()
    {
        
    }
}