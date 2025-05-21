# SimpleStationLauncher (Space Station: Beyond)

The SimpleStation launcher used to connect to SS14 servers and manage their info.

Differences from the Space Wizards' launcher:
- UI Scaling
- More default hubs
  - Default hubs are removable
- Supports multiple engines (RT, MV (though not MVAuth yet), and SME), and is really easy to add support for another if you want to make a PR
- Allows logging into multiple accounts
  - Supports multiple auth servers, also really easy to add support for extra default auth servers with a PR
- Improvements to how favorites work
  - Can edit name/IP
    - Can fetch server name from the listing while editing
  - Can raise/lower servers instead of fumbling with "move to top" and its inconsistencies
- Supports launching from the web (`ss14`/`ss14s` protocols) on Windows and Linux, nobody uses Mac ;)
- A lot more funny loading screen messages

# Development

Useful environment variables for development:
- `SS14_LAUNCHER_DATADIR=SimpleStation14` to change the directory the launcher stores all data in. This can be useful to avoid breaking your "normal" SS14 launcher data while developing something.
- `SS14_LAUNCHER_APPDATA_NAME=launcher` to change the user data directories the launcher stores its data in. This can be useful to avoid breaking your "normal" SS14 launcher data while developing something.
