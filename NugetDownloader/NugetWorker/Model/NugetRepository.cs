using System;
using System.Collections.Generic;
using System.Text;

namespace NugetWorker
{
    public class NugetRepository
    {
        public int Order { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        //
        // Summary:
        //     User name
        //
        // Summary:
        //     Indicates if password is stored in clear text.
        public bool IsPasswordClearText { get; set; }
        public string Username { get; set; }
        //
        // Summary:
        //     Retrieves password in clear text. Decrypts on-demand.
        public string Password { get; set; }
        //
        // Summary:
        //     Associated source ID
        public string Source { get; set; }
    }
}
