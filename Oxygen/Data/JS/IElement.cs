using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxygen.Data.JS
{
    internal interface IElement
    {
        List<IElement> children { get; }
        Dictionary<string,string> attributes { get; }
        string id { get; set; }
        string? innerText { get; set; }

        int AddControl(Panel panel, int y);

    }
}
