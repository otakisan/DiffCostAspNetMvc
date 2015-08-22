using DiffCost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace DiffCost.Controllers.Api
{
    public class CostComparisonsController : ApiController
    {
        private DiffCostContext db = new DiffCostContext();

        //[ResponseType(typeof(string))]
        public IHttpActionResult GetCostComparison(string projectname)
        {
            var facts = db.Facts.Where(fact => fact.ProjectName == projectname);
            var quotations = db.Quotations.Where(fact => fact.ProjectName == projectname);

            return Ok(new {quotations, facts});
        }

        // 戻り値はHttpResponseMessage、IHttpActionResultのどちらでもよい。string等でもいいし、voidでもいい。
        //[ResponseType(typeof(string))]
        public HttpResponseMessage GetCsv(string projectnamecsv)
        {
            string csvContents = this.GetCsvContents(projectnamecsv);
            HttpResponseMessage res = new HttpResponseMessage(HttpStatusCode.OK);
            res.Content = new StreamContent(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(csvContents)));
            res.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("{0}.csv", projectnamecsv)
            };

            return res;

            // 下記のコードはたぶん、カスタムクラスをCSVに変換するときに使用される気がする。
            //this.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
            //this.Configuration.Formatters.Add(new CostComparisonCsvFormatter(new QueryStringMapping("format", "csv", "text/csv")));
            //this.Configuration.Formatters.Add(new CostComparisonCsvFormatter());
            //return Ok("sss, bbb");
        }

        private string GetCsvContents(string projectName)
        {
            var facts = db.Facts.Where(fact => fact.ProjectName == projectName).ToList();
            var quotations = db.Quotations.Where(fact => fact.ProjectName == projectName).ToList();

            // どんなフォーマットなら使いやすい？？
            // いまだと、左右で対応付けされているわけではないから、頑張って左右に出してもなあ。

            // 横に並べる？？
            //StringBuilder lines = new StringBuilder();
            //lines.AppendLine("プロジェクト名,見積内容,見積工数（人日）,プロジェクト名,実績内容,実績工数（人日）");

            //int maxIndex = Math.Max(facts.Count, quotations.Count);
            //for (int i = 0; i < maxIndex; i++)
            //{
            //    if (quotations.Count > i)
            //    {
            //        lines.Append(string.Format("{0},{1},{2},", quotations[i].ProjectName, quotations[i].QuotationText, quotations[i].ManDay));
            //    }
            //    else
            //    {
            //        lines.Append(",,,");
            //    }
            //}

            // 縦に並べる？？
            StringBuilder lines = new StringBuilder();
            lines.AppendLine("No.,プロジェクト名,分類,内容,工数（人日）,登録日時,更新日時");
            int no = 0;
            quotations.ForEach(quotation => lines.AppendLine(string.Format(
                "{0},{1},{2},{3},{4},{5},{6}", 
                ++no, quotation.ProjectName, "見積", quotation.QuotationText, quotation.ManDay, quotation.CreatedAt.ToLocalTime(), quotation.UpdatedAt.ToLocalTime()
                )));
            facts.ForEach(fact => lines.AppendLine(string.Format(
                "{0},{1},{2},{3},{4},{5},{6}",
                ++no, fact.ProjectName, "実績", fact.FactText, fact.ManDay, fact.CreatedAt.ToLocalTime(), fact.UpdatedAt.ToLocalTime()
                )));

            return lines.ToString();
        }
    }

    public class CostComparisonCsvFormatter : BufferedMediaTypeFormatter
    {
        public CostComparisonCsvFormatter()
        {
            // Add the supported media type.
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));

            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));

            // New code:
            SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            SupportedEncodings.Add(Encoding.GetEncoding("iso-8859-1"));
        }

        public CostComparisonCsvFormatter(MediaTypeMapping mediaTypeMapping)
            : this()
        {
            MediaTypeMappings.Add(mediaTypeMapping);
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }
    }
}
