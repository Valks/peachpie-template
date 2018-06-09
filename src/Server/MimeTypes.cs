using System;
using Microsoft.AspNetCore.StaticFiles;

namespace Server
{
    public static class MimeTypes
    {
        public static FileExtensionContentTypeProvider GetKnownFileTypes()
        {
            var provider = new FileExtensionContentTypeProvider();

            provider.Mappings[".htm"]   = "text/html";
            provider.Mappings[".html"]  = "text/html";

            provider.Mappings[".css"]   = "application/css";
            provider.Mappings[".js"]    = "application/javascript";

            return provider;
        } 
    }
}