using Leopotam.EcsLite;

public class InitializationSystem : IEcsInitSystem
{
    private readonly StaticDataService _staticData;
    private readonly MainView _uiManager;

    private EcsWorld _world = null;
    private EcsPool<PlayerBalanceComponent> _balancePool = null;
    private EcsPool<BusinessComponent> _businessPool = null;

    private EcsPool<BalanceChangedEvent> _balanceChangedPool = null;
    private EcsPool<BusinessChangedEvent> _businessChangedPool = null;
    private EcsPool<BusinessViewReference> _viewPool;

    public InitializationSystem(StaticDataService staticData, MainView uiManager)
    {
        _staticData = staticData;
        _uiManager = uiManager;
    }

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _balancePool = _world.GetPool<PlayerBalanceComponent>();
        _businessPool = _world.GetPool<BusinessComponent>();
        _viewPool = _world.GetPool<BusinessViewReference>();

        _balanceChangedPool = _world.GetPool<BalanceChangedEvent>();
        _businessChangedPool = _world.GetPool<BusinessChangedEvent>();

        CreateBalance();
        CreateBusinesses();
    }

    private void CreateBalance()
    {
        var balanceEntity = _world.NewEntity();
        ref var balanceComponent = ref _balancePool.Add(balanceEntity);
        balanceComponent.Value = 0;

        _balanceChangedPool.Add(balanceEntity);
    }

    private void CreateBusinesses()
    {
        foreach (int id in _staticData.IDs)
        {
            var businessEntity = _world.NewEntity();
            ref var businessComponent = ref _businessPool.Add(businessEntity);

            InitializeBusiness(id, ref businessComponent);

            _businessChangedPool.Add(businessEntity);

            ref var viewRef = ref _viewPool.Add(businessEntity);
            viewRef.View = _uiManager.CreateBusiness(businessComponent.Id);
        }
    }

    private void InitializeBusiness(int id, ref BusinessComponent businessComponent)
    {
        businessComponent.Id = id;
        businessComponent.Level = _staticData.GetConfig(id).LevelOnStart;
        businessComponent.Progress = 0f;
        businessComponent.IsUpgrade1Bought = false;
        businessComponent.IsUpgrade2Bought = false;
    }
}