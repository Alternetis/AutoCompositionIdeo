using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCompositionIdeo.Model.Prestashop
{
	public class PsProductRepository
    {
        private static string ps_fields_light = "P.id_product, PL.name ";
        private static string ps_fields_light_order = "P.id_product, PL.name ";
        private static string ps_fields_update = "P.id_product, PL.name, P.date_upd, P.reference ";
        private static string ps_fields_update_order = "P.id_product, PL.name, P.date_upd, P.reference ";
        private static string ps_fields_resume = "P.id_product, PL.name, P.reference, CL.name as default_category ";
        private static string ps_fields_resume_order = "P.id_product, PL.name, P.reference, default_category ";

        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));

        private bool ExistInShop(uint IDProduct, uint IDShop)
        {
            return DBPrestashop.PsProductShop.Any(result => result.IDShop == IDShop && result.IDProduct == IDProduct);
        }

        public void Save()
        {
            DBPrestashop.SubmitChanges();
        }

        public void Add(PsProduct Obj, uint IDShop)
        {
            DBPrestashop.PsProduct.InsertOnSubmit(Obj);
            Save();

            //Si le produit n'existe pas dans la boutique, il est rajouté.
            if (!ExistInShop(Obj.IDProduct, IDShop))
            {
                DBPrestashop.PsProductShop.InsertOnSubmit(new PsProductShop()
                {
                    Active = Obj.Active,
                    AdditionalShippingCost = Obj.AdditionalShippingCost,
                    AdvancedStockManagement = Obj.AdvancedStockManagement,
                    AvailableDate = Obj.AvailableDate,
                    AvailableForOrder = Obj.AvailableForOrder,
                    CacheDefaultAttribute = Obj.CacheDefaultAttribute,
                    Customizable = Obj.Customizable,
                    DateAdd = Obj.DateAdd,
                    DateUpd = Obj.DateUpd,
                    EcOtAx = Obj.EcOtAx,
                    IDCategoryDefault = Obj.IDCategoryDefault,
                    IDProduct = Obj.IDProduct,
                    IDShop = IDShop,
                    IDTaxRulesGroup = Obj.IDTaxRulesGroup,
                    Indexed = Obj.Indexed,
                    MinimalQuantity = Obj.MinimalQuantity,
                    OnlineOnly = Obj.OnlineOnly,
                    OnSale = Obj.OnSale,
                    Price = Obj.Price,
                    ShowPrice = Obj.ShowPrice,
                    TextFields = Obj.TextFields,
                    UnitPriceRatio = Obj.UnitPriceRatio,
                    Unity = Obj.Unity,
                    UploadAbleFiles = Obj.UploadAbleFiles,
                    WholesalePrice = Obj.WholesalePrice,
#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172 || PRESTASHOP_VERSION_177 || PRESTASHOP_VERSION_171 || PRESTASHOP_VERSION_80 || PRESTASHOP_VERSION_81)
                    PackStockType = Obj.PackStockType,
#endif
                });
                DBPrestashop.SubmitChanges();
            }
        }

        public List<uint> ListId(uint IDShop)
        {
			//System.Linq.IQueryable<uint> Return = from Table in DBPrestashop.PsProduct
			//                                       select Table.IDProduct;
			//return Return.ToList();

			//List<uint> products = new List<uint>();

			//foreach (var product in DBPrestashop.ExecuteQuery<PsProduct>(
			//    "SELECT DISTINCT P.id_product, P.id_supplier, P.id_manufacturer, P.id_category_default, P.id_shop_default, "+
			//    " P.id_tax_rules_group, P.on_sale, P.online_only, P.ean13, P.upc, P.ecotax, P.quantity, P.minimal_quantity, "+
			//    " P.price, P.wholesale_price, P.unity, P.unit_price_ratio, P.additional_shipping_cost, P.reference, P.supplier_reference, "+
			//    " P.location, P.width, P.height, P.depth, P.weight, P.out_of_stock, P.quantity_discount, P.customizable, "+
			//    " P.uploadable_files, P.text_fields, P.active, P.available_for_order, P.available_date, P.show_price, P.indexed, "+
			//    " P.cache_is_pack, P.cache_has_attachments, P.is_virtual, P.cache_default_attribute, P.date_add, P.date_upd, P.advanced_stock_management " +
			//    " FROM al_product P " +
			//    " INNER JOIN al_product_shop PS ON PS.id_product = P.id_product " +
			//    " WHERE PS.id_shop = {0} " +
			//    " ", IDShop))
			//    products.Add(product.IDProduct);


			// <JG> 22/02/2013 correction récupération liste des id produits
			return DBPrestashop.PsProductShop.Where(t => t.IDShop == IDShop).Select(t => t.IDProduct).ToList();
        }

        public bool ExistId(uint Id)
        {
            //Core.Log.WriteLog(DateTime.Now + " -  Recherche existance Article Prestashop Id = " + Id);
            bool result = DBPrestashop.PsProduct.Any(Obj => Obj.IDProduct == Id);
            //Core.Log.WriteLog(DateTime.Now + " -  Existance = " + result);
            return result;
        }

        public bool ProductIsActive(uint Id)
        {
            return DBPrestashop.PsProduct.Any(Obj => Obj.IDProduct == Id)
                && DBPrestashop.PsProduct.FirstOrDefault(Obj => Obj.IDProduct == Id).Active == 1;
        }

        public PsProduct ReadId(uint Id)
        {
            //Core.Log.WriteLog(DateTime.Now + " -  Recherche Article Prestashop Id = " + Id);
            PsProduct result = DBPrestashop.PsProduct.FirstOrDefault(Obj => Obj.IDProduct == Id);
            //Core.Log.WriteLog(DateTime.Now + " - Article Id = " + result.IDProduct);
            return result;
        }

        public bool ExistDefaultCatalog(int Catalog)
        {
            return DBPrestashop.PsProduct.Any(p => p.IDCategoryDefault == Catalog);
        }

        public List<PsProduct> List()
        {
            return DBPrestashop.PsProduct.ToList();
        }

        public void WriteDate(uint Product)
        {
            string TxtSQL = "update ignore al_product "
                 + " set al_product.available_date = '0000-00-00 00:00:00' "
                 + " where al_product.id_product = " + Product
                 + " and al_product.available_date = '0001-01-01 00:00:00' ";
            DBPrestashop.ExecuteCommand(TxtSQL);
        }

        public void ResetDateDispo(uint Product)
        {
#if (PRESTASHOP_VERSION_81)
            string TxtSQL = "update ignore al_product "
                 + " set al_product.available_date = '0000-00-00' "
                 + " where al_product.id_product = " + Product ;
#else
            string TxtSQL = "update ignore al_product "
                 + " set al_product.available_date = '0000-00-00 00:00:00' "
                 + " where al_product.id_product = " + Product ;
#endif
            DBPrestashop.ExecuteCommand(TxtSQL);
        }
    }
}
