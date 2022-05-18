#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DOTSTemplate
{
    [CustomEditor(typeof(CurvedPathProvider))]
    public class CurvedPathProviderEditor : Editor
    {
        private static readonly Color polylineColor = new Color32(0, 82, 208, 255);
        private static readonly float polylineComplexity = 0.5f;
        private static readonly float polylineWidth = 10f;
        private static readonly float pointButtonSize = 25;
        private static readonly float radiusHandleSize = 0.1f;
        private static readonly float addButtonSize = 15;
        
        private CurvedPathProvider path;
        private Vector3[] polylinePointsArray = new Vector3[16];
        private HashSet<int> selectedPoints = new HashSet<int>();
        
        private GUIStyle pointButtonStyle;
        private GUIStyle addButtonStyle;
        
        public Texture2D PointNormal;
        public Texture2D PointSelected;
        public Texture2D AddNormal;

        private void OnEnable()
        {
            path = (CurvedPathProvider) target;
            
            if (PointNormal != null)
            {
                pointButtonStyle = new GUIStyle();
                pointButtonStyle.normal.background = PointNormal;
            }
            else
            {
                pointButtonStyle = EditorStyles.miniButton;
            }
            
            if (AddNormal != null)
            {
                addButtonStyle = new GUIStyle();
                addButtonStyle.normal.background = AddNormal;

                addButtonStyle.alignment = TextAnchor.MiddleCenter;
            }
            else
            {
                addButtonStyle = EditorStyles.miniButton;
            }
        }

        private void OnSceneGUI()
        {
            Handles.matrix = path.transform.localToWorldMatrix;
            Handles.color = polylineColor;

            using (var points = path.GetPath(polylineComplexity))
            {
                if (polylinePointsArray.Length < points.Length) 
                    polylinePointsArray = new Vector3[points.Length];
                
                NativeArray<Vector3>.Copy(
                    points.AsArray().Reinterpret<Vector3>(), 
                    polylinePointsArray, 
                    points.Length);
                
                Handles.DrawAAPolyLine(polylineWidth, points.Length, polylinePointsArray);
            }
            
            for (var index = 1; index < path.Points.Length; index++)
            {
                Handles.DrawDottedLine(path.Points[index - 1].Position, path.Points[index].Position, 3);
            }

            for (var index = 0; index < path.Points.Length; index++)
            {
                ref var point = ref path.Points[index];
                if (!selectedPoints.Contains(index))
                {
                    HandleUnselectedPoint(ref point, index);
                }
                else
                {
                    HandleSelectedPoint(ref point, index);
                }
                
                HandleInsertButton(ref point, index);
            }

            Tools.hidden = selectedPoints.Count > 0;
            if (selectedPoints.Count > 0)
            {
                if (Event.current.TryUseMouseDown(0))
                {
                    selectedPoints.Clear();
                }
                if (Event.current.TryUseKeyDown(KeyCode.Delete))
                {
                    DeleteSelectedPoints();
                }
            }
        }
        
        private void HandleInsertButton(ref PointWithRadius point, int index)
        {
            using (this.DrawGUI())
            {
                var pathPoint = (Vector3) point.Position;
                if (index == 0)
                {
                    var nextPoint = (Vector3) path.Points[1].Position;
                    var dir = (pathPoint - nextPoint).normalized;
                    var newPointPosition = pathPoint + dir * 2;

                    var pos = HandleUtility.WorldToGUIPoint(newPointPosition);
                    if (GUI.Button(pos.ToCenteredRect(15), GUIContent.none, addButtonStyle))
                    {
                        InsertPoint(0, newPointPosition);
                    }
                }
                else if (index > 0)
                {
                    var previousPoint = (Vector3) path.Points[index - 1].Position;
                    var newPointPosition = (previousPoint + pathPoint) * 0.5f;
                    var pos = HandleUtility.WorldToGUIPointWithDepth(newPointPosition);
                    if (GUI.Button(pos.ToCenteredRect(15), GUIContent.none, addButtonStyle))
                    {
                        InsertPoint(index, newPointPosition);
                    }
                    
                    if (index == path.Points.Length - 1)
                    {
                        var dir = (pathPoint - previousPoint).normalized;
                        newPointPosition = pathPoint + dir * 2;

                        pos = HandleUtility.WorldToGUIPoint(newPointPosition);
                        if (GUI.Button(pos.ToCenteredRect(15), GUIContent.none, addButtonStyle))
                        {
                            InsertPoint(path.Points.Length, newPointPosition);
                        }
                    }
                }
            }
        }
        
        private void InsertPoint(int index, Vector3 position)
        {
            Undo.RecordObject(path, "Insert point");
            path.Points = path.Points.Insert(index, new PointWithRadius(position, 0));
            selectedPoints.Clear();
            selectedPoints.Add(index);
        }
        
        private void DeleteSelectedPoints()
        {
            Undo.RecordObject(path, "Delete points");
            foreach (var index in selectedPoints.OrderByDescending(k => k))
            {
                path.Points = path.Points.RemoveAt(index);
            }
            
            selectedPoints.Clear();
        }

        private void HandleSelectedPoint(ref PointWithRadius point, int index)
        {
            var pathPoint = (Vector3) point.Position;

            if (PointSelected != null)
            {
                var pointScreenPosition = HandleUtility.WorldToGUIPointWithDepth(pathPoint);
                using (this.DrawGUI())
                {
                    GUI.DrawTexture(pointScreenPosition.ToCenteredRect(pointButtonSize), PointSelected);
                }
            }

            var newPosition = Handles.DoPositionHandle(pathPoint, Quaternion.identity);
            var deltaPosition = newPosition - pathPoint;
            if (deltaPosition.sqrMagnitude > float.Epsilon)
            {
                MoveSelectedPoints(deltaPosition);
            }

            if (index > 0 && index < path.Points.Length - 1)
            {
                var radius = point.GetRadiusOrDefault(path.DefaultRadius);

                var p0 = (Vector3) path.Points[index - 1].Position;
                var p1 = pathPoint;
                var p2 = (Vector3) path.Points[index + 1].Position;
                var d1 = (p0 - p1).normalized;
                var d2 = (p2 - p1).normalized;

                var halfAngle = Vector3.Angle(d1, d2) * 0.5f * Mathf.Deg2Rad;
                var sinAngle = Mathf.Sin(halfAngle);
                var toCenter = radius / sinAngle;
                var toTouchPoints = Mathf.Abs(toCenter * Mathf.Cos(halfAngle));

                var s = (d2 + d1).normalized;
                var radiusCenter = (p1 + s * toCenter);
                var p01 = p1 + d1 * toTouchPoints;
                var p12 = p1 + d2 * toTouchPoints;
                
                Handles.DrawLine(radiusCenter, p01);
                Handles.DrawLine(radiusCenter, p12);
                
                var size = HandleUtility.GetHandleSize(pathPoint) * radiusHandleSize;
                var newCenter = Handles.FreeMoveHandle(radiusCenter, 
                    Quaternion.identity, size, Vector3.one * 0.25f,
                    Handles.RectangleHandleCap);
                
                if (newCenter != radiusCenter)
                {
                    Undo.RecordObject(path, "Update radius");
                    point.Radius = Vector3.Distance(pathPoint, newCenter) * sinAngle;
                }
            }
        }
        
        private void MoveSelectedPoints(Vector3 delta)
        {
            Undo.RecordObject(path, "Move control points");
            foreach (var selectedPoint in selectedPoints)
            {
                path.Points[selectedPoint].Position += (float3) delta;
            }
        }

        private void HandleUnselectedPoint(ref PointWithRadius point, int index)
        {
            var pointScreenPosition = HandleUtility.WorldToGUIPointWithDepth(point.Position);
            var pointScreenRect = pointScreenPosition.ToCenteredRect(pointButtonSize);
            using (this.DrawGUI())
            {
                if (GUI.Button(pointScreenRect, GUIContent.none, pointButtonStyle))
                {
                    if ((Event.current.modifiers & EventModifiers.Control) == 0)
                        selectedPoints.Clear();
                    selectedPoints.Add(index);
                }
            }
        }
    }
}

#endif