using Leopotam.EcsLite;
using System;
using UnityEngine;

public class SaveService
{
    private const string SaveDataKey = "GameSaveData";

    private readonly StaticDataService _staticData;

    public SaveService(StaticDataService staticData)
    {
        _staticData = staticData;
    }

    public void Save(EcsWorld world)
    {
        var saveData = new SaveData();
        SaveBalance(world, saveData);
        SaveBusiness(world, saveData);
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SaveDataKey, json);
        PlayerPrefs.Save();
    }

    public void Load(EcsWorld world)
    {
        if (!PlayerPrefs.HasKey(SaveDataKey))
            return;

        try
        {
            string json = PlayerPrefs.GetString(SaveDataKey);
            var saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData == null)
                return;

            if (saveData.Businesses == null || saveData.Businesses.Length != _staticData.Count)
                Debug.LogWarning("Save data version mismatch. Some data may be reset.");

            LoadBalance(world, saveData);
            LoadBusiness(world, saveData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}\n{e.StackTrace}");
        }
    }

    private void SaveBalance(EcsWorld world, SaveData saveData)
    {
        var balanceFilter = world.Filter<PlayerBalanceComponent>().End();
        var balancePool = world.GetPool<PlayerBalanceComponent>();

        if (balanceFilter.GetEntitiesCount() > 0)
        {
            int entity = balanceFilter.GetRawEntities()[0];
            saveData.Balance = balancePool.Get(entity).Value;
        }
    }

    private void SaveBusiness(EcsWorld world, SaveData saveData)
    {
        var businessFilter = world.Filter<BusinessComponent>().End();
        var businessPool = world.GetPool<BusinessComponent>();

        saveData.Businesses = new BusinessSaveData[businessFilter.GetEntitiesCount()];
        int index = 0;

        foreach (var entity in businessFilter)
        {
            ref var business = ref businessPool.Get(entity);
            saveData.Businesses[index] = new BusinessSaveData
            {
                Id = business.Id,
                Level = business.Level,
                Progress = business.Progress,
                IsUpgrade1Bought = business.IsUpgrade1Bought,
                IsUpgrade2Bought = business.IsUpgrade2Bought
            };
            index++;
        }
    }

    private void LoadBalance(EcsWorld world, SaveData saveData)
    {
        var balanceFilter = world.Filter<PlayerBalanceComponent>().End();
        var balancePool = world.GetPool<PlayerBalanceComponent>();

        foreach (var entity in balanceFilter)
        {
            ref var balance = ref balancePool.Get(entity);
            balance.Value = saveData.Balance;
        }
    }

    private void LoadBusiness(EcsWorld world, SaveData saveData)
    {
        if (saveData.Businesses == null)
            return;

        var businessFilter = world.Filter<BusinessComponent>().End();
        var businessPool = world.GetPool<BusinessComponent>();

        foreach (var entity in businessFilter)
        {
            ref var business = ref businessPool.Get(entity);
            foreach (var businessSaveData in saveData.Businesses)
            {
                if (businessSaveData.Id == business.Id)
                {
                    business.Level = businessSaveData.Level;
                    business.Progress = businessSaveData.Progress;
                    business.IsUpgrade1Bought = businessSaveData.IsUpgrade1Bought;
                    business.IsUpgrade2Bought = businessSaveData.IsUpgrade2Bought;
                    break;
                }
            }
        }
    }
}