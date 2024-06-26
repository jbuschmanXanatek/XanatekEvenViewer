using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Xml;
using System.Xml.Linq;

namespace XanatekEventAPI
{
    public partial class EventViewer : UserControl
    {
        private EventCollection _events;
        private bool _trackInfo;
        private bool _trackWarning;
        private bool _trackError;
        private string _path;

        public EventViewer()
        {
            InitializeComponent();
            _path = "";
            _trackError = true;
            _trackWarning = false;
            _trackInfo = false;            
        }

        public void RefreshEventViewer()
        {
            SetEventViewerData(_path, _trackInfo, _trackWarning, _trackError);
        }



        public void AddEvent(LevelEnum level, DateTime stamp, string source, SectionEnum section, EventTypeEnum eventType, string description, string extra)
        {
            _events.AddNewEvent(level, stamp, source, section, eventType, description, extra);
        }
        
        public void SetEventViewerData(string path, bool trackInfo = true, bool trackWarning = false, bool trackError = false)
        {
            _path = path;
            _trackError = trackError;
            _trackWarning = trackWarning;
            _trackInfo = trackInfo;                        

            _events = EventCollection.LoadEventData(path);

            if (_events != null && _events.Events != null)
            {
                DataTable tempTable = new DataTable();
                tempTable.Columns.Add("UID", typeof(string));
                tempTable.Columns.Add("Level", typeof(string));
                tempTable.Columns.Add("Stamp", typeof(string));
                tempTable.Columns.Add("Source", typeof(string));
                tempTable.Columns.Add("Section", typeof(string));
                tempTable.Columns.Add("EventType", typeof(string));
                tempTable.Columns.Add("Description", typeof(string));
                tempTable.Columns.Add("Extra", typeof(string));                    

                foreach (var tempEvent in _events.Events)
                    tempTable.Rows.Add(new object[] {tempEvent.UID, tempEvent.Level, tempEvent.Stamp, tempEvent.Source, tempEvent.Section, tempEvent.EventType, tempEvent.Description, tempEvent.Extra });

                //dataGridView1.DataSource = tempTable;

                   
                gridControl1.DataSource = tempTable;
                gridView1.Columns["UID"].Visible = false;
                gridView1.Columns["Extra"].Visible = false;
                gridView1.Columns["Description"].Visible = false;

                gridControl1.Refresh();

                //dataGridView1.Refresh();                    
                lblInfo.Text = "Number of Events: " + _events.Events.Count();
                lblFile.Text = "Working File: " + Path.GetFileName(_path);

                if(_events.Events.Count() > 0)
                    UpdateFormControls(0);
            }            
        }

        private void EventViewer_Load(object sender, EventArgs e)
        {                        
            
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            UpdateFormControls(e.RowIndex);            
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshEventViewer();
        }       

        private void gridControl1_MouseClick(object sender, MouseEventArgs e)
        {
            var row = gridView1.GetFocusedDataRow();
            if (row != null)
            {
                var index = row.Table.Rows.IndexOf(row);
                UpdateFormControls(index);
            }
        }

        private void UpdateFormControls(int index)
        {
            lblEventType.Text = _events.Events[index].EventType.ToString();
            lblLevel.Text = _events.Events[index].Level.ToString();
            lblSource.Text = _events.Events[index].Source.ToString();
            lblStamp.Text = _events.Events[index].Stamp.ToString();
            lblUID.Text = _events.Events[index].UID.ToString();
            txtDescription.Text = _events.Events[index].Description.ToString();

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(Event));
                    xsSubmit.Serialize(writer, _events.Events[index]);                    
                    txtXML.Text = XDocument.Parse(sww.ToString()).ToString();
                }
            }
        }

        public void ClearData()
        {
            File.Delete(_path);
            RefreshEventViewer();            
        }

        public void InsertEvent()
        {

        }

        public void SaveCurrentDataView()
        {            
            var eventList = new List<Event>();

            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                eventList.Add(new Event()
                {
                    Description = gridView1.GetRowCellValue(i, "Description").ToString(),
                    EventType = Event.CastEnum<EventTypeEnum>(gridView1.GetRowCellValue(i, "EventType")),
                    Extra = gridView1.GetRowCellValue(i, "Extra").ToString(),
                    Level = Event.CastEnum<LevelEnum>(gridView1.GetRowCellValue(i, "Level")),
                    Section = Event.CastEnum<SectionEnum>(gridView1.GetRowCellValue(i, "Section")),
                    Source = gridView1.GetRowCellValue(i, "Source").ToString(),
                    Stamp = DateTime.Parse(gridView1.GetRowCellValue(i, "Stamp").ToString()),
                    UID = Guid.Parse(gridView1.GetRowCellValue(i, "UID").ToString())
                });
            }

            _events.Events = eventList.ToArray();
            _events.SaveCurrentData();
        }

        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || 
                e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right)
            {
                var row = gridView1.GetFocusedDataRow();
                if (row != null)
                {
                    var index = row.Table.Rows.IndexOf(row);
                    UpdateFormControls(index);
                }
            }
        }
    }
}
