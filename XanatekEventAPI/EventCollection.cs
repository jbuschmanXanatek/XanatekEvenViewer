using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XanatekEventAPI
{
    [XmlRoot("EventCollection")]
    public class EventCollection
    {        
        
        private Event[] _events;
        private string _path;
        public string Path { get { return _path; } }

        [XmlArray("Events"), XmlArrayItem("Event")]
        public Event[] Events
        {
            get { return _events; }
            set { _events = value; }
        }

        public EventCollection()
        {
        }
        public EventCollection(string dir,string fileName)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _path = dir +@"\"+ fileName+"_"+ DateTime.Now.ToString("MMddyyyy") + ".xml";
        }

        public void SaveCurrentData()
        {
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(EventCollection));
                    xsSubmit.Serialize(writer, _events);
                    var xml = sww.ToString();

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "XML-File | *.xml";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xml);
                        doc.Save(saveFileDialog.FileName);
                    }
                }
            }
        }
        
        public void AddNewEvent(LevelEnum level, DateTime stamp, string source, SectionEnum section, EventTypeEnum eventType, string description, string extra)
        {
            BuildEmptyFile(_path);

            string xmlString = System.IO.File.ReadAllText(_path);

            //do we want to save the sml with the replaced characters? ~jbuschman
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString.Replace("&", "&amp;"))))
            {
                var xdoc = XDocument.Load(memoryStream);
                var eventElement = new XElement("Event");

                eventElement.Add(new XElement("UID", Guid.NewGuid()));
                eventElement.Add(new XElement("Level", level));
                eventElement.Add(new XElement("Stamp", stamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz")));
                eventElement.Add(new XElement("Source", source));
                eventElement.Add(new XElement("Section", section));
                eventElement.Add(new XElement("EventType", eventType));
                eventElement.Add(new XElement("Description", description));
                eventElement.Add(new XElement("Extra", extra));

                var a = xdoc.Descendants("Events").FirstOrDefault();
                a.Add(eventElement);

                xdoc.Save(_path);
            }
        }

        public static void BuildEmptyFile(string path)
        {
            if (!File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);
                XmlNode eventCollectionNode = doc.CreateElement("EventCollection");
                doc.AppendChild(eventCollectionNode);
                eventCollectionNode.AppendChild(doc.CreateElement("Events"));
                doc.Save(path);
            }
        }
        public static EventCollection LoadEventData(string path)
        {
            EventCollection _events = null;
            BuildEmptyFile(path);
            XmlSerializer serializer = new XmlSerializer(typeof(EventCollection));

            string xmlString = System.IO.File.ReadAllText(path);


            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString.Replace("&", "&amp;"))))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(EventCollection));
                StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);
                _events = (EventCollection)xmlSerializer.Deserialize(reader);
                _events._path = path;
            }
            return _events;
        }
    }
}
