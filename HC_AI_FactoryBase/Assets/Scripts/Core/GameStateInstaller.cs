using Zenject;

public class GameStateInstaller : MonoInstaller<GameStateInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<GameState>().AsSingle().NonLazy();
    }
}