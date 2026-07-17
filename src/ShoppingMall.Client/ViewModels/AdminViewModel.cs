using System.Collections.ObjectModel;
using System.Windows.Input;
using ShoppingMall.Client.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.ViewModels;

public enum AdminTab { Users, Stores, Terminals, Config, Backups, Audit }

public class AdminViewModel : BaseViewModel
{
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;

    private AdminTab _selectedTab = AdminTab.Users;
    public AdminTab SelectedTab
    {
        get => _selectedTab;
        set
        {
            SetProperty(ref _selectedTab, value);
            OnPropertyChanged(nameof(IsUsersTab));
            OnPropertyChanged(nameof(IsStoresTab));
            OnPropertyChanged(nameof(IsTerminalsTab));
            OnPropertyChanged(nameof(IsConfigTab));
            OnPropertyChanged(nameof(IsBackupsTab));
            OnPropertyChanged(nameof(IsAuditTab));
            _ = LoadTabAsync();
        }
    }

    public bool IsUsersTab => SelectedTab == AdminTab.Users;
    public bool IsStoresTab => SelectedTab == AdminTab.Stores;
    public bool IsTerminalsTab => SelectedTab == AdminTab.Terminals;
    public bool IsConfigTab => SelectedTab == AdminTab.Config;
    public bool IsBackupsTab => SelectedTab == AdminTab.Backups;
    public bool IsAuditTab => SelectedTab == AdminTab.Audit;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string _statusText = "Ready";
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<Store> Stores { get; } = new();
    public ObservableCollection<Terminal> Terminals { get; } = new();
    public ObservableCollection<KeyValuePair<string, string>> ConfigEntries { get; } = new();
    public ObservableCollection<CloudBackup> Backups { get; } = new();
    public ObservableCollection<SyncLog> AuditLogs { get; } = new();

    public ICommand RefreshCommand { get; }
    public ICommand SelectUsersTabCommand { get; }
    public ICommand SelectStoresTabCommand { get; }
    public ICommand SelectTerminalsTabCommand { get; }
    public ICommand SelectConfigTabCommand { get; }
    public ICommand SelectBackupsTabCommand { get; }
    public ICommand SelectAuditTabCommand { get; }
    public ICommand TriggerBackupCommand { get; }

    public AdminViewModel(ApiClient api, AppConfiguration config)
    {
        _api = api;
        _config = config;

        RefreshCommand = new RelayCommand(async _ => await LoadTabAsync());
        SelectUsersTabCommand = new RelayCommand(_ => SelectedTab = AdminTab.Users);
        SelectStoresTabCommand = new RelayCommand(_ => SelectedTab = AdminTab.Stores);
        SelectTerminalsTabCommand = new RelayCommand(_ => SelectedTab = AdminTab.Terminals);
        SelectConfigTabCommand = new RelayCommand(_ => SelectedTab = AdminTab.Config);
        SelectBackupsTabCommand = new RelayCommand(_ => SelectedTab = AdminTab.Backups);
        SelectAuditTabCommand = new RelayCommand(_ => SelectedTab = AdminTab.Audit);
        TriggerBackupCommand = new RelayCommand(async _ => await TriggerBackupAsync());
    }

    public async Task LoadTabAsync()
    {
        IsLoading = true;
        StatusText = "Loading...";
        try
        {
            var cfg = _config.Load();

            switch (SelectedTab)
            {
                case AdminTab.Users:
                    await LoadUsersAsync();
                    break;
                case AdminTab.Stores:
                    await LoadStoresAsync();
                    break;
                case AdminTab.Terminals:
                    await LoadTerminalsAsync(cfg.StoreId);
                    break;
                case AdminTab.Config:
                    await LoadConfigAsync(cfg.StoreId);
                    break;
                case AdminTab.Backups:
                    await LoadBackupsAsync(cfg.StoreId);
                    break;
                case AdminTab.Audit:
                    await LoadAuditLogAsync();
                    break;
            }

            StatusText = "Loaded";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    private async Task LoadUsersAsync()
    {
        var users = await _api.HttpGetAsync<List<User>>("/api/admin/users");
        Users.Clear();
        if (users != null)
            foreach (var u in users) Users.Add(u);
    }

    private async Task LoadStoresAsync()
    {
        var stores = await _api.HttpGetAsync<List<Store>>("/api/admin/stores");
        Stores.Clear();
        if (stores != null)
            foreach (var s in stores) Stores.Add(s);
    }

    private async Task LoadTerminalsAsync(Guid storeId)
    {
        var terminals = await _api.HttpGetAsync<List<Terminal>>($"/api/admin/terminals/{storeId}");
        Terminals.Clear();
        if (terminals != null)
            foreach (var t in terminals) Terminals.Add(t);
    }

    private async Task LoadConfigAsync(Guid storeId)
    {
        var config = await _api.HttpGetAsync<Dictionary<string, string>>($"/api/admin/config/{storeId}");
        ConfigEntries.Clear();
        if (config != null)
            foreach (var kvp in config) ConfigEntries.Add(kvp);
    }

    private async Task LoadBackupsAsync(Guid storeId)
    {
        var backups = await _api.HttpGetAsync<List<CloudBackup>>($"/api/admin/backups/{storeId}");
        Backups.Clear();
        if (backups != null)
            foreach (var b in backups) Backups.Add(b);
    }

    private async Task LoadAuditLogAsync()
    {
        var logs = await _api.HttpGetAsync<List<SyncLog>>("/api/admin/audit-log");
        AuditLogs.Clear();
        if (logs != null)
            foreach (var l in logs) AuditLogs.Add(l);
    }

    private async Task TriggerBackupAsync()
    {
        IsLoading = true;
        try
        {
            var cfg = _config.Load();
            var result = await _api.HttpPostAsync<CloudBackup>($"/api/admin/backups/{cfg.StoreId}", new { });
            StatusText = result != null ? $"Backup created: {result.FileName}" : "Backup failed";
            await LoadBackupsAsync(cfg.StoreId);
        }
        catch (Exception ex)
        {
            StatusText = $"Backup error: {ex.Message}";
        }
        finally { IsLoading = false; }
    }
}
