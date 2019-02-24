using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace bomberman
{
    class ControlScheme
    {
        private Dictionary<VirtualKey, KeyAction> _scheme;
        private static ControlScheme _wasd;
        private static ControlScheme _arrows;
        private static ControlScheme _gamepad;

        private ControlScheme(Dictionary<VirtualKey, KeyAction> scheme)
        {
            _scheme = scheme;
        }

        public bool IsKeyInScheme(VirtualKey virtualKey)
        {
            return _scheme.ContainsKey(virtualKey);
        }

        public KeyAction GetAction (VirtualKey virtualKey)
        {
            return _scheme[virtualKey];
        }


        public static ControlScheme WASD
        {
            get
            {
                if (_wasd == null)
                {
                    Dictionary<VirtualKey, KeyAction> scheme = new Dictionary<VirtualKey, KeyAction>()
                    {
                        {VirtualKey.W, KeyAction.Up},
                        {VirtualKey.S, KeyAction.Down},
                        {VirtualKey.A, KeyAction.Left},
                        {VirtualKey.D, KeyAction.Right},
                        {VirtualKey.Space, KeyAction.PlaceBomb}
                    };
                    _wasd = new ControlScheme(scheme);
                }
                return _wasd;
            }
        }

        public static ControlScheme Arrows
        {
            get
            {
                if (_arrows == null)
                {
                    Dictionary<VirtualKey, KeyAction> scheme = new Dictionary<VirtualKey, KeyAction>()
                    {
                        {VirtualKey.Up, KeyAction.Up},
                        {VirtualKey.Down, KeyAction.Down},
                        {VirtualKey.Left, KeyAction.Left},
                        {VirtualKey.Right, KeyAction.Right},
                        {VirtualKey.Enter, KeyAction.PlaceBomb}
                    };
                    _arrows = new ControlScheme(scheme);
                }
                return _arrows;
            }
        }

        public static ControlScheme Gamepad
        {
            get
            {
                if (_gamepad == null)
                {
                    Dictionary<VirtualKey, KeyAction> scheme = new Dictionary<VirtualKey, KeyAction>()
                    {
                        {VirtualKey.GamepadDPadUp, KeyAction.Up},
                        {VirtualKey.GamepadDPadDown, KeyAction.Down},
                        {VirtualKey.GamepadDPadLeft, KeyAction.Left},
                        {VirtualKey.GamepadDPadRight, KeyAction.Right},
                        {VirtualKey.GamepadA, KeyAction.PlaceBomb}
                    };
                    _gamepad = new ControlScheme(scheme);
                }
                return _gamepad;
            }
        }

    }
}
