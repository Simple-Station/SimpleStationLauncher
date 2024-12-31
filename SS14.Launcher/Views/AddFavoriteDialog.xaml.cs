using System;
using Avalonia.Controls;
using ReactiveUI;
using SS14.Launcher.Models.ServerStatus;

namespace SS14.Launcher.Views;

public partial class AddFavoriteDialog : Window
{
    private readonly TextBox _nameBox;
    private readonly TextBox _addressBox;

    public AddFavoriteDialog(string name = "", string address = "")
    {
        InitializeComponent();

        _nameBox = NameBox;
        _nameBox.Text = name;
        _addressBox = AddressBox;
        _addressBox.Text = address;

        FetchButton.Command = ReactiveCommand.Create(async () =>
        {
            var addr = _addressBox.Text;
            if (DirectConnectDialog.IsAddressValid(addr))
            {
                var cache = new ServerStatusCache();
                await cache.InitialUpdateStatus(cache.GetStatusFor(addr));
                if (cache.GetStatusFor(addr).Name != null)
                    _nameBox.Text = cache.GetStatusFor(addr).Name;
                else
                    TxtInvalid.IsVisible = true;
            }
            else
                TxtInvalid.IsVisible = true;
        });
        SubmitButton.Command = ReactiveCommand.Create(TrySubmit);

        this.WhenAnyValue(x => x._nameBox.Text)
            .Subscribe(_ => UpdateSubmitValid());

        this.WhenAnyValue(x => x._addressBox.Text)
            .Subscribe(_ => UpdateSubmitValid());
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        _nameBox.Focus();
    }

    private void TrySubmit()
    {
        if (_nameBox.Text?.Trim() is not { Length: > 0 } name
            || _addressBox.Text?.Trim() is not { Length: > 0 } address)
            return;

        Close((name, address));
    }

    private void UpdateSubmitValid()
    {
        var validAddr = DirectConnectDialog.IsAddressValid(_addressBox.Text);
        var valid = validAddr && !string.IsNullOrEmpty(_nameBox.Text);

        SubmitButton.IsEnabled = valid;
        TxtInvalid.IsVisible = !validAddr;
    }
}
