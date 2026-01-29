using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.UI;
using InkRun = global::Ink.Runtime;

namespace Huge.Ink
{
    public class InkVariables
    {
        public static void SetPlayerName(InkRun.Story story, PageState pageState)
        {
            if (story.variablesState.GlobalVariableExistsWithName("player_name") && string.IsNullOrWhiteSpace(story.variablesState["player_name"] as string))
            {
                var playerName = pageState?.User?.Username;
                if (!string.IsNullOrWhiteSpace(playerName))
                {
                    story.variablesState["player_name"] = playerName;
                }
            }
        }
    }
}
