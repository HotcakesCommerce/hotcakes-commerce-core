using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DotNetNuke.Data;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Moslem.Modules.GDSApi.GDSModule;
using Moslem.Modules.GDSModuleMVC.Components;
using Newtonsoft.Json;
using static Stimulsoft.Report.StiOptions.Designer.ComponentsTypes;

namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{

  
    public class Packages
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateOfInserted { get; set; }

    }
    public class RoomsPackages
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateOfInserted { get; set; }
        public int Count { get; set; }

    }


    public class PackageController : DnnController
    {
        public ActionResult GetRoomPackageinPackage(int PackageID)
        {
            Response.StatusCode = 200;
            return (new Cards()).GetRoomPackageDetailininPackage(PackageID).ToJsonResult();
        }

        public ActionResult AddRoomPackageIntoPackages(int RoomPackageID, int PackageID)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "INSERT INTO [dbo].[GDS_RoomPackageinPackage]  ([PackageID],[RoomPackageID],[DateOfInserted]) VALUES (N'" + PackageID + "',N'" + RoomPackageID + "',getdate())");
                    Response.StatusCode = 200;
                    return Content("با موفقیت افزوده شد. ");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Content("خطایی در افزودن پکیج به وجود آمده است ");
            }
        }
        public ActionResult AddRoomsintoRoomsPackage(int HotelID, int RoomID, int PackageID, string Description)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "INSERT INTO [dbo].[GDS_RoomsInRoomPackage]  ([HotelID],[RoomID],[DateOfInserted],[PackageID],[Description]) VALUES (N'" + HotelID + "',N'" + RoomID + "',getdate(),'" + PackageID + "',N'" + Description + "')");
                    Response.StatusCode = 200;
                    return Content("با موفقیت افزوده شد. ");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Content("خطایی در افزودن پکیج به وجود آمده است ");
            }
        }
        public ActionResult GetRoomsintoRoomsPackage(int PackageID)
        {
            Response.StatusCode = 200;
            return (new Cards()).GetRoomsInRoomPackage(PackageID).ToJsonResult();

        }
        public ActionResult RemoveRoominRoomsPackage(int id)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "delete from [dbo].[GDS_RoomsInRoomPackage] where id= " + id);
                    Response.StatusCode = 200;
                    return Content("با موفقیت حذف شد. ");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Content("خطایی در حذف پکیج به وجود آمده است ");
            }
        }




        public ActionResult AddRoomsPackage(string Name, string Description, int Count)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "INSERT INTO [dbo].[GDS_RoomsPackages]  ([Name],[Description],[DateOfInserted],[Count]) VALUES (N'" + Name + "',N'" + Description + "',getdate()," + Count + ")");
                    Response.StatusCode = 200;
                    return Content("با موفقیت افزوده شد. ");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Content("خطایی در افزودن پکیج به وجود آمده است ");
            }
        }
        public ActionResult RemoveRoomsPackage(int id)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "delete from [dbo].[GDS_RoomsPackages] where id= " + id);
                    Response.StatusCode = 200;
                    return Content("با موفقیت حذف شد. ");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Content("خطایی در حذف پکیج به وجود آمده است ");
            }
        }
        public ActionResult GetAllRoomsPackages()
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                List<RoomsPackages> lstPackages = dataContext.ExecuteQuery<RoomsPackages>(CommandType.Text, "SELECT * FROM [dbo].[GDS_RoomsPackages]").OrderByDescending(p => p.ID).ToList<RoomsPackages>();
                Response.StatusCode = 200;
                return lstPackages.ToJsonResult();
            }
        }


        public ActionResult RemovePackage(int id)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "delete from [dbo].[GDS_Packages] where id= " + id);
                    Response.StatusCode = 200;
                    return Content("با موفقیت حذف شد. ");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Content("خطایی در حذف پکیج به وجود آمده است ");
            }
        }
        public ActionResult AddPackage(string Name, string Description)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "INSERT INTO [dbo].[GDS_Packages]  ([Name],[Description],[DateOfInserted]) VALUES (N'" + Name + "',N'" + Description + "',getdate())");
                    Response.StatusCode = 200;
                    return Content("با موفقیت افزوده شد. ");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Content("خطایی در افزودن پکیج به وجود آمده است ");
            }
        }
        public ActionResult GetAllPackages()
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                List<Packages> lstPackages = dataContext.ExecuteQuery<Packages>(CommandType.Text, "SELECT * FROM [dbo].[GDS_Packages]").OrderByDescending(p => p.ID).ToList<Packages>();
                Response.StatusCode = 200;
                return lstPackages.ToJsonResult();
            }
        }
        public ActionResult Index()
        {
            return base.View();
        }

        public ActionResult AddCard(string PassKey, string Description, int PackageID)
        {
            try
            {

                using (IDataContext dataContext = DataContext.Instance())
                {
                    if (dataContext.ExecuteScalar<int>(CommandType.Text, "select count(*) from [GDS_Cards] where [PassKey]= N'" + PassKey + "'") > 0)
                    {
                        Response.StatusCode = 500;
                        return Content("رمز عبور انتخابی تکراری می باشد.");
                    }
                    else
                        dataContext.Execute(CommandType.Text, "INSERT INTO [GDS_Cards]  ([PassKey]  ,[DateOfInserted]  ,[Description]  ,[PackageID]) VALUES  (N'" + PassKey + "' ,GETDATE(),N'" + Description + "' ," + PackageID + ")");
                }
                Response.StatusCode = 200;
                return Content("با موفقیت افزوده شد");

            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("ناموق");
            }
        }

        public ActionResult GetCards()
        {
            Cards ObjCards = new Cards();
            Response.StatusCode = 200;
            return ObjCards.GetCards().ToJsonResult();
        }
    }
}