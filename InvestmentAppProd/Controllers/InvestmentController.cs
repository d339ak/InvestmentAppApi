using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvestmentAppProd.Models;
using InvestmentAppProd.Data;

namespace InvestmentAppProd.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InvestmentController : Controller
    {
        private readonly InvestmentDBContext _context;

        public InvestmentController(InvestmentDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Investment>> FetchInvestment()
        {
            try
            {
                return Ok(_context.Investments.ToList());
            }
            catch (Exception e)
            {
                return NotFound(e.ToString());
            }
        }

        [HttpGet("name")]
        public ActionResult<Investment> FetchInvestment([FromQuery] string name)
        {
            try
            {
                var investment = _context.Investments.Find(name);
                if (investment == null)
                    return NotFound();

                return Ok(investment);
            }
            catch (Exception e)
            {
                return NotFound(e.ToString());
            }
        }


        [HttpPost]
        public ActionResult<Investment> AddInvestment([FromBody] Investment investment)
        {
            try
            {
                if (investment.StartDate > DateTime.Now)
                    return BadRequest("Investment Start Date cannot be in the future.");

                investment.CalculateValue();
                _context.ChangeTracker.Clear();
                _context.Investments.Add(investment);
                _context.SaveChanges();

                return CreatedAtAction("AddInvestment", investment.Name, investment);
            }
            catch (DbUpdateException dbE)
            {
                return Conflict(dbE.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPut("name")]
        public ActionResult UpdateInvestment([FromQuery] string name, [FromBody] Investment investment)
        {
            try
            {
                if (name != investment.Name)
                    return BadRequest("Name does not match the Investment you are trying to update.");

                if (investment.StartDate > DateTime.Now)
                    return BadRequest("Investment Start Date cannot be in the future.");

                investment.CalculateValue();
                _context.ChangeTracker.Clear();
                _context.Entry(investment).State = EntityState.Modified;
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.ToString());
            }
        }

        [HttpDelete("name")]
        public ActionResult DeleteInvestment([FromQuery] string name)
        {
            try
            {
                var investment = _context.Investments.Find(name);
                if (investment == null)
                {
                    return NotFound();
                }
                _context.ChangeTracker.Clear();
                _context.Investments.Remove(investment);
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }
    }
}
