using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WorkStationController.Core.Data;


namespace Visuals
{
    public class WorktableVisual
    {
        private Worktable worktable;
        
        public WorktableVisual()
        {
            this.worktable = Configurations.Instance.Worktable;
            
        }

        public void Draw(DrawingContext drawingContext)
        {
            
            for (int i = 0; i < worktable.GridCount; i++)
            {
                DrawPinsSameGrid(i, drawingContext);
            }
        }

        private void DrawPinsSameGrid(int grid, DrawingContext drawingContext)
        {
            Point ptStart = worktable.FirstPinPosition;
            int offset = grid * Worktable.distanceBetweenAdjacentPins;
            int firstPinX = (int)ptStart.X + offset;
            int firstPinY = (int)ptStart.Y;
            DrawPin(firstPinX, firstPinY,worktable.FirstRowPinSize,drawingContext);

            int secondPinX = firstPinX;
            int secondPinY = worktable.SecondPinYPosition;
            DrawPin(secondPinX, secondPinY, worktable.SecondRowPinSize, drawingContext);

            int thirdPinX = firstPinX;
            int thirdPinY = worktable.ThirdPinYPosition;
            DrawPin(thirdPinX, thirdPinY, worktable.ThirdRowPinSize, drawingContext);
        }

       

        private void DrawPin(int pinX, int pinY, Size size, DrawingContext drawingContext)
        {
            VisualCommon.DrawRect(pinX, pinY, size, drawingContext, Colors.Black);
        }

        
    }
}
