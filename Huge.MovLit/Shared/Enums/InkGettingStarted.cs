using Oqtane.Models;
using Oqtane.Shared;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text;

namespace Huge.MovLit.Enums
{
    public static class InkGettingStarted
    {
        public const string Title = "Welcome to Ink";
        public const string Description = "This starter story shows how to write basic Ink: variables, choices, diverts, tags, and simple navigation.";
        public const string Body =
@"
// Tags are lines beginning with #. Your module reads these.
// Use a lottie or image tag to show media for this scene.

// For local media: paste the URL shown by the uploader, for example:
//# lottie: /files/Users/1/No Internet Connection.lottie
//# image: /files/Public/image.png

// For external media: do not include https://. The system will prepend it.
// Example external values:
//# lottie: lottie.host/your-lottie-id/animation.json
//# image: cdn.example.com/images/cover.png

// --- Variables ---
VAR player_name = """"        // empty string by default
VAR courage = 0               // numbers start at 0 unless you set them
VAR clues = 0
VAR initialUrl = ""lottie.host/7fc1b618-9bfe-4f08-883f-75fba6a72c0c/GkXnMVkqkH.lottie""

->intro

=== intro ===
# lottie: {initialUrl}
Welcome. This short story teaches core Ink pieces while it runs. If you are an authenticated user, we can set your username throughout the story. Other wise we will call you Traveler. Nice to meet you, { player_name == """": Traveler | {player_name}}.

* [Get Started]
    ->getting_started

=== getting_started ===
# lottie: lottie.host/f8c8482a-3fdc-489e-a5d7-f57bcdf2318e/vrmcFvuJ2J.lottie
You will see choices below. Click a choice to change variables and move the story forward.

// The [label] form shows text in the menu but does not print it after selection.

* [Pick up the lantern]            // one-time choice
    ~clues += 1                     // increment a number variable
    The lantern flares to life. Clues is now {clues}.
    -> path_fork

* [Leave the lantern]
    Cautious. That is fine. Courage will help you later.
    ->path_fork

= path_fork
// A stitch is a labeled section inside this knot.
# lottie: lottie.host/1a87968b-52c8-4715-baf3-cfef3fda93c6/e4R02RKJVR.lottie
You step onto an old trail. Each choice below demonstrates behavior.

+ [Be bold and shout into the trees]
    ~courage += 1
    Your voice echoes. Courage is now {courage}.
    -> look_around

+ [Move quietly and watch]
    A smart approach.
    ->look_around

= look_around
// Simple conditional text:
 {courage > 0: The echo makes you smile. You feel a little braver. | Silence. You keep your focus.}
Now change the media tags at the top to your own.
For local files, use the uploader and copy the shown URL.
For external files, omit https:\/\/ and the system will add it.

You can also repeat a choice until a condition hides it.

+ [Practice courage(+1 each time)] {courage < 3}
            ~courage += 1
    Practicing... Courage is now {courage}.
    -> look_around

Time to wrap up. Choose an ending.

+ [Show my current stats]
    You have Courage = {courage}
            and Clues = {clues}.
    Try another ending below.
    -> endings

+ [Add a clue and continue]
    ~clues += 1
    Clues tick up to {clues}. Onward.
    ->endings

= endings
Thanks for trying Ink. You used:\\
Variables\\
Choices\\
Diverts\\
Tags\\
Conditionals\\

// Navigation examples. Your host provides navigateUrl.
// Edit the paths for your site.

+ [View other stories]
    ~navigateUrl(""/dashboard"")
    ->END

+ [Watch this story again]
    ~navigateUrl(""\#"")
    -> END
";
    }
}
