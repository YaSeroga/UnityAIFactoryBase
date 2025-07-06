using Zenject;

public class GameInstanceInstaller : MonoInstaller<GameInstanceInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<EventBus>().AsSingle().NonLazy();
        Container.Bind<GameInstance>().AsSingle().NonLazy();
        Container.Bind<SaveDataRegistry>().AsSingle().NonLazy();
        Container.Bind<SaveLoadManager>().AsSingle().NonLazy();
    }
}