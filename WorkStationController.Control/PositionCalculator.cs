using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using WorkstationController.Core.Data;

namespace WorkstationController.Control
{
    public class PositionCalculator
    {
        XYZR expectedPositionXYZR;
        XYZR speedXYZ;
        double acceleration = 10;
        double slowAcceleration = 5;
        double fastAcceleration = 10;
        double rotateSpeed = 3;
        double maxSpeed = 30; //30mm/s
        double oldTime = 0;
        Stopwatch watcher = new Stopwatch();
        public event EventHandler<XYZR> OnExpectedPositionChanged;
        public PositionCalculator(XYZR xyzr)
        {
            this.expectedPositionXYZR = xyzr;
            speedXYZ = new XYZR(0, 0, 0);
            //this.acceleration = slowAcceleration;
        }


        public void OnDirectionPressed(Direction dir)
        {
            //AccelerationSetting(isFastAcceleration);
            watcher.Start();
            double speed = GetDirectionSpeed(dir);
            double timeSeconds = watcher.ElapsedMilliseconds / 1000.0 - oldTime;
            timeSeconds = Math.Max(0.1,timeSeconds);
            oldTime += timeSeconds* 1000;
            double distance = distanceCalculator(dir, speed, timeSeconds);
            double speedChanged = timeSeconds * acceleration;
            distance = Math.Max(0.1, distance); //min 0.1mm
            UpdatePositionAndSpeed(dir, distance, speedChanged);
        
            if (OnExpectedPositionChanged != null)
            {
                OnExpectedPositionChanged(this, expectedPositionXYZR);
            }
        }

        private double distanceCalculator(Direction dir, double speed, double timeSeconds)
        {
            if (dir == Direction.RotateLeft || dir == Direction.RotateRight)
                return speed * timeSeconds;
            else
                return timeSeconds * speed + 0.5 * acceleration * timeSeconds * timeSeconds;
        }

        private void AccelerationSetting(bool isFastAcceleration)
        {
            if (isFastAcceleration)
                acceleration = fastAcceleration;
            else
                acceleration = slowAcceleration;
        }


        private void UpdatePositionAndSpeed(Direction dir, double distance, double speedChanged)
        {
            switch (dir)
            {
                case Direction.None:
                    break;
                case Direction.Up:
                    speedXYZ.Y = (speedXYZ.Y + speedChanged > maxSpeed) ? maxSpeed : (speedXYZ.Y + speedChanged);
                    expectedPositionXYZR.Y += distance;
                    break;
                case Direction.Left:
                    speedXYZ.X = (speedXYZ.X + speedChanged > maxSpeed) ? maxSpeed : (speedXYZ.X + speedChanged);
                    expectedPositionXYZR.X -= distance;
                    break;
                case Direction.ZUp:
                    speedXYZ.Z = (speedXYZ.Z + speedChanged > maxSpeed) ? maxSpeed : (speedXYZ.Z + speedChanged);
                    expectedPositionXYZR.Z += distance;
                    break;
                case Direction.Down:
                    speedXYZ.Y = (speedXYZ.Y + speedChanged > maxSpeed) ? maxSpeed : (speedXYZ.Y + speedChanged);
                    expectedPositionXYZR.Y -= distance;
                    break;
                case Direction.Right:
                    speedXYZ.X = (speedXYZ.X + speedChanged > maxSpeed) ? maxSpeed : (speedXYZ.X + speedChanged);
                    expectedPositionXYZR.X += distance;
                    break;
                case Direction.ZDown:
                    speedXYZ.Z = (speedXYZ.Z + speedChanged > maxSpeed) ? maxSpeed : (speedXYZ.Z + speedChanged);
                    expectedPositionXYZR.Z -= distance;
                    break;
                case Direction.RotateLeft:
                    expectedPositionXYZR.R -= distance;
                    break;
                case Direction.RotateRight:
                    expectedPositionXYZR.R += distance;
                    break;
                default:
                    break;
            }
            expectedPositionXYZR.X = Math.Max(0, expectedPositionXYZR.X);
            expectedPositionXYZR.Y = Math.Max(0, expectedPositionXYZR.Y);
            expectedPositionXYZR.Z = Math.Max(0, expectedPositionXYZR.Z);
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
                case Direction.RotateLeft:
                case Direction.RotateRight:
                    speed = rotateSpeed;
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
            oldTime = 0;
            watcher.Reset();
        }

    }


    public class InputDeviceInfo
    {
        //public static bool isMoving = false;
        public static bool isJoysMoving = false;
        public static bool isKeyBoardMoving = false;
    }
    public class InputChecker
    {
        PositionCalculator positionCalculator;
        SharpDX.DirectInput.Keyboard curKeyBoard;
        public InputChecker(PositionCalculator positionCalculator)
        {
            this.positionCalculator = positionCalculator;
        }
        public void JoysStart()
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

                var rotateLeftButton = joys.Buttons[3];
                var rotateRightButton = joys.Buttons[1];
                var clampOnButton = joys.Buttons[7];
                var clampOffButton = joys.Buttons[9];

                if (clampOnButton || clampOffButton)
                {
                    dir = clampOnButton ? Direction.ClampOn : Direction.ClampOff;
                    //clampCalculator.OnClampPressed(dir);
                    Debug.WriteLine(dir.ToString());
                    continue;
                }

                if (offset_Dir.ContainsKey(directionButton))
                    dir = offset_Dir[directionButton];
                if (zUpButton || zDownButton)
                    dir = zUpButton ? Direction.ZUp : Direction.ZDown;
                if (rotateLeftButton || rotateRightButton)
                    dir = rotateLeftButton ? Direction.RotateLeft : Direction.RotateRight;

                if (InputDeviceInfo.isKeyBoardMoving)
                    continue;
                InputDeviceInfo.isJoysMoving = directionButton != -1 || zUpButton || zDownButton || rotateLeftButton || rotateRightButton;

                if (!InputDeviceInfo.isJoysMoving)
                {
                    positionCalculator.Stop();
                }
                else
                {
                    positionCalculator.OnDirectionPressed(dir);
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
                Key curPressedKey = new Key(); ;
                if (curKeyboardState.PressedKeys.Count() > 0)
                    curPressedKey = curKeyboardState.PressedKeys[0];
                if (curPressedKey == SharpDX.DirectInput.Key.O || curPressedKey == SharpDX.DirectInput.Key.P)
                {
                    var dir = curPressedKey == SharpDX.DirectInput.Key.O ? Direction.ClampOn : Direction.ClampOff;
                    //clampCalculator.OnClampPressed(dir);
                    Debug.WriteLine(dir.ToString());
                    continue;
                }

                if (InputDeviceInfo.isJoysMoving)
                    continue;
                InputDeviceInfo.isKeyBoardMoving = curKeyboardState.PressedKeys.Count() > 0 && IsControlKey(curPressedKey);
                if (InputDeviceInfo.isKeyBoardMoving)
                {

                    //InputDeviceInfo.isKeyBoardMoving = true;
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
                        case SharpDX.DirectInput.Key.J:
                            positionCalculator.OnDirectionPressed(Direction.RotateLeft);
                            break;
                        case SharpDX.DirectInput.Key.L:
                            positionCalculator.OnDirectionPressed(Direction.RotateRight);
                            break;
                        default:
                            break;

                    }

                }
                else
                    positionCalculator.Stop();
            }
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
    }

    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
        ZUp,
        ZDown,
        RotateLeft,
        RotateRight,
        ClampOn,
        ClampOff
    }
}
