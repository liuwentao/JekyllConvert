using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new string[] { @"e:\MyFiles\document\blog\", @"e:\MyFiles\document\test\" };
            if (args.Length != 2)
            {
                Console.WriteLine("请输入两个目录,第一个为perlican的目录,第二个为jeklly的目录");
                return;
            }
            string srcPath = args[0];
            var files = Directory.GetFiles(srcPath, "*.md");
            foreach (var item in files)
            {
                convert2Jekyll(item, args[1]);
            }
        }

        /// <summary>
        /// 格式转换
        /// </summary>
        /// <param name="item"></param>
        /// <param name="p"></param>
        private static void convert2Jekyll(string item, string p)
        {
            var dict = new Dictionary<string, string>();
            string[] tags = new string[] { "Date", "Title", "Tags", "Slug", "Category" };

            using (StreamReader reader = new StreamReader(item, Encoding.UTF8))
            {
                for (int i = 0; i < 5; i++)
                {
                    string msg = reader.ReadLine();
                    if (string.IsNullOrEmpty(msg))
                    {
                        break;
                    }
                    foreach (var tag in tags)
                    {
                        if (!msg.ToLower().StartsWith(tag.ToLower()))
                        {
                            continue;
                        }
                        if (dict.ContainsKey(tag))
                        {
                            dict[tag] = msg.Substring(msg.IndexOf(':') + 1).Trim();
                        }
                        else
                        {
                            dict.Add(tag, msg.Substring(msg.IndexOf(':') + 1).Trim());
                        }
                    }
                }
                //---
                //layout: post
                //title:  "Welcome to Jekyll!"
                //date:   2014-05-11 19:25:17
                //categories: jekyll update
                //---
                if (dict.Count == 0)
                {
                    return;
                }
                //tags暂时不处理
                string targetPath = Path.Combine(p, string.Format("{0}-{1}.markdown", dict["Date"].Split(' ')[0], dict["Slug"]));
                using (StreamWriter writer = new StreamWriter(targetPath, false, new UTF8Encoding(false)))
                {
                    writer.WriteLine("---");
                    writer.WriteLine("layout: post");
                    writer.WriteLine("title:  \"{0}\"", dict["Title"]);
                    writer.WriteLine("date:   {0}", dict["Date"]);
                    writer.WriteLine("categories: {0}", dict["Category"]);
                    writer.WriteLine("---");

                    //TODO write
                    while (!reader.EndOfStream)
                    {
                        var msg = reader.ReadLine();
                        writer.WriteLine(msg);
                    }
                }

            }
        }
    }
}
