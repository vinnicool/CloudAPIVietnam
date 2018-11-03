using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class TableInfo
    {
        public int id { get; set; }
        public string title { get; set; }
        public string region { get; set; }
    }

    public class Column
    {
        public string type { get; set; }
        public string value { get; set; }
        public int columnNumber { get; set; }
    }

    public class FormTemplateModel
    {
        public TableInfo tableInfo { get; set; }
        public List<Column> columns { get; set; }
    }

    public class FormulierWithoutContents
    {
        public int id { get; set; }
        public string title { get; set; }
        public string region { get; set; }
    }
}