using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MyCreation.Helper
{

    public static class WordRender
    {
        #region word固定模板
        static void ReplaceParserTag(this OpenXmlElement elem, Dictionary<string, string> data)
        {
            var pool = new List<Run>();
            var matchText = string.Empty;
            var hiliteRuns = elem.Descendants<Run>() //找出鮮明提示
                .Where(o => o.RunProperties?.Elements<Highlight>().Any() ?? false).ToList();

            foreach (var run in hiliteRuns)
            {
                var t = run.InnerText;
                if (t.StartsWith("["))
                {
                    pool = new List<Run> { run };
                    matchText = t;
                }
                else
                {
                    matchText += t;
                    pool.Add(run);
                }
                if (t.EndsWith("]"))
                {
                    var m = Regex.Match(matchText, @"\[\$(?<n>\w+)\$\]");
                    if (m.Success && data.ContainsKey(m.Groups["n"].Value))
                    {
                        var firstRun = pool.First();
                        firstRun.RemoveAllChildren<Text>();
                        firstRun.RunProperties.RemoveAllChildren<Highlight>();
                        var newText = data[m.Groups["n"].Value];
                        var firstLine = true;
                        foreach (var line in Regex.Split(newText, @"\n"))
                        {
                            if (firstLine) firstLine = false;
                            else firstRun.Append(new Break());
                            firstRun.Append(new Text(line));
                        }
                        pool.Skip(1).ToList().ForEach(o => o.Remove());
                    }
                }

            }
        }

        public static byte[] GenerateDocx(byte[] template, Dictionary<string, string> data)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(template, 0, template.Length);
                using (var docx = WordprocessingDocument.Open(ms, true))
                {
                    docx.MainDocumentPart.HeaderParts.ToList().ForEach(hdr =>
                    {
                        hdr.Header.ReplaceParserTag(data);
                    });
                    docx.MainDocumentPart.FooterParts.ToList().ForEach(ftr =>
                    {
                        ftr.Footer.ReplaceParserTag(data);
                    });
                    docx.MainDocumentPart.Document.Body.ReplaceParserTag(data);
                    docx.Save();
                }
                return ms.ToArray();
            }
        }
        #endregion

        #region word模板+動態生成
        public static byte[] GenVocQuizPaper(string tmplPath, string[] vocList)
        {
            var tmplContent = File.ReadAllBytes(tmplPath);
            using (var ms = new MemoryStream())
            {
                ms.Write(tmplContent, 0, (int)tmplContent.Length);
                using (var doc = WordprocessingDocument.Open(ms, true))
                {

                    var tbl = new Table();
                    //設定框線
                    var tp = new TableProperties(
                        //指定田字形六條線的樣式及線寬
                        new TableBorders(
                            //Size 單位為 1/8 點 [註]
                            new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 }
                       )
                    );
                    tbl.AppendChild<TableProperties>(tp);
                    var que = new Queue<string>(vocList);
                    //A4 直式，兩欄
                    while (que.Any())
                    {
                        var tr = new TableRow();
                        //每一列放四欄
                        for (var i = 0; i < 2; i++)
                        {
                            var tc = new TableCell();
                            //第一欄為單字
                            tc.Append(new TableCellProperties(new TableCellWidth()
                            {
                                //寬度取15%
                                Type = TableWidthUnitValues.Pct,
                                Width = "15"
                            }));
                            var text = que.Any() ? que.Dequeue() : string.Empty;
                            tc.Append(new Paragraph(new Run(new Text(text))));
                            tr.Append(tc);
                            //第二欄為填空
                            tc = new TableCell();
                            tc.Append(new TableCellProperties(new TableCellWidth()
                            {
                                Type = TableWidthUnitValues.Pct,
                                Width = "35"
                            }));
                            tc.Append(new Paragraph(new Run(new Text(string.Empty))));
                            tr.Append(tc);
                        }
                        tbl.Append(tr);
                    }
                    doc.MainDocumentPart.Document.Body.Append(tbl);
                }
                return ms.ToArray();
            }
        }
        #endregion

        #region 未完成
        static void ReplaceParserTag(this OpenXmlElement elem, Dictionary<string, string> data, WordprocessingDocument doc)
        {
            var pool = new List<Run>();
            var matchText = string.Empty;
            var hiliteRuns = elem.Descendants<Run>() //找出鮮明提示
                .Where(o => o.RunProperties?.Elements<Highlight>().Any() ?? false).ToList();

            foreach (var run in hiliteRuns)
            {
                var t = run.InnerText;
                if (t.StartsWith("["))
                {
                    pool = new List<Run> { run };
                    matchText = t;
                }
                else
                {
                    matchText += t;
                    pool.Add(run);
                }
                if (t.EndsWith("]"))
                {
                    var m = Regex.Match(matchText, @"\[\$(?<n>\w+)\$\]");
                    if (m.Success && data.ContainsKey(m.Groups["n"].Value))
                    {
                        var firstRun = pool.First();
                        firstRun.RemoveAllChildren<Text>();
                        firstRun.RunProperties.RemoveAllChildren<Highlight>();
                        var newText = data[m.Groups["n"].Value];
                        var firstLine = true; 
                        foreach (var line in Regex.Split(newText, @"\n"))
                        {
                            if (firstLine) 
                                firstLine = false;
                            else 
                                firstRun.Append(new Break());
                           firstRun.Append(new Text(line));                           
                        }
                        pool.Skip(1).ToList().ForEach(o => o.Remove());
                    }
                }

            }
        }

        public static byte[] Gettest(string tmplPath, string[] vocList)
        {
            var tmplContent = File.ReadAllBytes(tmplPath);

            using (var ms = new MemoryStream())
            {
                ms.Write(tmplContent, 0, (int)tmplContent.Length);
                using (var doc = WordprocessingDocument.Open(ms, true))
                {

                    var tbl = new Table();
                    //設定框線
                    var tp = new TableProperties(
                        //指定田字形六條線的樣式及線寬
                        new TableBorders(
                            //Size 單位為 1/8 點 [註]
                            new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 }
                       )
                    );
                    tbl.AppendChild<TableProperties>(tp);
                    var que = new Queue<string>(vocList);
                    //A4 直式，兩欄
                    while (que.Any())
                    {
                        var tr = new TableRow();
                        for (var i = 0; i < 6; i++)
                        {


                            var tc = new TableCell();
                            //第一欄為單字
                            tc.Append(new TableCellProperties(new TableCellWidth()
                            {
                                //寬度取15%
                                Type = TableWidthUnitValues.Pct,
                                Width = "15"
                            }));
                            var text = que.Any() ? que.Dequeue() : string.Empty;
                            tc.Append(new Paragraph(new Run(new Text(text))));
                            tr.Append(tc);
                        }
                        tbl.Append(tr);
                    }
                    doc.MainDocumentPart.Document.Body.Append(tbl);
                    //doc.MainDocumentPart.Document.Body.ReplaceParserTag(new Dictionary<string, string>() { ["Table"] = tbl }, doc);
        
                }
                return ms.ToArray();
            }
        }
        #endregion
    }
}