using Newtonsoft.Json;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace SignalRTestingClient;

/// <summary>
/// Interaction logic for AddArgumentPrompt.xaml
/// </summary>
public partial class AddArgumentPrompt : Window
{
    public Action<object>? AddBtnClicked { get; set; }

    public AddArgumentPrompt()
    {
        InitializeComponent();
    }

    private void AddBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var argumentType = (string)((ComboBoxItem)ArgType.SelectedItem).Content;

            object argumentToSend = default!;

            if (argumentType.ToLower() == "Numeric value".ToLower())
            {
                argumentToSend = double.Parse(Argument.Text);
            }
            else if (argumentType.ToLower() == "String value".ToLower())
            {
                argumentToSend = JsonConvert.SerializeObject(Argument.Text);
            }
            else if (argumentType.ToLower() == "JSON".ToLower())
            {
                argumentToSend = Argument.Text;
            }

            AddBtnClicked?.Invoke(argumentToSend);

            Close();
        }
        catch (NullReferenceException)
        {
            ShowError("You haven't selected the type!");
        }
        catch (FormatException)
        {
            ShowError("Invalid number!");
        }
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ShowError(string message)
    {
        MessageBox.Show(message, "An error occurred!", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}