using OpenTK;
using System.Collections.Generic;
using VectorEngine.Core.Rendering.LowLevel;

namespace VectorEngine.Core.Rendering
{
    public static class TextMeshGenerator
    {
        public static Mesh RegenerateMesh(string text, FontData font, Mesh mesh, double fontSize, double lineHeight)
        {
            char[] chars = text.ToCharArray();

            Queue<Vector3d> positions = new Queue<Vector3d>();
            Queue<Vector2d> textureCoords = new Queue<Vector2d>();
            Queue<int> triangles = new Queue<int>();

            double cursorPosition = 0;
            int lineNumber = 0; 
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\n')
                {
                    cursorPosition = 0;
                    lineNumber++;
                }
                else if (font.characterData.ContainsKey(chars[i]))
                {
                    FontData.CharacterData charData = font.characterData[chars[i]];
                    triangles.Enqueue(positions.Count);
                    triangles.Enqueue(positions.Count + 1);
                    triangles.Enqueue(positions.Count + 2);
                    triangles.Enqueue(positions.Count + 2);
                    triangles.Enqueue(positions.Count + 3);
                    triangles.Enqueue(positions.Count);

                    positions.Enqueue(new Vector3d(cursorPosition + charData.CharacterOffset.X * fontSize, -lineNumber * lineHeight * fontSize - charData.CharacterOffset.Y * fontSize, 0));
                    positions.Enqueue(new Vector3d(cursorPosition + charData.CharacterOffset.X * fontSize, -lineNumber * lineHeight * fontSize - charData.CharacterOffset.Y * fontSize - charData.Height * fontSize, 0));
                    positions.Enqueue(new Vector3d(cursorPosition + charData.CharacterOffset.X * fontSize + charData.Width * fontSize, -lineNumber * lineHeight * fontSize - charData.CharacterOffset.Y * fontSize - charData.Height * fontSize, 0));
                    positions.Enqueue(new Vector3d(cursorPosition + charData.CharacterOffset.X * fontSize + charData.Width * fontSize, -lineNumber * lineHeight * fontSize - charData.CharacterOffset.Y * fontSize, 0));

                    textureCoords.Enqueue(charData.CharacterTopLeft);
                    textureCoords.Enqueue(new Vector2d(charData.CharacterTopLeft.X, charData.CharacterBottomRight.Y));
                    textureCoords.Enqueue(charData.CharacterBottomRight);
                    textureCoords.Enqueue(new Vector2d(charData.CharacterBottomRight.X, charData.CharacterTopLeft.Y));

                    cursorPosition += charData.XAdvance * fontSize;
                }
                else
                {
                    cursorPosition += 20 * fontSize;
                }
            }

            return RenderDataLoader.LoadMeshData2d(positions.ToArray(), triangles.ToArray(), textureCoords.ToArray(), mesh.VaoID);
        }
    }
}
