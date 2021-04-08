using System;
using System.Runtime.InteropServices;
using Beacon.Windows;
using Terminal.Gui;

namespace Beacon.Screens
{
    public class App : Window, IApp
    {
        private Window mainWindow;

        public void Run()
        {
            //Application.UseSystemConsole = true;
            Application.Init();
            Application.HeightAsBuffer = true;

            var top = Application.Top;
            this.mainWindow = new Window(
                "Beacon"
            )
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),

            // ColorScheme = new ColorScheme
            // {
            //     Focus = Application.Driver.MakeAttribute(
            //         Color.Black,
            //         Color.White
            //     ),
            //     Normal = Application.Driver.MakeAttribute(
            //         Color.White,
            //         Color.Black
            //     ),
            //     HotNormal = Application.Driver.MakeAttribute(
            //         Color.White,
            //         Color.Black
            //     ),
            //     HotFocus = Application.Driver.MakeAttribute(
            //         Color.Blue,
            //         Color.Black
            //     ),
            //
            // },
            };

            var statusBar = new StatusBar
            {
                Visible = true,
                Items = new StatusItem[]
                {
                    new StatusItem(
                        Key.Q
                        | Key.CtrlMask,
                        "~CTRL-Q~ Quit",
                        () =>
                        {
                            Application.UseSystemConsole = false;
                            Application.RequestStop();
                            Console.Clear();
                        }
                    )
                }
            };
            top.Add(statusBar);

            top.Add(mainWindow);
            this.SwitchTo<MainMenuWindow>();
            Application.Run(top);

            this.Unloaded += () =>
            {
                Application.UseSystemConsole = false;
            };
        }

        void IApp.Add(View view)
        {
            this.mainWindow.Add(view);
        }

        public void SwitchTo<T>()
            where T : IWindow, new()
        {
            this.mainWindow.RemoveAll();
            var nextWindow = new T();
            nextWindow.Initialize(this);
            this.mainWindow.Title = "Beacon - " + nextWindow.GetTitle();
        }

        public void RegisterKeyPress(Action<KeyEventEventArgs> action)
        {
            this.mainWindow.KeyPress += action;
        }
    }

    public interface IApp
    {
        void Add(View view);
        void SwitchTo<T>()
            where T : IWindow, new();

        void RegisterKeyPress(Action<View.KeyEventEventArgs> action);
    }


    public interface IWindow
    {
        void Initialize(IApp app);
        string GetTitle();
    }
}
