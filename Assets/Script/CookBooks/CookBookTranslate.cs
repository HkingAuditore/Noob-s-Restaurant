using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using CookBooks;
using System.IO;
using System.Linq;

namespace CookBookUtilities
{
    public class CookBookTranslate : MonoBehaviour
    {

        public static CookBookBase LoadCookBookFromXML(string path)
        {
            CookBookBase bookBase = new CookBookBase();
            List<FormatNode> FormatCookNodes = new List<FormatNode>();

            if (File.Exists(path))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNodeList xmlNode = xmlDoc.SelectSingleNode("NodeCanvas/Nodes").ChildNodes;
                #region 遍历节点
                foreach (XmlElement ele in xmlNode)
                {
                    string type = ele.GetAttribute("type");
                    if (type == "rootCookBookNode")
                    {
                        RootCookNode node = new RootCookNode();
                        FormatRootNode formatRoot = new FormatRootNode();
                        formatRoot.Node = node;
                        FormatCookNodes.Add(formatRoot);
                        formatRoot.ID = ele.GetAttribute("ID");
                        bookBase.Info = new CookBoosInfo();
                        foreach (XmlElement subject in ele)
                        {
                            if (subject.Name == "Variable")
                            {
                                if (subject.GetAttribute("name") == "CookBookName")
                                {
                                    XmlNodeList objectNode = xmlDoc.SelectSingleNode("NodeCanvas/Objects").ChildNodes;
                                    foreach (XmlElement o in objectNode)
                                    {
                                        if (o.GetAttribute("refID") == subject.GetAttribute("refID"))
                                        {
                                            bookBase.Info.CookBookName = o.SelectSingleNode("string").InnerText;
                                        }
                                    }
                                }
                                else if (subject.GetAttribute("name") == "Description")
                                {
                                    XmlNodeList objectNode = xmlDoc.SelectSingleNode("NodeCanvas/Objects").ChildNodes;
                                    foreach (XmlElement o in objectNode)
                                    {
                                        if (o.GetAttribute("refID") == subject.GetAttribute("refID"))
                                        {
                                            bookBase.Info.Description = o.SelectSingleNode("string").InnerText;
                                        }
                                    }
                                }
                            }
                            if (subject.Name == "Port")
                            {
                                formatRoot.PortID = subject.GetAttribute("ID");
                            }
                        }
                    }
                    else if (type == "originalCookBookNode")
                    {
                        OriginalCookNode node = new OriginalCookNode();
                        node.OriginalType = FormatOriginalType(ele.SelectSingleNode("NodeOriginalType").InnerText);

                        FormatOriginalNode formatRoot = new FormatOriginalNode();
                        formatRoot.Node = node;
                        FormatCookNodes.Add(formatRoot);
                        formatRoot.ID = ele.GetAttribute("ID");
                        foreach (XmlElement subject in ele)
                        {
                            if (subject.Name == "Port")
                            {
                                formatRoot.PortID = subject.GetAttribute("ID");
                            }
                        }
                    }
                    else if (type == "assembleCookBookNode")
                    {
                        AssembleCookNode node = new AssembleCookNode();

                        FormatAssembleNode formatRoot = new FormatAssembleNode();
                        formatRoot.Node = node;
                        FormatCookNodes.Add(formatRoot);
                        formatRoot.ID = ele.GetAttribute("ID");

                        foreach (XmlElement subject in ele)
                        {
                            if (subject.Name == "Port")
                            {
                                if(subject.GetAttribute("name") == "Input_1")
                                {
                                    formatRoot.In1PortID = subject.GetAttribute("ID");
                                }
                                if (subject.GetAttribute("name") == "Input_2")
                                {
                                    formatRoot.In2PortID = subject.GetAttribute("ID");
                                }
                                if (subject.GetAttribute("name") == "Output")
                                {
                                    formatRoot.OutPortID = subject.GetAttribute("ID");
                                }
                            }
                        }
                    }
                    else if (type == "processCookBookNode")
                    {
                        ProcessCookNode node = new ProcessCookNode();

                        FormatProcessNode formatRoot = new FormatProcessNode();
                        formatRoot.Node = node;
                        FormatCookNodes.Add(formatRoot);
                        formatRoot.ID = ele.GetAttribute("ID");
                        foreach (XmlElement subject in ele)
                        {
                            if (subject.GetAttribute("name") == "InputLast")
                            {
                                formatRoot.InPortID = subject.GetAttribute("ID");
                            }
                            if (subject.GetAttribute("name") == "OutputNext")
                            {
                                formatRoot.OutPortID = subject.GetAttribute("ID");
                            }
                        }
                        node.ProcessTag = FormatProcessTag(ele.SelectSingleNode("NodeProcessTag").InnerText);
                    }
                }
                #endregion
                #region 遍历连结
                XmlNodeList conNode = xmlDoc.SelectSingleNode("NodeCanvas/Connections").ChildNodes;
                foreach (XmlElement ele in xmlNode)
                {
                    string port1ID = ele.GetAttribute("port1ID");
                    string port2ID = ele.GetAttribute("port2ID");
                    foreach (var item in FormatCookNodes)
                    {
                        if(item is FormatRootNode)
                        {
                            FormatRootNode target = item as FormatRootNode;
                            bookBase.RootNode = target.Node;
                            if (port1ID == target.PortID)
                            {
                                foreach (XmlElement node in xmlNode)
                                {
                                    foreach (XmlElement subject in node)
                                    {
                                        if (subject.Name == "Port")
                                        {
                                            if (subject.GetAttribute("ID") == port2ID)
                                            {
                                                target.Node.InNode = FormatCookNodes.Find(x => x.ID == node.GetAttribute("ID")).CastNode;
                                            }
                                        }
                                    }
                                }
                            }
                            if (port2ID == target.PortID)
                            {
                                foreach (XmlElement node in xmlNode)
                                {
                                    foreach (XmlElement subject in node)
                                    {
                                        if (subject.Name == "Port")
                                        {
                                            if (subject.GetAttribute("ID") == port1ID)
                                            {
                                                target.Node.InNode = FormatCookNodes.Find(x => x.ID == node.GetAttribute("ID")).CastNode;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (item is FormatOriginalNode)
                        {
                            FormatOriginalNode target = item as FormatOriginalNode;
                        }
                        else if (item is FormatProcessNode)
                        {
                            FormatProcessNode target = item as FormatProcessNode;
                            if (port1ID == target.InPortID)
                            {
                                foreach (XmlElement node in xmlNode)
                                {
                                    foreach (XmlElement subject in node)
                                    {
                                        if (subject.Name == "Port")
                                        {
                                            if (subject.GetAttribute("ID") == port2ID)
                                            {
                                                target.Node.InNode = FormatCookNodes.Find(x => x.ID == node.GetAttribute("ID")).CastNode;
                                            }
                                        }
                                    }
                                }
                            }
                            if (port2ID == target.InPortID)
                            {
                                foreach (XmlElement node in xmlNode)
                                {
                                    foreach (XmlElement subject in node)
                                    {
                                        if (subject.Name == "Port")
                                        {
                                            if (subject.GetAttribute("ID") == port1ID)
                                            {
                                                target.Node.InNode = FormatCookNodes.Find(x => x.ID == node.GetAttribute("ID")).CastNode;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (item is FormatAssembleNode)
                        {
                            FormatAssembleNode target = item as FormatAssembleNode;

                            if (port1ID == target.In1PortID)
                            {
                                foreach (XmlElement node in xmlNode)
                                {
                                    foreach (XmlElement subject in node)
                                    {
                                        if (subject.Name == "Port")
                                        {
                                            if (subject.GetAttribute("ID") == port2ID)
                                            {
                                                target.Node.InNodes[0] = FormatCookNodes.Find(x => x.ID == node.GetAttribute("ID")).CastNode;
                                            }
                                        }
                                    }
                                }
                            }
                            if (port2ID == target.In1PortID)
                            {
                                foreach (XmlElement node in xmlNode)
                                {
                                    foreach (XmlElement subject in node)
                                    {
                                        if (subject.Name == "Port")
                                        {
                                            if (subject.GetAttribute("ID") == port1ID)
                                            {
                                                target.Node.InNodes[0] = FormatCookNodes.Find(x => x.ID == node.GetAttribute("ID")).CastNode;
                                            }
                                        }
                                    }
                                }
                            }
                            if (port1ID == target.In2PortID)
                            {
                                foreach (XmlElement node in xmlNode)
                                {
                                    foreach (XmlElement subject in node)
                                    {
                                        if (subject.Name == "Port")
                                        {
                                            if (subject.GetAttribute("ID") == port2ID)
                                            {
                                                target.Node.InNodes[1] = FormatCookNodes.Find(x => x.ID == node.GetAttribute("ID")).CastNode;
                                            }
                                        }
                                    }
                                }
                            }
                            if (port2ID == target.In2PortID)
                            {
                                foreach (XmlElement node in xmlNode)
                                {
                                    foreach (XmlElement subject in node)
                                    {
                                        if (subject.Name == "Port")
                                        {
                                            if (subject.GetAttribute("ID") == port1ID)
                                            {
                                                target.Node.InNodes[1] = FormatCookNodes.Find(x => x.ID == node.GetAttribute("ID")).CastNode;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            return bookBase;
        }

        public static Original.OriginalType FormatOriginalType(string type)
        {
            //public enum OriginalType { Tomato = 0, Potato = 1, Egg = 2, Beef = 3 };
            if(type == "Tomato")
            {
                return Original.OriginalType.Tomato;
            }
            else if (type == "Potato")
            {
                return Original.OriginalType.Potato;
            }
            else if (type == "Egg")
            {
                return Original.OriginalType.Egg;
            }
            else if (type == "Beef")
            {
                return Original.OriginalType.Beef;
            }
            else
            {
                Debug.LogError("Error Type");
                return Original.OriginalType.Beef;
            }
        }

        public static CookBooks.ProcessTag FormatProcessTag(string type)
        {
            //public enum ProcessTag
            //{
            //    腌制, 切丁, 切块, 切条, 打碎
            //}
            if (type == "腌制")
            {
                return ProcessTag.腌制;
            }
            else if (type == "切丁")
            {
                return ProcessTag.切丁;
            }
            else if (type == "切块")
            {
                return ProcessTag.切块;
            }
            else if (type == "切条")
            {
                return ProcessTag.切条;
            }
            else if (type == "打碎")
            {
                return ProcessTag.打碎;
            }
            else 
            {
                Debug.LogError("Error Type");
                return ProcessTag.打碎;
            }
        }

        public class FormatNode
        {
            public string ID;
            public CookNode CastNode;
        }

        public class FormatRootNode : FormatNode
        {
            private RootCookNode node;
            public string PortID;

            public RootCookNode Node
            {
                get
                {
                    return node;
                }

                set
                {
                    CastNode = value;
                    node = value;
                }
            }
        }
        public class FormatOriginalNode : FormatNode
        {
            private OriginalCookNode node;
            public string PortID;

            public OriginalCookNode Node
            {
                get
                {
                    return node;
                }

                set
                {
                    CastNode = value;
                    node = value;
                }
            }
        }
        public class FormatProcessNode : FormatNode
        {
            private ProcessCookNode node;
            public string InPortID;
            public string OutPortID;

            public ProcessCookNode Node
            {
                get
                {
                    return node;
                }

                set
                {
                    CastNode = value;
                    node = value;
                }
            }
        }
        public class FormatAssembleNode : FormatNode
        {
            private AssembleCookNode node;
            public string In1PortID;
            public string In2PortID;
            public string OutPortID;

            public AssembleCookNode Node
            {
                get
                {
                    return node;
                }

                set
                {
                    CastNode = value;
                    node = value;
                }
            }
        }

        public class FormatConnection
        {
            public string port1ID;
            public string port2ID;
        }
    }
}
