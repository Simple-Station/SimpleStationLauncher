using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Splat;
using ReactiveUI;
using SS14.Launcher.Models.ServerStatus;
using SS14.Launcher.Localization;
using SS14.Launcher.Utility;

namespace SS14.Launcher.ViewModels.MainWindowTabs;

public class ClassicServerListTabViewModel : MainWindowTabViewModel
{
    private readonly MainWindowViewModel _mainWindow;
    private readonly ClassicServerListCache _cache;
    private readonly SS14.Launcher.Models.Data.DataManager _cfg;

    private readonly LocalizationManager _loc = LocalizationManager.Instance;

    public override string Name => _loc.GetString("tab-servers-byond-title");

    private string? _searchString;

    public string? SearchString
    {
        get => _searchString;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchString, value);
            UpdateList();
        }
    }

    public ObservableCollection<ClassicServerEntryViewModel> AllServers { get; } = new();
    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> RefreshPressed { get; }

    public ClassicServerListTabViewModel(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
        _cache = Locator.Current.GetRequiredService<ClassicServerListCache>();
        _cfg = Locator.Current.GetRequiredService<SS14.Launcher.Models.Data.DataManager>();
        RefreshPressed = ReactiveCommand.CreateFromTask(_cache.Refresh);

        // Initial populate if any
        UpdateList();

        ((INotifyCollectionChanged)_cache.AllServers).CollectionChanged += OnServersChanged;
    }

    private void OnServersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateList();
    }

    private void UpdateList()
    {
        AllServers.Clear();
        // Filter then Sort by Players descending
        var filtered = _cache.AllServers.Where(DoesSearchMatch);
        var sorted = filtered.OrderByDescending(s => s.PlayerCount).ToList();

        foreach (var s in sorted)
        {
            AllServers.Add(new ClassicServerEntryViewModel(_mainWindow, s, null, _cfg));
        }
    }

    private bool DoesSearchMatch(ClassicServerStatusData data)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        return data.Name.Contains(_searchString, System.StringComparison.CurrentCultureIgnoreCase);
    }

    public override async void Selected()
    {
        await _cache.Refresh();
    }
}
