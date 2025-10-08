using UnityEngine;
using AF.UI;

namespace AF.Inputs
{
    public class UIShortcuts : MonoBehaviour
    {
        public void GoMainMenu()  => UIRouter.Instance?.Show("MainMenu");
        public void GoDashboard() => UIRouter.Instance?.Show("Dashboard");
        public void GoTalents()   => UIRouter.Instance?.Show("Talents");
        public void GoPlayers()   => UIRouter.Instance?.Show("Players");
        public void GoClubs()     => UIRouter.Instance?.Show("Clubs");
        public void GoNews()      => UIRouter.Instance?.Show("News");
        public void GoFinance()   => UIRouter.Instance?.Show("Finance");
        public void GoSettings()  => UIRouter.Instance?.Show("Settings");
    }
}
