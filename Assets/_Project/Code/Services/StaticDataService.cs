using System.Collections.Generic;
using UnityEngine;

public class StaticDataService
{
    private readonly GameConfig _gameConfig;
    private readonly LocalizationConfig _localizationConfig;

    private Dictionary<int, BusinessConfig> _configDict = new();
    private Dictionary<int, BusinessLocalization> _localizationDict = new();

    public StaticDataService(GameConfig gameConfig, LocalizationConfig localizationConfig)
    {
        _gameConfig = gameConfig;
        _localizationConfig = localizationConfig;

        foreach (var config in _gameConfig.BusinessConfigs)
            _configDict[config.Id] = config;
        
        foreach (var config in _localizationConfig.BusinessLocalizations)
            _localizationDict[config.Id] = config;
    }

    public int Count => _configDict.Count;
    public IEnumerable<int> IDs => _configDict.Keys;
    public LocalizationLables Lables => _localizationConfig.Lables;

    public BusinessConfig GetConfig(int id)
    {
        if (_configDict.TryGetValue(id, out var config))
            return config;

        Debug.LogError($"Config not found for business ID: {id}");
        return null;
    }

    public BusinessLocalization GetLocalization(int id)
    {
        if (_localizationDict.TryGetValue(id, out var loc))
            return loc;

        Debug.LogError($"Localization not found for business ID: {id}");
        return null;
    }
}
