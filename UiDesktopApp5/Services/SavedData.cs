using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Documents;
using Windows.Devices.PointOfService;

namespace Eon.Services.Yes
{
    class SavedData
    {
        // Saved Data So We Dont Need To Keep Calling Files And Stuff

        public static string Token { get; set; } // Unproper :Gr:
        public static string Email { get; set; }
        public static string Password { get; set; }

        public static List FortniteBuilds { get; set; }
        public static List<JObject> RececdsBuilds { get; set; } = new List<JObject>();

        public static Dictionary<int, byte[]> NewsBBG { get; set; } = new Dictionary<int, byte[]>();

        public static dynamic Da;
    }
}
