using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Models
{
    public class TargetFolderSettings
    {
        public string Directory { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public bool IsToCreateDirectiory { get; set; } = false;
        public bool IsRelative { get; set; } = false;
    }
}
