using CommunityToolkit.Mvvm.ComponentModel;

namespace Mordorings.ViewModels.Abstractions;

public abstract class ViewModelBase : ObservableObject, IViewModel
{
    public virtual string Instructions => throw new InvalidOperationException("Instructions not implemented.");
}
