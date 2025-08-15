using System.Windows;

namespace Mordorings.Factories;

public class DialogFactory : IDialogFactory
{
    /// <inheritdoc />
    public void ShowMessage(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK);
    }

    /// <inheritdoc />
    public void ShowWarningMessage(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    /// <inheritdoc />
    public void ShowErrorMessage(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <inheritdoc />
    public bool ShowYesNoQuestion(string message, string title)
    {
        MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }

    /// <inheritdoc />
    public bool ShowOkCancelQuestion(string message, string title)
    {
        MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        return result == MessageBoxResult.OK;
    }
}
