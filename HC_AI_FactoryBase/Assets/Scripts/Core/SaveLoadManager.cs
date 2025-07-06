using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

public class SaveLoadManager : IInitializable
{
    private const string SAVE_FILE_NAME = "gamedata.json";
    private string _saveFilePath;

    [Inject] private GameInstance _gameInstance;
    [Inject] private SaveDataRegistry _saveDataRegistry;

    public void Initialize()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        Debug.Log($"Save file path: {_saveFilePath}");
        
        // Auto-load on startup
        LoadGame();
    }

    public void SaveGame()
    {
        try
        {
            var saveData = new SaveData
            {
                gameInstanceData = _gameInstance.GetSaveData(),
                componentData = _saveDataRegistry.CollectAllSaveData(),
                saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(_saveFilePath, json);
            
            Debug.Log($"Game saved successfully at {saveData.saveTime}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        try
        {
            if (!File.Exists(_saveFilePath))
            {
                Debug.Log("No save file found, starting fresh");
                return;
            }

            string json = File.ReadAllText(_saveFilePath);
            var saveData = JsonUtility.FromJson<SaveData>(json);
            
            _gameInstance.LoadSaveData(saveData.gameInstanceData);
            _saveDataRegistry.LoadAllSaveData(saveData.componentData);
            
            Debug.Log($"Game loaded successfully from {saveData.saveTime}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    public void DeleteSave()
    {
        try
        {
            if (File.Exists(_saveFilePath))
            {
                File.Delete(_saveFilePath);
                Debug.Log("Save file deleted");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete save file: {e.Message}");
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(_saveFilePath);
    }

    public string GetSaveFileInfo()
    {
        if (!HasSaveFile())
            return "No save file";

        try
        {
            var fileInfo = new FileInfo(_saveFilePath);
            return $"Last saved: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}";
        }
        catch
        {
            return "Save file corrupted";
        }
    }
}

[System.Serializable]
public struct SaveData
{
    public GameInstanceData gameInstanceData;
    public Dictionary<string, Dictionary<string, object>> componentData;
    public string saveTime;
}