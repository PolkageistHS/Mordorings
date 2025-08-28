using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Mordorings.Controls;

public partial class NumericOnlyBehavior : Behavior<TextBox>
{
    public int? MinValue { get; set; }

    public int? MaxValue { get; set; }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewTextInput += OnPreviewTextInput;
        AssociatedObject.TextChanged += OnTextChanged;
        AssociatedObject.LostFocus += OnLostFocus;
        DataObject.AddPastingHandler(AssociatedObject, OnPaste);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
        AssociatedObject.TextChanged -= OnTextChanged;
        AssociatedObject.LostFocus -= OnLostFocus;
        DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !ValidateInput(e.Text);
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

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            if (MinValue != null)
            {
                textBox.Text = MinValue.Value.ToString();
            }
            return;
        }
        if (!int.TryParse(textBox.Text, out int value))
            return;
        if (value < MinValue)
        {
            textBox.Text = MinValue.Value.ToString();
        }
        else if (value > MaxValue)
        {
            textBox.Text = MaxValue.Value.ToString();
        }
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(typeof(string)))
            return;
        string? pastedText = (string?)e.DataObject.GetData(typeof(string));
        if (pastedText == null)
            return;
        string numericText = CheckForNumbers().Replace(pastedText, "");
        if (pastedText == numericText && !ValidateInput(pastedText))
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

    private bool ValidateMinMax(string text)
    {
        if (MinValue is null && MaxValue is null)
            return true;
        if (string.IsNullOrWhiteSpace(text))
            return true;
        int value = int.Parse(text);
        bool isInRange = true;
        if (MinValue != null)
        {
            isInRange = value >= MinValue;
        }
        if (MaxValue != null)
        {
            isInRange &= value <= MaxValue;
        }
        return isInRange;
    }

    private bool ValidateInput(string text) =>
        !string.IsNullOrEmpty(text) && text.All(char.IsDigit) && ValidateMinMax(AssociatedObject.Text + text);

    [GeneratedRegex("[^0-9]")]
    private static partial Regex CheckForNumbers();
}
