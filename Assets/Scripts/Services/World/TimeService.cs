using AF.Core;

namespace AF.Services.World
{
    public static class TimeService
    {
        public static void AdvanceWeek(SaveData save)
        {
            save.Time.Week++;
            if (save.Time.Week % 52 == 0)
            {
                save.Time.Season++;
                save.Time.TransferWindow = 1;
            }
            else if (save.Time.Week % 8 == 0) // ejemplo de ventana de fichajes
            {
                save.Time.TransferWindow++;
            }
        }
    }
}
