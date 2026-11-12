using System.Collections.Generic;
using System.IO;
using System.Xml;
using ExamPortal.Legacy.Web.Models;

namespace ExamPortal.Legacy.Web.Services
{
    /// <summary>Reads candidate rosters that test centers upload as XML.</summary>
    public static class RosterImporter
    {
        public static List<Candidate> Parse(string xml)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = new XmlUrlResolver()
            };

            var document = new XmlDocument { XmlResolver = new XmlUrlResolver() };
            using (var reader = XmlReader.Create(new StringReader(xml), settings))
            {
                document.Load(reader);
            }

            var candidates = new List<Candidate>();
            foreach (XmlNode node in document.SelectNodes("/roster/candidate"))
            {
                candidates.Add(new Candidate
                {
                    FirstName = Text(node, "firstName"),
                    LastName = Text(node, "lastName"),
                    Email = Text(node, "email")
                });
            }
            return candidates;
        }

        private static string Text(XmlNode parent, string name)
        {
            var child = parent.SelectSingleNode(name);
            return child == null ? null : child.InnerText;
        }
    }
}
