using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Azimecha.Stupidchat.Client;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class MessageControl : UserControl {
        public static readonly DirectProperty<MessageControl, IMessage> MessageProperty =
            AvaloniaProperty.RegisterDirect<MessageControl, IMessage>(nameof(Message), w => w.Message, (w, v) => w.Message = v);

        public static readonly DirectProperty<MessageControl, string> MessageTextProperty =
            AvaloniaProperty.RegisterDirect<MessageControl, string>(nameof(MessageText), w => w.MessageText);

        private Border _ctlAvatarBorder;
        private AvatarControl _ctlAvatar;

        public MessageControl() {
            InitializeComponent();

            _ctlAvatarBorder = this.FindControl<Border>("AvatarBorder");
            _ctlAvatar = new AvatarControl() { Clickable = true };
            _ctlAvatarBorder.Child = _ctlAvatar;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private IMessage _message;
        public IMessage Message {
            get => _message;
            set => SetAndRaise(MessageProperty, ref _message, value);
        }

        private string _strBodyText;
        public string MessageText {
            get => _strBodyText;
            private set => SetAndRaise(MessageTextProperty, ref _strBodyText, value);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if ((change.Property == MessageProperty) && change.IsEffectiveValueChange)
                OnMessageChanged(change.OldValue.HasValue ? (IMessage)change.OldValue.Value : null);

            base.OnPropertyChanged(change);
        }

        private void OnMessageChanged(IMessage msgOld) {
            MessageText = (_message is null) ? "" : _message.SignedData.Text;
            _ctlAvatar.User = _message?.Sender?.User;
        }
    }
}
