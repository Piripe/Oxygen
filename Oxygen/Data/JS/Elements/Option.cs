using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Drawing;
using Oxygen.Modules;
using System.Windows.Forms;

namespace Oxygen.Data.JS.Elements
{
    internal class Option : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id", ""); set => attributes.SetOrAdd("id", value); }
        public string value
        {
            get => attributes.GetOrDefault("value", "0"); set
            {
                attributes.SetOrAdd("value", value.ToString());
            }
        }
        private string? innerText_;
        public string? innerText { get => innerText_; set
            {
                innerText_ = value;
                if (innerTextChanged != null)
                {
                    innerTextChanged.Invoke(this, EventArgs.Empty);
                }
            } }
        internal event EventHandler? innerTextChanged;

        internal Option(XElement element)
        {
            attributes = new Dictionary<string, string>();
            foreach (XAttribute attr in element.Attributes())
            {
                attributes.Add(attr.Name.ToString(), attr.Value);
            }
            if (!attributes.ContainsKey("value")) attributes.Add("value", (element.Parent??new XElement("","")).Elements().ToList().IndexOf(element).ToString());
            children = new List<IElement>();

            innerText = element.Value;
        }
        internal Option(string value,string innerText = "")
        {
            attributes = new Dictionary<string, string>();
            attributes.Add("value",value);
            children = new List<IElement>();

            this.innerText = innerText;
        }
        public int AddControl(Panel panel,int y)
        {
            return 0;
        }
        public override string ToString()
        {
            return innerText??id;
        }
    }
}
