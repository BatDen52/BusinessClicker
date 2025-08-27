using Leopotam.EcsLite;

public class BuyLevelSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly StaticDataService _staticData;
    private readonly BusinessCalculator _calculator;

    private EcsWorld _world;

    private EcsFilter _buyLevelFilter;
    private EcsFilter _balanceFilter;

    private EcsPool<BuyLevelEvent> _buyLevelPool;
    private EcsPool<BusinessComponent> _businessPool;
    private EcsPool<PlayerBalanceComponent> _balancePool;
    private EcsPool<BusinessChangedEvent> _businessChangedPool;
    private EcsPool<BalanceChangedEvent> _balanceChangedPool;

    public BuyLevelSystem(StaticDataService staticData, BusinessCalculator calculator)
    {
        _staticData = staticData;
        _calculator = calculator;
    }

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _buyLevelFilter = _world.Filter<BuyLevelEvent>().Inc<BusinessComponent>().End();
        _balanceFilter = _world.Filter<PlayerBalanceComponent>().End();

        _buyLevelPool = _world.GetPool<BuyLevelEvent>();
        _businessPool = _world.GetPool<BusinessComponent>();
        _balancePool = _world.GetPool<PlayerBalanceComponent>();
        _businessChangedPool = _world.GetPool<BusinessChangedEvent>();
        _balanceChangedPool = _world.GetPool<BalanceChangedEvent>();
    }

    public void Run(IEcsSystems systems)
    {
        if (_balanceFilter.GetEntitiesCount() == 0)
            return;

        int balanceEntity = _balanceFilter.GetRawEntities()[0];
        ref var balance = ref _balancePool.Get(balanceEntity);

        foreach (var businessEntity in _buyLevelFilter)
        {
            ref var business = ref _businessPool.Get(businessEntity);
            BusinessConfig config = _staticData.GetConfig(business.Id);

            if (_calculator.CanBuyLevelUp(balance.Value, business.Level, config))
            {
                balance.Value -= _calculator.CalculateLevelUpCost(business.Level, config);
                business.Level++;

                if (_balanceChangedPool.Has(balanceEntity) == false)
                    _balanceChangedPool.Add(balanceEntity);

                if (_businessChangedPool.Has(businessEntity) == false)
                    _businessChangedPool.Add(businessEntity);
            }

            _buyLevelPool.Del(businessEntity);
        }
    }
}