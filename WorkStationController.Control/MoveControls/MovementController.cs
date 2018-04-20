using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace MoveControl
{
    class PositionCalculator
    {
        XYZ positionXYZ;
        XYZ speedXYZ;

        const double acceleration = 10;
        const double maxSpeed = 30; //30mm/s
        double lastTime = 0;
        Stopwatch watcher = new Stopwatch();
        public event EventHandler<XYZ> OnPositionChanged;
        public PositionCalculator(XYZ xyz)
        {
            this.positionXYZ = xyz;
            speedXYZ = new XYZ(0, 0, 0);
        }
        public void OnDirectionPressed(Direction dir)
        {
            //AccelerationSetting(isFastAcceleration);
            watcher.Start();
            double speed = GetDirectionSpeed(dir);
            double timeSeconds = watcher.ElapsedMilliseconds / 1000.0 - lastTime;
            lastTime = watcher.ElapsedMilliseconds / 1000.0; 
            double distance = timeSeconds * speed + 0.5 * acceleration * timeSeconds * timeSeconds;
            double speedChanged = timeSeconds * acceleration;
         
            UpdatePositionAndSpeed(dir, distance,speedChanged);
            Debug.WriteLine("V:{0}", GetDirectionSpeed(dir));
            if(OnPositionChanged != null)
            {
                OnPositionChanged(this, positionXYZ);
            }
        }

     
        private void UpdatePositionAndSpeed(Direction dir, double distance, double speedChanged)
        {
            switch (dir)
            {
                case Direction.None:
                    break;
                case Direction.Up:
                    speedXYZ.Y = speedXYZ.Y + speedChanged; 
                    positionXYZ.Y += distance;
                    break;
                case Direction.Left:
                    speedXYZ.X = speedXYZ.X + speedChanged; 
                    positionXYZ.X -= distance;
                    break;
                case Direction.ZUp:
                    speedXYZ.Z = speedXYZ.Z + speedChanged; 
                    positionXYZ.Z += distance;
                    break;
                case Direction.Down:
                    speedXYZ.Y = speedXYZ.Y + speedChanged;
                    positionXYZ.Y -= distance;
                    break;
                case Direction.Right:
                    speedXYZ.X = speedXYZ.X + speedChanged;
                    positionXYZ.X += distance;
                    break;
                case Direction.ZDown:
                    speedXYZ.Z = speedXYZ.Z + speedChanged;
                    positionXYZ.Z -= distance;
                    break;
                default:
                    break;
            }
            speedXYZ.X = Math.Max(speedXYZ.X, maxSpeed);
            speedXYZ.Y = Math.Max(speedXYZ.Y, maxSpeed);
            speedXYZ.Z = Math.Max(speedXYZ.Z, maxSpeed);

        }

        private double GetDirectionSpeed(Direction dir)
        {
            double speed = 0;
            switch (dir)
            {
                case Direction.None:
                    break;
                case Direction.Up:
                case Direction.Down:
                    speed = speedXYZ.Y;
                    break;
                case Direction.Left:
                case Direction.Right:
                    speed = speedXYZ.X;
                    break;
                case Direction.ZUp:
                case Direction.ZDown:
                    speed = speedXYZ.Z;
                    break;
                default:
                    break;
            }
            return speed;
        }

        public void Stop()
        {
            speedXYZ.X = 0;
            speedXYZ.Y = 0;
            speedXYZ.Z = 0;
            lastTime = 0;
            watcher.Reset();
        }

    }
    class InputChecker
    {
        PositionCalculator positionCalculator;
        SharpDX.DirectInput.Keyboard curKeyBoard;
        public InputChecker(PositionCalculator positionCalculator)
        {
            this.positionCalculator = positionCalculator;
        }
        public void JoysStart ()
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
                return;
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
           
            Dictionary<int, Direction> offset_Dir = GetOffsetDirMapping();
            while (true)
            {
                joystick.Poll();
                Thread.Sleep(100);
                Direction dir = Direction.None;
                var joys = joystick.GetCurrentState();
    
                int directionButton = joys.PointOfViewControllers[0];
                var zUpButton = joys.Buttons[4];
                var zDownButton = joys.Buttons[0];
                var accelerationChangeButton = joys.Buttons[7];
                if(offset_Dir.ContainsKey(directionButton))
                    dir = offset_Dir[directionButton];
                if (zUpButton || zDownButton)
                    dir = zUpButton ? Direction.ZUp : Direction.ZDown;
                MovingDeviceInfo.isJoysMoving = directionButton != -1 || zUpButton || zDownButton;
                if (MovingDeviceInfo.isJoysMoving)
                {
                    positionCalculator.OnDirectionPressed(dir);
                }
                else 
                {
                    if (!MovingDeviceInfo.isKeyBoardMoving)
                    {
                        positionCalculator.Stop();
                    }
                }
            }
        }



        public void KeyBoardStart()
        {
            var dirInput = new SharpDX.DirectInput.DirectInput();
            var allDevices = dirInput.GetDevices();
            foreach (var item in allDevices)
            {
                if (SharpDX.DirectInput.DeviceType.Keyboard == item.Type)
                {
                    curKeyBoard = new SharpDX.DirectInput.Keyboard(dirInput);
                    curKeyBoard.Acquire();
                }
            }
            while (true)
            {
                Thread.Sleep(100);
                var curKeyboardState = curKeyBoard.GetCurrentState();
                var curPressedKeys = curKeyboardState.PressedKeys;
                MovingDeviceInfo.isKeyBoardMoving = curKeyboardState.PressedKeys.Count() > 0 && IsControlKey(curKeyboardState.PressedKeys[0]);
                if (MovingDeviceInfo.isKeyBoardMoving)
                {
                    switch (curKeyboardState.PressedKeys[0])
                    {
                        case SharpDX.DirectInput.Key.W:
                            positionCalculator.OnDirectionPressed(Direction.Up);
                            break;
                        case SharpDX.DirectInput.Key.A:
                            positionCalculator.OnDirectionPressed(Direction.Left);
                            break;
                        case SharpDX.DirectInput.Key.S:
                            positionCalculator.OnDirectionPressed(Direction.Down);
                            break;
                        case SharpDX.DirectInput.Key.D:
                            positionCalculator.OnDirectionPressed(Direction.Right);
                            break;
                        case SharpDX.DirectInput.Key.I:
                            positionCalculator.OnDirectionPressed(Direction.ZUp);
                            break;
                        case SharpDX.DirectInput.Key.K:
                            positionCalculator.OnDirectionPressed(Direction.ZDown);
                            break;
                        default:
                            break;

                    }
                
                }
                else
                {
                    if (!MovingDeviceInfo.isJoysMoving) // only stop moving when joystick is not moving
                    {
                        positionCalculator.Stop();
                    }
                }
            }
        }

        private bool IsControlKey(Key key)
        {
            return (key == Key.W) || (key == Key.A) || (key == Key.S) || (key == Key.D) || (key == Key.I) || (key == Key.K);
        }

        private Dictionary<int, Direction> GetOffsetDirMapping()
        {
            Dictionary<int, Direction> stateDir = new Dictionary<int,Direction>();
            stateDir.Add(0,Direction.Up);
            stateDir.Add(9000,Direction.Right);
            stateDir.Add(18000,Direction.Down);
            stateDir.Add(27000,Direction.Left);
            return stateDir;
        }

        private bool IsPointofView(JoystickUpdate state)
        {
            return state.Offset == JoystickOffset.PointOfViewControllers0 ;
        }

        private  bool IsDirectionPressed(JoystickUpdate state)
        {
            return state.Offset == JoystickOffset.Buttons4 || state.Offset == JoystickOffset.Buttons0 || IsPointofView(state);
        }
    }

    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
        ZUp,
        ZDown
    }
}
