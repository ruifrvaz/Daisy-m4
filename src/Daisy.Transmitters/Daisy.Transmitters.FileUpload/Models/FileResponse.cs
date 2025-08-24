using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Transmitters.VectorStore.Models
{
    internal class FileResponse
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int bytes { get; set; }
        public int created_at { get; set; }
        public string filename { get; set; }
        public string purpose { get; set; }
    }
}
