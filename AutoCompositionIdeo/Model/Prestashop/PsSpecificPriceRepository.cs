using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;

namespace AutoCompositionIdeo.Model.Prestashop
{
    public class PsSpecificPriceRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));


        public void Save()
        {
            DBPrestashop.SubmitChanges();
        }

        //public void SaveReductionTypeFromDateToDate(PsSpecificPrice Obj)
        //{
        //    /*
        //    String TxtSQL = "update al_specific_price set reduction_type = '" + Obj.ReductionType + "' where id_specific_price = " + Obj.IDSpecificPrice;
        //    DBPrestashop.ExecuteCommand(TxtSQL);
        //    TxtSQL = "update al_specific_price set al_specific_price.from = '0000-00-00 00:00:00' where id_specific_price = " + Obj.IDSpecificPrice;
        //    DBPrestashop.ExecuteCommand(TxtSQL);
        //    TxtSQL = "update al_specific_price set al_specific_price.to = '0000-00-00 00:00:00 ' where id_specific_price = " + Obj.IDSpecificPrice;
        //    DBPrestashop.ExecuteCommand(TxtSQL);
        //    Save();
        //     */
        //    string TxtSQL = "update ignore al_specific_price "
        //         + " set al_specific_price.reduction_type = '" + Obj.ReductionType + "', "
        //         + " al_specific_price.from = '0000-00-00 00:00:00', "
        //         + " al_specific_price.to = '0000-00-00 00:00:00' "
        //         + " where id_specific_price = " + Obj.IDSpecificPrice;
        //    DBPrestashop.ExecuteCommand(TxtSQL);
        //}

        //public void WriteDateToDate(PsProduct Obj)          // pour éviter les boucles infinies - max 10 itérations
        //{
        //    int iteration = 10;
        //    while (iteration > 0)
        //    {
        //        try
        //        {
        //            DateToDate(Obj);
        //            iteration = 0;
        //        }
        //        catch
        //        {
        //            Thread.Sleep(100);
        //            iteration--;
        //        }
        //    }
        //}

        //private void DateToDate(PsProduct Obj)
        //{
        //    string TxtSQL = "update ignore al_specific_price "
        //         + " set al_specific_price.from = '0000-00-00 00:00:00', "
        //         + " al_specific_price.to = '0000-00-00 00:00:00' "
        //         + " where id_product = " + Obj.IDProduct;
        //    if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
        //        TxtSQL += " and al_specific_price.from = '0001-01-01 00:00:00' "
        //            + " and al_specific_price.to = '0001-01-01 00:00:00' ";
        //    DBPrestashop.ExecuteCommand(TxtSQL);
        //}
        //
        //public void WriteAllDateToDate()
        //{
        //    try
        //    {
        //        string TxtSQL = "update ignore al_specific_price "
        //             + " set al_specific_price.from = '0000-00-00 00:00:00', "
        //             + " al_specific_price.to = '0000-00-00 00:00:00' ";
        //        if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
        //            TxtSQL += " where al_specific_price.from = '0001-01-01 00:00:00' "
        //                + " and al_specific_price.to = '0001-01-01 00:00:00' ";
        //        DBPrestashop.ExecuteCommand(TxtSQL);
        //    }
        //    catch
        //    { }
        //
        //    try
        //    {
        //        string TxtSQL = "update ignore al_specific_price "
        //         + " set al_specific_price.from = '0000-00-00 00:00:00' ";
        //        if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
        //            TxtSQL += " where al_specific_price.from = '0001-01-01 00:00:00' ";
        //        DBPrestashop.ExecuteCommand(TxtSQL);
        //    }
        //    catch
        //    { }
        //
        //    try
        //    {
        //        string TxtSQL = "update ignore al_specific_price "
        //         + " set al_specific_price.to = '0000-00-00 00:00:00' ";
        //        if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
        //            TxtSQL += " where al_specific_price.to = '0001-01-01 00:00:00' ";
        //        DBPrestashop.ExecuteCommand(TxtSQL);
        //    }
        //    catch
        //    { }
        //}

        //public void WriteReductionType(PsProduct Obj)
        //{
        //    int iteration = 10;
        //    while (iteration > 0)
        //    {
        //        try
        //        {
        //            ReductionType(Obj);
        //            iteration = 0;
        //        }
        //        catch
        //        {
        //            Thread.Sleep(100);
        //            iteration--;
        //        }
        //    }
        //}

        //public void ReductionType(PsProduct Obj)
        //{
        //
        //    string TxtSQL = "update ignore al_specific_price "
        //         + " set al_specific_price.reduction_type = 'percentage' "
        //         + " where id_product = " + Obj.IDProduct
        //         + " and reduction <> 0 ";
        //    if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
        //        TxtSQL += " and al_specific_price.from = '0001-01-01 00:00:00' "
        //            + " and al_specific_price.to = '0001-01-01 00:00:00' ";
        //    DBPrestashop.ExecuteCommand(TxtSQL);
        //}
        //
        //public void DeleteFromProduct(uint Product)
        //{
        //    string TxtSQL = "delete ignore from al_specific_price where id_product = " + Product;
        //    if (Properties.Settings.Default.PRESTASHOPIdShop > 0)
        //         TxtSQL += " and id_shop = " + Properties.Settings.Default.PRESTASHOPIdShop /*+ " and id_shop_group = " + Core.Global.CurrentShop.IDShopGroup*/;
        //    if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
        //        TxtSQL += " and al_specific_price.from <= '0001-01-01 00:00:00' "
        //         + " and al_specific_price.to <= '0001-01-01 00:00:00' ";
        //    DBPrestashop.ExecuteCommand(TxtSQL);
        //    Save();
        //}
        //
        public void DeleteFromProductAttribute(uint Product, int ProductAttribute)
        {
            string TxtSQL = "delete ignore from al_specific_price where id_product = " + Product +" and id_product_attribute = " + ProductAttribute;
            //if (Properties.Settings.Default.PRESTASHOPIdShop > 0)
            //    TxtSQL += " and id_shop = " + Properties.Settings.Default.PRESTASHOPIdShop /*+ " and id_shop_group = " + Core.Global.CurrentShop.IDShopGroup*/;
            //if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
            //    TxtSQL += " and al_specific_price.from <= '0001-01-01 00:00:00' "
            //     + " and al_specific_price.to <= '0001-01-01 00:00:00' ";
            DBPrestashop.ExecuteCommand(TxtSQL);
            Save();
        }

        public void Add(PsSpecificPrice Obj)
        {
            DBPrestashop.PsSpecificPrice.InsertOnSubmit(Obj);
            Save();
        }

        public void AddList(IEnumerable<PsSpecificPrice> list)
        {
            DBPrestashop.PsSpecificPrice.InsertAllOnSubmit(list);
            Save();
        }
    }
}
