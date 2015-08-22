using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DiffCost.Models;

namespace DiffCost.Controllers.Api
{
    public class QuotationsController : ApiController
    {
        private DiffCostContext db = new DiffCostContext();

        // GET: api/Quotations
        public IQueryable<Quotation> GetQuotations()
        {
            return db.Quotations;
        }

        // GET: api/Quotations/5
        [ResponseType(typeof(Quotation))]
        public IHttpActionResult GetQuotation(int id)
        {
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return NotFound();
            }

            return Ok(quotation);
        }

        // PUT: api/Quotations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuotation(int id, Quotation quotation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quotation.Id)
            {
                return BadRequest();
            }

            quotation.UpdatedAt = DateTime.Now;

            db.Entry(quotation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuotationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Quotations
        [ResponseType(typeof(Quotation))]
        public IHttpActionResult PostQuotation(Quotation quotation)
        {
            // TODO: DB的には必須だけど、リクエスト的には不要な場合ってどうすればいいのかな
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            quotation.CreatedAt = DateTime.Now;
            quotation.UpdatedAt = DateTime.Now;

            db.Quotations.Add(quotation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = quotation.Id }, quotation);
        }

        // DELETE: api/Quotations/5
        [ResponseType(typeof(Quotation))]
        public IHttpActionResult DeleteQuotation(int id)
        {
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return NotFound();
            }

            db.Quotations.Remove(quotation);
            db.SaveChanges();

            return Ok(quotation);
        }

        // GET: api/Quotations/Projects
        public IQueryable<Quotation> GetProjects(string projectgroup)
        {
            //var ret = db.Quotations.GroupBy(quotation => quotation.ProjectName);
            //var ret2 = ret.Select(new Func<IGrouping<string, Quotation>, Quotation>(g => g.First()));
            //return ret2;

            // FirstOrDefaultでないとコンパイルエラー
            return db.Quotations.GroupBy(quotation => quotation.ProjectName).Select(g => g.FirstOrDefault());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuotationExists(int id)
        {
            return db.Quotations.Count(e => e.Id == id) > 0;
        }
    }
}