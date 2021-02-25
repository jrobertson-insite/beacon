using System;
using System.Collections.Generic;
using System.Linq;

namespace Beacon
{
    public class ClientsViewMode : ConsoleMode
    {
        protected int StartRow { get; set; } = 0;
        protected int HighlightedRow { get; set; } = 0;
        protected string Search { get; set; } = "";
        protected IList<ClientProject> clientProjects = ApplicationState.GetSortedClientProjects().ToList();

        public override ConsoleMode DoWork()
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                return this.OnEnterKey(clientProjects[HighlightedRow]);
            }
            if (key.Key == ConsoleKey.UpArrow)
            {
                if (StartRow == 0 && HighlightedRow == 0)
                {
                    return this;
                }

                MoveClientTable(true);
                return this;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                if ((StartRow == clientProjects.Count - GetRows() 
                    && HighlightedRow == GetRows() - 1)
                    || HighlightedRow == clientProjects.Count - 1)
                {
                    return this;
                }

                MoveClientTable(false);
                return this;
            }

            if (key.Key == ConsoleKey.Backspace || char.IsLetter(key.KeyChar) || char.IsNumber(key.KeyChar))
            {
                StartRow = 0;
                HighlightedRow = 0;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (Search.Length > 0)
                    {
                        Search = Search.Substring(0, Search.Length - 1);
                    }
                    else
                    {
                        return new MainMenuMode();
                    }
                }
                else
                {
                    Search += key.KeyChar;
                }

                PrintClientTable();
                Console.Write(Search);

                return this;
            }

            return this;
        }

        protected virtual ConsoleMode OnEnterKey(ClientProject clientProject)
        {
            return this;
        }

        public override void OnEntered()
        {
            PrintClientTable();
        }

        protected virtual void PrintHeader()
        {
            Print(Pad(" Name", 25) + "|" + Pad(" Git Repository", 85));
        }

        protected virtual void PrintClient(ClientProject clientProject, bool highlight = false)
        {
            var line = Pad(" " + clientProject.Name, 25) + "|" + " " + Pad(clientProject.GitUrl, 85);
            if (highlight)
            {
                Print(line, ConsoleColor.White, ConsoleColor.DarkBlue);
            }
            else
            {
                Print(line);
            }
        }
        
        private void PrintClientTable()
        {
            Console.Clear();
            PrintHeader();
            
            if (!Search.IsBlank())
            {
                clientProjects = ApplicationState.GetSortedClientProjects().Where(o => o.Name.ContainsIgnoreCase(Search) || o.GitUrl.ContainsIgnoreCase(Search)).ToList();
            }
            else
            {
                clientProjects = ApplicationState.GetSortedClientProjects().ToList();
            }

            var x = 0;
            foreach (var clientProject in clientProjects.Skip(StartRow).Take(GetRows()))
            {
                PrintClient(clientProject, x == HighlightedRow);
                x++;
            }

            while (x < GetRows())
            {
                Console.WriteLine();
                x++;
            }
        }
        
        private void MoveClientTable(bool up)
        {
            var originalTop = Console.CursorTop;
            
            var top = Console.CursorTop - GetRows();

            if (up)
            {
                var moveRow = true;
                ClientProject clientProject;
                if (HighlightedRow == 0)
                {
                    Console.MoveBufferArea(0, top, 120, GetRows() - 1, 0, top + 1);
                    Console.SetCursorPosition(0, top);
                    clientProject = clientProjects.Skip(StartRow).FirstOrDefault();
                    PrintClient(clientProject);
                    moveRow = false;
                }

                if (moveRow)
                {
                    HighlightedRow -= 1;
                }
                Console.SetCursorPosition(0, top + HighlightedRow);
                clientProject = clientProjects[StartRow + HighlightedRow];
                PrintClient(clientProject, true);
                clientProject = clientProjects[StartRow + HighlightedRow + 1];
                PrintClient(clientProject);
            }
            else
            {
                var moveRow = true;
                if (HighlightedRow == GetRows() - 1)
                {
                    StartRow += 1;
                    Console.MoveBufferArea(0, top - 1, 120, GetRows() - 1, 0, top - 2);
                    Console.SetCursorPosition(0, originalTop);
                    moveRow = false;
                }

                if (moveRow)
                {
                    HighlightedRow += 1;
                }

                Console.SetCursorPosition(0, top + HighlightedRow - 1);
                var clientProject = clientProjects[StartRow + HighlightedRow - 1];
                PrintClient(clientProject);
                clientProject = clientProjects[StartRow + HighlightedRow];
                PrintClient(clientProject, true);   
            }
            
            Console.SetCursorPosition(0, originalTop);
        }
        
        protected virtual int GetRows()
        {
            return Console.WindowHeight - 2;
        }
    }
}