using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using WorkstationController.Core.Data;
using WorkstationController.Hardware;

namespace WorkstationController.Control
{
  
    public class InputChecker
    {
        Dictionary<SharpDX.DirectInput.Key, Direction> keyboard_Direction;
        public event EventHandler<Direction> OnStartMove;
        public event EventHandler<Direction> OnStopMove;
        bool stopChecking = false;
        public InputChecker()
        {
            keyboard_Direction = new Dictionary<Key, Direction>();
            keyboard_Direction.Add(SharpDX.DirectInput.Key.W, Direction.Up);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.S, Direction.Down);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.I, Direction.ZUp);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.K, Direction.ZDown);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.A, Direction.Left);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.D, Direction.Right);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.J, Direction.RotateLeft);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.L, Direction.RotateRight);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.O, Direction.ClampOn);
            keyboard_Direction.Add(SharpDX.DirectInput.Key.P, Direction.ClampOff);
        }
        public void Start()
        {
            Thread th = new Thread(new ThreadStart(StartImpl));
            th.Start();
        }

        private void StartImpl()
        {
            var joyStick = StartJoyStick();
            var keyBoard = StartKeyboard();
            stopChecking = false;
            Dictionary<int, Direction> offset_Dir = GetXYDirMapping();
            Direction dir = Direction.None;
            Direction lastDir = Direction.None;
            while (true)
            {
                if (stopChecking)
                    break;
                dir = Direction.None;
                if (joyStick != null)
                {
                    dir = GetDirectionFromJoystick(joyStick);
                }
                if (dir == Direction.None)
                {
                    dir = GetDirectionFromKeyboard(keyBoard);
                }

                if(dir != lastDir) 
                {
                    if(dir == Direction.None)
                    {
                        if (OnStopMove != null)
                            OnStopMove(this, dir);
                    }
                    else
                    {
                        if (OnStartMove != null)
                             OnStartMove(this, dir);
                    }
                }
                lastDir = dir;
            }
        }

        private Direction GetDirectionFromKeyboard(Keyboard keyBoard)
        {
            var curKeyboardState = keyBoard.GetCurrentState();
            Key curPressedKey = new Key(); ;
            if (curKeyboardState.PressedKeys.Count() > 0)
                curPressedKey = curKeyboardState.PressedKeys[0];
            if (curPressedKey == SharpDX.DirectInput.Key.O || curPressedKey == SharpDX.DirectInput.Key.P)
            {
                var dir = curPressedKey == SharpDX.DirectInput.Key.O ? Direction.ClampOn : Direction.ClampOff;
            }
            if (keyboard_Direction.ContainsKey(curPressedKey))
                return keyboard_Direction[curPressedKey];
            else
                return Direction.None;
        }

        private Keyboard StartKeyboard()
        {
            var dirInput = new SharpDX.DirectInput.DirectInput();
            var allDevices = dirInput.GetDevices();
            Keyboard keyboard = null;
            foreach (var item in allDevices)
            {
                if (SharpDX.DirectInput.DeviceType.Keyboard == item.Type)
                {
                    keyboard = new SharpDX.DirectInput.Keyboard(dirInput);
                    keyboard.Acquire();
                }
            }
            return keyboard;
        }

        private Direction GetDirectionFromJoystick(Joystick joystick)
        {
            Direction dir = Direction.None;
            joystick.Poll();
            var joys = joystick.GetCurrentState();

            int directionButton = joys.PointOfViewControllers[0];
            var zUpButton = joys.Buttons[4];
            var zDownButton = joys.Buttons[0];

            var rotateLeftButton = joys.Buttons[3];
            var rotateRightButton = joys.Buttons[1];
            var clampOnButton = joys.Buttons[7];
            var clampOffButton = joys.Buttons[9];

            if (clampOnButton || clampOffButton)
            {
                dir = clampOnButton ? Direction.ClampOn : Direction.ClampOff;
            }

            var xyDir = GetXYDirMapping();
            if (xyDir.ContainsKey(directionButton))
                dir = xyDir[directionButton];
            if (zUpButton || zDownButton)
                dir = zUpButton ? Direction.ZUp : Direction.ZDown;
            if (rotateLeftButton || rotateRightButton)
                dir = rotateLeftButton ? Direction.RotateLeft : Direction.RotateRight;
            return dir;
        }

        private Dictionary<int, Direction> GetXYDirMapping()
        {
            Dictionary<int, Direction> stateDir = new Dictionary<int, Direction>();
            stateDir.Add(0, Direction.Up);
            stateDir.Add(9000, Direction.Right);
            stateDir.Add(18000, Direction.Down);
            stateDir.Add(27000, Direction.Left);
            return stateDir;
        }

        private Joystick StartJoyStick()
        {
            var directInput = new DirectInput();
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;
            if (joystickGuid == Guid.Empty)
            {
                Debug.WriteLine("No joystick/Gamepad found.");
                return null;
            }

            var joystick = new Joystick(directInput, joystickGuid);
            Debug.WriteLine("Found Joystick/Gamepad with GUID: {0}", joystickGuid);
            // Query all suported ForceFeedback effects 
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Debug.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data. 
            joystick.Properties.BufferSize = 128;
            // Acquire the joystick 
            joystick.Acquire();
            return joystick;
        }



       

        private bool IsControlKey(Key key)
        {
            return (key == Key.W) || (key == Key.A) || (key == Key.S) || (key == Key.D) || (key == Key.I) || (key == Key.K) || (key == Key.J) || (key == Key.L);
        }

        private Dictionary<int, Direction> GetOffsetDirMapping()
        {
            Dictionary<int, Direction> stateDir = new Dictionary<int, Direction>();
            stateDir.Add(0, Direction.Up);
            stateDir.Add(9000, Direction.Right);
            stateDir.Add(18000, Direction.Down);
            stateDir.Add(27000, Direction.Left);
            return stateDir;
        }

        private bool IsPointofView(JoystickUpdate state)
        {
            return state.Offset == JoystickOffset.PointOfViewControllers0;
        }

        private bool IsDirectionPressed(JoystickUpdate state)
        {
            return state.Offset == JoystickOffset.Buttons4 || state.Offset == JoystickOffset.Buttons0 || IsPointofView(state);
        }

        internal void Stop()
        {
            stopChecking = true;
        }
    }

   
}
