using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform;
using NUnit.Framework;
using InvestmentAppProd.Models;
using InvestmentAppProd.Controllers;
using InvestmentAppProd.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading;

namespace InvestmentAppProd.Tests
{
    [TestFixture]
    public class TestInvestmentController
    {
        private static DbContextOptions<InvestmentDBContext> dbContextOptions = new DbContextOptionsBuilder<InvestmentDBContext>()
            .UseInMemoryDatabase(databaseName: "InvestmentsDbTest")
            .Options;
        InvestmentDBContext context;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new InvestmentDBContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            var newInvestments = new List<Investment>();

            newInvestments.Add(
                new Investment
                {
                    Name = "Investment 1",
                    StartDate = DateTime.Parse("2022-03-01"),
                    InterestType = "Simple",
                    InterestRate = 3.875,
                    PrincipalAmount = 10000
                });
            newInvestments.Add(
                new Investment
                {
                    Name = "Investment 2",
                    StartDate = DateTime.Parse("2022-04-01"),
                    InterestType = "Simple",
                    InterestRate = 4,
                    PrincipalAmount = 15000
                });
            newInvestments.Add(
                new Investment
                {
                    Name = "Investment 3",
                    StartDate = DateTime.Parse("2022-05-01"),
                    InterestType = "Compound",
                    InterestRate = 5,
                    PrincipalAmount = 20000
                });

            context.Investments.AddRange(newInvestments);
            context.SaveChanges();
        }

        private void EmptyDatabase()
        {
            context.Investments.RemoveRange(context.Investments.ToList<Investment>());
            context.SaveChanges();
        }

        [Test]
        public void GetAllInvestments_WithExistingItems_ShouldReturnAllInvestments()
        {
            // ARRANGE
            var controller = new InvestmentController(context);

            // ACT
            var result = controller.FetchInvestment();
            var obj = result.Result as ObjectResult;
            var objListResult = (List<Investment>)obj.Value;
            //var objCountResult = ((List<Investment>)obj.Value).Count();

            // ASSERT   : Status code 200 ("Ok") + Count of objects returned is correct + Object returned (first) is of Type Investment.
            Assert.AreEqual(200, (obj.StatusCode));
            Assert.AreEqual(context.Investments.Count(), objListResult.Count());
            Assert.IsInstanceOf<Investment>(objListResult.First());
        }

        [Test]
        public void GetInvestment_WithSingleItem_ShouldReturnSingleInvestment()
        {
            // Arrange
            var controller = new InvestmentController(context);
            var name = "Investment 1";

            // Act
            var result = controller.FetchInvestment(name);
            var obj = result.Result as ObjectResult;
            var objInvResult = obj.Value as Investment;

            // Assert   : Status code 200 ("Ok") + Object returned is of Type Investment + Object name is same.
            Assert.AreEqual(200, (obj.StatusCode));
            Assert.IsInstanceOf<Investment>(objInvResult);
            Assert.AreEqual(name, objInvResult.Name);
        }

        [Test]
        public void AddInvestment_SingleItem_ShouldAddInvestment()
        {
            // Arrange
            var controller = new InvestmentController(context);
            var newInvestnment = new Investment
            {
                Name = "Investment 4",
                StartDate = DateTime.Parse("2022-05-01"),
                InterestType = "Simple",
                InterestRate = 7.7,
                PrincipalAmount = 25000
            };

            // Act
            var result = controller.AddInvestment(newInvestnment);
            var obj = result.Result as ObjectResult;
            //var objInvResult = obj.Value as Investment;

            // Assert   : Status code 201 ("Created")
            Assert.AreEqual(201, (obj.StatusCode));
        }

        [Test]
        public void UpdateInvestment_SingleItem_ShouldUpdateInvestment()
        {
            // Arrange
            CleanUp();
            Setup();
            var controller = new InvestmentController(context);
            var updateInvestment = "Investment 2";
            var newInvestment = new Investment
            {
                Name = "Investment 2",
                StartDate = DateTime.Parse("2022-06-01"),
                InterestType = "Compound",
                InterestRate = 8,
                PrincipalAmount = 30000
            };

            // Act
            var result = controller.UpdateInvestment(updateInvestment, newInvestment);
            var obj = result as NoContentResult;

            // Assert   : Status code 204 ("No Content")
            Assert.AreEqual(204, obj.StatusCode);
        }

        [Test]
        public void DeleteInvestment_SingleItem_ShouldDeleteInvestment()
        {
            // Arrange
            var controller = new InvestmentController(context);
            var deleteInvestment = "Investment 2";

            // Act
            var result = controller.DeleteInvestment(deleteInvestment);
            var obj = result as NoContentResult;

            // Assert   : Status code 204 ("No Content")
            Assert.AreEqual(204, obj.StatusCode);
        }
    }
}
