using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SignalRTestingClient.Models;
using System.Dynamic;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace SignalRTestingClient;

/// <summary>
/// Interaction logic for AddArgumentPrompt.xaml
/// </summary>
public partial class AddArgumentPrompt : Window
{
    public Action<SignalRArgument>? AddBtnClicked { get; set; }

    public AddArgumentPrompt()
    {
        InitializeComponent();
        Topmost = true;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    #region Methods

    private void ShowError(string message)
    {
        MessageBox.Show(message, "An error occurred!", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    #endregion

    #region Handlers

    private void AddBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var argumentType = (string)((ComboBoxItem)ArgType.SelectedItem).Content;

            SignalRArgument argumentToSend = new();
            argumentToSend.Name = Argument.Text;

            if (argumentType.ToLower() == "Numeric value".ToLower())
            {
                argumentToSend.Content = double.Parse(Argument.Text);
            }
            else if (argumentType.ToLower() == "JSON".ToLower())
            {
                argumentToSend.Content = JsonConvert.DeserializeObject(Argument.Text)!;
            }
            else if (argumentType.ToLower() == "String value".ToLower())
            {
                argumentToSend.Name = '"' + Argument.Text + '"';
                argumentToSend.Content = Argument.Text;
            }
            
            AddBtnClicked?.Invoke(argumentToSend);

            Close();
        }
        catch (NullReferenceException)
        {
            ShowError("You haven't selected the type!");
        }
        catch (JsonReaderException)
        {
            ShowError("Invalid JSON!");
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

    #endregion
}