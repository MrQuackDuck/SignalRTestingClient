using System.Windows;

namespace SignalRTestingClient;

/// <summary>
/// Interaction logic for SetAuthHeaderPrompt.xaml
/// </summary>
public partial class SetAuthHeaderPrompt : Window
{
    public Action<string>? SetBtnClicked { get; set; }

    public SetAuthHeaderPrompt(string initialJwt)
    {
        InitializeComponent();
        JWT.Text = initialJwt;
    }

    private void SetBtn_Click(object sender, RoutedEventArgs e)
    {
        SetBtnClicked?.Invoke(JWT.Text);
        Close();
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
