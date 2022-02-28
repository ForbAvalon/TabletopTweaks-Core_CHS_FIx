﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using TabletopTweaks.Core.Localization;
using static TabletopTweaks.Core.Main;
using static UnityModManagerNet.UnityModManager;

namespace TabletopTweaks.Core.Config {
    [Obsolete("Needs to be replaced with instance version")]
    static class ModSettings {
        public static ModEntry ModEntry;
        public static Fixes Fixes;
        public static AddedContent AddedContent;
        public static Homebrew Homebrew;
        public static Blueprints Blueprints;
        public static MultiLocalizationPack ModLocalizationPack = new MultiLocalizationPack();
        private static string userConfigFolder => ModEntry.Path + "UserSettings";
        private static string localizationFolder => ModEntry.Path + "Localization";
        private static JsonSerializerSettings cachedSettings;
        private static JsonSerializerSettings SerializerSettings {
            get {
                if (cachedSettings == null) {
                    cachedSettings = new JsonSerializerSettings {
                        CheckAdditionalContent = false,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        FloatParseHandling = FloatParseHandling.Double,
                        Formatting = Formatting.Indented,
                        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        StringEscapeHandling = StringEscapeHandling.Default,
                    };
                }
                return cachedSettings;
            }
        }

        public static void LoadAllSettings() {
            LoadSettings("Fixes.json", ref Fixes);
            LoadSettings("AddedContent.json", ref AddedContent);
            LoadSettings("Homebrew.json", ref Homebrew);
            LoadSettings("Blueprints.json", ref Blueprints);
            LoadLocalization();
        }
        public static void LoadLocalization() {
            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            var fileName = "LocalizationPack.json";
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"TabletopTweaks.Core.Localization.{fileName}"; ;
            var localizationPath = $"{localizationFolder}{Path.DirectorySeparatorChar}{fileName}";
            Directory.CreateDirectory(localizationFolder);
            if (File.Exists(localizationPath)) {
                using (StreamReader streamReader = File.OpenText(localizationPath))
                using (JsonReader jsonReader = new JsonTextReader(streamReader)) {
                    try {
                        MultiLocalizationPack localization = serializer.Deserialize<MultiLocalizationPack>(jsonReader);
                        ModLocalizationPack = localization;
                    } catch {
                        ModLocalizationPack = new MultiLocalizationPack();
                        TTTContext.Logger.LogError("Failed to localization. Settings will be rebuilt.");
                        try { File.Copy(localizationPath, ModEntry.Path + $"{Path.DirectorySeparatorChar}BROKEN_{fileName}", true); } catch { TTTContext.Logger.LogError("Failed to archive broken localization."); }
                    }
                }
            } else {
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                using (StreamReader streamReader = new StreamReader(stream))
                using (JsonReader jsonReader = new JsonTextReader(streamReader)) {
                    ModLocalizationPack = serializer.Deserialize<MultiLocalizationPack>(jsonReader);
                }
            }
        }
        public static void SaveLocalization(string fileName, MultiLocalizationPack localizaiton) {
            localizaiton.Strings.Sort((x, y) => string.Compare(x.SimpleName, y.SimpleName));
            Directory.CreateDirectory(userConfigFolder);
            var localizationPath = $"{localizationFolder}{Path.DirectorySeparatorChar}{fileName}";

            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            using (StreamWriter streamWriter = new StreamWriter(localizationPath))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter)) {
                serializer.Serialize(jsonWriter, localizaiton);
            }
            TTTContext.Logger.Log($"Localization: {ModLocalizationPack.Strings.Count}");
        }
        private static void LoadSettings<T>(string fileName, ref T setting) where T : IUpdatableSettings {
            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"TabletopTweaks.Core.Config.{fileName}";
            var userPath = $"{userConfigFolder}{Path.DirectorySeparatorChar}{fileName}";

            Directory.CreateDirectory(userConfigFolder);
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader)) {
                setting = serializer.Deserialize<T>(jsonReader);
                setting.Init();
            }
            if (File.Exists(userPath)) {
                using (StreamReader streamReader = File.OpenText(userPath))
                using (JsonReader jsonReader = new JsonTextReader(streamReader)) {
                    try {
                        T userSettings = serializer.Deserialize<T>(jsonReader);
                        setting.OverrideSettings(userSettings);
                    } catch {
                        TTTContext.Logger.LogError("Failed to load user settings. Settings will be rebuilt.");
                        try { File.Copy(userPath, userConfigFolder + $"{Path.DirectorySeparatorChar}BROKEN_{fileName}", true); } catch { TTTContext.Logger.LogError("Failed to archive broken settings."); }
                    }
                }
            }
            SaveSettings(fileName, setting);
        }

        public static void SaveSettings(string fileName, object setting) {
            Directory.CreateDirectory(userConfigFolder);
            var userPath = $"{userConfigFolder}{Path.DirectorySeparatorChar}{fileName}";

            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            using (StreamWriter streamWriter = new StreamWriter(userPath))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter)) {
                serializer.Serialize(jsonWriter, setting);
            }
        }
    }
}
