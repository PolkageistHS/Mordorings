namespace Mordorings.ViewModels.Abstractions;

public abstract class ViewModelBase : ObservableObject, IViewModel
{
    public abstract string Instructions { get; }
}
