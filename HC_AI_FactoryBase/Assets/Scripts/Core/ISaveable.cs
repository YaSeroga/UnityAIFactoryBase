using System.Collections.Generic;

public interface ISaveable
{
    string GetSaveKey();
    Dictionary<string, object> GetSaveData();
    void LoadSaveData(Dictionary<string, object> data);
}