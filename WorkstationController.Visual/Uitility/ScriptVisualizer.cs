using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement.Uitility
{
    public class ScriptVisualizer
    {
        Grid _container;
        int currentCmdIndex  = 0;
        List<LabwareUIElement> _labwareUIElements = new List<LabwareUIElement>();
        List<IPipettorCommand> _commands;
        Dictionary<IPipettorCommand, LabwareUIElement> cmd_UIElement = new Dictionary<IPipettorCommand,LabwareUIElement>();
        public ScriptVisualizer(Grid container, List<IPipettorCommand> commands)
        {
            _commands = commands;
            //check labware all exists
            Dictionary<string, LabwareUIElement> label_LabwareUI = new Dictionary<string, LabwareUIElement>();

            foreach(UIElement uiElement in container.Children)
            {
                if (uiElement is LabwareUIElement)
                {
                    LabwareUIElement labwareUIElement = (LabwareUIElement)uiElement;
                    label_LabwareUI.Add(labwareUIElement.Label,labwareUIElement);
                    _labwareUIElements.Add(labwareUIElement);
                }
                    
            }
            
            int lineNum = 1;
            List<PipettingCommand> pipettingCmds = new List<PipettingCommand>();
            foreach(var command in commands)
            {
                
                if(command is PipettingCommand)
                {
                    PipettingCommand pipettingCmd = (PipettingCommand)command;
                    pipettingCmds.Add(pipettingCmd);
                    string label = pipettingCmd.LabwareLabel;
                    if(!_labwareUIElements.Exists(x=>x.Label == label))
                    {
                        throw new Exception(string.Format("Command: {0}'s labware cannot be found!",label));
                    }
                    else //check wellID
                    {
                        var labware = label_LabwareUI[label].Labware;
                        int totalWellCnt = labware.WellsInfo.NumberOfWellsX * labware.WellsInfo.NumberOfWellsY;
                        if (pipettingCmd.SelectedWellIDs.Max() > totalWellCnt)
                            throw new Exception(string.Format("Command : {0}'s well ID doesn't exist in labware:{1}", lineNum, label));
                    }
                    cmd_UIElement.Add(pipettingCmd, label_LabwareUI[label]);
                    
                }
                lineNum++;
            }
        }


        public void ExecuteOneCommand()
        {
            if(currentCmdIndex > 0)
            {
                ClearLast();
            }
            var currentCmd = _commands[currentCmdIndex];
            var currentUI = cmd_UIElement[currentCmd];
            if(currentCmd is PipettingCommand)
            {
                PipettingCommand pipettingCmd = (PipettingCommand)currentCmd;
                if(pipettingCmd.IsAspirate)
                {
                    currentUI.AspirateWellIDs = pipettingCmd.SelectedWellIDs;
                }
                else
                {
                    currentUI.DispenseWellIDs = pipettingCmd.SelectedWellIDs;
                }
                currentUI.Dispatcher.Invoke(() =>
                {
                    currentUI.InvalidateVisual();
                });
            }
            currentCmdIndex++;
        }

        private void ClearLast()
        {
            var lastCmd = _commands[currentCmdIndex - 1];
            var lastUI = cmd_UIElement[lastCmd];
            lastUI.AspirateWellIDs.Clear();
            lastUI.DispenseWellIDs.Clear();
            lastUI.Dispatcher.Invoke(() =>
            {
                lastUI.InvalidateVisual();
            });
        }


        private object ClearAspirateWellIDs(LabwareUIElement labwareUIElement)
        {
            labwareUIElement.AspirateWellIDs.Clear();
            labwareUIElement.Dispatcher.Invoke(() =>
            {
                labwareUIElement.InvalidateVisual();
            });
            return null;
        }


        private LabwareUIElement FindLabwareUIElement(string name)
        {
            LabwareUIElement findLabwareUIElement = null;
            foreach (var uiElement in _container.Children)
            {
                if (uiElement is LabwareUIElement)
                {
                    LabwareUIElement labwareUIElement = (LabwareUIElement)uiElement;
                    if (labwareUIElement.Label == name)
                    {
                        findLabwareUIElement = labwareUIElement;
                    }

                }
            }
            return findLabwareUIElement;
        }

        public void Finish()
        {
            ClearLast();
        }
    }
}
