## AgentsClient SDK

The AgentsClientSDK for Windows is available as a NuGet package and its release repository can be found here.
This repository contains the source code for the SampleApp, which demonstrates how to use the SDK in a WinUI 3 application.

# SampleApp

SampleApp is a Windows desktop application built with WinUI 3 (.NET 8) that demonstrates how to integrate multimodal conversational AI using the [MultimodalClientSDK]. The app provides a chat interface supporting both text and speech interactions with an AI agent, leveraging Microsoft Cognitive Services and Adaptive Cards for rich, interactive conversations.

## Features

- **Conversational UI**: Chat with an AI agent using text or speech.
- **Speech Recognition**: Optional speech input/output powered by Microsoft Cognitive Services Speech SDK.
- **Adaptive Cards**: Rich message rendering using Adaptive Cards for WinUI 3.
- **Agent Configuration**: Prompt-based configuration for agent schema and environment at first launch.
- **Persistent Settings**: User configuration is saved locally for future sessions.
- **Modern Windows Experience**: Built with WinUI 3 and packaged as an MSIX for Windows 10/11.

## Getting Started

### Prerequisites

- Windows 10 version 19041 (20H1) or later
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 with the **Desktop development with C++** and **.NET Desktop Development** workloads

### Build and Run

1. **Clone the repository** and open `SampleApp.sln` in Visual Studio 2022.
2. **Restore NuGet packages** (should happen automatically).
3. If you are running the app for the first time/using different agent settings, you will need to configure your agent settings. Follow the instructions in the **First Launch Configuration** section below.
4. **Build the solution**.
5. **Run the app** (F5 or Ctrl+F5).

### First Launch Configuration

The appsettings.json  file provides the necessary environment, agent, and speech settings required by the
SDK to connect and function correctly. The file should look like this:
{
  "user": {
    "environmentId": "", //environment in which agent is created
    "schemaName": "", // schema name of agent. Both are available in agent Metadata
    "environment": "", // mapping given below
    "isAuthEnabled": false, // remains false for this release
    "auth": {               // furure scope. No need to input anything for now
      "clientId": "",
      "tenantId": "",
      "redirectUri": ""
    }
  },
  "speech": {               // furure scope. No need to input anything for now
    "speechSubscriptionKey": "",
    "speechServiceRegion": ""
  }
}
 
Environment mapping:
 
copilotstudio.microsoft.com -> prod
copilotstudio.preview.microsoft.com -> prod
copilotstudio.preprod.microsoft.com -> preprod

These are required to connect to your AI agent backend. The configuration is saved locally in `SampleApp\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\appsettings.json`.

### Enabling Speech

To enable speech features, set the `EnableSpeech` property to `true` in the configuration dialog or in the `config.json` file. You will also need to provide valid `SpeechApiKey` and `SpeechRegion` values.

## Project Structure

- `App.xaml.cs` – Application entry point.
- `main/MainWindow.xaml.cs` – Main window logic, configuration prompts, and chat view loading.
- `Controls/ChatView.xaml.cs` – Chat UI logic, message handling, and speech integration.
- `configurations/ConfigManager.cs` – Configuration management and persistence.
- `Converters/` – UI value converters for chat message rendering.

## Dependencies

- [Microsoft.WindowsAppSDK](https://www.nuget.org/packages/Microsoft.WindowsAppSDK)
- [AdaptiveCards.ObjectModel.WinUI3](https://www.nuget.org/packages/AdaptiveCards.ObjectModel.WinUI3)
- [AdaptiveCards.Rendering.WinUI3](https://www.nuget.org/packages/AdaptiveCards.Rendering.WinUI3)
- [Microsoft.CognitiveServices.Speech](https://www.nuget.org/packages/Microsoft.CognitiveServices.Speech)
- [Microsoft.Bot.Connector.DirectLine](https://www.nuget.org/packages/Microsoft.Bot.Connector.DirectLine)
- [MultimodalClientSDK](https://www.nuget.org/packages/MultimodalClientSDK)
- [NAudio](https://www.nuget.org/packages/NAudio)
- Other supporting libraries (see `.csproj` for full list)

## Customization

- **Agent Integration**: Update the agent configuration in `config.json` or via the app prompt.
- **Speech Settings**: Provide your own Azure Speech API key and region.
- **Branding**: Replace assets in the `Assets/` folder for custom logos and backgrounds.

## License

This project is provided as a sample and is subject to the license terms in the repository.

---

*For more information on WinUI 3, see [WinUI documentation](https://learn.microsoft.com/windows/apps/winui/). For details on Adaptive Cards, visit [adaptivecards.io](https://adaptivecards.io/).*