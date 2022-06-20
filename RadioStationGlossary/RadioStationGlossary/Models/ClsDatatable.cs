using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;          //

namespace RadioStationGlossary.Models
{
    public class ClsDatatable
    {
        public DataTable glTable;
        private int maxLoadLine; 

        public void Initialize()
        {
            glTable = new DataTable();
            glTable.Columns.Add("Id", typeof(int));
            glTable.Columns.Add("Name", typeof(string));
            glTable.Columns.Add("Discription", typeof(string));
            glTable.Columns.Add("Remarks", typeof(string));
            glTable.Columns.Add("ImageData", typeof(string));

            maxLoadLine = Properties.Settings.Default.DB_READ_LIMIT;
        }

        public void Clear()
        {
            DataRow[] rows;
            if(glTable != null)
            {
                rows = glTable.Select();
                Array.ForEach<DataRow>(rows, row => glTable.Rows.Remove(row));
            }
        }

        public void Load(IEnumerable<Glos> datas)
        {
            this.Clear();

            foreach (Glos row in datas)
            {
                if (this.Add(row) != 0) break;
            }

        }

        public int Add(Glos data)
        {
            int result = 0;     // 正常
            // 0:   正常
            // 1:   ライン数オーバー
            // 2:   エラー

            try
            {
                DataRow row = glTable.NewRow();
                row["Id"] = data.Id;
                row["Name"] = data.Name;
                row["Discription"] = data.Discription;
                row["Remarks"] = data.Remarks;
                row["ImageData"] = data.ImageData;
                glTable.Rows.Add(row);
                if (glTable.Rows.Count >= maxLoadLine)
                {
                    result = 1;
                }
            }
            catch(Exception ex)
            {
                result = 2;
                Console.WriteLine(ex?.Message);
            }

            return result;
        }
    }
}
