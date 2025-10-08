using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AF.Services.Save
{
    public class LocalBackend : ISaveBackend
    {
        public string Name => "Local";

        string SlotPath(string slot) =>
            Path.Combine(Application.persistentDataPath, $"save_{slot}.json");

        public async Task<bool> SaveAsync(string slot, string json)
        {
            try
            {
                var path = SlotPath(slot);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var sw = new StreamWriter(path, false);
                await sw.WriteAsync(json);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"LocalBackend.SaveAsync error: {e}");
                return false;
            }
        }

        public async Task<(bool ok, string json)> LoadAsync(string slot)
        {
            try
            {
                var path = SlotPath(slot);
                if (!File.Exists(path)) return (false, null);
                using var sr = new StreamReader(path);
                var txt = await sr.ReadToEndAsync();
                return (true, txt);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"LocalBackend.LoadAsync error: {e}");
                return (false, null);
            }
        }

        public Task<string[]> ListAsync()
        {
            var dir = Application.persistentDataPath;
            var files = Directory.GetFiles(dir, "save_*.json")
                                 .Select(Path.GetFileName)
                                 .Select(n => n!.Replace("save_", "").Replace(".json", ""))
                                 .ToArray();
            return Task.FromResult(files);
        }

        public Task<bool> DeleteAsync(string slot)
        {
            var path = SlotPath(slot);
            if (File.Exists(path)) File.Delete(path);
            return Task.FromResult(true);
        }
    }
}
