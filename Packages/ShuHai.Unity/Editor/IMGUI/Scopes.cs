using System;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Editor.IMGUI
{
    public struct IndentScope : IDisposable
    {
        public bool Disposed { get; private set; }

        public int Level { get; }

        public IndentScope(int level = 1)
        {
            Disposed = false;

            Level = level;

            EditorGUI.indentLevel += Level;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            EditorGUI.indentLevel -= Level;

            Disposed = true;
        }
    }

    public struct FoldoutHeaderGroupScope : IDisposable
    {
        public bool Disposed { get; private set; }

        public FoldoutHeaderGroupScope(ref bool foldout, string content,
            GUIStyle style = null, Action<Rect> menuAction = null, GUIStyle menuIcon = null)
        {
            Disposed = false;

            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, content, style, menuAction, menuIcon);
        }

        public FoldoutHeaderGroupScope(bool foldout, GUIContent content,
            GUIStyle style = null, Action<Rect> menuAction = null, GUIStyle menuIcon = null)
        {
            Disposed = false;

            EditorGUILayout.BeginFoldoutHeaderGroup(foldout, content, style, menuAction, menuIcon);
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            EditorGUILayout.EndFoldoutHeaderGroup();

            Disposed = true;
        }
    }
}
