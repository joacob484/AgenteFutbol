using System.Threading.Tasks;
using AF.Core;
using AF.Services.Save;
using UnityEngine;

namespace AF.Services.Game
{
    /// <summary>
    /// Auto-guardado simple al slot1. Se usa como "fire-and-forget".
    /// </summary>
    public static class AutoSave
    {
        static bool _saving;

        public static async void QueueSave(string slot = "slot1")
        {
            if (_saving || GameRoot.Current == null) return;
            _saving = true;
            try
            {
                await SaveService.SaveAsync(slot, GameRoot.Current);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"AutoSave error: {e}");
            }
            finally { _saving = false; }
        }
    }
}
