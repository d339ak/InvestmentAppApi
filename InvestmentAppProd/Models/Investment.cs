using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InvestmentAppProd.Models
{
	public class Investment
	{
		[Required]
		[Key]
		public string Name { get; set; }

		public DateTime StartDate { get; set; }

		public string InterestType { get; set; }

		public double InterestRate { get; set; }

		public double PrincipalAmount { get; set; }

		public double CurrentValue { get; set; } = 0;

		public Investment()
		{
		}

		public Investment(string name, DateTime startDate, string interestType, double rate, double principal)
		{
			Name = name;
			StartDate = startDate;
			InterestType = interestType;
			InterestRate = rate;
			PrincipalAmount = principal;
		}

		public void CalculateValue()
		{
			double r;
			double t;
			double n;
			double simpleInterestFinalAmount;
			double compoundInterestFinalAmount;
			double monthsDiff;

			// Interest rate is divided by 100.
			r = this.InterestRate / 100;

			// Time t is calculated to the nearest month.
			monthsDiff = 12 * (this.StartDate.Year - DateTime.Now.Year) + this.StartDate.Month - DateTime.Now.Month;
			monthsDiff = Math.Abs(monthsDiff);
			t = monthsDiff / 12;

			// SIMPLE INTEREST.
			simpleInterestFinalAmount = this.PrincipalAmount * (1 + (r * t));

			// COMPOUND INTEREST.
			// Compounding period is set to monthly (i.e. n = 12).
			n = 12;
			compoundInterestFinalAmount = this.PrincipalAmount * Math.Pow((1 + (r / n)), (n * t));

			if (this.InterestType == "Simple")
				this.CurrentValue = Math.Round(simpleInterestFinalAmount, 2);
			else
				this.CurrentValue = Math.Round(compoundInterestFinalAmount, 2);
		}
	}
}
