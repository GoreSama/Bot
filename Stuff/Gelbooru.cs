using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoreSama.Stuff
{
    public class Gelbooru
    {
        public string source { get; set; }
        public string directory { get; set; }
        public string hash { get; set; }
        public int height { get; set; }
        public int id { get; set; }
        public string image { get; set; }
        public string change { get; set; }
        public string owner { get; set; }
        public string parent_id { get; set; }
        public string rating { get; set; }
        public bool sample { get; set; }
        public int sample_height { get; set; }
        public int sample_width { get; set; }
        public int score { get; set; }
        public string tags { get; set; }
        public int width { get; set; }
        public string file_url { get; set; }
        public string created_at { get; set; }
    }
}
