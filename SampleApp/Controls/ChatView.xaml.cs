using AdaptiveCards.Rendering.WinUI3;
using Microsoft.Agents.Client.Wpf;
using Microsoft.Agents.Client.Wpf.Models;
using Microsoft.Agents.Client.Wpf.Services;
using Microsoft.Agents.Client.Wpf.Services.Speech;
using Microsoft.Agents.Client.Wpf.Utils;
using Microsoft.Agents.Client.Wpf.Utils.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp.Controls
{
    public sealed partial class ChatView : UserControl
    {
        public ObservableCollection<AgentActivity> Messages { get; } = new();
        public MainWindow mainWindow { get; private set; }= MainWindow.Current;

        private SpeechService speechService;
        //private bool controlsVisible = true;
        private static CancellationTokenSource speechCancellationToken = new CancellationTokenSource();
        private bool _isMicrophoneActive = true;
        private SolidColorBrush _micActiveBackground = new SolidColorBrush(Microsoft.UI.Colors.SkyBlue);
        private SolidColorBrush _micInactiveBackground = new SolidColorBrush(Microsoft.UI.Colors.Red);
        private SolidColorBrush _defaultChatMessageBackground = new SolidColorBrush(Microsoft.UI.Colors.LightGray);
        private SolidColorBrush _agentChatMessageBackground = new SolidColorBrush(Microsoft.UI.Colors.DeepSkyBlue);
        private bool _isMuted;
        private bool _isTextModeActive = true;

        AdaptiveCardRenderer renderer;
        private readonly string _tokenUrl;
        private bool _isSpeechModeActive = false;
        private AgentsClientSDK _clientSDK;
        private AgentActivity? _agentWelcomeMessage;

        public ChatView(AppConfig config, AgentsClientSDK clientSDK, AgentActivity agentWelcomeMessage )
        {
            this.InitializeComponent();
            _tokenUrl = config.AppSettings.GetDirectLineTokenEndpoint();
            if (_tokenUrl == null)
            {
                throw new ArgumentNullException(nameof(config.TokenUrl), "TokenUrl cannot be null.");
            }
            _isSpeechModeActive = config.EnableSpeech;
            _clientSDK = clientSDK;
            _agentWelcomeMessage = agentWelcomeMessage;
            this.Loaded += ChatView_Loaded;
        }

        private async void ChatView_Loaded(object sender, RoutedEventArgs e)
        {
            // Use _tokenUrl to connect to backend, load messages, etc.
            Logger.LogMessage($"Chat connected with token URL: {_tokenUrl}");
            if (_isSpeechModeActive)
            {
                // Initialize speech service if enabled
                InitializeSpeech();
            }
            await InitializeUI();
            mainWindow.HideProgressBar();
        }

        private void InitializeSpeech()
        {
            if(_clientSDK.GetSpeechService() == null)
            {
                Logger.LogMessage("Speech service is not available. Please check your configuration.");
                return;
            }
            speechService = _clientSDK.GetSpeechService() as SpeechService;
            speechService.OnSpeechRecognized += OnSpeechRecognized;
            //speechService.OnAgentResponse += OnAgentMessageReceived;

            //await speechService.InitializeSpeechAsync(); // Ensure recognizer and synthesizer are ready
            //await speechService.StartListeningAsync();
        }

        private void OnSpeechRecognized(string text)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                MessageTextBox.Text = text;
                var chatMessage = new AgentActivity(text, MessageRole.User);
                AddMessage(chatMessage, _defaultChatMessageBackground);
            });
        }

        private async Task InitializeUI()
        {
            // Set focus to the MessageTextBox by default
            MessageTextBox.Loaded += (s, e) => MessageTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);

            InitializeEventHandlers();
            if (_agentWelcomeMessage != null)
            {
                AddMessage(_agentWelcomeMessage, _agentChatMessageBackground);
                Logger.LogMessage($"Agent response: {_agentWelcomeMessage.Text}");
            }
            else
            {
                Logger.LogMessage("No welcome message available.");
            }
        }

        private void InitializeEventHandlers()
        { 
            MessageTextBox.KeyDown += MessageTextBox_KeyDown;

            // Subscribe to SDK events
            AgentEventNotifierService.AgentTyping += OnAgentTyping;
            AgentEventNotifierService.AgentMessageReceived += OnAgentMessageReceived;
        }

        private void MessageTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            MessageTextBox.KeyDown += (sender, e) =>
            
            {
                if (e.Key == Windows.System.VirtualKey.Enter && !string.IsNullOrWhiteSpace(MessageTextBox.Text))
                {
                    SendButton_Click(sender, e);
                }
            };
        }

        private AgentActivity? _typingBubble = null; // Initialize as null to avoid null dereference

        public void ShowTypingBubble()
        {
            if (_typingBubble == null) // Check if _typingBubble is null
            {
                _typingBubble = new AgentActivity(string.Empty, MessageRole.Agent); // Create a new instance
                _typingBubble.IsAgentTyping = true;

                AddMessage(_typingBubble, _agentChatMessageBackground);
            }
        }

        public void HideTypingBubble()
        {
            if (_typingBubble != null) // Check if _typingBubble is not null
            {
                Messages.Remove(_typingBubble);
                _typingBubble = null; // Set to null after removing
            }
        }
        private void OnAgentTyping()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                ShowTypingBubble();
            });
        }

        public void OnAgentMessageReceived(AgentActivity msg)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                HideTypingBubble();

                AddMessage(msg, _agentChatMessageBackground);
            });
        }

        private async void SwitchToSpeechMode_Click(object sender, RoutedEventArgs e)
        {
            // Hide text input controls
            TextInputControlsContainer.Visibility = Visibility.Collapsed;
            MessageListView.Visibility = Visibility.Collapsed;
            _isTextModeActive = false;

            // Show speech controls
            SpeechControlsPanel.Visibility = Visibility.Visible;
            await speechService.StartContinuousListeningAsync();

        }

        private async void SwitchToTextMode_Click(object sender, RoutedEventArgs e)
        {
            // Hide speech controls
            SpeechControlsPanel.Visibility = Visibility.Collapsed;
            if (!_isTextModeActive)
            {
                await speechService.StopContinuousListeningAsync();
            }

            // Show text input controls
            TextInputControlsContainer.Visibility = Visibility.Visible;
            MessageListView.Visibility = Visibility.Visible;
            _isTextModeActive = true;
            MessageTextBox.Text = string.Empty;

        }

        private async void MicrophoneButton_Click(object sender, RoutedEventArgs e)
        {
            _isMicrophoneActive = !_isMicrophoneActive;
            MicrophoneButton.Background = _isMicrophoneActive ? _micActiveBackground : _micInactiveBackground;

            Logger.LogMessage($"Microphone {(_isMicrophoneActive ? "activated" : "deactivated")}");

            if (_isMicrophoneActive && !speechService.IsListening)
            {
                await speechService.StartContinuousListeningAsync();
            }
            else
            {
                await speechService.StopContinuousListeningAsync();
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
                {
                    AgentActivity userMessage = new AgentActivity(MessageTextBox.Text, MessageRole.User);

                    MessageTextBox.Text = string.Empty;
                    AddMessage(userMessage, _defaultChatMessageBackground);
                    if (!_isTextModeActive)
                    { 
                        
                        await speechService.HandleUserSpeech(userMessage);
                    }
                    else
                    {
                        _clientSDK.SendMessageAsync(userMessage.Text);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
            }
        }

        public void AddMessage(AgentActivity chatMessage, Brush? background = null)
        {
            chatMessage.Text = chatMessage.Text != null ? ChatMessageHelper.CapitalizeFirstLetter(chatMessage.Text) : string.Empty;

            Messages.Add(chatMessage);
            MessageListView.UpdateLayout();
            MessageListView.ScrollIntoView(Messages[^1]);
        }
    }
}
