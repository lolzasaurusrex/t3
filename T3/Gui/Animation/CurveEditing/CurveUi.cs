﻿using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using T3.Core.Animation;
using T3.Gui.Graph;
using T3.Gui.Windows.TimeLine;

namespace T3.Gui.Animation.CurveEditing
{
    /// <summary>
    /// A graphical representation of a <see cref="CurveEditing"/>. Handles style and selection states.
    /// </summary>
    public class CurveUi
    {
        public bool IsHighlighted { get; set; }
        public List<CurvePointUi> CurvePoints { get; set; }

        public CurveUi(Curve curve, ICanvas canvas)
        {
            _curve = curve;
            _canvas = canvas;
            _drawlist = ImGui.GetWindowDrawList();

            CurvePoints = new List<CurvePointUi>();
            foreach (var pair in curve.GetPoints())
            {
                var key = pair.Value;
                CurvePoints.Add(new CurvePointUi(key, curve, canvas));
            }
        }


        public void Draw()
        {
            foreach (var p in CurvePoints)
            {
                p.Draw();
            }
            DrawLine();
        }


        private void DrawLine()
        {
            var step = 3f;
            var width = (float)ImGui.GetWindowWidth();

            double dU = _canvas.InverseTransformDirection(new Vector2(step, 0)).X;
            double u = _canvas.InverseTransformPosition(_canvas.WindowPos).X;
            float x = _canvas.WindowPos.X;

            var steps = (int)(width / step);
            if (_points.Length != steps)
            {
                _points = new Vector2[steps];
            }

            for (int i = 0; i < steps; i++)
            {
                _points[i] = new Vector2(
                    x,
                    _canvas.TransformPosition(new Vector2(0, (float)_curve.GetSampledValue(u))).Y
                    );

                u += dU;
                x += step;
            }
            _drawlist.AddPolyline(ref _points[0], steps, Color.Gray, false, 1);
        }

        private Curve _curve;
        private static Vector2[] _points = new Vector2[2];
        private ICanvas _canvas;
        private ImDrawListPtr _drawlist;
    }
}
