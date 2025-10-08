using System.Threading.Tasks;
using UnityEngine;
using AF.Core;

namespace AF.Services.Save
{
    /// <summary>
    /// Punto único para guardar/cargar SaveData en el backend elegido.
    /// </summary>
    public static class SaveService
    {
        static ISaveBackend _backend = new LocalBackend();
        public static void SetBackend(ISaveBackend backend) => _backend = backend ?? _backend;

        public static async Task<bool> SaveAsync(string slot, SaveData data)
        {
            var json = JsonUtility.ToJson(data, true);
            return await _backend.SaveAsync(slot, json);
        }

        public static async Task<(bool ok, SaveData data)> LoadAsync(string slot)
        {
            var (ok, json) = await _backend.LoadAsync(slot);
            if (!ok || string.IsNullOrEmpty(json)) return (false, null);
            var data = JsonUtility.FromJson<SaveData>(json);
            return (true, data);
        }

        public static Task<string[]> ListAsync() => _backend.ListAsync();
        public static Task<bool> DeleteAsync(string slot) => _backend.DeleteAsync(slot);
        public static string BackendName => _backend.Name;
    }
}
