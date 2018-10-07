using DotNetNuke.Data;
using Moslem.Modules.GDSApi.GDSModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Moslem.Modules.GDSModuleMVC.Components
{
    public class Card
    {
        public int id { get; set; }
        public string PassKey { get; set; }
        public DateTime DateOfInserted { get; set; }
        public string Description { get; set; }
        public int PackageID { get; set; }
        public string Name { get; set; }
    }
    public class RoomInRoomPackage
    {
        public int ID { get; set; }
        public string HotelID { get; set; }
        public string RoomID { get; set; }
        public string PackageID { get; set; }
        public DateTime DateOfInserted { get; set; }
        public string Description { get; set; }
    }
    public class GetRoomPackageinPackage
    {
        public int ID { get; set; }
        public int PackageID { get; set; }
        public int RoomPackageID { get; set; }
        public DateTime DateOfInserted { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }
    public class Cards
    {
        public List<Card> GetCards(string PassKey)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                return dataContext.ExecuteQuery<Card>(CommandType.Text, "select [GDS_Cards].id,[PassKey],[GDS_Cards].[DateOfInserted],[GDS_Cards].[Description],[Name],[GDS_Cards].[PackageID] from [GDS_Cards]  join [GDS_Packages] on [GDS_Cards].[PackageID] = [GDS_Packages].id where PassKey = N'" + PassKey + "'").OrderByDescending(p => p.id).ToList<Card>();
            }
        }
        public List<Card> GetCards()
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                return dataContext.ExecuteQuery<Card>(CommandType.Text, "select [GDS_Cards].id,[PassKey],[GDS_Cards].[DateOfInserted],[GDS_Cards].[Description],[Name],[GDS_Cards].[PackageID] from [GDS_Cards]  join [GDS_Packages] on [GDS_Cards].[PackageID] = [GDS_Packages].id").OrderByDescending(p => p.id).ToList<Card>();
            }
        }
        public bool IsValid(string PassKey)
        {
            if (string.IsNullOrEmpty(PassKey))
                return false;
            else if (GetCards(PassKey).Count == 0)
                return false;
            else
                return true;
        }
        public List<RoomInRoomPackage> GetRoomsInRoomPackage(int PackageID)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                var returnValue = dataContext.ExecuteQuery<RoomInRoomPackage>(CommandType.Text, "SELECT * FROM [dbo].[GDS_RoomsInRoomPackage] where PackageID = '" + PackageID + "'").OrderByDescending(p => p.ID);
                if (returnValue != null)
                {
                    return returnValue.ToList<RoomInRoomPackage>();
                }
                else

                    return null;
            }
        }
        public List<GetRoomPackageinPackage> GetRoomPackageDetailininPackage(int PackageID)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                return dataContext.ExecuteQuery<GetRoomPackageinPackage>(CommandType.Text, "SELECT [GDS_RoomPackageinPackage].ID,[PackageID],[RoomPackageID],[Name],[Description],[Count] FROM [dbo].[GDS_RoomPackageinPackage] left join [GDS_RoomsPackages] on [GDS_RoomPackageinPackage].roompackageid = GDS_RoomsPackages.id where PackageID = '" + PackageID + "'").OrderByDescending(p => p.ID).ToList<GetRoomPackageinPackage>();
            }
        }
        public List<GetRoomPackageinPackage> GetRoomPackageDetailininCard(Card ObjCard)
        {
            if (ObjCard == null)
            {
                return null;
            }
            using (IDataContext dataContext = DataContext.Instance())
            {

                var returnValue = dataContext.ExecuteQuery<GetRoomPackageinPackage>(CommandType.Text, "SELECT [GDS_RoomPackageinPackage].ID,[PackageID],[RoomPackageID],[Name],[Description],[Count] FROM [dbo].[GDS_RoomPackageinPackage] left join [GDS_RoomsPackages] on [GDS_RoomPackageinPackage].roompackageid = GDS_RoomsPackages.id where PackageID = '" + ObjCard.PackageID + "'").OrderByDescending(p => p.ID);
                if (returnValue != null)
                {
                    return returnValue.ToList<GetRoomPackageinPackage>();
                }
                else

                    return null;
            }
        }
        public class DiscountModelForCard
        {
            public int HotelID { get; set; }
            public string[] RoomID { get; set; }
            public string Card_PassKey { get; set; }
            public string[] Count { get; set; }
            public int[] night { get; set; }
            public string ReserveID { get; set; }
            public int?[] RackPrice { get; set; }
            public int ALLNight { get; set; }
        }
        public bool UpdateReserveIDOfCard(string Old, string New)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "update [dbo].[GDS_CardsUsed] set ReserveID = '" + New + "' where  ReserveID = '" + Old + "'");
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool FinalizeCard(string ReserveID)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "update [dbo].[GDS_CardsUsed] set finalized = 1 where  ReserveID = '" + ReserveID + "'");
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool RemoveTempCards(string ReserveID)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "delete from [dbo].[GDS_CardsUsed] where ReserveID = '" + ReserveID + "'");
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool AddCardUse(int CardID, int PackagID, int RoomPackageID, int HotelID, string RoomID, int Count, string ReserveID)
        {
            try
            {
                using (IDataContext dataContext = DataContext.Instance())
                {
                    dataContext.Execute(CommandType.Text, "INSERT INTO [dbo].[GDS_CardsUsed]([Card_ID],[Pakcage_ID],[RoomPackage_ID],[Hotel_ID],[Room_ID],[Count],[DateOfInserted],finalized,ReserveID) VALUES ('" + CardID + "','" + PackagID + "','" + RoomPackageID + "','" + HotelID + "','" + RoomID + "','" + Count + "', GETDATE(),0,N'" + ReserveID + "')");
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }
        public int GetUsedCountOfCard(int CardID, int PackagID, int RoomPackageID, int HotelID, int RoomID)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                return dataContext.ExecuteScalar<int>(CommandType.Text, "select COALESCE(sum([Count]),0) from [dbo].[GDS_CardsUsed] where [Card_ID] =" + CardID + " and [Pakcage_ID] =" + PackagID + " and [RoomPackage_ID] =" + RoomPackageID + " and [Hotel_ID] =" + HotelID + " and [Room_ID] =" + RoomID + " and finalized = 1");
            }
        }
        public DiscountModelForCard ApplyCardDiscount(DiscountModelForCard AfterDiscount)
        {

            Card ObjCard = GetCards(AfterDiscount.Card_PassKey).FirstOrDefault();
            RemoveTempCards(AfterDiscount.ReserveID);
            if (ObjCard == null)
            {
                return AfterDiscount;
            }
            List<GetRoomPackageinPackage> lstRoomPackagesOfCard = GetRoomPackageDetailininCard(ObjCard);
            foreach (var item in lstRoomPackagesOfCard)
            {
                List<RoomInRoomPackage> objRooms = GetRoomsInRoomPackage(item.RoomPackageID);
                foreach (var room in objRooms)
                {
                    item.Count = item.Count - GetUsedCountOfCard(ObjCard.id, ObjCard.PackageID, item.RoomPackageID, Int32.Parse(room.HotelID), Int32.Parse(room.RoomID));
                    for (int i = 0; i < AfterDiscount.RoomID.Count(); i++)
                    {
                        if (AfterDiscount.RoomID[i].ToString() == room.RoomID && AfterDiscount.HotelID.ToString() == room.HotelID)
                        {
                            if (Int32.Parse(AfterDiscount.Count[i]) * AfterDiscount.ALLNight < item.Count)
                            {
                                int Used = Int32.Parse(AfterDiscount.Count[i]) * AfterDiscount.ALLNight;
                                AfterDiscount.night[i] = Used;
                                AddCardUse(ObjCard.id, ObjCard.PackageID, item.RoomPackageID, AfterDiscount.HotelID, AfterDiscount.RoomID[i], Used, AfterDiscount.ReserveID);
                            }
                            else if (Int32.Parse(AfterDiscount.Count[i]) * AfterDiscount.ALLNight > item.Count)
                            {
                                int Used = item.Count;
                                AfterDiscount.night[i] = Used;
                                //AfterDiscount.Count[i] = ((Int32.Parse(AfterDiscount.Count[i]) * AfterDiscount.night[i]) - item.Count).ToString();
                                AddCardUse(ObjCard.id, ObjCard.PackageID, item.RoomPackageID, AfterDiscount.HotelID, AfterDiscount.RoomID[i], Used, AfterDiscount.ReserveID);
                            }
                            else
                            {
                                int Used = item.Count;
                                AfterDiscount.night[i] = Used;
                                //AfterDiscount.Count[i] = "0";
                                AddCardUse(ObjCard.id, ObjCard.PackageID, item.RoomPackageID, AfterDiscount.HotelID, AfterDiscount.RoomID[i], Used, AfterDiscount.ReserveID);
                            }
                        }
                    }
                }

            }
            return AfterDiscount;
        }

    }
}