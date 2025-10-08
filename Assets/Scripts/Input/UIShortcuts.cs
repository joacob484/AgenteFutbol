using UnityEngine;
using AF.UI;

namespace AF.Inputs
{
    public class UIShortcuts : MonoBehaviour
    {
        public void GoMainMenu()   => UIRouter.Go("MainMenu");
        public void GoDashboard()  => UIRouter.Go("Dashboard");
        public void GoTalents()    => UIRouter.Go("Talents");
        public void GoPlayers()    => UIRouter.Go("Players");
        public void GoClubs()      => UIRouter.Go("Clubs");
        public void GoNews()       => UIRouter.Go("News");
        public void GoFinance()    => UIRouter.Go("Finance");
        public void GoSettings()   => UIRouter.Go("Settings");
    }
}
