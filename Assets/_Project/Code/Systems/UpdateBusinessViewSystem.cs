using Leopotam.EcsLite;

public class UpdateBusinessViewSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly StaticDataService _staticData;
    private readonly BusinessCalculator _calculator;

    private EcsFilter _businessFilter;
    private EcsFilter _businessChangedFilter;
    private EcsFilter _progressChangedFilter;
    private EcsFilter _balanceFilter;

    private EcsPool<BusinessComponent> _businessPool;
    private EcsPool<PlayerBalanceComponent> _balancePool;
    private EcsPool<BusinessViewReference> _viewPool;
    private EcsPool<BusinessChangedEvent> _businessChangedPool;
    private EcsPool<ProgressChangedEvent> _progressChangedPool;

    public UpdateBusinessViewSystem(StaticDataService staticData, BusinessCalculator calculator)
    {
        _staticData = staticData;
        _calculator = calculator;
    }

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        _businessFilter = world.Filter<BusinessComponent>().Inc<BusinessViewReference>().End();
        _businessChangedFilter = world.Filter<BusinessComponent>().Inc<BusinessChangedEvent>().End();
        _progressChangedFilter = world.Filter<BusinessComponent>().Inc<ProgressChangedEvent>().End();
        _balanceFilter = world.Filter<PlayerBalanceComponent>().End();

        _businessPool = world.GetPool<BusinessComponent>();
        _balancePool = world.GetPool<PlayerBalanceComponent>();
        _viewPool = world.GetPool<BusinessViewReference>();
        _businessChangedPool = world.GetPool<BusinessChangedEvent>();
        _progressChangedPool = world.GetPool<ProgressChangedEvent>();

        UpdateBusinesses();
        UpdateProgress();
    }

    public void Run(IEcsSystems systems)
    {
        if (_progressChangedFilter.GetEntitiesCount() > 0)
        {
            UpdateProgress();

            foreach (var entity in _progressChangedFilter)
                _progressChangedPool.Del(entity);
        }

        if (_businessChangedFilter.GetEntitiesCount() > 0)
        {
            UpdateBusinesses();

            foreach (var entity in _businessChangedFilter)
                _businessChangedPool.Del(entity);
        }
    }

    private void UpdateProgress()
    {
        foreach (var entity in _businessFilter)
        {
            ref var viewRef = ref _viewPool.Get(entity);
            ref var business = ref _businessPool.Get(entity);

            viewRef.View.UpdateProgressView(business.Progress);
        }
    }

    private void UpdateBusinesses()
    {
        int balance = _balanceFilter.GetEntitiesCount() > 0
            ? _balancePool.Get(_balanceFilter.GetRawEntities()[0]).Value
            : 0;

        foreach (var entity in _businessFilter)
        {
            ref var business = ref _businessPool.Get(entity);
            ref var viewRef = ref _viewPool.Get(entity);
            var config = _staticData.GetConfig(business.Id);

            viewRef.View.UpdateView(
                business.Level,
                _calculator.CalculateIncome(business, config),
                _calculator.CalculateLevelUpCost(business.Level, config),
                _calculator.CanBuyLevelUp(balance, business.Level, config),
                business.IsUpgrade1Bought,
                business.IsUpgrade2Bought,
                config.Upgrade1Cost,
                config.Upgrade2Cost,
                _calculator.CanBuyUpgrade(balance, config.Upgrade1Cost, business.IsUpgrade1Bought, business.Level),
                _calculator.CanBuyUpgrade(balance, config.Upgrade2Cost, business.IsUpgrade2Bought, business.Level)
            );
        }
    }
}