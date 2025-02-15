using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using SS14.Launcher.Localization;
using SS14.Launcher.ViewModels;
using static SS14.Launcher.ViewModels.HubSettingsViewModel;

namespace SS14.Launcher.Views;

public partial class HubSettingsDialog : Window
{
    private readonly HubSettingsViewModel _viewModel;
    private readonly LocalizationManager _loc = LocalizationManager.Instance;

    public HubSettingsDialog()
    {
        InitializeComponent();

        _viewModel = (DataContext as HubSettingsViewModel)!; // Should have been set in XAML
        _viewModel.HubList.CollectionChanged += (_, _) => Verify();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        _viewModel.Populate();
        Verify(); // Just in case the settings are messed up somehow.
    }

    private void Done(object? sender, RoutedEventArgs args)
    {
        _viewModel.Save();
        Close();
    }

    private void Cancel(object? sender, RoutedEventArgs args) => Close();

    private void Reset(object? sender, RoutedEventArgs args)
    {
        _viewModel.Reset();
        if (!Verify())
        {
            // Directly delete all hubs from data, something's gone wrong with the database
            _viewModel.HardReset();
        }
    }

    private void HubTextChanged(object? sender, AvaloniaPropertyChangedEventArgs e) => Verify();

    private bool Verify()
    {
        var dupes = _viewModel.GetDupes();

        foreach (var t in Hubs.GetLogicalDescendants().OfType<TextBox>())
        {
            if (!IsValidHubUri(t.Text))
                t.Classes.Add("Invalid");
            else
                t.Classes.Remove("Invalid");

            if (dupes.Contains(NormalizeHubUri(t.Text ?? "")))
                t.Classes.Add("Duplicate");
            else
                t.Classes.Remove("Duplicate");
        }

        var allValid = _viewModel.HubList.All(h => IsValidHubUri(h.Address));
        var noDupes = !dupes.Any();

        DoneButton.IsEnabled = allValid && noDupes;

        if (!allValid)
            Warning.Text = _loc.GetString("hub-settings-warning-invalid");
        else if (!noDupes)
            Warning.Text = _loc.GetString("hub-settings-warning-duplicate");
        else
            Warning.Text = "";

        return noDupes;
    }
}
