using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.UI;
using InkRun = global::Ink.Runtime;

namespace Huge.Ink
{
    public class InkVariables
    {
        public static void SetInitialUrl(InkRun.Story story, SiteState siteState)
        {
            if (story.variablesState.GlobalVariableExistsWithName("initialUrl") && string.IsNullOrWhiteSpace(story.variablesState["initialUrl"] as string))
            {
                var lottie = siteState.Properties.Lottie;
                var image = siteState.Properties.Image;

                var source = !string.IsNullOrWhiteSpace(lottie) ? lottie
                           : !string.IsNullOrWhiteSpace(image) ? image
                           : null;

                if (!string.IsNullOrWhiteSpace(source))
                {
                    story.variablesState["initialUrl"] = source;
                }
            }
        }

        public static void SetPlayerName(InkRun.Story story, PageState pageState)
        {
            if (story.variablesState.GlobalVariableExistsWithName("player_name") && string.IsNullOrWhiteSpace(story.variablesState["player_name"] as string))
            {
                var playerName = pageState.User.Username;
                if (!string.IsNullOrWhiteSpace(playerName))
                {
                    story.variablesState["player_name"] = playerName;
                }
            }
        }
    }
}
