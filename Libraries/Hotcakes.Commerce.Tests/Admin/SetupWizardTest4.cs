using Hotcakes.Commerce.Taxes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Hotcakes.Commerce.Tests.Admin
{
    /// <summary>
    /// Summary description for SetupWizardTest4
    /// </summary>
    [TestClass]
    public class SetupWizardTest4 : BaseTest
    {

        /// <summary>
        /// Execute all test in order.
        /// </summary>
        [TestMethod]
        public void TestInOrder()
        {
            AddTaxtSchedule();
            EditTaxtSchedule();
            AddTax();
            DeleteTax();
            DeleteTaxtSchedule();

        }


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Loads taxt schedule.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        public void LoadTaxtSchedule()
        {
            //Arrange
            var store = _application.CurrentStore;
            var count = _irepo.GetStoreTaxScheduleCount();

            //Act
            var resultcount = _applicationDB.OrderServices.TaxSchedules.FindAll(store.Id).Count;

            //Assert
            Assert.AreEqual(count, resultcount);
        }

        /// <summary>
        /// Adds the taxt schedule.
        /// </summary>
        //[TestMethod]
        //[Priority(2)]
        public void AddTaxtSchedule()
        {
            #region Arrange
            var taxtschedule0 = _irepo.GetAddTaxSchedule();
            var t = new TaxSchedule
            {
                StoreId = _application.CurrentStore.Id,
                Name = taxtschedule0.Name,
                DefaultRate = taxtschedule0.DefaultRate,
                DefaultShippingRate = taxtschedule0.DefaultShippingRate,
            };
            #endregion

            //Act/Arrange
            Assert.IsTrue(_application.OrderServices.TaxSchedules.Create(t));

            //Act
            var resulttaxschedule = _application.OrderServices.TaxSchedules.FindByNameForThisStore(taxtschedule0.Name);

            //Assert
            Assert.AreEqual(taxtschedule0.Name, resulttaxschedule.Name);

        }

        /// <summary>
        /// Edits the taxt schedule.
        /// </summary>
        //[TestMethod]
        //[Priority(3)]
        public void EditTaxtSchedule()
        {
            #region Arrange
            var taxtschedule0 = _irepo.GetEditTaxSchedule();
            var taxtschedule1 = _application.OrderServices.TaxSchedules.FindByNameForThisStore(taxtschedule0.Name);

            taxtschedule1.StoreId = _application.CurrentStore.Id;
            taxtschedule1.Name = taxtschedule0.Name;
            taxtschedule1.DefaultRate = taxtschedule0.DefaultRate;
            taxtschedule1.DefaultShippingRate = taxtschedule0.DefaultShippingRate; 
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.TaxSchedules.Update(taxtschedule1));

            //Act
            var resulttaxschedule = _application.OrderServices.TaxSchedules.FindByNameForThisStore(taxtschedule0.Name);

            //Act/Assert
            Assert.AreEqual(taxtschedule0.Name, resulttaxschedule.Name);
        }


        /// <summary>
        /// Adds the tax.
        /// </summary>
        //[TestMethod]
        //[Priority(4)]
        public void AddTax()
        {
            #region Arrange
            var tax0 = _irepo.GetAddTax();

            var taxschedule = _application.OrderServices.TaxSchedules.FindByNameForThisStore(tax0.FirstOrDefault().Key);
            var oldcount = _application.OrderServices.Taxes.FindByTaxSchedule(_application.CurrentStore.Id, taxschedule.Id).Count;

            var t = new Tax
            {
                CountryIsoAlpha3 = tax0.FirstOrDefault().Value.CountryIsoAlpha3,
                ApplyToShipping = tax0.FirstOrDefault().Value.ApplyToShipping,
                PostalCode = tax0.FirstOrDefault().Value.PostalCode,
                Rate = tax0.FirstOrDefault().Value.Rate,
                ShippingRate = tax0.FirstOrDefault().Value.ShippingRate,
                RegionAbbreviation = tax0.FirstOrDefault().Value.RegionAbbreviation,
                StoreId = _application.CurrentStore.Id,
                TaxScheduleId = taxschedule.Id,
            }; 
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.Taxes.Create(t));
           
            //Act
            var resultcount = _application.OrderServices.Taxes.FindByTaxSchedule(_application.CurrentStore.Id, taxschedule.Id).Count;
           
            //Assert
            Assert.AreEqual(oldcount + 1, resultcount);
        }

        /// <summary>
        /// Deletes the tax.
        /// </summary>
        //[TestMethod]
        //[Priority(5)]
        public void DeleteTax()
        {
            //Arrange
            var tax0 = _irepo.GetDeleteTax();
            var taxschedule = _application.OrderServices.TaxSchedules.FindByNameForThisStore(tax0.FirstOrDefault().Key);
            var oldcount = _application.OrderServices.Taxes.FindByTaxSchedule(_application.CurrentStore.Id, taxschedule.Id).Count;
            var tax1 = _application.OrderServices.Taxes.FindByTaxSchedule(_application.CurrentStore.Id, taxschedule.Id).FirstOrDefault();

            //Act/Assent
            Assert.IsTrue(_application.OrderServices.Taxes.Delete(tax1.Id));
            
            //Act
            var resultcount = _application.OrderServices.Taxes.FindByTaxSchedule(_application.CurrentStore.Id, taxschedule.Id).Count;
          
            //Assert
            Assert.AreEqual(oldcount - 1, resultcount);
        }

        /// <summary>
        /// Deletes the taxt schedule.
        /// </summary>
        //[TestMethod]
        //[Priority(6)]
        public void DeleteTaxtSchedule()
        {
            //Arrange
            var store = _application.CurrentStore;
            var deltaxtscheduleName = _irepo.GetDeleteTaxSchedule();
            var resulttaxtschedule =
                _application.OrderServices.TaxSchedules.FindAll(store.Id)
                            .FirstOrDefault(x => x.Name.Equals(deltaxtscheduleName));

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.TaxSchedulesDestroy(resulttaxtschedule.Id));
        }


    }
}
