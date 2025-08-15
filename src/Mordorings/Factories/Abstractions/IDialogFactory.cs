namespace Mordorings.Factories;

public interface IDialogFactory
{
    void ShowMessage(string message, string title);

    void ShowWarningMessage(string message, string title);

    void ShowErrorMessage(string message, string title);

    bool ShowYesNoQuestion(string message, string title);

    bool ShowOkCancelQuestion(string message, string title);
}
