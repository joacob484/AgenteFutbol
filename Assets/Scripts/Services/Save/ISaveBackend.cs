using System.Threading.Tasks;

namespace AF.Services.Save
{
    public interface ISaveBackend
    {
        string Name { get; }
        Task<bool> SaveAsync(string slot, string json);
        Task<(bool ok, string json)> LoadAsync(string slot);
        Task<string[]> ListAsync();
        Task<bool> DeleteAsync(string slot);
    }
}
