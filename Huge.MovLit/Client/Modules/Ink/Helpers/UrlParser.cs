using Microsoft.AspNetCore.Components;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Huge.Ink
{
    public static class UrlParser
    {

        public static string ParseTagUrl(IEnumerable<string> tags, string prefix, NavigationManager nav, PageState pageState = null, bool useUserFolder = false)
        {
            // look for a tag that starts with the prefix (case insensitive) "lottie:" or "image:"
            var tag = tags.FirstOrDefault(t => t.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

            // if no tag found, return empty string
            if (string.IsNullOrWhiteSpace(tag))
            {
                return string.Empty;
            }

            var value = tag.Substring(prefix.Length).Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            // Case 1: explicit absolute URL
            if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            // Case 2: User folder handling (for lottie and images)
            if (useUserFolder && pageState?.User?.UserId != null)
            {
                var userId = pageState.User.UserId;
                
                // Handle tilde (~) as user folder shorthand
                if (value.StartsWith("~") && userId != -1)
                {
                    // Replace ~ with /files/Users/{UserId}
                    // Handle both ~/file.lottie and ~file.lottie
                    if (value.StartsWith("~/"))
                    {
                        value = $"/files/Users/{userId}{value.Substring(1)}";
                    }
                    else
                    {
                        value = $"/files/Users/{userId}/{value.Substring(1)}";
                    }
                }
                // For relative paths without ~ or /, prepend user folder
                else if (!value.StartsWith("/") && !value.Contains(":") && userId != -1)
                {
                    value = $"/files/Users/{userId}/{value}";
                }
            }
            // Case 3: app-relative (~/files...) - non-user folder scenario
            else if (value.StartsWith("~"))
            {
                // turn "~/files/..." into an absolute URL based on NavigationManager.BaseUri
                return nav.BaseUri.TrimEnd('/') + value[1..];
            }
            
            // Case 4: Absolute path (/files...)
            if (value.StartsWith("/"))
            {
                return nav.BaseUri.TrimEnd('/') + value;
            }

            // Case 5: Assume external URL, add https://
            return $"https://{value}";
        }
    }
}
