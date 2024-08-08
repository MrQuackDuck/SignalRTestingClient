using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using SignalRTestingClient.Exceptions;
using SignalRTestingClient.Models;
using System.Net;
using System.Net.Http;
using System.Windows;

namespace SignalRTestingClient;

public partial class MainWindow : Window
{
    private HubConnection _connection = default!;
    private List<SignalRArgument> _arguments = [];
    private SignalRArgument _currentSelectedArgument = default!;
    private string jwt = string.Empty;
    private List<string> methodsToListen = [];

    public MainWindow()
    {
        InitializeComponent();
    }

    #region Methods

    /// <summary>
    /// Connects to the SignalR server
    /// </summary>
    /// <param name="address">URL to connect to</param>
    /// <returns>A SignalR connection with the server</returns>
    private HubConnection BuildHubConnection(string address)
    {
        return new HubConnectionBuilder()
            .WithUrl(address, httpConfigure =>
            {
                if (jwt.Length == 0) return;
                httpConfigure.Headers.Add("Authorization", $"Bearer {jwt}");
            })
            .AddNewtonsoftJsonProtocol()
            .Build();
    }

    /// <summary>
    /// Logs the message to logs section
    /// </summary>
    /// <param name="message">The message to log</param>
    private void Log(string message)
    {
        // Adding the message to log list
        var now = DateTime.Now;
        message = $"[{now:HH:mm:ss}] {message}";
        LogList.Items.Add(message);

        // Enabling the button to clear the logs
        ClearLogsBtn.IsEnabled = true;
    }

    /// <summary>
    /// Grabs the value from "AcceptMethods" input, separates it by comma and subscribes on those methods to be logged
    /// </summary>
    private void ApplyHandlersForIncomingMethods()
    {
        if (_connection is null) return;

        // Clearing the list
        foreach (var method in methodsToListen)
            _connection.Remove(method);

        methodsToListen = [];

        // Getting the value of "AcceptMethods" input, removing whitespaces
        var methodsStr = AcceptMethods.Text.Replace(" ", string.Empty);
        // Separating by comma into a list
        var methodsList = methodsStr.Split(',');

        foreach (var method in methodsList)
        {
            // Subscribing on each method
            _connection.On<object>(method, (o) =>
            {
                Dispatcher.Invoke(() => Log($"{method}: {o}"));
            });
        }
    }

    /// <summary>
    /// Refreshes the list of arguments to show new values from the list object
    /// </summary>
    private void UpdateArgumentList()
    {
        ArgumentsList.ItemsSource = null;
        ArgumentsList.ItemsSource = _arguments;
    }

    #endregion

    #region Handlers

    /// <summary>
    /// Tries to connect to the server
    /// </summary>
    private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ConnectBtn.IsEnabled = false;
            _connection = BuildHubConnection(URL.Text);
            ApplyHandlersForIncomingMethods();
            await _connection.StartAsync();

            // What will happen when the connection'll have closed
            _connection.Closed += (arg) =>
            {
                var message = arg?.Message ?? "Connection closed.";

                Dispatcher.Invoke(() =>
                {
                    AuthBtn.IsEnabled = true;
                    ConnectBtn.IsEnabled = true;
                    DisconnectBtn.IsEnabled = false;
                    InvokeBtn.IsEnabled = false;
                    Log(message);
                });

                return Task.CompletedTask;
            };

            Log("Connected!");

