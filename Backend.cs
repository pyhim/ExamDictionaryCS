using System.Reflection;

namespace ExamDictionary;

[HomeDirectory("/home/dgalytskyi/Projects/RiderProjects/ExamDictionary")]
public sealed class Backend
{
    private string _homeDirectory;
    
    private string _cacheDirectory;

    private string _exportDirectory;

    public Backend()
    {
        var attributeType = typeof(WordDictionary);
        var homeDirectoryAttribute = 
            attributeType.GetCustomAttribute(typeof(HomeDirectoryAttribute), false) as HomeDirectoryAttribute;
        
        _homeDirectory = homeDirectoryAttribute!.HomeDirectory;
        _cacheDirectory = $"{_homeDirectory}/cache";
        _exportDirectory = $"{_homeDirectory}/export";
    }
    
    public Dictionary<string, WordDictionary> GetWordDictionaries()
    {
        string[] configFiles = Directory.GetFiles(_cacheDirectory, "*.cfg");
        var wordDictionaries = new Dictionary<string, WordDictionary>();
        
        
    }
}

internal sealed class ConfigFile
{
    public static Dictionary<string, string> Deserialize(string configFilePath)
    {
        var file = new StreamReader(new FileStream(configFilePath, FileMode.Open, FileAccess.Read));
        var deserializedConfig = 
            file.ReadToEnd()
                .Split('\n')
                .Select(line => line.Split('='))
                .ToDictionary(tokens => tokens[0], tokens => tokens[1]);
        file.Close();
        
        return deserializedConfig;
    }
}