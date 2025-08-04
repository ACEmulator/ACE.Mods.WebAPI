# ACE.Mods.WebAPI

A RESTful API for ACEmulator servers

## Features

- **Real-time Player Tracking**: Monitors all online players and their positions
- **Map Visualization Integration**: Exports player data for visualization with [DerethMaps](https://github.com/Thwargle/DerethMaps) and [Asheron's Atlas](https://github.com/AsheronDB/asheronsatlas)
- **Configurable**: Customizable update intervals, output paths, and logging

## Installation

1. **Prerequisites**
   - .NET 8.0 runtime
   - [ACEmulator](https://github.com/ACEmulator/ACE) server installation

2. **Installation Steps**
   - Download the latest release zip file from the releases page
   - Extract the zip file to your ACEmulator server's `Mods` directory
   - Ensure the extracted folder is named `ACE.Mods.WebAPI`
   - Ensure the plugin is enabled in your ACEmulator server configuration
   - Restart your ACEmulator server or reload mods using `/mod f` command from console or in-game.



## Configuration

Edit `Settings.json` to customize the plugin behavior:

```json
{

}
```

### Configuration Options





## Troubleshooting

**Plugin not loading:**
- Check that the plugin is in the correct Mods directory
- Verify .NET 8.0 is installed
- Check server logs for error messages

**Settings not loading:**
- Ensure Settings.json is properly formatted
- Check for JSON syntax errors
- Verify file permissions

## Dependencies

- .NET 8.0 Runtime
- [ACEmulator](https://github.com/ACEmulator/ACE) Server (with mod support)
- Lib.Harmony bundled with ACEmulator (for patching)
- ACE.Shared library bundled with this plugin
- GenHTTP libraries bundled with this plugin
