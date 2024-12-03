using MySql.Data.MySqlClient;
using System;
using System.Linq;

namespace AutoCompositionIdeo.Model.Prestashop
{
	public class PsProductAttributeShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));

        public void Save()
        {
            DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttributeShop Obj)
        {
            DBPrestashop.PsProductAttributeShop.InsertOnSubmit(Obj);
            Save();
        }

        public bool ExistPsProductAttributeShop(uint ProductAttribute, uint Shop)
        {
			return DBPrestashop.PsProductAttributeShop.Any(Obj => Obj.IDProductAttribute == ProductAttribute && Obj.IDShop == Shop);
        }

        public PsProductAttributeShop ReadPsProductAttributeShop(uint ProductAttribute, uint Shop)
        {
            return DBPrestashop.PsProductAttributeShop.FirstOrDefault(Obj => Obj.IDProductAttribute == ProductAttribute && Obj.IDShop == Shop);
        }

        public void WriteDate(uint ProductAttribute)
        {
            string TxtSQL = "update ignore al_product_attribute_shop "
                 + " set al_product_attribute_shop.available_date = '0000-00-00 00:00:00' "
                 + " where al_product_attribute_shop.id_product_attribute = " + ProductAttribute
                 + " and al_product_attribute_shop.available_date = '0001-01-01 00:00:00' ";
            DBPrestashop.ExecuteCommand(TxtSQL);
        }

        public void EraseDefault(uint Product, uint ProductAttribute)
        {
            try
            {
				string TxtSQL = "update al_product_attribute_shop "
                     + " set al_product_attribute_shop.default_on = "
#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172 || PRESTASHOP_VERSION_177 || PRESTASHOP_VERSION_178 || PRESTASHOP_VERSION_171 || PRESTASHOP_VERSION_80)
                     + " null where al_product_attribute_shop.id_product = " + Product
#else
                     + " 0 where al_product_attribute_shop.id_product_attribute in (select al_product_attribute.id_product_attribute from al_product_attribute where al_product_attribute.id_product = " + Product + ") "
#endif
                     + " and al_product_attribute_shop.id_product_attribute != " + ProductAttribute;
                DBPrestashop.ExecuteCommand(TxtSQL);
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(ex.ToString());
            }
        }
    }
}
