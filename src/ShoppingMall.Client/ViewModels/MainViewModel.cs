using ShoppingMall.Client.Services;

namespace ShoppingMall.Client.ViewModels;

public enum AppView { Login, Pos, Inventory, Reports, Admin }

public class MainViewModel : BaseViewModel
{
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;

    private AppView _currentView = AppView.Login;
    public AppView CurrentView
    {
        get => _currentView;
        set
        {
            SetProperty(ref _currentView, value);
            OnPropertyChanged(nameof(IsPosView));
            OnPropertyChanged(nameof(IsLoginView));
        }
    }

    public bool IsPosView => CurrentView == AppView.Pos;
    public bool IsLoginView => CurrentView == AppView.Login;

    private string _userDisplayName = "";
    public string UserDisplayName
    {
        get => _userDisplayName;
        set => SetProperty(ref _userDisplayName, value);
    }

    private string _userRole = "";
    public string UserRole
    {
        get => _userRole;
        set => SetProperty(ref _userRole, value);
    }

    public PosViewModel PosVM { get; }

    public ICommand NavigateToPosCommand { get; }
    public ICommand NavigateToInventoryCommand { get; }
    public ICommand NavigateToReportsCommand { get; }
    public ICommand NavigateToAdminCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel(ApiClient api, AppConfiguration config, PosViewModel posVM)
    {
        _api = api;
        _config = config;
        PosVM = posVM;

        NavigateToPosCommand = new RelayCommand(_ => CurrentView = AppView.Pos);
        NavigateToInventoryCommand = new RelayCommand(_ => CurrentView = AppView.Inventory);
        NavigateToReportsCommand = new RelayCommand(_ => CurrentView = AppView.Reports);
        NavigateToAdminCommand = new RelayCommand(_ => CurrentView = AppView.Admin);
        LogoutCommand = new RelayCommand(async _ =>
        {
            _api.ClearSessionId();
            CurrentView = AppView.Login;
            await Task.CompletedTask;
        });
    }

    public void OnLoginSucceeded(LoginResult result)
    {
        UserDisplayName = result.DisplayName;
        UserRole = result.Role;
        CurrentView = AppView.Pos;
    }
}
