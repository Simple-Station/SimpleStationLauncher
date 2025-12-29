# SimpleStationLauncher (Space Station: Beyond)

## [Direct Download (GitHub)](https://github.com/Simple-Station/SimpleStationLauncher/releases/latest) | [Steam (not yet public)](https://store.steampowered.com/app/3731580/Space_Station_Beyond/) | [Website](https://simplestation.org) | [Discord](https://discord.gg/49KeKwXc8g)

**Note:** Do not copy your launcher data from your Space Wizards launcher install, as it may make this launcher unable to launch due to incompatible data.

## Differences from the Space Wizards' launcher

- UI Scaling
- More default hubs
  - Default hubs are removable
- Supports multiple engines (RT, MV (though not MVAuth yet), and SME), and is really easy to add support for another if you want to make a PR
- Allows logging into multiple accounts
  - Supports multiple auth servers, also really easy to add support for extra default auth servers with a PR
- Displays server tags and allowed auth methods alongside the description
- Improvements to how favorites work
  - Can edit name/IP
    - Can fetch server name from the listing while editing
  - Can raise/lower servers instead of fumbling with "move to top" and its inconsistencies
- Supports launching from the web (`ss14`/`ss14s` protocols) on Windows and Linux, nobody uses Mac ;)
- A lot more (maybe) funny loading screen messages

## Development

Useful environment variables for development:
- `SS14_LAUNCHER_DATADIR=SimpleStation14` to change the directory the launcher stores all configs/logs/content in.
- `SS14_LAUNCHER_APPDATA_NAME=launcher` to change the directory within DATADIR that the launcher stores launcher configs and content in.
