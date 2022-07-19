using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Oxygen.Modules;

namespace Oxygen.Data.JS
{
    internal class Document : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id", ""); set => attributes.SetOrAdd("id", value); }
        public string? innerText { get; set; }

        internal Document(XElement element, Jint.Engine JSEngine)
        {
            JSEngine.SetValue("document", this);

            attributes = new Dictionary<string, string>();
            foreach(XAttribute attr in element.Attributes())
            {
                attributes.Add(attr.Name.ToString(), attr.Value);
            }
            children = new List<IElement>();
            foreach (XElement childElement in element.Elements())
            {
                switch (childElement.Name.ToString())
                {
                    case "category":
                        Elements.Category newCategory = new Elements.Category();
                        children.Add(newCategory);
                        newCategory.InitCategory(childElement, JSEngine);
                        break;
                    case "script":
                        XAttribute? srcFile = childElement.Attributes().ToList().Find((x) => x.Name == "src");
                        if (srcFile == null)
                        {
                            try
                            {
                                JSEngine.Execute(childElement.Value);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.Error(ex.Message, "settings.xml");
                            }
                        }
                        else
                        {
                            try
                            {
                                JSEngine.Execute(File.ReadAllText(Path.Combine(Path.GetTempPath(), "Oxygen", "skin", srcFile.Value)));
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.Error(ex.Message, srcFile.Value, string.Join(":", ((ex.InnerException??new Exception()).StackTrace??"?:?:?").Split(':')[^2..^0]));
                            }
                        }
                        break;
                }
            }

        }

        public IElement? getElementById(string id)
        {
            IElement? getElementByIdIn(IEnumerable<IElement> elements)
            {
                foreach (IElement x in elements)
                {
                    if (x.id == id)
                    {
                        return x;
                    }
                    else
                    {
                        if (x.children != null)
                        {
                            if (x.children.Count > 0)
                            {
                                IElement? result = getElementByIdIn(x.children);
                                if ( result != null)
                                {
                                    return result;
                                }
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                return null;
            }

            return getElementByIdIn(children);

        }
        public int AddControl(Panel panel, int y)
        {
            return 0;
        }
    }
}
