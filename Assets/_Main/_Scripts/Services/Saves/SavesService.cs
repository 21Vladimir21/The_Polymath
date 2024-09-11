using System;
using System.IO;
using _Main._Scripts.LetterPooLogic;
using _Main._Scripts.Services.Saves;
using UnityEngine;

public class SavesService :IService
{
    private string _filePath;
    public Saves Saves { get; set; }

    public SavesService() => LoadSaveData();


    public void InitSaves()
    {
        Saves.LoadSaves();
    }

    public void LoadSaveData(Action success = null)
    {
#if  !UNITY_EDITOR
             LoadCloudSaves(success);
#endif
#if  UNITY_EDITOR
            LoadLocalSaves(success);
#endif
    }

    private void LoadCloudSaves(Action success)
    {
        // var json = Cloud.GetValue<string>(Saves.SaveKey);
        //
        // if (json == default)
        // {
        //     Saves = new Saves();
        //     Saves.InvokeSave();
        //     Debug.Log("Сохранения не найдены");
        // }
        // else
        // {
        //     var save = JsonUtility.FromJson<Saves>(json);
        //     Saves = save;
        //     Debug.Log("Игра загружена");
        // }

    }

    private void LoadLocalSaves(Action success)
    {
        _filePath = Saves.GetFilePath();

        if (!File.Exists(_filePath))
        {
            Debug.Log("Игра не загружена");
            Saves = new Saves();
            Saves.InvokeSave();

            success?.Invoke();
            return;
        }

        var json = File.ReadAllText(_filePath);
        var save = JsonUtility.FromJson<Saves>(json);
        Saves = save;

        success?.Invoke();
        Debug.Log("Игра загружена");
    }
}
