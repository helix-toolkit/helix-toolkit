using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillboardDemo
{
    public static class FlagsCollection
    {
        public static Flag[] Flags;
        const float offSetX = 1 / 8f;
        const float offSetY = 1 / 8f;
        const float borderThickness = 0.02f;
        static FlagsCollection()
        {
            Flags = new Flag[]
            {
                new Flag("China", new Vector3(1.26659f,-3.595175f,2.323998f), GetCoordRowColumn(4, 7)),
                new Flag("USA", new Vector3(0.6496152f, 3.337777f, 2.847751f), GetCoordRowColumn(5, 2)),
                new Flag("Japan", new Vector3(2.708371f, -2.284147f, 2.697974f), GetCoordRowColumn(2, 0)),
                new Flag("Canada", new Vector3(0.6104228f, 2.78573f, 3.415769f), GetCoordRowColumn(0, 4)),
                new Flag("Brazil", new Vector3(), GetCoordRowColumn(0, 5)),
                new Flag("South Korea", new Vector3(), GetCoordRowColumn(2, 1)),
                new Flag("Australia", new Vector3(), GetCoordRowColumn(3, 0)),
                new Flag("United Kingdom", new Vector3(), GetCoordRowColumn(7, 0)),
                new Flag("Russia", new Vector3(), GetCoordRowColumn(4, 6)),
                new Flag("Turkey", new Vector3(), GetCoordRowColumn(4, 3)),
            };
        }

        public static Vector4 GetCoordRowColumn(int row, int column)
        {
            return new Vector4(column * offSetX + borderThickness, row * offSetY + borderThickness,
                (column + 1) * offSetX - borderThickness, (row + 1) * offSetY - borderThickness);
        }
    }

    public class Flag : ImageInfo
    {
        public string Name { get; }

        public Flag(string name, Vector3 pos, Vector4 coord)
        {
            Name = name;
            Position = pos;
            UV_TopLeft = new Vector2(coord.X, coord.Y);
            UV_BottomRight = new Vector2(coord.Z, coord.W);
            Width = 0.5f;
            Height = 0.4f;
        }
    }
}
