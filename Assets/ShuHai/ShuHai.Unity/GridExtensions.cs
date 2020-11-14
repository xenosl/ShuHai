using UnityEngine;

namespace ShuHai.Unity
{
    public static class GridExtensions
    {
        public static Vector3 Align(this Grid self, Vector3 position)
        {
            var cell = self.WorldToCell(position);
            var aligned = self.CellToWorld(cell);
            position = aligned;
            return position;
        }
    }
}