using System.Collections.Generic;

namespace _Main._Scripts.GameDatas
{
    public class GameData
    {
        public List<string> CreatedWords { get; private set; } = new();
        public int PlayerPoints;
        public int PCPoints;
    }
}