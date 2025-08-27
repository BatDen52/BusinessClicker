using Leopotam.EcsLite;

public class UpdateBalanceViewSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _balanceFilter;
    private EcsFilter _balanceChangedFilter;
    private EcsPool<PlayerBalanceComponent> _balancePool;
    private EcsPool<BalanceChangedEvent> _balanceChangedPool;

    private MainView _uiManager;

    public UpdateBalanceViewSystem(MainView uiManager)
    {
        _uiManager = uiManager;
    }

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        _balanceFilter = world.Filter<PlayerBalanceComponent>().End();
        _balanceChangedFilter = world.Filter<BalanceChangedEvent>().End();
        _balancePool = world.GetPool<PlayerBalanceComponent>();
        _balanceChangedPool = world.GetPool<BalanceChangedEvent>();

        UpdateBalance();
    }

    public void Run(IEcsSystems systems)
    {
        if (_balanceChangedFilter.GetEntitiesCount() > 0)
        {
            UpdateBalance();

            foreach (var entity in _balanceChangedFilter)
                _balanceChangedPool.Del(entity);
        }
    }

    private void UpdateBalance()
    {
        int balance = 0;

        if (_balanceFilter.GetEntitiesCount() > 0)
            balance = _balancePool.Get(_balanceFilter.GetRawEntities()[0]).Value;

        _uiManager.UpdateBalance(balance);
    }
}
