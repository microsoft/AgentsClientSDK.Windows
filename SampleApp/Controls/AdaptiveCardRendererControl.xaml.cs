using AdaptiveCards.ObjectModel.WinUI3;
using AdaptiveCards.Rendering.WinUI3;
using Com.Microsoft.AgentsClientSDK.Services.Protocol;
using Com.Microsoft.AgentsClientSDK.Utils.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace SampleApp.Controls
{
    public sealed partial class AdaptiveCardRendererControl : UserControl
    {

        private static RenderedAdaptiveCard? renderedAdaptiveCard;

        public AdaptiveCardRendererControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty AdaptiveCardJsonProperty =
            DependencyProperty.Register(nameof(AdaptiveCardJson), typeof(string), typeof(AdaptiveCardRendererControl),
                new PropertyMetadata(null, OnAdaptiveCardJsonChanged));

        public string AdaptiveCardJson
        {
            get => (string)GetValue(AdaptiveCardJsonProperty);
            set => SetValue(AdaptiveCardJsonProperty, value);
        }

        private static void OnAdaptiveCardJsonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AdaptiveCardRendererControl)d;
            control.RenderCard((string)e.NewValue);
        }

        private void RenderCard(string json)
        {
            RootGrid.Children.Clear();

            if (string.IsNullOrWhiteSpace(json)) return;

            try
            {
                var parseResult = AdaptiveCard.FromJsonString(json);
                var renderer = new AdaptiveCardRenderer();
                renderedAdaptiveCard = renderer.RenderAdaptiveCard(parseResult.AdaptiveCard);

                if (renderedAdaptiveCard.FrameworkElement != null)
                {
                    // Attach the event handler for action click events
                    renderedAdaptiveCard.Action += RenderedAdaptiveCard_Action;
                    RootGrid.Children.Add(renderedAdaptiveCard.FrameworkElement);
                }
            }
            catch (Exception ex)
            {
                // Log or handle rendering errors
                var errorText = new TextBlock { Text = $"Invalid AdaptiveCard: {ex.Message}" };
                RootGrid.Children.Add(errorText);
            }
        }

        private async void RenderedAdaptiveCard_Action(RenderedAdaptiveCard sender, AdaptiveActionEventArgs args)
        {
            IAgentCommunicationService clientSDKMainService = MainWindow.Current.ClientSDK.GetMainService();
            await AdaptiveCardHelper.HandleAdaptiveCardActionAsync(clientSDKMainService, sender, args);
        }
    }
}
