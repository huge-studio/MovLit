using Oqtane.Modules;
using Oqtane.Shared;
using System.Collections.Generic;
using InkRun = global::Ink.Runtime;
using Oqtane.UI;
using Microsoft.AspNetCore.Components;


// In keeping with the ink documentation, we will need to create a class that will contain the functions that we want to expose to the ink script.
// https://github.com/inkle/ink/blob/master/Documentation/RunningYourInk.md#external-functions


namespace Huge.Ink
{
    internal class InkFunctions
    {
        // get ink headers
        public static string GetHeaders()
        {
            string headers = @"// Begin Ink Headers added by InkFunctions.GetHeaders
                EXTERNAL playSound(soundUrl)
                EXTERNAL showImage(imageUrl)
                EXTERNAL showLottie(lottieUrl)
                EXTERNAL navigateUrl(url)
                EXTERNAL navigatePage(pageName)
                // End Ink Headers";

            return headers;
        }

        public static void BindExternalFunctions(InkRun.Story story, SiteState siteState, ModuleBase moduleBase, NavigationManager nav)
        {
            story.BindExternalFunction("playSound", (string url) => PlaySound(url, siteState));
            story.BindExternalFunction("showImage", (string url) => ShowImage(url, siteState, nav));
            story.BindExternalFunction("showLottie", (string url) => ShowLottie(url, siteState, nav));
            story.BindExternalFunction("navigateUrl", (string url) => NavigateUrl(url, moduleBase, nav));
            story.BindExternalFunction("navigatePage", (string name) => NavigatePage(name, moduleBase, nav));
        }

        /// <summary>
        /// ensure that the user state is in sync with the ink story variables.
        /// get the user state from the pagestate and set the ink variables
        /// set the sitestate properties when the ink variables change to keep the user state in sync
        /// </summary>
        /// <param name="story"></param>
        /// <param name="siteState"></param>
        /// <param name="pageState"></param>
        public static void SyncUserState(InkRun.Story story, SiteState siteState, PageState pageState)
        {
            var vars = story.variablesState;
            // look thru the list and set with any value found in the
            // pagestate user settings starting with "inkVariable:"
            foreach (var variable in vars)
            {
                var key = $"ink:{variable}";
                if (pageState.User.Settings.ContainsKey(key))
                {
                    vars[variable] = pageState.User.Settings[key];
                }

                // attach watchers to the variables to update the userstate when they change
                story.ObserveVariable(variable, (string varName, object newValue) =>
                {
                    var newKey = $"ink:{varName}";
                    pageState.User.Settings[newKey] = newValue.ToString();

                    // message any listening components that the inkvariable has changed
                    siteState.Properties.InkVariable = new
                        KeyValuePair<string, string>(varName, newValue.ToString());
                });
            }
        }

        // play sound
        public static void PlaySound(string soundUrl, SiteState siteState)
        {
            siteState.Properties.SoundUrl = soundUrl;
        }
        // show image
        public static void ShowImage(string imageUrl, SiteState siteState, NavigationManager nav)
        {
            imageUrl = UrlParser.ParseTagUrl(new List<string> { $"image:{imageUrl}" }, "image:", nav);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                siteState.Properties.Image = imageUrl;
            }
        }
        // show lottie
        public static void ShowLottie(string lottieUrl, SiteState siteState, NavigationManager nav)
        {
            lottieUrl = UrlParser.ParseTagUrl(new List<string> { $"lottie:{lottieUrl}" }, "lottie:", nav);
            if (!string.IsNullOrEmpty(lottieUrl))
            {
                siteState.Properties.Lottie = lottieUrl;
            }
        }
        // navigate url
        public static void NavigateUrl(string url, ModuleBase moduleBase, NavigationManager nav)
        {
            if (url == "#")
            {
                nav.NavigateTo(nav.Uri, true);
            }
            var page = moduleBase.NavigateUrl(url);
            nav.NavigateTo(page);
        }
        // navigate page
        public static void NavigatePage(string pageName, ModuleBase moduleBase, NavigationManager nav)
        {
            var page = moduleBase.NavigateUrl(pageName);
            nav.NavigateTo(page);
        }
    }
}
