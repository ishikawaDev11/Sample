using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioStationGlossary.Models
{
    public class Glos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public string Remarks { get; set; }
        public string ImageData { get; set; }       // 相対パスとする
    }
}
