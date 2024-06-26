using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using XanatekEventAPI;

namespace XanatekEvenViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();



            // string path = @"C:\Users\jbuschman\Desktop\test.xml";

            // XmlSerializer serializer = new XmlSerializer(typeof(Event));

            // StreamReader reader = new StreamReader(path);
            //var e = (Event)serializer.Deserialize(reader);
            // reader.Close();


            eventViewer1.SetEventViewerData(@"C:\temp\03202018.xml");            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();            
            dialog.InitialDirectory = @"C:\";
            dialog.Title = "Select Event XML";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "xml";
            dialog.Filter = "Text files All files (*.*)|*.*|(*.xml)|*.xml";
            dialog.FilterIndex = 2;            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                eventViewer1.SetEventViewerData(dialog.FileName);                
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eventViewer1.ClearData();
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eventViewer1.SaveCurrentDataView();
        }

        private void addEventToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            
            eventViewer1.AddEvent(LevelEnum.Error, DateTime.Now, "source123", SectionEnum.None, EventTypeEnum.None,"this is a description for a new event.", "extra xml stuff");                
        }

        private void generateLargeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int filename = rnd.Next(1, 50000);
            eventViewer1.SetEventViewerData(@"C:\Users\jbuschman\Desktop\LargeFile"+filename.ToString()+".xml");

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
            

            for (int i = 0; i < 1000; i++)
            {
                int ev = rnd.Next(0, 2);

                eventViewer1.AddEvent((LevelEnum)ev, DateTime.Now,"Source.dll",SectionEnum.None,EventTypeEnum.None, RandomString(rnd.Next(1,5000),rnd), RandomString(rnd.Next(1, 2000), rnd));
            }
        }

        public string RandomString(int length, Random rnd)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
