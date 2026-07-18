using ShoppingMall.Client.Services;

namespace ShoppingMall.Client.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;

    private string _username = "";
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _pin = "";
    public string Pin
    {
        get => _pin;
        set => SetProperty(ref _pin, value);
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

    public ICommand LoginCommand { get; }
    public event EventHandler<LoginResult>? LoginSucceeded;

    public LoginViewModel(ApiClient api, AppConfiguration config)
    {
        _api = api;
        _config = config;
        LoginCommand = new AsyncRelayCommand(async _ => await LoginAsync());
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Pin))
        {
            ErrorMessage = "Enter username and PIN";
            return;
        }

        IsLoading = true;
        ErrorMessage = "";

        try
        {
            var config = _config.Load();
            var result = await _api.LoginAsync(Username, Pin, config.TerminalId);
            if (result == null)
            {
                ErrorMessage = "Invalid credentials";
                return;
            }
            LoginSucceeded?.Invoke(this, result);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Connection error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
