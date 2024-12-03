using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace AutoCompositionIdeo.Model.Prestashop
{
	public class PsProductAttributeCombinationRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));

        public void Save()
        {
            DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttributeCombination Obj)
        {
            DBPrestashop.PsProductAttributeCombination.InsertOnSubmit(Obj);
            Save();
        }

        public void Delete(PsProductAttributeCombination Obj)
        {
            DBPrestashop.PsProductAttributeCombination.DeleteOnSubmit(Obj);
            Save();
        }

        public void DeleteAll(List<PsProductAttributeCombination> list)
        {
            DBPrestashop.PsProductAttributeCombination.DeleteAllOnSubmit(list);
            Save();
        }

        public List<PsProductAttributeCombination> List()
        {
			return DBPrestashop.PsProductAttributeCombination.ToList();
        }

        public List<PsProductAttributeCombination> ListProductAttribute(uint ProductAttribute)
        {
			return DBPrestashop.PsProductAttributeCombination.Where(Table => Table.IDProductAttribute == ProductAttribute).ToList();
        }

        public bool ExistAttributeProductAttribute(uint Attribute, uint ProductAttribute)
        {
			return DBPrestashop.PsProductAttributeCombination.Any(Obj => Obj.IDAttribute == Attribute && Obj.IDProductAttribute == ProductAttribute);
        }

        public bool ExistAttributeProductAttribute(uint ProductAttribute)
        {
            return DBPrestashop.PsProductAttributeCombination.Any(Obj => Obj.IDProductAttribute == ProductAttribute);
        }

        public PsProductAttributeCombination ReadAttributeProductAttribute(uint ProductAttribute)
        {
            return DBPrestashop.PsProductAttributeCombination.FirstOrDefault(Obj => Obj.IDProductAttribute == ProductAttribute);
        }

        public PsProductAttributeCombination ReadAttributeProductAttribute(uint Attribute, uint ProductAttribute)
        {
            return DBPrestashop.PsProductAttributeCombination.FirstOrDefault(Obj => Obj.IDAttribute == Attribute && Obj.IDProductAttribute == ProductAttribute);
        }
    }
}
