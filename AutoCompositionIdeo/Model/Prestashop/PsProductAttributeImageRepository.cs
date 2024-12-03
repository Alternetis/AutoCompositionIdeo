using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace AutoCompositionIdeo.Model.Prestashop
{
	public class PsProductAttributeImageRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));

        public void Save()
        {
            DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttributeImage Obj)
        {
            DBPrestashop.PsProductAttributeImage.InsertOnSubmit(Obj);
            Save();
        }

        public void Delete(PsProductAttributeImage Obj)
        {
            DBPrestashop.PsProductAttributeImage.DeleteOnSubmit(Obj);
            Save();
        }

        public void DeleteAll(List<PsProductAttributeImage> Obj)
        {
            DBPrestashop.PsProductAttributeImage.DeleteAllOnSubmit(Obj);
            Save();
        }

        public List<PsProductAttributeImage> List()
        {
			return DBPrestashop.PsProductAttributeImage.ToList();
        }

        public List<PsProductAttributeImage> ListProductAttribute(uint ProductAttribute)
        {
			return DBPrestashop.PsProductAttributeImage.Where(Table => Table.IDProductAttribute == ProductAttribute).ToList();
        }

        public List<PsProductAttributeImage> ListImage(uint Image)
        {
            return DBPrestashop.PsProductAttributeImage.Where(o => o.IDImage == Image).ToList();
        }

        public bool ExistProductAttributeImage(uint ProductAttribute, uint Image)
        {
			return DBPrestashop.PsProductAttributeImage.Any(Obj => Obj.IDProductAttribute == ProductAttribute && Obj.IDImage == Image);
        }

        public bool ExistProductAttribute(uint ProductAttribute)
        {
			return DBPrestashop.PsProductAttributeImage.Any(Table => Table.IDProductAttribute == ProductAttribute);
        }
    }
}
