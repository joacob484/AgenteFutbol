using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using AF.Core;

namespace AF.Services.Save
{
    public static class SaveService
    {
        // Siempre obtener el path en el hilo principal ANTES de ir a Task.Run
        static string GetPath(string slot)
        {
            var dir = Application.persistentDataPath; // main thread
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"{slot}.json");
        }

        public static Task<bool> ExistsAsync(string slot)
        {
            var path = GetPath(slot); // main thread
            return Task.Run(() => File.Exists(path));
        }

        public static Task SaveAsync(string slot, SaveData data)
        {
            var path = GetPath(slot); // main thread
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            return Task.Run(() => File.WriteAllText(path, json));
        }

        public static Task<(bool ok, SaveData data)> LoadAsync(string slot)
        {
            var path = GetPath(slot); // main thread
            return Task.Run<(bool, SaveData)>(() =>
            {
                if (!File.Exists(path)) return (false, null);
                var json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<SaveData>(json);
                return (data != null, data);
            });
        }
    }
}