            AuthBtn.IsEnabled = false;
            DisconnectBtn.IsEnabled = true;
            InvokeBtn.IsEnabled = true;
        }
        catch (HttpRequestException exception)
        {
            ConnectBtn.IsEnabled = true;

            if (exception.StatusCode == null)
            {
                Log(exception.Message);
                return;
            }

            if (exception.StatusCode == HttpStatusCode.NotFound)
                Log("Not Found! (404)");
            else if (exception.StatusCode == HttpStatusCode.Unauthorized)
                Log("Unauthorized! (401)");
            else
                Log($"{exception.StatusCode}! ({(int)exception.StatusCode!})");
        }
        catch (ConnectionClosedException)
        {
            ConnectBtn.IsEnabled = true;
            Log("Connection not started!");
        }
        catch (NotSupportedException)
        {
            ConnectBtn.IsEnabled = true;
            Log("Provided URL is not valid!");
        }
        catch (UriFormatException)
        {
            ConnectBtn.IsEnabled = true;
            Log("Provided URL is not valid!");
        }
    }

    /// <summary>
    /// Waits for "Enter" to activate the "ConnectBtn" button
    /// </summary>
    private void URL_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter && ConnectBtn.IsEnabled)
            ConnectBtn_Click(this, null!);
    }

    /// <summary>
    /// Waits for "Enter" to activate the "InvokeBtn" button
    /// </summary>
    private void InvokationMethod_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter && InvokeBtn.IsEnabled)
            InvokeBtn_Click(this, null!);
    }

    /// <summary>
    /// Invokes the SignalR method on the server
    /// </summary>
    private async void InvokeBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            object invocationResult = default!;

            // Sorry for that code, but it's the fault of the SignalR client library, not mine ¯\_(ツ)_/¯
            if (_arguments.Count == 0)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text);
            else if (_arguments.Count == 1)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content);
            else if (_arguments.Count == 2)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content);
            else if (_arguments.Count == 3)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content, _arguments[2].Content);
            else if (_arguments.Count == 4)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content, _arguments[2].Content, _arguments[3].Content);
            else if (_arguments.Count == 5)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content, _arguments[2].Content, _arguments[3].Content, _arguments[4].Content);
            else if (_arguments.Count == 6)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content, _arguments[2].Content, _arguments[3].Content, _arguments[4].Content, _arguments[5].Content);
            else if (_arguments.Count == 7)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content, _arguments[2].Content, _arguments[3].Content, _arguments[4].Content, _arguments[5].Content, _arguments[6].Content);
            else if (_arguments.Count == 8)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content, _arguments[2].Content, _arguments[3].Content, _arguments[4].Content, _arguments[5].Content, _arguments[6].Content, _arguments[7].Content);
            else if (_arguments.Count == 9)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content, _arguments[2].Content, _arguments[3].Content, _arguments[4].Content, _arguments[5].Content, _arguments[6].Content, _arguments[7].Content, _arguments[8].Content);
            else if (_arguments.Count == 10)
                invocationResult = await _connection.InvokeAsync<object>(InvokationMethod.Text, _arguments[0].Content, _arguments[1].Content, _arguments[2].Content, _arguments[3].Content, _arguments[4].Content, _arguments[5].Content, _arguments[6].Content, _arguments[7].Content, _arguments[8].Content, _arguments[9].Content);

            Log(invocationResult.ToString()!);
        }
        catch (TaskCanceledException)
        {
            Log("TaskCanceledException: Task cancelled");
        }
        catch (HubException exception)
        {
            Log(exception.Message);
        }
    }

    /// <summary>
    /// Clears the logs section
    /// </summary>
    private void ClearLogsBtn_Click(object sender, RoutedEventArgs e)
    {
        ClearLogsBtn.IsEnabled = false;
        LogList.Items.Clear();
    }

    /// <summary>
    /// Disconnects from the server
    /// </summary>
    private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
    {
        _connection.StopAsync();
    }

    /// <summary>
    /// Opens a window with ability to set authorization JWT token
    /// </summary>
    private void AuthBtn_Click(object sender, RoutedEventArgs e)
    {
        var prompt = new SetAuthHeaderPrompt(jwt);
        prompt.SetBtnClicked += token=>
        {
            jwt = token;
        };

        prompt.Show();
    }

    /// <summary>
    /// Listens for each time "AcceptMethods" input is changed
    /// </summary>
    private void AcceptMethods_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        ApplyHandlersForIncomingMethods();
    }

    /// <summary>
    /// Opens up a pop-up window, where you can add an argument to send with the request
    /// </summary>
    private void AddArgument_Click(object sender, RoutedEventArgs e)
    {
        var prompt = new AddArgumentPrompt();
        prompt.AddBtnClicked += o =>
        {
            _arguments.Add(o);
            Dispatcher.Invoke(() =>
            {
                UpdateArgumentList();
            });
        };
        prompt.Show();
    }

    /// <summary>
    /// Listens whether the arguments list's item was selected. 
    /// If so, enables the button to "remove the selected argument"
    /// </summary>
    private void ArgumentsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ArgumentsList.SelectedItem == null) return;

        _currentSelectedArgument = (SignalRArgument)ArgumentsList.SelectedItem;
        RemoveArgument.IsEnabled = true;
    }

    /// <summary>
    /// Removes selected argument from the list of arguments
    /// </summary>
    private void RemoveArgument_Click(object sender, RoutedEventArgs e)
    {
        _arguments.Remove(_currentSelectedArgument);
        RemoveArgument.IsEnabled = false;
        UpdateArgumentList();
    }

    #endregion
}