using Zenject;

public class GameInstanceInstaller : MonoInstaller<GameInstanceInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<GameInstance>().AsSingle().NonLazy();
        print("hello");
    }
}