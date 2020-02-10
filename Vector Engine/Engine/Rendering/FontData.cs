using OpenTK;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using VectorEngine.Core.Rendering.LowLevel;
using VectorEngine.Engine;

namespace VectorEngine.Core.Rendering
{
    public class FontData
    {
        public int TextureId;
        public Dictionary<char, CharacterData> characterData = new Dictionary<char, CharacterData>();

        public struct CharacterData
        {
            public Vector2d CharacterOffset;
            public Vector2d CharacterTopLeft;
            public Vector2d CharacterBottomRight;
            public double Height;
            public double Width;
            public double XAdvance;
        }

        public FontData(string textureLocation, string fontLocation, int resolution)
        {
            TextureId = RenderDataLoader.LoadTexture(textureLocation);

            ParseFontFile(fontLocation, resolution);
        }

        private void ParseFontFile(string location, int resolution)
        {
            if (File.Exists(location) == false)
            {
                Debug.Log($"Error while reading font file: File does not exist at path '{location}' !");
                return;
            }

            using (FileStream stream = new FileStream(location, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Read these lines, simply dumping the results.
                    // These lines are just data we don't need and can get rid of.
                    for (int i = 0; i < 3; i++)
                    {
                        reader.ReadLine();
                    }

                    string characterCount = reader.ReadLine();
                    if(characterCount.Contains("chars count="))
                    {
                        int linesToRead = int.Parse(characterCount.Substring(12));

                        string readLine;
                        string[] splitResult;
                        Queue<int> readData = new Queue<int>();
                        for (int i = 0; i < linesToRead; i++)
                        {
                            readLine = reader.ReadLine();

                            splitResult = Regex.Split(readLine, @"\D+");

                            int addedCount = 0;
                            for (int z = 0; z < splitResult.Length; z++)
                            {
                                // We only want the first 8 entries on a line, any more is garbage for us
                                if (addedCount == 8)
                                    break;

                                // Make sure the string isn't empty, and add the value to the read data
                                if (!string.IsNullOrEmpty(splitResult[z]))
                                {
                                    addedCount++;
                                    readData.Enqueue(int.Parse(splitResult[z]));
                                }
                            }
                        }

                        // While we have enough data to construct a data entry
                        while(readData.Count >= 8)
                        {
                            char character = (char)readData.Dequeue();
                            double texTopLeftX = readData.Dequeue() / (double)resolution;
                            double texTopLeftY = readData.Dequeue() / (double)resolution;
                            double width = readData.Dequeue();
                            double height = readData.Dequeue();
                            double xOffset = readData.Dequeue();
                            double yOffset = readData.Dequeue();
                            double xAdvance = readData.Dequeue();

                            characterData.Add(character, new CharacterData()
                            {
                                CharacterTopLeft = new Vector2d(texTopLeftX, texTopLeftY),
                                CharacterBottomRight = new Vector2d(width / (double)resolution + texTopLeftX, height / (double)resolution + texTopLeftY),
                                CharacterOffset = new Vector2d(xOffset, yOffset),
                                Height = height,
                                Width = width,
                                XAdvance = xAdvance
                            });
                        }
                    }
                    else
                    {
                        Debug.Log($"Error while reading font file: File is invalid, aborting!");
                        return;
                    }
                }
            }
        }
    }
}
