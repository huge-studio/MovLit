using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huge.MovLit.Enums
{
    public static class StoryTags
    {
        public const string Adventure = "Adventure";
        public const string Mystery = "Mystery";
        public const string Fantasy = "Fantasy";
        public const string SciFi = "Sci-Fi";
        public const string Horror = "Horror";
        public const string Romance = "Romance";
        public const string Thriller = "Thriller";
        public const string Puzzle = "Puzzle";
        public const string ChoicesMatter = "Choices-Matter";

        public static List<string> GetAllTags => new()
        {
        Adventure,
        Mystery,
        Fantasy,
        SciFi,
        Horror,
        Romance,
        Thriller,
        Puzzle,
        ChoicesMatter
    };
    }

}
