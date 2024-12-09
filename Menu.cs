namespace ExamDictionary;

public sealed class Menu
{
    private Action? _currentMenu;
    
    private Backend _backend;
    
    private string? _chosenDictionary;

    private void SwitchMenuTo(Action? to) => _currentMenu = to;

    public Menu()
    {
        _currentMenu = DictionaryOptionMenu;
        _backend = new Backend();
        _backend.AcquireWordDictionaries();
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

    private void DictionaryOptionMenu()
    {
        Console.Write("1. Create new dictionary\n2. Choose existing dictionary\n3. Exit\n:");
        int choice = Convert.ToInt32(Console.ReadLine());

        switch (choice)
        {
            case 1:
                SwitchMenuTo(CreateDictionaryMenu);
                return;
            case 2:
                SwitchMenuTo(ChooseDictionaryMenu);
                return;
            case 3:
                SwitchMenuTo(null);
                return;
            default:
                Console.Error.WriteLine("Invalid input!");
                return;
        }
    }

    private void CreateDictionaryMenu()
    {
        Console.Write("Enter the name of the dictionary you want to create (e.g. 'English-Ukrainian')\n:");
        string input = Console.ReadLine() ?? string.Empty;
        
        _backend.CreateWordDictionary(input);
        SwitchMenuTo(DictionaryOptionMenu);
    }

    private void ChooseDictionaryMenu()
    {
        string[] dictTypes = _backend.GetWordDictTypes();
        
        int i = 1;
        foreach (string dictType in dictTypes)
        {
            Console.WriteLine($"{i}. {dictType}");
            i++;
        }
        
        Console.Write("Choose the dictionary number to perform action: "); 
        int index = Convert.ToInt32(Console.ReadLine()) - 1;
        _chosenDictionary = dictTypes[index];

        SwitchMenuTo(PerformOperationOnDictMenu);
    }

    private void PerformOperationOnDictMenu()
    {
        Console.Write("1. Add a new word-translation\n2. Add translation to existing keyword\n" +
                          "3. Change existing keyword\n4. Change existing translation\n" +
                          "5. Delete existing pair\n6. Delete existing translation\n" +
                          "7. Export a pair to JSON file\n8. Search for translation\n9. Back\n:");
        
        var input = Convert.ToInt32(Console.ReadLine());
        
        switch (input)
        {
            case 1:
                SwitchMenuTo(AddPairMenu);
                return;
            case 2:
                SwitchMenuTo(AddTranslationMenu);
                return;
            case 3:
                SwitchMenuTo(ChangeKeywordMenu);
                return;
            case 4:
                SwitchMenuTo(ChangeTranslationMenu);
                return;
            case 5:
                SwitchMenuTo(DeletePairMenu);
                return;
            case 6:
                SwitchMenuTo(DeleteTranslationMenu);
                return;
            case 7:
                SwitchMenuTo(ExportMenu);
                return;
            case 8:
                SwitchMenuTo(SearchMenu);
                return;
            case 9:
                SwitchMenuTo(DictionaryOptionMenu);
                return;
            default:
                Console.Error.WriteLine("Invalid input!");
                return;
        }
    }

    private void AddPairMenu()
    {
        Console.Write("Enter the keyword: ");
        string inputKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        Console.Write("Enter translations dividing by space: ");
        var inputTranslation = 
            (Console.ReadLine() ?? throw new ArgumentNullException()).Split(' ').ToList();
                
        _backend.AddPairToDictionary(_chosenDictionary!, inputKeyword, inputTranslation);
        SwitchMenuTo(PerformOperationOnDictMenu);
    }

    private void AddTranslationMenu()
    {
        Console.Write("Enter the keyword: ");
        string inputKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        Console.Write("Enter translation: ");
        string inputTranslation = Console.ReadLine() ?? throw new ArgumentNullException();

        _backend.AddTranslationToDictionary(_chosenDictionary!, inputKeyword, inputTranslation);
        SwitchMenuTo(PerformOperationOnDictMenu);
    }

    private void ChangeKeywordMenu()
    {
        Console.Write("Enter the keyword: ");
        string inputOldKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        Console.Write("Enter a new keyword: ");
        string inputNewKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        
        _backend.ChangeKeyInDictionary(_chosenDictionary!, inputOldKeyword, inputNewKeyword);
        SwitchMenuTo(PerformOperationOnDictMenu);
    }

    private void ChangeTranslationMenu()
    {
        Console.Write("Enter the keyword: ");
        string inputKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        var translations = _backend.GetTranslations(_chosenDictionary!, inputKeyword);
        
        int i = 1;
        foreach (string translation in translations)
        {
            Console.WriteLine($"{i}. {translation}");
            i++;
        }
        
        Console.Write("Choose translation to change: ");
        int inputTranslationIndex = Convert.ToInt32(Console.ReadLine()) - 1;
        Console.Write("Enter the new translation: ");
        string newTranslation = Console.ReadLine() ?? throw new ArgumentNullException();
        
        _backend.ChangeTranslationInDictionary(
            _chosenDictionary!, inputKeyword, translations[inputTranslationIndex], newTranslation
            );
        SwitchMenuTo(PerformOperationOnDictMenu);
    }

    private void DeletePairMenu()
    {
        Console.Write("Enter the keyword: ");
        string inputKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        
        _backend.DeletePairInDictionary(_chosenDictionary!, inputKeyword);
        SwitchMenuTo(PerformOperationOnDictMenu);
    }

    private void DeleteTranslationMenu()
    {
        Console.Write("Enter the keyword: ");
        string inputKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        var translations = _backend.GetTranslations(_chosenDictionary!, inputKeyword);
        
        int i = 1;
        foreach (string translation in translations)
        {
            Console.WriteLine($"{i}. {translation}");
            i++;
        }
        
        Console.Write("Choose translation to delete: ");
        int toDeleteIndex = Convert.ToInt32(Console.ReadLine()) - 1;
        
        _backend.DeleteTranslationInDictionary(_chosenDictionary!, inputKeyword, translations[toDeleteIndex]);
        SwitchMenuTo(PerformOperationOnDictMenu);
    }

    private void ExportMenu()
    {
        Console.Write("Enter the keyword: ");
        string inputKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        Console.Write("Enter the name of export file (without extension): ");
        string exportFileName = Console.ReadLine() ?? throw new ArgumentNullException();
        
        _backend.ExportPairFromDictionary(_chosenDictionary!, inputKeyword, exportFileName);
        SwitchMenuTo(PerformOperationOnDictMenu);
    }

    private void SearchMenu()
    {
        Console.Write("Enter the keyword: ");
        string inputKeyword = Console.ReadLine() ?? throw new ArgumentNullException();
        var translations = _backend.SearchForTranslationInDictionary(_chosenDictionary!, inputKeyword);

        if (translations == null)
        {
            SwitchMenuTo(PerformOperationOnDictMenu);
            return;
        }
        
        int i = 1;
        Console.WriteLine("Here are your translations:");
        foreach (string translation in translations)
        {
            Console.WriteLine($"{i}. {translation}");
            i++;
        }
        Thread.Sleep(5000);
        
        SwitchMenuTo(PerformOperationOnDictMenu);
    }
}