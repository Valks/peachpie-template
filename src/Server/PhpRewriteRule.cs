using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Rewrite.Logging;

namespace Server
{
    public class PhpRewriteRule : IRule
    {
        public PathString Prefix { get; }
        public bool SkipRemainingRules { get; }

        public PhpRewriteRule(string extension, string prefix, bool skipRemainingRules)
        {
            if(string.IsNullOrEmpty(prefix))
            {
                throw new ArgumentException(nameof(prefix));
            }

            if(prefix.StartsWith("/", StringComparison.Ordinal))
            {
                Prefix = prefix;
            }
            else
            {
                Prefix = new PathString($"/{prefix}");
            }
            SkipRemainingRules = skipRemainingRules;
        }

        public virtual void ApplyRule(RewriteContext context)
        {
            var path = context.HttpContext.Request.Path;
            bool match = false;
            if(path == PathString.Empty)
            {
                match = true;
            }
            else
            {
                var lastSegment = path.ToString().Substring(path.ToString().LastIndexOf("/", StringComparison.Ordinal));
                match = !lastSegment.Contains(".", StringComparison.Ordinal) || lastSegment.Contains(".php", StringComparison.Ordinal);
            }

            if(match)
            {
                var request = context.HttpContext.Request;

                if(SkipRemainingRules)
                {
                    context.Result = RuleResult.SkipRemainingRules;
                }

                request.Path = new PathString(new PathString(Prefix).Add(path));
            }
        }
    }
}