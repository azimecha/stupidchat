using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public class Settings {
        private static string _strFilePath = System.IO.Path.Combine(Program.DataFolder, "settings.json");
        private static Lazy<Settings> _instance = new Lazy<Settings>(LoadOrCreate, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        public static Settings Instance => _instance.Value;

        public Settings() {
            _data = new SettingsData() { Servers = new List<KnownServer>() };
        }

        [Newtonsoft.Json.JsonIgnore]
        public string Username {
            get => Profile.Username;
            set {
                if (value is null) throw new ArgumentNullException(nameof(value));
                ModifyProfile((ref Core.Structures.UserProfile p) => p.Username = value);
            }
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

        public Core.Structures.UserProfile Profile => _data.Profile;
        public List<KnownServer> Servers => _data.Servers;

        [DataContract]
        private struct SettingsData {
            [DataMember] public Core.Structures.UserProfile Profile;
            [DataMember] public List<KnownServer> Servers;
        }

        private SettingsData _data;

        private delegate void ProfileModifierDelegate(ref Core.Structures.UserProfile prof);

        private void ModifyProfile(ProfileModifierDelegate procModifyProfile) {
            Core.Structures.UserProfile profCur = Profile;
            procModifyProfile(ref profCur);
            profCur.UpdateTime = DateTime.Now.Ticks;
            _data.Profile = profCur;
            ProfileChanged?.Invoke(profCur);
        }

        public void Save() {
            System.IO.File.WriteAllText(_strFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }

        private static Settings LoadOrCreate() {
            Settings instance = new Settings();

            try {
                if (System.IO.File.Exists(_strFilePath))
                    instance._data = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsData>(System.IO.File.ReadAllText(_strFilePath));
            } catch (Exception ex) {
                Debug.WriteLine($"[{nameof(Settings)}/{nameof(LoadOrCreate)}] Error loading settings: {ex}");
            }

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
