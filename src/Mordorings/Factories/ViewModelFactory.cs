namespace Mordorings.Factories;

public class ViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    public T CreateViewModel<T>() where T : class => serviceProvider.GetRequiredService<T>();
}
