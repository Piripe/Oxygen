using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Oxygen.Modules;
using System.IO;

namespace Oxygen.Data.JS.Elements
{
    internal class Category : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id", ""); set => attributes.SetOrAdd("id", value); }
        private string title_ = "";
        public string title { get => title_; set {
                title_ = value;
                if (Global.Editor != null && Global.Document != null) Global.Editor.categoriesListBox.Items[Global.Document.children.IndexOf(this)] = value;
            } }
        public string? innerText { get; set; }
        private Control? control { get; }

        internal Category()
        {
            attributes = new Dictionary<string, string>();
            children = new List<IElement>();

        }
            internal void InitCategory(XElement element, Jint.Engine JSEngine)
            {
            foreach (XAttribute attr in element.Attributes())
            {
                attributes.Add(attr.Name.ToString(), attr.Value);
            }
            foreach (XElement childElement in element.Elements())
            {
                switch (childElement.Name.ToString())
                {
                    case "script":
                        XAttribute? srcFile = childElement.Attributes().ToList().Find((x) => x.Name == "src");
                        if (srcFile == null)
                        {
                            JSEngine.Execute(childElement.Value);
                        }
                        else
                        {
                            JSEngine.Execute(File.ReadAllText(Path.Combine(Path.GetTempPath(), "Oxygen", "skin", srcFile.Value)));
                        }
                        break;
                    case "title":
                        title = childElement.Value;
                        break;
                    case "track":
                        children.Add(new Track(childElement));
                        break;
                    case "color":
                        children.Add(new Color(childElement));
                        break;
                    case "checkbox":
                        children.Add(new Checkbox(childElement));
                        break;
                    case "image":
                        children.Add(new Image(childElement));
                        break;
                    case "combo":
                        children.Add(new Combo(childElement));
                        break;
                    case "link":
                        children.Add(new Link(childElement));
                        break;
                    default:
                        children.Add(new Label(childElement));
                        break;
                }
            }
        }

        public int AddControl(System.Windows.Forms.Panel panel, int y)
        {
            return 0;
        }
    }
}
