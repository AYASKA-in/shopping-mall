using System.Net;
using System.Net.Sockets;
using System.Net.Http.Json;
using ShoppingMall.Client.Services;

namespace ShoppingMall.Client.ViewModels;

public enum SetupStep { Server, Terminal, Confirm }

public class SetupWizardViewModel : BaseViewModel
{
    private readonly AppConfiguration _config;
    private readonly HttpClient _http;

    private SetupStep _currentStep = SetupStep.Server;
    public SetupStep CurrentStep
    {
        get => _currentStep;
        set
        {
            SetProperty(ref _currentStep, value);
            OnPropertyChanged(nameof(IsStepServer));
            OnPropertyChanged(nameof(IsStepTerminal));
            OnPropertyChanged(nameof(IsStepConfirm));
            OnPropertyChanged(nameof(StepTitle));
            OnPropertyChanged(nameof(NextButtonText));
            OnPropertyChanged(nameof(IsBackVisible));
        }
    }

    public bool IsStepServer => CurrentStep == SetupStep.Server;
    public bool IsStepTerminal => CurrentStep == SetupStep.Terminal;
    public bool IsStepConfirm => CurrentStep == SetupStep.Confirm;

    public string StepTitle => CurrentStep switch
    {
        SetupStep.Server => "Connect to Server",
        SetupStep.Terminal => "Terminal Registration",
        SetupStep.Confirm => "Confirm Settings",
        _ => ""
    };

    public string NextButtonText => CurrentStep == SetupStep.Confirm ? "Finish" : "Next";
    public string NextButtonWidth => CurrentStep == SetupStep.Confirm ? "120" : "80";
    public bool IsBackVisible => CurrentStep != SetupStep.Server;

    private string _serverUrl = "http://localhost:5194";
    public string ServerUrl
    {
        get => _serverUrl;
        set => SetProperty(ref _serverUrl, value);
    }

    private string _terminalName = "POS-1";
    public string TerminalName
    {
        get => _terminalName;
        set => SetProperty(ref _terminalName, value);
    }

    private string _storeCode = "";
    public string StoreCode
    {
        get => _storeCode;
        set => SetProperty(ref _storeCode, value);
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string _discoverStatus = "";
    public string DiscoverStatus
    {
        get => _discoverStatus;
        set => SetProperty(ref _discoverStatus, value);
    }

    public string ConfirmServerUrl => $"Server: {_serverUrl}";
    public string ConfirmTerminalName => $"Terminal: {_terminalName}";
    public string ConfirmStoreName => $"Store: {_storeCode}";

    public ICommand AutoDiscoverCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand BackCommand { get; }

    public event EventHandler? SetupCompleted;

    public SetupWizardViewModel(AppConfiguration config)
    {
        _config = config;
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        AutoDiscoverCommand = new AsyncRelayCommand(async _ => await AutoDiscoverAsync());
        NextCommand = new AsyncRelayCommand(async _ => await OnNextAsync());
        BackCommand = new RelayCommand(_ => GoBack());
    }

    private async Task AutoDiscoverAsync()
    {
        DiscoverStatus = "Scanning local network...";
        IsLoading = true;
        ErrorMessage = "";

        await Task.Run(async () =>
        {
            using var udp = new UdpClient { EnableBroadcast = true };
            udp.Client.ReceiveTimeout = 3000;
            var broadcast = new byte[] { 0x53, 0x4D, 0x50, 0x49, 0x4E, 0x47 };
            udp.Send(broadcast, broadcast.Length, new IPEndPoint(IPAddress.Broadcast, 52000));

            var tasks = new List<Task>();
            for (int port = 5000; port <= 5010; port++)
            {
                var p = port;
                tasks.Add(TryProbeAsync(p));
            }
            await Task.WhenAll(tasks);
        });

        IsLoading = false;
        if (string.IsNullOrEmpty(ErrorMessage))
            DiscoverStatus = "No server found. Enter address manually.";
    }

    private async Task TryProbeAsync(int port)
    {
        try
        {
            var url = $"http://localhost:{port}";
            var response = await _http.GetAsync($"{url}/api/admin/health");
            if (response.IsSuccessStatusCode)
            {
                ServerUrl = url;
                DiscoverStatus = $"Found server at {url}";
            }
        }
        catch { }
    }

    private async Task OnNextAsync()
    {
        ErrorMessage = "";

        switch (CurrentStep)
        {
            case SetupStep.Server:
                if (string.IsNullOrWhiteSpace(ServerUrl))
                {
                    ErrorMessage = "Enter server address";
                    return;
                }
                ServerUrl = ServerUrl.TrimEnd('/');
                IsLoading = true;
                try
                {
                    var response = await _http.GetAsync($"{ServerUrl}/api/admin/health");
                    if (!response.IsSuccessStatusCode)
                    {
                        ErrorMessage = "Cannot reach server. Check address.";
                        return;
                    }
                }
                catch
                {
                    ErrorMessage = "Connection failed. Verify server is running.";
                    return;
                }
                finally { IsLoading = false; }
                CurrentStep = SetupStep.Terminal;
                break;

            case SetupStep.Terminal:
                if (string.IsNullOrWhiteSpace(TerminalName))
                {
                    ErrorMessage = "Enter terminal name";
                    return;
                }
                if (string.IsNullOrWhiteSpace(StoreCode))
                {
                    ErrorMessage = "Enter store code";
                    return;
                }
                CurrentStep = SetupStep.Confirm;
                OnPropertyChanged(nameof(ConfirmServerUrl));
                OnPropertyChanged(nameof(ConfirmTerminalName));
                OnPropertyChanged(nameof(ConfirmStoreName));
                break;

            case SetupStep.Confirm:
                IsLoading = true;
                try
                {
                    var registerResponse = await _http.PostAsJsonAsync(
                        $"{ServerUrl}/api/admin/terminals/register",
                        new { storeCode = StoreCode, name = TerminalName });

                    if (!registerResponse.IsSuccessStatusCode)
                    {
                        var err = await registerResponse.Content.ReadAsStringAsync();
                        ErrorMessage = $"Registration failed: {err}";
                        return;
                    }

                    var terminal = await registerResponse.Content.ReadFromJsonAsync<TerminalRegistration>();
                    if (terminal == null)
                    {
                        ErrorMessage = "Invalid server response";
                        return;
                    }

                    _config.Save(new ClientConfig
                    {
                        ServerUrl = ServerUrl,
                        TerminalId = terminal.Id,
                        StoreId = terminal.StoreId,
                        TerminalName = TerminalName,
                        AutoConnect = true,
                        IsConfigured = true
                    });

                    SetupCompleted?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Setup error: {ex.Message}";
                }
                finally { IsLoading = false; }
                break;
        }
    }

    private void GoBack()
    {
        CurrentStep = CurrentStep switch
        {
            SetupStep.Terminal => SetupStep.Server,
            SetupStep.Confirm => SetupStep.Terminal,
            _ => SetupStep.Server
        };
    }

    private record TerminalRegistration(Guid Id, Guid StoreId, string Name, string StoreName);
}
