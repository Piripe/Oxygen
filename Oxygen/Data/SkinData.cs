using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Oxygen.Data
{
    internal class SkinData
    {
        public List<string> Vars { get; set; } = new List<string>();
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public string DownloadLink { get; set; } = "";
        public string InfosLink { get; set; } = "";

    }
}
