using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement.Uitility
{
    class WareInstaller
    {
        public static void MountThis(BasewareUIElement baseUIElement, Point position, Grid container, Layout workingLayout)
        {
            bool bValid = IsValid(baseUIElement, position, container);
            if (!bValid)
            {
                if(baseUIElement is LabwareUIElement) //if labware has parent, let it go back
                {
                    LabwareUIElement labwareUIElement = (LabwareUIElement)baseUIElement;
                    if (labwareUIElement.Labware.ParentCarrier != null)
                        return;
                }

                container.Children.Remove(baseUIElement);
                if(baseUIElement is CarrierUIElement)
                {
                    RemoveUIElementsOnCarrier(container,baseUIElement as CarrierUIElement);
                }
                baseUIElement = null;
                return;
            }

            int grid = VisualCommon.FindCorrespondingGrid(position.X);
            if (baseUIElement is CarrierUIElement)
            {
                CarrierUIElement carrierUIElement = (CarrierUIElement)baseUIElement;
                carrierUIElement.Grid = grid;

            }

            if(baseUIElement is LabwareUIElement)
            {
                LabwareUIElement labwareUIElement = (LabwareUIElement)baseUIElement;
                Labware labware = labwareUIElement.Labware;
                CarrierUIElement carrierUIElement = null;
                int siteID = -1;
                bool bFound = FindSuitableCarrier(position, labware.TypeName, container, ref carrierUIElement, ref siteID);
                if(bFound)
                {
                    if (labware.ParentCarrier != null)
                        labware.ParentCarrier.Labwares.Remove(labware);
                    labware.SiteID = siteID;
                    if(labware.IsDitiBox)
                    {
                        var ditiInfo = workingLayout.DitiInfo.DitiInfoItems.Find(x => x.label == labware.Label);
                        if(ditiInfo == null)
                        {
                            workingLayout.DitiInfo.DitiInfoItems.Add(new DitiInfoItem(labware.Label, 96));
                        }
                    }
                }
                carrierUIElement.Carrier.AddLabware(labware);
            }
            
        }

        

         
        

        //private static bool NotMoved(Point position, Labware labware, Grid container)
        //{
        //    CarrierUIElement carrierUIElement;
        //    foreach (UIElement uiElement in container.Children)
        //    {
        //        if (!(uiElement is CarrierUIElement))
        //            continue;
        //        carrierUIElement = uiElement as CarrierUIElement;
        //        if(carrierUIElement.Carrier.Labwares.Contains(labware))
        //        {
        //            int siteID = CarrierUIElement.InvalidSiteIndex;
        //            bool bFound = carrierUIElement.GetSiteIDAcceptsTheLabware(position, labware.TypeName, ref siteID);
        //            if (bFound && siteID == labware.SiteID)
        //                return true;
        //        }
        //    }
        //    return false;
        //}

        private static void RemoveUIElementsOnCarrier(Grid container, CarrierUIElement carrierUIElement)
        {
            Carrier carrier = carrierUIElement.Carrier;
            foreach(var labware in carrier.Labwares)
            {
                RemoveLabware(container, labware);
            }
            carrier.Labwares.Clear();
           
        }

        private static void RemoveLabware(Grid container, Labware labware)
        {
            for (int i = 0; i < container.Children.Count; i++)
            {
                if (container.Children[i] is LabwareUIElement)
                {
                    if (((LabwareUIElement)container.Children[i]).Labware.Equals(labware))
                        container.Children.RemoveAt(i);
                }
            }
        }

       
        private static bool IsValid(BasewareUIElement baseUIElement, Point position, Grid container)
        {
            int gridPos = VisualCommon.FindCorrespondingGrid(position.X);
            int totalGrid = Configurations.Instance.Worktable.GridCount;

            //1 moves out of worktable
            if (gridPos < 1 || gridPos > totalGrid)
                return false;

            //2 if is Carrier, see whether there are enough grids for it
            if (baseUIElement is CarrierUIElement)
            {
                CarrierUIElement carrierUIElement = (CarrierUIElement)baseUIElement;
                bool outOfRange = IsOutofRange(gridPos, carrierUIElement);
                if (outOfRange)
                    return false;

                bool overLapped = OverlapChecker.IsOverlapped(container, carrierUIElement, gridPos);
                if (overLapped)
                    return false;
                
            }

            //3 if is labware, see whether there are suitable carrier for mounting onto
            if (baseUIElement is LabwareUIElement)
            {
                bool hasSuitableSite2Mount = HasSuitableSite(position,baseUIElement.Ware.TypeName,container);
                if (!hasSuitableSite2Mount)
                    return false;
            }

            //baseUIElement.Selected = false;
            return true;
        }

        private static bool FindSuitableCarrier(Point pt, string labwareTypeName, Grid container,ref CarrierUIElement carrierUIElement,ref int siteID)
        {
            bool bFound = false;
            foreach (UIElement uiElement in container.Children)
            {
                if (!(uiElement is CarrierUIElement))
                    continue;
                carrierUIElement = uiElement as CarrierUIElement;

                bFound = carrierUIElement.GetSiteIDAcceptsTheLabware(pt, labwareTypeName, ref siteID);
                if (bFound)
                    break;
            }
            return bFound;
        }

        private static bool HasSuitableSite(Point pt, string labwareTypeName,Grid container)
        {
            CarrierUIElement carrierUIElement = null;
            int siteID = -1;
            return FindSuitableCarrier(pt, labwareTypeName, container, ref carrierUIElement, ref siteID);
        }

        private static bool IsOutofRange(int grid, CarrierUIElement carrierUIElement)
        {
            double length = carrierUIElement.Dimension.XLength;
            int gridsNeed =  (int)(Math.Ceiling(length / (double)Worktable.DistanceBetweenAdjacentPins));
            return  grid + gridsNeed - 1 > Configurations.Instance.Worktable.GridCount;
        }
    }

    class OverlapChecker
    {
        public static bool IsOverlapped(Grid uiContainer, CarrierUIElement newCarrierUI,int newUICurGrid)
        {
            bool bOverlapped = false;
            double newCarrierXStart = newCarrierUI.GetBoundingRectXStart(newUICurGrid);
            double newCarrierXEnd = newCarrierXStart +  newCarrierUI.Dimension.XLength;

            foreach (UIElement uiElement in uiContainer.Children)
            {
                //jump self
                if (!(uiElement is CarrierUIElement))
                    continue;
                CarrierUIElement carrierUIElement = uiElement as CarrierUIElement;
                if (carrierUIElement == newCarrierUI)
                    continue;

                double oldCarrierXStart = carrierUIElement.GetBoundingRectXStart();
                double oldCarrierXEnd = oldCarrierXStart + carrierUIElement.Dimension.XLength;
                bOverlapped = IsOverlapped(newCarrierXStart, newCarrierXEnd, oldCarrierXStart, oldCarrierXEnd);
                if (bOverlapped)
                    break;
            }
            return bOverlapped;
        }

        private static bool IsOverlapped(double newCarrierXStart, double newCarrierXEnd, double oldCarrierXStart, double oldCarrierXEnd)
        {
            if (newCarrierXStart == oldCarrierXStart)
                return true;
            if (newCarrierXEnd == oldCarrierXEnd)
                return true;

            bool newStartBetweenExisting = IsBetween(newCarrierXStart, oldCarrierXStart, oldCarrierXEnd);
            bool newEndBetweenExisting =  IsBetween(newCarrierXEnd, oldCarrierXStart, oldCarrierXEnd);
            bool existingStartBetweenNew = IsBetween(oldCarrierXStart, newCarrierXStart, newCarrierXEnd);
            bool existingEndBetweenNew = IsBetween(oldCarrierXEnd, newCarrierXStart, newCarrierXEnd);
            return newStartBetweenExisting ||
                   newEndBetweenExisting ||
                   existingStartBetweenNew ||
                   existingEndBetweenNew;

        }

        private static bool IsBetween(double test, double start, double end)
        {
            return test > start && test < end;
        }



      

   

  
    }
}
