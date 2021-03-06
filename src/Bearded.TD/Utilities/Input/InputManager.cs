﻿using System.Collections.ObjectModel;
using System.Linq;
using OpenTK;
using OpenTK.Input;

namespace Bearded.TD.Utilities.Input
{
    partial class InputManager
    {
        private readonly MouseDevice mouse;

        private readonly AsyncAtomicUpdating<KeyboardState> keyboardState = new AsyncAtomicUpdating<KeyboardState>();
        private readonly AsyncAtomicUpdating<MouseState> mouseState = new AsyncAtomicUpdating<MouseState>();

        private bool windowWasActiveLastUpdate;

        public ReadOnlyCollection<GamePadStateManager> GamePads { get; }

        public InputManager(MouseDevice mouse)
        {
            this.mouse = mouse;

            this.GamePads = Enumerable.Range(0, int.MaxValue - 1)
                    .TakeWhile(i => GamePad.GetState(i).IsConnected)
                    .Select(GamePadStateManager.ForId)
                    .ToList().AsReadOnly();
        }

        public void ProcessEventsAsync()
        {
            keyboardState.SetLastKnownState(Keyboard.GetState());
            mouseState.SetLastKnownState(mouse.GetCursorState());

            foreach (var gamepad in GamePads)
            {
                gamepad.ProcessEventsAsync();
            }
        }

        public void Update(bool windowIsActive)
        {
            if (windowIsActive)
            {
                keyboardState.Update();
                mouseState.Update();
                if (!windowWasActiveLastUpdate)
                {
                    // mouse state is updated in a special way so that scroll delta doesnt jump
                    mouseState.UpdateTo(mouseState.Current);
                }
            }
            else
            {
                keyboardState.UpdateToDefault();
            }

            foreach (var gamepad in GamePads)
            {
                gamepad.Update(windowIsActive);
            }

            windowWasActiveLastUpdate = windowIsActive;
        }

        public bool IsKeyPressed(Key k) => keyboardState.Current.IsKeyDown(k);
        public bool IsKeyHit(Key k) => IsKeyPressed(k) && keyboardState.Previous.IsKeyUp(k);
        public bool IsKeyReleased(Key k) => !IsKeyPressed(k) && keyboardState.Previous.IsKeyDown(k);

        public Vector2 MousePosition => new Vector2(mouse.X, mouse.Y);
        public bool MouseMoved => mouseState.Current.X != mouseState.Previous.X
                               || mouseState.Current.Y != mouseState.Previous.Y;
        public int DeltaScroll => mouseState.Current.ScrollWheelValue - mouseState.Previous.ScrollWheelValue;

        public bool IsMouseButtonPressed(MouseButton button) => mouseState.Current[button];
        public bool IsMouseButtonHit(MouseButton button) => mouseState.Current[button] && !mouseState.Previous[button];
        public bool IsMouseButtonReleased(MouseButton button) => !mouseState.Current[button] && mouseState.Previous[button];

        public bool LeftMousePressed => IsMouseButtonPressed(MouseButton.Left);
        public bool LeftMouseHit => IsMouseButtonHit(MouseButton.Left);
        public bool LeftMouseReleased => IsMouseButtonReleased(MouseButton.Left);

        public bool RightMousePressed => IsMouseButtonPressed(MouseButton.Right);
        public bool RightMouseHit => IsMouseButtonHit(MouseButton.Right);
        public bool RightMouseReleased => IsMouseButtonReleased(MouseButton.Right);

        public bool MiddleMousePressed => IsMouseButtonPressed(MouseButton.Middle);
        public bool MiddleMouseHit => IsMouseButtonHit(MouseButton.Middle);
        public bool MiddleMouseReleased => IsMouseButtonReleased(MouseButton.Middle);

        public bool IsMouseInRectangle(System.Drawing.Rectangle rect) => rect.Contains(mouseState.Current.X, mouseState.Current.Y);
    }
}