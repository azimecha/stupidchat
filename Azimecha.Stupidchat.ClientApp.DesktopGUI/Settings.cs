using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    [DataContract]
    public class Settings {
        private static string _strFilePath = System.IO.Path.Combine(Program.DataFolder, "settings.json");
        private static Lazy<Settings> _instance = new Lazy<Settings>(LoadOrCreate, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        public static Settings Instance => _instance.Value;

        public Settings() {
            Servers = new List<KnownServer>();
        }

        public string Username {
            get => Profile.Username;
            set => ModifyProfile((ref Core.Structures.UserProfile p) => p.Username = value);
        }

        public string DisplayName {
            get => Profile.DisplayName;
            set => ModifyProfile((ref Core.Structures.UserProfile p) => p.DisplayName = value);
        }

        public string Bio {
            get => Profile.Bio;
            set => ModifyProfile((ref Core.Structures.UserProfile p) => p.Bio = value);
        }

        public string AvatarURL {
            get => Profile.AvatarURL;
            set => ModifyProfile((ref Core.Structures.UserProfile p) => p.AvatarURL = value);
        }

        public DateTime ProfileUpdateTime => new DateTime(Profile.UpdateTime);

        [DataMember] public Core.Structures.UserProfile Profile { get; private set; }
        [DataMember] public List<KnownServer> Servers { get; private set; }

        private delegate void ProfileModifierDelegate(ref Core.Structures.UserProfile prof);

        private void ModifyProfile(ProfileModifierDelegate procModifyProfile) {
            Core.Structures.UserProfile profCur = Profile;
            procModifyProfile(ref profCur);
            profCur.UpdateTime = DateTime.Now.Ticks;
            Profile = profCur;
            ProfileChanged?.Invoke(profCur);
        }

        public void Save() {
            System.IO.File.WriteAllText(_strFilePath, System.Text.Json.JsonSerializer.Serialize(this));
        }

        private static Settings LoadOrCreate() {
            Settings instance;

            if (System.IO.File.Exists(_strFilePath))
                instance = System.Text.Json.JsonSerializer.Deserialize<Settings>(System.IO.File.ReadAllText(_strFilePath));
            else
                instance = new Settings();

            instance.Save();
            return instance;
        }

        public event Action<Core.Structures.UserProfile> ProfileChanged;

        public static void Discard() {
            System.IO.File.Delete(_strFilePath);
            _instance = new Lazy<Settings>(LoadOrCreate, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }
    }

    [DataContract]
    public struct KnownServer {
        [DataMember] public string Address;
        [DataMember] public int Port;
        [DataMember] public byte[] PublicKey;
    }
}
