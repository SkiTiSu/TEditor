using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Models
{

    [Verb("batchgen", HelpText = "批量生成")]
    internal class BatchGenOptions
    {
        [Option('d', "datasource", HelpText = "数据源（csv文件）", Required = true)]
        public string DataSource { get; set; }

        [Option('i', "tedfile", HelpText = "Ted模板文件", Required = true)]
        public string TedFile { get; set; }

        [Option('o', "outputdirectory", HelpText = "输出目录", Required = true)]
        public string OutputDirectory { get; set; }

        [Option('n', "filename", HelpText = "文件名模板", Required = true)]
        public string Filename { get; set; }

        [Option('s', "start", HelpText = "起始行", Required = true)]
        public int Start { get; set; }

        [Option('e', "end", HelpText = "结束行", Required = true)]
        public int End { get; set; }

        [Option('r', "repeats", HelpText = "副本个数")]
        public int Repeats { get; set; }
        
        [Option('x', "offsetx", HelpText = "副本X偏移")]
        public int OffsetX { get; set; }
        
        [Option('y', "offsety", HelpText = "副本Y偏移")]
        public int OffsetY { get; set; }
    }
}
