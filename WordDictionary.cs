using System.Collections;
using System.Text.Json;
// ReSharper disable All

namespace ExamDictionary;

public class DictType
{
    public string From { get; init; }
    public string To { get; init; }

    public DictType(string from, string to)
    {
        From = from;
        To = to;
    }
}

public sealed class WordDictionary : IEnumerable<KeyValuePair<string, List<string>>>
{
    private static JsonSerializerOptions _serializationOptions = new JsonSerializerOptions{ WriteIndented = true };
    
    private readonly string _homeDirectory;
    
    private readonly string _cacheDirectory;

    private readonly string _exportDirectory;
    
    private readonly string _jsonFilePath;
    
    public DictType Type { get; init; }

    public string FilePath => _jsonFilePath;

    public List<string> this[string word] => ReadJsonFile()[word];
    
    // Creates new dictionary
    public WordDictionary(string from, string to, string homeDirectory)
    {
        _homeDirectory = homeDirectory;
        _cacheDirectory = $"{_homeDirectory}/cache";
        _exportDirectory = $"{_homeDirectory}/export";
        
        Type = new DictType(from, to);
        _jsonFilePath = $"{_cacheDirectory}/{Type.From}-{Type.To}.json";
        
        var jsonFileStream = new StreamWriter(new FileStream(_jsonFilePath, FileMode.Create, FileAccess.Write));
        jsonFileStream.WriteLine("{}");
        jsonFileStream.Close();
        
        string configFilePath = $"{_cacheDirectory}/{Type.From}-{Type.To}.cfg";
        var configFileStream = new StreamWriter(new FileStream(configFilePath, FileMode.Create, FileAccess.Write));
        configFileStream.WriteLine($"jsonFilePath={_jsonFilePath}");
        configFileStream.WriteLine($"type={Type.From}-{Type.To}");
        configFileStream.Close();
    }

    public WordDictionary(string configFilePath, string homeDirectory)
    {
        _homeDirectory = homeDirectory;
        _cacheDirectory = $"{_homeDirectory}/cache";
        _exportDirectory = $"{_homeDirectory}/export";
        
        var configFile = ConfigFile.Deserialize(configFilePath);

        _jsonFilePath = configFile["jsonFilePath"];
        string dictType = configFile["type"];
        string[] dictTypeArray = dictType.Split('=').Last().Split('-');
        Type = new DictType(dictTypeArray[0], dictTypeArray[1]);
    }

    public void AddNewKeyWord(string key, List<string> words)
    {
        var dict = ReadJsonFile();
        dict[key] = words;
        
        WriteJsonFile(dict);
    }

    // Adds word to existing key
    public void AddWord(string key, string word)
    {
        var dict = ReadJsonFile();
        dict[key].Add(word);
        
        WriteJsonFile(dict);
    }

    public void ChangeKey(string oldKey, string newKey)
    {
        var dict = ReadJsonFile();
        dict[newKey] = dict[oldKey];
        dict.Remove(oldKey);
        
        WriteJsonFile(dict);
    }

    public void ChangeTranslation(string key, string oldWord, string newWord)
    {
        var dict = ReadJsonFile();
        int index = dict[key].IndexOf(oldWord);
        
        if (index == -1) throw new KeyNotFoundException("Word to change is not found");
        
        dict[key][index] = newWord;
        WriteJsonFile(dict);
    }

    public void DeletePair(string key)
    {
        var dict = ReadJsonFile();
        dict.Remove(key);
        
        WriteJsonFile(dict);
    }

    public void DeleteTranslation(string key, string toDelete)
    {
        var dict = ReadJsonFile();
        int index = dict[key].IndexOf(toDelete);
        
        if (index == -1) throw new KeyNotFoundException("Word to delete is not found");
        
        dict[key].RemoveAt(index);
        WriteJsonFile(dict);
    }

    public void ExportPairToJson(string key, string nameOfJson)
    {
        var jsonExportFilePath = $"{_exportDirectory}/{nameOfJson}.json";
        var jsonExportFileStream = 
            new StreamWriter(new FileStream(jsonExportFilePath, FileMode.Create, FileAccess.Write));
        jsonExportFileStream.WriteLine("{}");
        jsonExportFileStream.Close();
        
        var wordDict = ReadJsonFile();
        var exportDict = ReadJsonFile(jsonExportFilePath);
        exportDict[key] = wordDict[key];
        
        WriteJsonFile(exportDict, jsonExportFilePath);
    }

    private Dictionary<string, List<string>> ReadJsonFile()
    {
        return ReadJsonFile(_jsonFilePath);
    }

    private void WriteJsonFile(Dictionary<string, List<string>> dict)
    {
        WriteJsonFile(dict, _jsonFilePath);
    }

    public static Dictionary<string, List<string>> ReadJsonFile(string jsonFilePath)
    {
        var jsonString = File.ReadAllText(jsonFilePath);
        
        return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString) ?? 
               new Dictionary<string, List<string>>();
    }

    public static void WriteJsonFile(Dictionary<string, List<string>> dict, string jsonFilePath)
    {
        var jsonString = JsonSerializer.Serialize(dict, _serializationOptions);
        
        File.WriteAllText(jsonFilePath, jsonString);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
    {
        return ReadJsonFile().GetEnumerator();
    }
}