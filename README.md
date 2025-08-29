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

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the
instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted
the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see
the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or
comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of
Microsoft
trademarks or logos is subject to and must follow
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion
or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
