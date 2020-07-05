﻿using System.Numerics;
using ImGuiNET;
using UiHelpers;

namespace T3.Gui.Interaction
{
    /// <summary>
    /// Provides fence selection interaction
    /// </summary>
    public static class SelectionFence
    {
        public static States UpdateAndDraw(States state)
        {
            if (state == States.CompletedAsArea || state == States.CompletedAsClick)
                state = States.Inactive;

            if (state == States.Inactive)
            {
                if (ImGui.IsAnyItemHovered() || !ImGui.IsWindowHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup) || !ImGui.IsMouseClicked(ImGuiMouseButton.Left) || ImGui.GetIO().KeyAlt)
                    return state;
                
                _startPositionInScreen = ImGui.GetMousePos();
                return States.PressedButNotMoved;
            }

            if (ImGui.IsMouseReleased(0))
            {
                state = state == States.PressedButNotMoved ? States.CompletedAsClick : States.CompletedAsArea;
                BoundsInScreen = ImRect.RectBetweenPoints(_startPositionInScreen, ImGui.GetMousePos());
                return state;
            }

            var positionInScreen = ImGui.GetMousePos();

            if (state == States.PressedButNotMoved && (positionInScreen - _startPositionInScreen).LengthSquared() < 4)
                return state;

            BoundsInScreen = ImRect.RectBetweenPoints(_startPositionInScreen, ImGui.GetMousePos());

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(BoundsInScreen.Min, BoundsInScreen.Max, new Color(0.1f), 1);
            state = States.Updated;
            return state;
        }

        public enum SelectModes
        {
            Add = 0,
            Remove,
            Replace
        }

        public enum States
        {
            Inactive,
            PressedButNotMoved,
            Updated,
            CompletedAsArea,
            CompletedAsClick,
        }

        public static SelectModes SelectMode
        {
            get
            {
                var selectMode = SelectModes.Replace;
                if (ImGui.GetIO().KeyShift)
                {
                    selectMode = SelectModes.Add;
                }
                else if (ImGui.GetIO().KeyCtrl)
                {
                    selectMode = SelectModes.Remove;
                }

                return selectMode;
            }
        }

        public static ImRect BoundsInScreen;
        private static Vector2 _startPositionInScreen;
    }
}