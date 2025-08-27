using Leopotam.EcsLite;

public class SaveLoadSystem : IEcsInitSystem
{
    private readonly SaveService _saveService;
    private readonly EcsWorld _world;

    public SaveLoadSystem(SaveService saveService, EcsWorld world)
    {
        _saveService = saveService;
        _world = world;
    }

    public void Init(IEcsSystems systems)
    {
        _saveService.Load(_world);
    }
}
