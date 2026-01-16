using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Splat;
using SS14.Launcher.Models.ServerStatus;
using SS14.Launcher.Localization;
using SS14.Launcher.Utility;

namespace SS14.Launcher.ViewModels.MainWindowTabs;

public class ClassicServerListTabViewModel : MainWindowTabViewModel
{
    private readonly ClassicServerListCache _cache;

    private readonly LocalizationManager _loc = LocalizationManager.Instance;

    public override string Name => _loc.GetString("tab-servers-classic-title"); 

    public ObservableCollection<ClassicServerEntryViewModel> AllServers { get; } = new();

    public ClassicServerListTabViewModel()
    {
        _cache = Locator.Current.GetRequiredService<ClassicServerListCache>();
        
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
        // Sort by Players descending
        var sorted = _cache.AllServers.OrderByDescending(s => s.PlayerCount).ToList();
        
        foreach (var s in sorted)
        {
            AllServers.Add(new ClassicServerEntryViewModel(s));
        }
    }

    public override async void Selected()
    {
        await _cache.Refresh();
    }
}
