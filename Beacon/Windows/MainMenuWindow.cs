using System.Collections.Generic;
using Beacon.Screens;
using Terminal.Gui;

namespace Beacon.Windows
{
    public class MainMenuWindow : IWindow
    {
        private ListView listView;

        public void Initialize(IApp app)
        {
            this.listView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var source = new List<string>
            {
                "  Setup client - Get a client repo, branch your local repo to a specific version, load their blueprint/themes/extensions",
                "  Clone client - Get a client repo",
                "  Clean commerce - clean up after #1",
                "  Exit"
            };
            this.listView.SetSource(source);
            this.listView.OpenSelectedItem += args =>
            {
                if (args.Item == 1)
                {
                    app.SwitchTo<CloneClientWindow>();
                }
            };

            app.Add(this.listView);
        }

        public string GetTitle()
        {
            return "Main";
        }
    }
}
