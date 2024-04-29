using InstagramApiSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderApp.MVVM.Model
{
    public class LoginModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public IInstaApi? Api { get; set; }
    }
}
