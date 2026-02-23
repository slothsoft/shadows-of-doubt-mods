using System;
using System.IO;
using System.Text.Json;
using SOD.Common;
using SOD.Common.Custom;
using SOD.Common.Helpers;

namespace LiveYourLife.Common;

public class SaveDataManager<TSaveData> 
    where TSaveData : new()
{
    public const string JsonExtension = ".json";
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = false, 
        Converters = { new TimeDataJsonConverter() },
    };
    
    public string FileName { get; }
    public TSaveData SaveData { get; set; }
    public Action<string> Log { get; set; } = Console.WriteLine;
    
    public SaveDataManager(string fileName)
    {
        FileName = fileName;
        
        Lib.SaveGame.OnBeforeNewGame += (_, e) => Clear();
        Lib.SaveGame.OnBeforeLoad += (_, e) => Load(e);
        Lib.SaveGame.OnBeforeSave += (_, e) => Save(e);
        Lib.SaveGame.OnBeforeDelete += (_, e) => Delete(e);
    }

    internal void Clear()
    {
        SaveData = new TSaveData();
    }

    internal void Load(SaveGameArgs e)
    {
        var saveGamePath = Path.Combine(Lib.SaveGame.GetSaveGameDataPath(e), FileName + JsonExtension);
        Log($"Loading from {saveGamePath}");
        
        Clear();

        if (File.Exists(saveGamePath))
        {
            var json = File.ReadAllText(saveGamePath);
            SaveData = JsonSerializer.Deserialize<TSaveData>(json, JsonOptions) ??  new TSaveData();
        }
        else
        {
            Clear();
        }
    }

    internal void Save(SaveGameArgs e)
    {
        var saveGamePath = Path.Combine(Lib.SaveGame.GetSaveGameDataPath(e), FileName + JsonExtension);
        Log($"Saving to {saveGamePath}");

        var json = JsonSerializer.Serialize(SaveData, JsonOptions);
        File.WriteAllText(saveGamePath, json);
    }

    internal void Delete(SaveGameArgs e)
    {
        var saveGamePath = Path.Combine(Lib.SaveGame.GetSaveGameDataPath(e), FileName + JsonExtension);
        Log($"Deleting {saveGamePath}");
        File.Delete(saveGamePath);
    }
}