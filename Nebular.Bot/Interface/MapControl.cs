using Nebular.Bot.Game.World;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nebular.Bot.Interface
{
    [Obfuscation(Exclude = true)]
    public partial class MapControl : UserControl
    {
        private Map currentMap; // La carte actuellement affichée dans le contrôle
        private List<(Rectangle rect, Cell Cell)> rectangles = new List<(Rectangle rect, Cell Cell)>();

        public MapControl()
        {
            DoubleBuffered = true;
        }

        // Méthode pour mettre à jour la carte affichée dans le contrôle
        public void UpdateMap(Map map)
        {
            currentMap = map;
            Invalidate();
        }

        protected async override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Vérifiez si le clic a eu lieu dans l'un des rectangles
            foreach (var rectangle in rectangles)
            {
                Rectangle rect = rectangle.rect;
                if (rect.Contains(e.Location))
                {
                    Console.WriteLine("X: " + rectangle.Cell.X + "  Y: " + rectangle.Cell.Y);
                    _ = Task.Run(() => rectangle.Cell.Click());
                    break;
                }
            }
        }


        private void rawnRect(Cell cell, int xPos, int yPos, int TileWidth, int TileHeight, Graphics g)
        {
            Brush color = Brushes.White;
            if (cell == null) return;

            if(MainUI.client.Account.Character.Cell!= null && cell.Id == MainUI.client.Account.Character.Cell.Id)
            {
                color = Brushes.Blue;
            }
            else if (cell.IsTeleport())
            {
                color = Brushes.Yellow;
            }
            else if (currentMap.GetMonsters().FirstOrDefault(mob => mob.Cell.Id == cell.Id) != null)
            {
                color = Brushes.Red;
            }
            else if (currentMap.GetCharacters().FirstOrDefault(character => character.Cell.Id == cell.Id) != null)
            {
                color = Brushes.Teal;
            }
            else if (currentMap.GetInteractive(cell.Id) != null)
            {
                color = currentMap.GetInteractive(cell.Id).IsUsable ? Brushes.Green: Brushes.Brown;
            }
            else if(!cell.IsWalkable())
            {
                color = Brushes.Black;
            }


            Rectangle tileRect = new Rectangle(xPos, yPos, TileWidth, TileHeight);
            rectangles.Add((tileRect, cell));
            PointF rotationPoint = new PointF(tileRect.Left + tileRect.Width / 2, tileRect.Top + tileRect.Height / 2);
            g.TranslateTransform(rotationPoint.X, rotationPoint.Y);
            g.RotateTransform(45);
            g.TranslateTransform(-rotationPoint.X, -rotationPoint.Y);

            g.FillRectangle(color, tileRect);
            g.DrawRectangle(Pens.Black, tileRect);

            g.ResetTransform();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (currentMap == null)
                return;
            Graphics g = e.Graphics;

            int mapHeight = (currentMap.Height - 1) * 2;
            int mapWidth = (currentMap.Width - 1);

            int TileWidth = (Width - 10) / mapWidth;
            int TileHeight = (Height - 10) / mapHeight;

            TileWidth = TileWidth > TileHeight ? TileHeight : TileWidth;
            TileHeight = TileHeight > TileWidth ? TileWidth : TileHeight;

            int cellid = 0;


            for (int y = 0; y <= (2 * currentMap.Height) - 1; ++y)
            {
                if ((y % 2) == 0)
                {
                    for (int x = 0; x <= currentMap.Width - 1; x++)
                    {
                        int xPos = (int)Math.Round(x * TileWidth * Math.Sqrt(2) + 10);
                        int yPos = (int)Math.Round(y * TileHeight * Math.Sqrt(2) / 2 + 10);
                        if (cellid >= currentMap.Cells.Length) break;
                        rawnRect(currentMap.Cells[cellid], xPos, yPos, TileWidth, TileHeight, g);
                        cellid++;
                    }
                }
                else
                {
                    for (int x = 0; x <= currentMap.Width - 2; x++)
                    {
                        int xPos = (int)Math.Round(x * TileWidth * Math.Sqrt(2) + 10);
                        int yPos = (int)Math.Round(y * TileHeight * Math.Sqrt(2) / 2 + 10);
                        xPos += (int)Math.Round(TileHeight * Math.Sqrt(2) / 2);
                        if (cellid >= currentMap.Cells.Length) break;
                        rawnRect(currentMap.Cells[cellid], xPos, yPos, TileWidth, TileHeight, g);
                        cellid++;
                    }
                }
            }
        }

    }
}




