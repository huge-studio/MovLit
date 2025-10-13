using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Huge.Ink
{
    public static class UrlParser
    {

        public static string ParseTagUrl(IEnumerable<string> tags, string prefix, NavigationManager nav)
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

            // Case 2: app-relative (~/files...)
            if (value.StartsWith("~"))
            {
                // turn "~/files/..." into an absolute URL based on NavigationManager.BaseUri
                return nav.BaseUri.TrimEnd('/') + value[1..];
            }
            // Case 3: showLottie method writen as showLottie("/files...")
            if (value.StartsWith("/"))
            {
                // Case 3: showLottie method writen as showLottie("/files...")
                return nav.BaseUri.TrimEnd('/') + value;
            }

            // Case 4: Add https://
            return $"https://{value}";
        }
    }
}
