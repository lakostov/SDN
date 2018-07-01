using Microsoft.XmlDiffPatch;
using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SDN.Processors
{
    public class XmlDiffHtml
    {
        public string GetXmlDiffHtml()
        {

            var htmlString = "";
            var result = new XDocument();
            var writer = result.CreateWriter();

            //Make an instance of XmlDiff and ignore options
            var diff = new XmlDiff(XmlDiffOptions.IgnoreChildOrder |
                                    XmlDiffOptions.IgnoreNamespaces |
                                    XmlDiffOptions.IgnorePrefixes |
                                    XmlDiffOptions.IgnoreWhitespace | 
                                    XmlDiffOptions.IgnoreXmlDecl);

            //Get project directory path
            var path = System.AppDomain.CurrentDomain.BaseDirectory.Replace("bin", "");
            var oldFile = $"{path}/sdn_xml/SDN.xml";
            var newFile = "https://www.treasury.gov/ofac/downloads/sdn.xml";
            var newPath = $"{path}/sdn_xml/newSDN.xml";

            //Check if source  SDN file and local SDN file are equal
            bool isEqual = diff.Compare(oldFile, newFile, false, writer);
            writer.Flush();
            writer.Close();



            if (isEqual)
            {
                htmlString = "up to date";
                //If SDN files are equal - return
                return htmlString;
            }

            //Save Diff in new file newSDN.xml
            result.Save(newPath);

            XmlDocument diffDoc = new XmlDocument();

            //Load the Diff flie
            diffDoc.Load(newPath);

            StringBuilder sb = new StringBuilder();

            //Get arrays of xml elements
            XmlNodeList added = diffDoc.GetElementsByTagName("xd:add");
            XmlNodeList changed = diffDoc.GetElementsByTagName("xd:change");
            XmlNodeList removed = diffDoc.GetElementsByTagName("xd:remove");

            //Build the return string
            sb.Append($"<p>LIST UPDATED - US - OFAC Specially Designated Nationals (SDN) List {DateTime.Now}</p>");
            sb.Append("<p>Added: ");
            sb.Append(added.Count);
            sb.Append(", Changed: ");
            sb.Append(changed.Count);
            sb.Append(", Removed: ");
            sb.Append(changed.Count);
            sb.Append("</p>");
            htmlString = sb.ToString();
         
            return htmlString;
        }

        public void PatchList()
        {
            //Get project directory path
            var path = System.AppDomain.CurrentDomain.BaseDirectory.Replace("bin", "");

            string originalFile = $"{path}/sdn_xml/SDN.xml";
            String diffGramFile = $"{path}/sdn_xml/newSDN.xml";

            XmlDocument sourceDoc = new XmlDocument(new NameTable());

            //Load local SDN file
            sourceDoc.Load(originalFile);

            XmlTextReader diffgramReader = new XmlTextReader(diffGramFile);

            XmlPatch xmlpatch = new XmlPatch();

            //Patch the local SDN file with the cntent of the Diff
            xmlpatch.Patch(sourceDoc, diffgramReader);

            XmlTextWriter output = new XmlTextWriter(originalFile, Encoding.UTF8);
            output.Formatting = Formatting.Indented;

            //Save the file
            sourceDoc.Save(output);
            output.Close();
        }
    }
}