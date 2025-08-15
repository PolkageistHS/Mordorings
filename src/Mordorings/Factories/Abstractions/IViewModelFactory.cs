namespace Mordorings.Factories;

public interface IViewModelFactory
{
    T CreateViewModel<T>() where T : class;
}
