using Leopotam.EcsLite;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private LocalizationConfig _localizationConfig;

    [Header("UI")]
    [SerializeField] private MainView _uiManager;

    private EcsSystems _systems;
    private EcsWorld _world;
    private SaveService _saveService;

    private void Start()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        var calculator = new BusinessCalculator();
        var staticData = new StaticDataService(_gameConfig, _localizationConfig);
        _saveService = new SaveService(staticData);

        _uiManager.Initialize(staticData, _world);

        _systems
            .Add(new InitializationSystem(staticData, _uiManager))
            .Add(new SaveLoadSystem(_saveService, _world))
            .Add(new BuyLevelSystem(staticData, calculator))
            .Add(new BuyUpgradeSystem(staticData, calculator))
            .Add(new IncomeProgressSystem(staticData, calculator))
            .Add(new UpdateBusinessViewSystem(staticData, calculator))
            .Add(new UpdateBalanceViewSystem(_uiManager))
            .Init();
    }

    private void Update()
    {
        _systems?.Run();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            _saveService.Save(_world);
    }

    private void OnApplicationQuit()
    {
        _saveService.Save(_world);
    }

    private void OnDestroy()
    {
        if (_systems != null)
        {
            _systems.Destroy();
            _systems = null;
        }

        if (_world != null)
        {
            _world.Destroy();
            _world = null;
        }
    }
}