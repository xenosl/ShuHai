using UnityEditor;

namespace ShuHai.Unity.Editor
{
    public static class SerializedObjectExtensions
    {
        public static SerializedProperty EnsureProperty(this SerializedObject self, string name)
        {
            Ensure.Argument.NotNullOrEmpty(name, nameof(name));

            var p = self.FindProperty(name);
            if (p == null)
                throw new SerializedPropertyNotFoundException(nameof(name));
            return p;
        }
    }
}
