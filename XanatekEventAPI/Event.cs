using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XanatekEventAPI
{
    public enum LevelEnum
    {        
        Info = 0,        
        Warning = 1,        
        Error = 2
    }

    public enum SectionEnum
    {
        None = 0
    }
    
    public enum EventTypeEnum
    {
        None = 0,
        Refresh = 1,
        Add = 2,
        Edit = 3
    }

    public class Event
    {
        private Guid _UID;
        public Guid UID
        {
            get { return _UID;}
            set { _UID = value; }            
        }

        private LevelEnum _level;        
        public LevelEnum Level
        {
            get { return _level; }            
            set { _level = value; }            
        }

        private DateTime _stamp;        
        public DateTime Stamp
        {
            get { return _stamp; }
            set { _stamp = value; }            
        }
        private string _source = "";        
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        private SectionEnum _section;
        public SectionEnum Section
        {
            get { return _section; }
            set { _section = value; }
        }

        private EventTypeEnum _eventType;
        public EventTypeEnum EventType
        {
            get { return _eventType; }
            set { _eventType = value;}
        }

        private string _description = "";

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _extra = "";
        public string Extra
        {
            get { return _extra; }
            set { _extra = value; }
        }


        static public T CastEnum<T>(object o)
        {
            T enumVal = (T)Enum.Parse(typeof(T), o.ToString());
            return enumVal;
        }
    }
}
