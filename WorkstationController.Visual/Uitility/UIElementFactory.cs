using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;

namespace WorkstationController.VisualElement.Uitility
{
    /// <summary>
    /// factory produce UIElement from ware
    /// </summary>
    public  class UIElementFactory
    {
        /// <summary>
        /// create UIElement
        /// </summary>
        /// <param name="wareBase"></param>
        /// <param name="existingUIElements"/>
        /// <returns></returns>
        public static BasewareUIElement CreateUIElement(WareBase wareBase, UIElementCollection existingUIElements)
        {
            BasewareUIElement newUIElement;
            if (wareBase is Labware)
            {
                
                Labware labware = ((Labware)wareBase).Clone() as Labware;
                List<string> existingLabels = GetExistingLabels(existingUIElements);
                string newLabel = FindNextLabelName(existingLabels);
                labware.Label = newLabel;
                newUIElement = new LabwareUIElement(labware);
            }
            else
            {
                Carrier carrier = ((Carrier)wareBase).Clone() as Carrier;
                newUIElement = new CarrierUIElement((Carrier)carrier);
            }
            newUIElement.Selected = true;
            return newUIElement;
        }

        private static string FindNextLabelName(List<string> existingLabels)
        {
            int labelID = 1;
            for(;;)
            {
                string labelCandidate = string.Format("label{0}", labelID);
                if (!existingLabels.Contains(labelCandidate))
                    return labelCandidate;
                labelID++;
                if (labelID > 1000)
                    throw new Exception("Cannot find a suitable label!");
            }
        }

        private static List<string> GetExistingLabels(UIElementCollection existingUIElements)
        {
            List<string> labels = new List<string>();
            foreach(var uiElement in existingUIElements)
            {
                if (!(uiElement is LabwareUIElement))
                    continue;
                labels.Add(((LabwareUIElement)uiElement).Label);
            }
            return labels;
        }
    }
}
