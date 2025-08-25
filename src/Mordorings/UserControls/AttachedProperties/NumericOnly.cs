using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mordorings.UserControls;

public static partial class NumericOnly
{
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
        "Enabled", typeof(bool), typeof(NumericOnly), new PropertyMetadata(false, OnEnabledChanged));

    public static bool GetEnabled(DependencyObject obj) => (bool)obj.GetValue(EnabledProperty);

    public static void SetEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(EnabledProperty, value);
    }

    private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
            return;
        if ((bool)e.NewValue)
        {
            textBox.PreviewTextInput += OnPreviewTextInput;
            textBox.TextChanged += OnTextChanged;
            DataObject.AddPastingHandler(textBox, OnPaste);
        }
        else
        {
            textBox.PreviewTextInput -= OnPreviewTextInput;
            textBox.TextChanged -= OnTextChanged;
            DataObject.RemovePastingHandler(textBox, OnPaste);
        }
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsNumeric(e.Text);
    }

    private static void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;
        int caretIndex = textBox.CaretIndex;
        string cleanedText = CheckForNumbers().Replace(textBox.Text, "");
        if (textBox.Text == cleanedText)
            return;
        textBox.Text = cleanedText;
        textBox.CaretIndex = Math.Min(caretIndex, cleanedText.Length);
    }

    private static void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(typeof(string)))
            return;
        string? pastedText = (string?)e.DataObject.GetData(typeof(string));
        if (pastedText == null)
            return;
        string numericText = CheckForNumbers().Replace(pastedText, "");
        if (pastedText == numericText)
            return;
        e.CancelCommand();
        if (sender is not TextBox textBox || string.IsNullOrEmpty(numericText))
            return;
        int caretIndex = textBox.CaretIndex;
        int selectionLength = textBox.SelectionLength;
        string currentText = textBox.Text;
        string beforeSelection = currentText.Substring(0, caretIndex);
        string afterSelection = currentText.Substring(caretIndex + selectionLength);
        textBox.Text = beforeSelection + numericText + afterSelection;
        textBox.CaretIndex = caretIndex + numericText.Length;
    }

    private static bool IsNumeric(string text) =>
        !string.IsNullOrEmpty(text) && text.All(char.IsDigit);

    [GeneratedRegex("[^0-9]")]
    private static partial Regex CheckForNumbers();
}
