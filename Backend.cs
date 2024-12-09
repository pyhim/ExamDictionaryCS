using System.Reflection;

namespace ExamDictionary;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
internal sealed class HomeDirectoryAttribute : Attribute
{
    public string HomeDirectory { get; init; }
    
    public HomeDirectoryAttribute(string homeDirectory) => HomeDirectory = homeDirectory;
}

[HomeDirectory("/home/dgalytskyi/Projects/RiderProjects/ExamDictionary")]
public sealed class Backend
{
    private readonly string _homeDirectory;
    
    private readonly string _cacheDirectory;
    
    private readonly Dictionary<string, WordDictionary> _dictionaries;

    public List<string> GetTranslations(string dictTypeName, string key) => _dictionaries[dictTypeName][key];

    public Backend()
    {
        var attributeType = typeof(Backend);
        var homeDirectoryAttribute = 
            attributeType.GetCustomAttribute(typeof(HomeDirectoryAttribute), false) as HomeDirectoryAttribute;
        
        _homeDirectory = homeDirectoryAttribute!.HomeDirectory;
        _cacheDirectory = $"{_homeDirectory}/cache";
        _dictionaries = new Dictionary<string, WordDictionary>();
    }
    
    public void AcquireWordDictionaries()
    {
        string[] configFiles = Directory.GetFiles(_cacheDirectory, "*.cfg");

        foreach (string configFile in configFiles)
        {
            var deserializedConfig = ConfigFile.Deserialize(configFile);
            string wordDictType = deserializedConfig["type"];
            var wordDict = new WordDictionary(configFile, _homeDirectory);
            _dictionaries.Add(wordDictType, wordDict);
        }
    }

    public string[] GetWordDictTypes()
    {
        return _dictionaries.Keys.ToArray();
    }

    public void CreateWordDictionary(string type)
    {
        string[] typeTokens = type.Split('-');
        
        _dictionaries.Add(type, new WordDictionary(typeTokens[0], typeTokens[1], _homeDirectory));
    }

    public void AddPairToDictionary(string dictTypeName, string key, List<string> translations)
    {
        HandleKeyNotFoundException(() => _dictionaries[dictTypeName].AddNewKeyWord(key, translations));
    }

    public void AddTranslationToDictionary(string dictTypeName, string key, string translation)
    {
        HandleKeyNotFoundException(() => _dictionaries[dictTypeName].AddWord(key, translation));
    }

    public void ChangeKeyInDictionary(string dictTypeName, string oldKey, string newKey)
    {
        HandleKeyNotFoundException(() => _dictionaries[dictTypeName].ChangeKey(oldKey, newKey));
    }

    public void ChangeTranslationInDictionary(string dictTypeName, string key, string oldWord, string newWord)
    {
        HandleKeyNotFoundException(() => _dictionaries[dictTypeName].ChangeTranslation(key, oldWord, newWord));
    }

    public void DeletePairInDictionary(string dictTypeName, string key)
    {
        HandleKeyNotFoundException(() => _dictionaries[dictTypeName].DeletePair(key));
    }

    public void DeleteTranslationInDictionary(string dictTypeName, string key, string toDelete)
    {
        HandleKeyNotFoundException(() => _dictionaries[dictTypeName].DeleteTranslation(key, toDelete));
    }

    public void ExportPairFromDictionary(string dictTypeName, string key, string nameOfFile)
    {
        HandleKeyNotFoundException(() => _dictionaries[dictTypeName].ExportPairToJson(key, nameOfFile));
    }

    public List<string>? SearchForTranslationInDictionary(string dictTypeName, string key)
    {
        try
        {
            return _dictionaries[dictTypeName][key];
        }
        catch (KeyNotFoundException)
        {
            Console.Beep();
            Console.Error.WriteLine("Nothing was found in dictionary");
            return null;
        }
    }

    private void HandleKeyNotFoundException(Action action)
    {
        try
        {
            action();
        }
        catch (KeyNotFoundException)
        {
            Console.Beep();
            Console.Error.WriteLine("Key not found!");
            throw;
        }
    }
}

internal static class ConfigFile
{
    public static Dictionary<string, string> Deserialize(string configFilePath)
    {
        var file = new StreamReader(new FileStream(configFilePath, FileMode.Open, FileAccess.Read));
        var deserializedConfig = 
            file.ReadToEnd()
                .Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split('='))
                .ToDictionary(tokens => tokens[0], tokens => tokens[1]);
        file.Close();
        
        return deserializedConfig;
    }
}