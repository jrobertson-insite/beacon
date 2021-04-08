using System;
using System.Collections.Generic;
using System.Linq;
using Beacon.Screens;
using Terminal.Gui;

namespace Beacon.Windows
{
    public class CloneClientWindow : IWindow
    {
        protected string Search { get; set; } = "";
        protected IList<ClientProject> ClientProjects = ApplicationState.GetSortedClientProjects()
            .ToList();

        private ListView listView;

        public void Initialize(IApp app)
        {
            var labelKeypress = new Label(
                "Search: "
            )
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                TextAlignment = TextAlignment.Left,
                ColorScheme = Colors.TopLevel,

            };
            app.Add(labelKeypress);

            this.listView = new ListView()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            this.listView.SetSource(
                ClientProjects.Select(PrintClient).ToList()
            );

            this.listView.KeyPress += args =>
            {
                var theKey = args.KeyEvent.ToString().Replace("Numlock-", "");
                var key = theKey.Length == 1 ? theKey.ToCharArray()[0] : '|';
                var redo = false;
                if (args.KeyEvent.Key == Key.Backspace)
                {
                    if (Search.Length > 0)
                    {
                        Search = Search.Substring(0, Search.Length - 1);
                        redo = true;
                    }
                }
                else if (char.IsLetter(key) || char.IsNumber(key))
                {
                    Search += key;
                    redo = true;
                }

                if (redo)
                {
                    labelKeypress.Text = "Search: " + Search;
                    if (!Search.IsBlank())
                    {
                        this.ClientProjects = ApplicationState.GetSortedClientProjects()
                            .Where(
                                o => o.Name.ContainsIgnoreCase(Search)
                                || o.GitUrl.ContainsIgnoreCase(Search)
                            )
                            .ToList();
                    }
                    else
                    {
                        this.ClientProjects = ApplicationState.GetSortedClientProjects()
                            .ToList();
                    }

                    this.listView.SetSource(
                        this.ClientProjects.Select(PrintClient).ToList()
                    );
                    args.Handled = true;
                }
            };

            app.Add(this.listView);
        }

        public string GetTitle()
        {
            return "Clone Client";
        }

        protected virtual string PrintClient(ClientProject clientProject)
        {
            return Pad(" " + clientProject.Name, 25) + "|" + " " + Pad(
                clientProject.GitUrl,
                85
            );
        }

        protected static string Pad(string value, int size)
        {
            while (value.Length < size)
            {
                value += " ";
            }

            return value;
        }
    }
}
