# 📦 Using `AgentsClientSDK` NuGet Package in a WinUI 3 App

This guide shows how to use the `AgentsClientSDK` NuGet package in your **WinUI 3 (.NET 8)** project by downloading it from GitHub Releases and setting it up manually via a local NuGet source.

---

## 🔗 Step 1: Download the NuGet Package

1. Go to the [Releases](https://github.com/microsoft/AgentsClientSDK.Windows/releases) section of this repository.
2. Download the `.nupkg` file (e.g., `AgentsClientSDK.1.0.0.nupkg`) from the latest release.

---

## 🗂️ Step 2: Create a Local NuGet Package Source

1. Create a folder on your machine to act as a local NuGet feed, e.g.:

```bash
mkdir C:\LocalNuGet
```

2. Move the downloaded `.nupkg` file into that folder.

3. Register the folder as a NuGet source:

```bash
dotnet nuget add source "C:\LocalNuGet" --name LocalGitHubRelease
```

ℹ️ You only need to do this once per machine.

---

## 🛠️ Step 3: Create or Open Your WinUI 3 App

If you don’t already have a WinUI 3 app, you can create one:

```bash
dotnet new winui3 -n MyWinUIApp
cd MyWinUIApp
```

---

## 📦 Step 4: Add the Package to Your Project

Run this in your project folder:

```bash
dotnet add package Microsoft.AgentsClientSDK --version 1.0.0
```

Or use Visual Studio:

- Right-click the project → Manage NuGet Packages
- Select the `LocalGitHubRelease` source
- Search for `Microsoft.AgentsClientSDK` and install it

---

## 💬 Step 5: Use the Package in Your Code

Example usage:

```csharp
using Com.Microsoft.AgentsClientSDK;
using Com.Microsoft.AgentsClientSDK.Config;
using Com.Microsoft.AgentsClientSDK.Services;

public sealed partial class MainWindow : Window
{
    private AgentsClientSDK _clientSDK;

    public MainWindow()
    {
        this.InitializeComponent();
        InitializeSDK();
    }

    private async void InitializeSDK()
    {
        _clientSDK = new AgentsClientSDK();

        var properties = new ClientSDKProperties
        {
            AgentId = "your-agent-id",
            DirectLineSecret = "your-directline-secret",
            EnableSpeech = true
        };

        await _clientSDK.StartConversationAsync(properties);
    }
}
```

---

## 🔧 Advanced Usage

```csharp
// Speech service integration
var speechService = ClientSDKServiceFactory.GetSpeechService();
await speechService.StartListeningAsync();

// Agent event handling
var eventNotifier = ClientSDKServiceFactory.GetAgentEventNotifierService();
eventNotifier.OnAgentActivity += (sender, activity) => {
    Console.WriteLine($"Agent: {activity.Text}");
};

// Logging integration
var logger = Com.Microsoft.AgentsClientSDK.Utils.Logger.Instance;
logger.Information("SDK initialized successfully");
```

---

## 🔍 Troubleshooting

### Common Issues

- **Package Not Found**: Check that the `.nupkg` file exists and the local source is registered via `dotnet nuget list source`
- **Build Errors**: Ensure the project targets `.NET 8` and uses `Microsoft.WindowsAppSDK`
- **Runtime Issues**: Validate DirectLine and Azure credentials

### Debug Configuration

```csharp
var config = new ClientSDKProperties
{
    LogLevel = LogLevel.Debug,
    EnableConsoleLogging = true
};

var isConnected = await _clientSDK.TestConnectionAsync();
if (!isConnected)
{
    // Handle connection issues
}
```