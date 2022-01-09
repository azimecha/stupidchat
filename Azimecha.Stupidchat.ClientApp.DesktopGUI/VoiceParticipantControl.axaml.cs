using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Client;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class VoiceParticipantControl : UserControl {
        public static readonly DirectProperty<VoiceParticipantControl, IMember> MemberProperty =
            AvaloniaProperty.RegisterDirect<VoiceParticipantControl, IMember>(nameof(Member), w => w.Member, (w, v) => w.Member = v);

        private Border _ctlBorder;

        public VoiceParticipantControl() {
            InitializeComponent();

            _ctlBorder = this.Find<Border>("ControlBorder");
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private IMember _memb;
        public IMember Member {
            get => _memb;
            set => SetAndRaise(MemberProperty, ref _memb, value);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if (change.IsEffectiveValueChange && (change.Property == MemberProperty))
                OnMemberChanged();

            base.OnPropertyChanged(change);
        }

        private void OnMemberChanged() {
            _ctlBorder.Background = (_memb is null) ? Avalonia.Media.Brushes.Transparent
                : new Avalonia.Media.SolidColorBrush(new Avalonia.Media.Color(255, _memb.Info.PublicKey[0], _memb.Info.PublicKey[1], _memb.Info.PublicKey[2]));
        }
    }
}
