using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTemplate
{
    //Based on the Account class, with variable names changed to keep consistency.
    public class Task
    {
        public int id;
        public int managerid;
        public string title;
        public string description;
        public string date; //See if this is fine for now, may have to convert to date object.
        public string starttime; //See if this is fine for now, may have to convert to time object/date object. 
        public int hours;
        public string location;
        public int volunteerid;
    }
}