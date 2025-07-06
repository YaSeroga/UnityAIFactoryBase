using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SaveDataRegistry : IInitializable
{
    private readonly List<ISaveable> _saveableComponents = new List<ISaveable>();
    private readonly Dictionary<string, Dictionary<string, object>> _globalSaveData = new Dictionary<string, Dictionary<string, object>>();

    public void Initialize()
    {
        Debug.Log("SaveDataRegistry initialized");
    }

    public void RegisterSaveable(ISaveable saveable)
    {
        if (!_saveableComponents.Contains(saveable))
        {
            _saveableComponents.Add(saveable);
            Debug.Log($"Registered saveable component: {saveable.GetSaveKey()}");
        }
    }

    public void UnregisterSaveable(ISaveable saveable)
    {
        if (_saveableComponents.Contains(saveable))
        {
            _saveableComponents.Remove(saveable);
            Debug.Log($"Unregistered saveable component: {saveable.GetSaveKey()}");
        }
    }

    public Dictionary<string, Dictionary<string, object>> CollectAllSaveData()
    {
        var allSaveData = new Dictionary<string, Dictionary<string, object>>();

        foreach (var saveable in _saveableComponents)
        {
            if (saveable != null)
            {
                string key = saveable.GetSaveKey();
                var data = saveable.GetSaveData();
                
                if (data != null && data.Count > 0)
                {
                    allSaveData[key] = data;
                }
            }
        }

        // Include global save data
        foreach (var kvp in _globalSaveData)
        {
            allSaveData[kvp.Key] = kvp.Value;
        }

        Debug.Log($"Collected save data from {allSaveData.Count} components");
        return allSaveData;
    }

    public void LoadAllSaveData(Dictionary<string, Dictionary<string, object>> allSaveData)
    {
        if (allSaveData == null)
        {
            Debug.LogWarning("No save data to load");
            return;
        }

        // Load data into registered components
        foreach (var saveable in _saveableComponents)
        {
            if (saveable != null)
            {
                string key = saveable.GetSaveKey();
                if (allSaveData.ContainsKey(key))
                {
                    saveable.LoadSaveData(allSaveData[key]);
                }
            }
        }

        // Store global data for later use
        _globalSaveData.Clear();
        foreach (var kvp in allSaveData)
        {
            _globalSaveData[kvp.Key] = kvp.Value;
        }

        Debug.Log($"Loaded save data for {allSaveData.Count} components");
    }

    public void SetGlobalData(string key, string dataKey, object value)
    {
        if (!_globalSaveData.ContainsKey(key))
        {
            _globalSaveData[key] = new Dictionary<string, object>();
        }
        
        _globalSaveData[key][dataKey] = value;
    }

    public T GetGlobalData<T>(string key, string dataKey, T defaultValue = default)
    {
        if (_globalSaveData.ContainsKey(key) && _globalSaveData[key].ContainsKey(dataKey))
        {
            try
            {
                return (T)_globalSaveData[key][dataKey];
            }
            catch
            {
                Debug.LogWarning($"Failed to cast global data {key}.{dataKey} to {typeof(T)}");
            }
        }
        
        return defaultValue;
    }

    public bool HasGlobalData(string key, string dataKey)
    {
        return _globalSaveData.ContainsKey(key) && _globalSaveData[key].ContainsKey(dataKey);
    }

    public void ClearGlobalData(string key = null)
    {
        if (key == null)
        {
            _globalSaveData.Clear();
            Debug.Log("Cleared all global save data");
        }
        else if (_globalSaveData.ContainsKey(key))
        {
            _globalSaveData.Remove(key);
            Debug.Log($"Cleared global save data for key: {key}");
        }
    }

    public List<string> GetRegisteredKeys()
    {
        return _saveableComponents.Where(s => s != null).Select(s => s.GetSaveKey()).ToList();
    }
}