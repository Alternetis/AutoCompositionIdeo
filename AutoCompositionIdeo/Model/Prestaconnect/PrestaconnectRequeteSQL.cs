using AutoCompositionIdeo.Core;
using System;
using System.Data.SqlClient;
using static AutoCompositionIdeo.Program;
using System.Collections.Generic;
using System.ComponentModel;

namespace AutoCompositionIdeo.Model.Prestaconnect
{
    class PrestaconnectRequeteSQL
    {
        public static int SearchCatIdArticle(string connectionSageString, int CL_no, string connectionPrestaconnectString)
        {
            try
            {
                using (SqlConnection sageConnection = new SqlConnection(connectionSageString))
                {
                    sageConnection.Open();
                    using (SqlCommand commandSage = new SqlCommand(string.Format("SELECT cbMarq FROM F_CATALOGUE WHERE " +
                        $"CL_No = '{CL_no}'"), sageConnection))
                    {
                        int CatSagId = 0;
                        CatSagId = (int)commandSage.ExecuteScalar();
                        if (CatSagId != 0)
                        {
                            using (SqlConnection prestaconnectConnection = new SqlConnection(connectionPrestaconnectString))
                            {
                                prestaconnectConnection.Open();
                                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT Cat_Id FROM Catalog WHERE Sag_id = " + CatSagId), prestaconnectConnection))
                                {
                                    using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                                    {
                                        if (readerPrestaconnect.Read())
                                        {
                                            return readerPrestaconnect.GetInt32(0);
                                        }
                                        else
                                        {
                                            readerPrestaconnect.Close();
                                            using (SqlCommand commandSageParent = new SqlCommand(string.Format($"SELECT CL_NoParent FROM F_CATALOGUE WHERE CL_No = '{CL_no}'"), sageConnection))
                                            {
                                                using (SqlDataReader readerSageParent = commandSageParent.ExecuteReader())
                                                {
                                                    if (readerSageParent.Read())
                                                    {
                                                        int Cl_NoParent = readerSageParent.GetInt32(0);
                                                        if (Cl_NoParent != 0)
                                                        {
                                                            return SearchCatIdArticle(connectionSageString, Cl_NoParent, connectionPrestaconnectString);
                                                        }
                                                        else
                                                            return 0;
                                                    }
                                                    else
                                                    {
                                                        return 0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR lors de la recherche de la catégorie Prestaconnect de destination : " + ex.ToString());
                Core.Log.WriteLog(DateTime.Now + " - ERREUR lors de la recherche de la catégorie Prestaconnect de destination : " + ex.ToString());
                return 0;
            }
        }
        public static int SearchArtIdPrestaconnect(string connectionPrestaconnectString, int cbMarq, string Art_Ref)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT A.Art_ID FROM Article A " +
                    "LEFT JOIN CompositionArticle CA on A.Art_id = CA.ComArt_ArtId Where CA.ComArt_F_ARTICLE_SagId = " + cbMarq + " OR A.Art_Ref = '" + Art_Ref + "'"), connectionPrestaconnect))
                {
                    using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                    {
                        if (readerPrestaconnect.Read())
                        {
                            return readerPrestaconnect.GetInt32(0);
                        }
                        else
                        {
                            return -1;
                        }
                    }

                }
            }
        }

        public static string SearchArtnamePrestaconnectById(string connectionPrestaconnectString, int Art_Id)
        {
            try
            {
                using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
                {
                    connectionPrestaconnect.Open();
                    using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT TOP 1 Art_Name FROM Article " +
                        " Where Art_Id = " + Art_Id), connectionPrestaconnect))
                    {
                        using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                        {
                            if (readerPrestaconnect.Read())
                            {
                                return readerPrestaconnect.GetString(0);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                return null;
            }
        }

        public static int SearchArtIdPrestaconnectByRef(string connectionPrestaconnectString, string Art_Ref)
        {
            try
            {
                using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
                {
                    connectionPrestaconnect.Open();
                    using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT TOP 1 Art_ID FROM Article " +
                        " Where Art_Ref LIKE '" + Core.Formattage.EscapeSqlString(Art_Ref) + "%'"), connectionPrestaconnect))
                    {
                        using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                        {
                            if (readerPrestaconnect.Read())
                            {
                                return readerPrestaconnect.GetInt32(0);
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                return -1;
            }
        }

        public static int SearchArtIdPrestaconnectByName(string connectionPrestaconnectString, string Art_Name)
        {
            try
            {
                using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
                {
                    connectionPrestaconnect.Open();
                    using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT TOP 1 Art_ID FROM Article " +
                        " Where Art_Name = '" + Core.Formattage.EscapeSqlString(Art_Name) + "'"), connectionPrestaconnect))
                    {
                        using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                        {
                            if (readerPrestaconnect.Read())
                            {
                                return readerPrestaconnect.GetInt32(0);
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                return -1;
            }
        }

        public static int SearchSagIdPrestaconnect(string connectionPrestaconnectString, int Sag_Id)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT Art_Id FROM Article " +
                    " Where Sag_Id = " + Sag_Id), connectionPrestaconnect))
                {
                    using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                    {
                        if (readerPrestaconnect.Read())
                        {
                            return readerPrestaconnect.IsDBNull(readerPrestaconnect.GetOrdinal("Art_Id")) ? 0 : (int)readerPrestaconnect["Art_Id"];
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
        }

        public static int SearchPreIdPrestaconnect(string connectionPrestaconnectString, int Art_Id)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT Pre_Id FROM Article " +
                    " Where Art_Id = " + Art_Id), connectionPrestaconnect))
                {
                    using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                    {
                        if (readerPrestaconnect.Read())
                        {
                            return readerPrestaconnect.IsDBNull(readerPrestaconnect.GetOrdinal("Pre_Id")) ? 0 : (int)readerPrestaconnect["Pre_Id"];
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
        }

        public static int CreationSimple(string connectionSageString, string connectionPrestaconnectString, Article article, int catId)
        {
            int ArtId = -1;
            //int Sag_Id = 0;
            bool exist = false;
            // Vérifie si une composition existe déjà
            //Sag_Id = Sage.SageRequeteSQL.SearchSageId(connectionSageString, composition);
            //Console.WriteLine("Sag_Id = " + Sag_Id);
            //Core.Log.WriteLog("Sag_Id = " + Sag_Id);
            string reference = article.AR_Ref.Substring(1);
            exist = ArticleRefExist(connectionPrestaconnectString, reference);
            Console.WriteLine("exist = " + exist);
            Core.Log.WriteLog("exist = " + exist);


            if (exist == false
                )
            {
                string dft = Formattage.RemovePurge(article.AR_Design, 255);
                dft = Formattage.EscapeSqlString(dft);
                // Crée la composition si elle n'éxiste pas
                using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
                {
                    connectionPrestaconnect.Open();

                    Console.WriteLine("création article simple " + reference);
                    Core.Log.WriteLog("création article simple " + reference);
                    using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("INSERT INTO Article "
                                                    + $@" ([Art_Name]
                                                      ,[Art_Description]
                                                      ,[Art_Description_Short]
                                                      ,[Art_LinkRewrite]
                                                      ,[Art_MetaTitle]
                                                      ,[Art_MetaKeyword]
                                                      ,[Art_MetaDescription]
                                                      ,[Art_Ref]
                                                      ,[Art_Ean13]
                                                      ,[Art_Pack]
                                                      ,[Art_Solde]
                                                      ,[Art_Active]
                                                      ,[Art_Sync]
                                                      ,[Art_Date]
                                                      ,[Art_RedirectType]
                                                      ,[Art_RedirectProduct]
                                                      ,[Art_Manufacturer]
                                                      ,[Art_Supplier]
                                                      ,[Sag_Id]
                                                      ,[Pre_Id]
                                                      ,[Cat_Id]
                                                      ,[Art_Type]
                                                      ,[Art_SyncPrice]) "
                                                    + " VALUES ('"
                                                    + Formattage.EscapeSqlString(article.AR_Design) + "','" // Art_Name
                                                    + Formattage.EscapeSqlString(article.AR_Design) + "','" // Art_Description
                                                    + Formattage.EscapeSqlString(article.AR_Design) + "','" // Art_Description_Short
                                                    + Formattage.ReadLinkRewrite(dft) + "','" // Art_Link_Rewrite
                                                    + dft + "','" // Art_metaTitle
                                                    + Formattage.RemovePurgeMeta(dft, 255) + "','" // Art_Metakeyword
                                                    + dft + "','"  // Art_MetaDescription
                                                    + reference + "' , '' ,"
                                                    + 0 + "," + 0 + "," + 1 + "," + 1 + ",'" + DateTime.Now + "'," + "'404'"
                                                    + "," + 0 + "," + 0 + "," + 0 + ",'" + 0 + "'," + "null" + ",'" + catId + "'," + 0 + "," + 1 + ")"), connectionPrestaconnect))
                    {
                        try
                        {
                            SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                            readerPrestaconnect.Close();
                        }
                        catch (Exception ex)
                        {
                            Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + commandPrestaconnect.CommandText + " - " + ex.ToString());
                            Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                        }
                    }
                    using (SqlCommand commandPrestaconnect2 = new SqlCommand(string.Format("SELECT TOP 1 Art_Id, Cat_Id FROM Article " +
                        "WHERE Art_Ref ='" + reference + "' OR Art_Name = '" + Core.Formattage.EscapeSqlString(article.AR_Design) + "'"), connectionPrestaconnect))
                    {
                        SqlDataReader readerPrestaconnect2 = commandPrestaconnect2.ExecuteReader();
                        readerPrestaconnect2.Read();
                        ArtId = readerPrestaconnect2.GetInt32(0);
                        int CatId = readerPrestaconnect2.GetInt32(1);
                        Console.WriteLine("ArtId = " + ArtId + "; CatId = " + CatId);
                        Core.Log.WriteLog("ArtId = " + ArtId + "; CatId = " + CatId);
                        readerPrestaconnect2.Close();

                        using (SqlCommand commandPrestaconnect4 = new SqlCommand(string.Format("SELECT TOP 1 Art_ID FROM ArticleCatalog " +
                            "WHERE Art_Id = " + ArtId + " AND Cat_Id = " + CatId), connectionPrestaconnect))
                        {
                            using (SqlDataReader readerPrestaconnect4 = commandPrestaconnect4.ExecuteReader())
                            {
                                if (!readerPrestaconnect4.Read())
                                {
                                    Console.WriteLine(commandPrestaconnect4.CommandText);
                                    Core.Log.WriteLog(commandPrestaconnect4.CommandText);
                                    readerPrestaconnect4.Close();
                                    using (SqlCommand commandPrestaconnect3 = new SqlCommand(string.Format("INSERT INTO ArticleCatalog " +
                                    "VALUES(" + ArtId + "," + CatId + ")"), connectionPrestaconnect))
                                    {
                                        SqlDataReader readerPrestaconnect3 = commandPrestaconnect3.ExecuteReader();
                                        readerPrestaconnect3.Close();
                                    }
                                }
                                else
                                {
                                    readerPrestaconnect4.Close();
                                }
                            }
                        }
                    }
                    return ArtId;
                }
            }
            return -1;
        }

        public static int CreationComposition(string connectionSageString, string connectionPrestaconnectString, Composition composition, int catId)
        {
            int ArtId = -1;
            //int Sag_Id = 0;
            bool exist = false;

            exist = ArticleRefExist(connectionPrestaconnectString, composition.InfoProduit);
            Console.WriteLine("exist = " + exist);
            Core.Log.WriteLog("exist = " + exist);

            Article articleBase = composition.Articles[0];

            if (exist == false
                && articleBase != null
                )
            {
                string dft = Formattage.RemovePurge(articleBase.AR_Design, 255);
                dft = Formattage.EscapeSqlString(dft);
                // Crée la composition si elle n'éxiste pas
                using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
                {
                    connectionPrestaconnect.Open();
                    Console.WriteLine("création composition " + composition.InfoProduit);
                    Core.Log.WriteLog("création composition " + composition.InfoProduit);
                    using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("INSERT INTO Article "
                                                    + $@" ([Art_Name]
                                                      ,[Art_Description]
                                                      ,[Art_Description_Short]
                                                      ,[Art_LinkRewrite]
                                                      ,[Art_MetaTitle]
                                                      ,[Art_MetaKeyword]
                                                      ,[Art_MetaDescription]
                                                      ,[Art_Ref]
                                                      ,[Art_Ean13]
                                                      ,[Art_Pack]
                                                      ,[Art_Solde]
                                                      ,[Art_Active]
                                                      ,[Art_Sync]
                                                      ,[Art_Date]
                                                      ,[Art_RedirectType]
                                                      ,[Art_RedirectProduct]
                                                      ,[Art_Manufacturer]
                                                      ,[Art_Supplier]
                                                      ,[Sag_Id]
                                                      ,[Pre_Id]
                                                      ,[Cat_Id]
                                                      ,[Art_Type]
                                                      ,[Art_SyncPrice]) "
                                                    + " VALUES ('"
                                                    + Formattage.EscapeSqlString(articleBase.AR_Design) + "','" // Art_Name
                                                    + Formattage.EscapeSqlString(articleBase.AR_Design) + "','" // Art_Description
                                                    + Formattage.EscapeSqlString(articleBase.AR_Design) + "','" // Art_Description_Short
                                                    + Formattage.ReadLinkRewrite(dft) + "','" // Art_Link_Rewrite
                                                    + dft + "','" // Art_metaTitle
                                                    + Formattage.RemovePurgeMeta(dft, 255) + "','" // Art_Metakeyword
                                                    + dft + "','"  // Art_MetaDescription
                                                    + composition.InfoProduit + "' , '' ,"
                                                    + 0 + "," + 0 + "," + 1 + "," + 1 + ",'" + DateTime.Now + "'," + "'404'"
                                                    + "," + 0 + "," + 0 + "," + 0 + ",'" + 0 + "'," + "null" + ",'" + catId + "'," + 1 + "," + 1 + ")"), connectionPrestaconnect))
                    {
                        try
                        {
                            SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                            readerPrestaconnect.Close();
                        }
                        catch (Exception ex)
                        {
                            Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + commandPrestaconnect.CommandText + " - " + ex.ToString());
                            Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                        }
                    }
                    using (SqlCommand commandPrestaconnect2 = new SqlCommand(string.Format("SELECT TOP 1 Art_Id, Cat_Id FROM Article " +
                        "WHERE Art_Ref ='" + composition.InfoProduit + "' OR Art_Name = '" + Core.Formattage.EscapeSqlString(articleBase.AR_Design) + "'"), connectionPrestaconnect))
                    {
                        SqlDataReader readerPrestaconnect2 = commandPrestaconnect2.ExecuteReader();
                        readerPrestaconnect2.Read();
                        ArtId = readerPrestaconnect2.GetInt32(0);
                        int CatId = readerPrestaconnect2.GetInt32(1);
                        Console.WriteLine("ArtId = " + ArtId + "; CatId = " + CatId);
                        Core.Log.WriteLog("ArtId = " + ArtId + "; CatId = " + CatId);
                        readerPrestaconnect2.Close();

                        using (SqlCommand commandPrestaconnect4 = new SqlCommand(string.Format("SELECT TOP 1 Art_ID FROM ArticleCatalog " +
                            "WHERE Art_Id = " + ArtId + " AND Cat_Id = " + CatId), connectionPrestaconnect))
                        {
                            using (SqlDataReader readerPrestaconnect4 = commandPrestaconnect4.ExecuteReader())
                            {
                                if (!readerPrestaconnect4.Read())
                                {
                                    Console.WriteLine(commandPrestaconnect4.CommandText);
                                    Core.Log.WriteLog(commandPrestaconnect4.CommandText);
                                    readerPrestaconnect4.Close();
                                    using (SqlCommand commandPrestaconnect3 = new SqlCommand(string.Format("INSERT INTO ArticleCatalog " +
                                    "VALUES(" + ArtId + "," + CatId + ")"), connectionPrestaconnect))
                                    {
                                        SqlDataReader readerPrestaconnect3 = commandPrestaconnect3.ExecuteReader();
                                        readerPrestaconnect3.Close();
                                    }
                                }
                                else
                                {
                                    readerPrestaconnect4.Close();
                                }
                            }
                        }
                    }
                    return ArtId;
                }
            }
            else
            {
                Console.WriteLine("La composition " + composition.InfoProduit + " existe déjà sur Prestaconnect.");
                Core.Log.WriteLog("La composition " + composition.InfoProduit + " existe déjà sur Prestaconnect.");
            }
            return -1;
        }

        private static bool ArticleRefExist(string connectionPrestaconnectString, string Art_Ref)
        {
            bool exist = false;
            exist = SearchArtIdPrestaconnectByRef(connectionPrestaconnectString, Art_Ref) != -1;
            return exist;
        }

        private static bool SageIdExist(string connectionPrestaconnectString, int Sag_Id)
        {
            bool exist = false;
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                exist = SageIdCompositionArticleExist(Sag_Id, connectionPrestaconnect) || SageIdArticleExist(Sag_Id, connectionPrestaconnect);

            }

            return exist;
        }

        private static string NameCompositionArticleRead(int Sag_Id, string connectionPrestaconnectString)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("Select TOP 1 A.Art_Name FROM CompositionArticle CA INNER JOIN Article A ON A.Art_Id = CA.ComArt_ArtId where ComArt_F_ARTICLE_SagId =" + Sag_Id), connectionPrestaconnect))
                {
                    using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                    {
                        if (readerPrestaconnect.Read())
                        {
                            return readerPrestaconnect.GetString(0);
                        }
                    }
                }
            }
            return "";
        }

        private static int SageIdCompositionArticleRead(int Sag_Id, string connectionPrestaconnectString)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("Select TOP 1 ComArt_ArtId FROM CompositionArticle where ComArt_F_ARTICLE_SagId =" + Sag_Id), connectionPrestaconnect))
                {
                    using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                    {
                        if (readerPrestaconnect.Read())
                        {
                            return readerPrestaconnect.GetInt32(0);
                        }
                    }
                }
            }
            return -1;
        }

        private static bool SageIdCompositionArticleExist(int Sag_Id, SqlConnection connectionPrestaconnect)
        {
            using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("Select TOP 1 ComArt_ArtId FROM CompositionArticle where ComArt_F_ARTICLE_SagId =" + Sag_Id), connectionPrestaconnect))
            {
                using (SqlDataReader readerSage = commandPrestaconnect.ExecuteReader())
                {
                    if (readerSage.Read())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool SageIdArticleExist(int Sag_Id, SqlConnection connectionPrestaconnect)
        {
            using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("Select TOP 1 Art_Id FROM Article where Sag_Id =" + Sag_Id), connectionPrestaconnect))
            {
                using (SqlDataReader readerSage = commandPrestaconnect.ExecuteReader())
                {
                    if (readerSage.Read())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool ExistG_AttributeGroup(string connectionPrestaconnectString, int ArtId, int PS_IdAttributeGroup)
        {

            Core.Log.WriteLog($"Vérification du groupe d'attribut \n ID_Prestashop : {PS_IdAttributeGroup} \n ID_ArticlePrestaconnect : {ArtId}");
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT TOP 1 * FROM CompositionArticleAttributeGroup " +
                    "WHERE Cag_ArtId = " + ArtId + " and Cag_AttributeGroup_PreId = " + PS_IdAttributeGroup), connectionPrestaconnect))
                {
                    SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    readerPrestaconnect.Read();
                    int Exist = readerPrestaconnect.GetInt32(0);
                    readerPrestaconnect.Close();
                    if (Exist == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void RemoveG_AttributeGroup(string connectionPrestaconnectString, int ArtId, int PS_IdAttributeGroup)
        {

            Core.Log.WriteLog($"Suppression du groupe d'attribut \n ID_Prestashop : {PS_IdAttributeGroup} \n ID_ArticlePrestaconnect : {ArtId}");
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT COUNT(*) FROM CompositionArticleAttributeGroup " +
                    "WHERE Cag_ArtId = " + ArtId + " and Cag_AttributeGroup_PreId = " + PS_IdAttributeGroup), connectionPrestaconnect))
                {
                    SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    readerPrestaconnect.Read();
                    int Exist = readerPrestaconnect.GetInt32(0);
                    readerPrestaconnect.Close();
                    if (Exist != 0)
                    {
                        using (SqlCommand commandPrestaconnect2 = new SqlCommand(string.Format("DELETE FROM CompositionArticleAttributeGroup " +
                            "Values(" + ArtId + " , " + PS_IdAttributeGroup + ")"), connectionPrestaconnect))
                        {
                            SqlDataReader readerPrestaconnect2 = commandPrestaconnect2.ExecuteReader();
                        }
                    }
                }
            }
        }

        public static void InsertG_AttributeGroup(string connectionPrestaconnectString, int ArtId, int PS_IdAttributeGroup)
        {

            Core.Log.WriteLog($"Ajout du groupe d'attribut \n ID_Prestashop : {PS_IdAttributeGroup} \n ID_ArticlePrestaconnect : {ArtId}");
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT COUNT(*) FROM CompositionArticleAttributeGroup " +
                    "WHERE Cag_ArtId = " + ArtId + " and Cag_AttributeGroup_PreId = " + PS_IdAttributeGroup), connectionPrestaconnect))
                {
                    SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    readerPrestaconnect.Read();
                    int Exist = readerPrestaconnect.GetInt32(0);
                    readerPrestaconnect.Close();
                    if (Exist == 0)
                    {
                        using (SqlCommand commandPrestaconnect2 = new SqlCommand(string.Format("Insert Into CompositionArticleAttributeGroup " +
                            "Values(" + ArtId + " , " + PS_IdAttributeGroup + ")"), connectionPrestaconnect))
                        {
                            SqlDataReader readerPrestaconnect2 = commandPrestaconnect2.ExecuteReader();
                        }
                    }
                }
            }
        }
        public static bool InsertAttribute(string connectionPrestaconnectString, int AttributeGroupId, int AttributeId, int SageId, int PCArticleId)
        {
            int D_ArtId = -1;
            bool inserted = false;
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("Select ComArt_Id FROM CompositionArticle " +
                    $"WHERE ComArt_F_ARTICLE_SagId = {SageId} AND ComArt_ArtId = {PCArticleId} "), connectionPrestaconnect))
                {
                    Core.Log.WriteLog(commandPrestaconnect.CommandText);
                    SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    if (readerPrestaconnect.Read())
                    {
                        if (readerPrestaconnect.IsDBNull(0) == false)
                        {
                            D_ArtId = readerPrestaconnect.GetInt32(0);
                        }
                    }
                }
            }

            if (D_ArtId != -1)
            {
                using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
                {
                    connectionPrestaconnect.Open();
                    using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT TOP 1 * FROM CompositionArticleAttribute " +
                        "WHERE Caa_ComArtId = " + D_ArtId + " and Caa_AttributeGroup_PreId = " + AttributeGroupId), connectionPrestaconnect))
                    {
                        SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                        if (!readerPrestaconnect.Read())
                        {
                            readerPrestaconnect.Close();
                            using (SqlCommand commandPrestaconnect2 = new SqlCommand(string.Format("Insert Into CompositionArticleAttribute " +
                                "Values(" + D_ArtId + "," + AttributeGroupId + "," + AttributeId + ")"), connectionPrestaconnect))
                            {
                                Core.Log.WriteLog(commandPrestaconnect2.CommandText);
                                Console.WriteLine(commandPrestaconnect2.CommandText);
                                SqlDataReader readerPrestaconnect2 = commandPrestaconnect2.ExecuteReader();
                                inserted = true;
                            }
                        }
                        else
                        {
                            CompositionArticleAttribute CAA = Model.Prestaconnect.CompositionArticleAttribute.Construct(readerPrestaconnect);
                            if (CAA.Caa_Attribute_PreId != AttributeId)
                            {
                                readerPrestaconnect.Close();
                                using (SqlCommand commandPrestaconnect2 = new SqlCommand(string.Format("UPDATE CompositionArticleAttribute " +
                                    " SET Caa_Attribute_PreId = " + AttributeId + " WHERE Caa_ComArtId = " + D_ArtId + " AND Caa_AttributeGroup_PreId = " + AttributeGroupId + ""), connectionPrestaconnect))
                                {
                                    Core.Log.WriteLog(commandPrestaconnect2.CommandText);
                                    Console.WriteLine(commandPrestaconnect2.CommandText);
                                    SqlDataReader readerPrestaconnect2 = commandPrestaconnect2.ExecuteReader();
                                    inserted = true;
                                }
                            }
                        }
                    }
                }
            }
            return inserted;
        }

        public static bool DeclinaisonExist(string connectionPrestaconnectString, int SageId)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT count(*) FROM CompositionArticle WHERE " +
                    $"ComArt_F_ARTICLE_SagId = {SageId}"
                    /*+ $"AND ComArt_ArtId = " + Art_Id*/), connectionPrestaconnect))
                {
                    SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    readerPrestaconnect.Read();
                    if (readerPrestaconnect.GetInt32(0) > 0)
                    { return true; }
                    else { return false; }
                }
            }
        }
        public static int NombreDeclinaison(string connectionPrestaconnectString, int Art_Id)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT count(*) FROM CompositionArticle Where " +
                    "ComArt_ArtId = " + Art_Id), connectionPrestaconnect))
                {
                    using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                    {
                        if (readerPrestaconnect.Read())
                        {
                            return readerPrestaconnect.GetInt32(0);
                        }
                    }
                }
                return -1;
            }
        }

        public static void ActiveArticle(string connectionPrestaconnectString, int Art_Id, int active)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format($@"UPDATE Article SET Art_Active = {active} WHERE Art_Id = '{Art_Id}'"), connectionPrestaconnect))
                {
                    commandPrestaconnect.ExecuteNonQuery();
                }
            }
        }

        public static void InsertDeclinaisonArticle(string connectionPrestaconnectString, int Art_Id, int sageId, int Default)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("Insert Into CompositionArticle ([ComArt_ArtId],[ComArt_F_ARTICLE_SagId],[ComArt_F_ARTENUMREF_SagId],[ComArt_F_CONDITION_SagId],[ComArt_Quantity],[ComArt_Default],[ComArt_Sync],[ComArt_Active],[Pre_Id]) Values("
                    + Art_Id + "," + sageId + ",null,null, 1.000000," + Default + ",1,1,NULL)"), connectionPrestaconnect))
                {
                    //SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    //
                    //readerPrestaconnect.Close();
                    try
                    {
                        commandPrestaconnect.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + commandPrestaconnect.CommandText + " - " + ex.ToString());
                        Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                    }
                }
            }
        }

        public static void UpdateArticle(string connectionPrestaconnectString, int Art_Id)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("UPDATE ARTICLE SET Art_Date = GETDATE() WHERE Art_ID = " + Art_Id), connectionPrestaconnect))
                {
                    //SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    //
                    //readerPrestaconnect.Close();
                    try
                    {
                        commandPrestaconnect.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + commandPrestaconnect.CommandText + " - " + ex.ToString());
                        Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
                    }
                }
            }
        }
    }

    public class CompositionArticle : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            PropertyChanging?.Invoke(this, emptyChangingEventArgs);
        }

        protected virtual void SendPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int ComArt_Id { get; set; }
        public int ComArt_ArtId { get; set; }
        public int ComArt_F_ARTICLE_SagId { get; set; }
        public int? ComArt_F_ARTENUMREF_SagId { get; set; }
        public int? ComArt_F_CONDITION_SagId { get; set; }
        public decimal ComArt_Quantity { get; set; }
        public bool ComArt_Default { get; set; }
        public bool ComArt_Sync { get; set; }
        public bool ComArt_Active { get; set; }
        public int? Pre_Id { get; set; }

        public static CompositionArticle DeclinaisonGet(string connectionPrestaconnectString, int SageId)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT TOP 1 * FROM CompositionArticle WHERE " +
                    $"ComArt_F_ARTICLE_SagId = {SageId}"
                /* + " AND ComArt_ArtId = " + Art_Id*/), connectionPrestaconnect))
                {
                    SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    if (readerPrestaconnect.Read())
                    {
                        if (readerPrestaconnect.GetInt32(0) > 0)
                        {
                            return Construct(readerPrestaconnect);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
        }

        public static List<CompositionArticle> DeclinaisonList(string connectionPrestaconnectString, int SageId)
        {
            List<CompositionArticle> ListCA = new List<CompositionArticle>();
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT * FROM CompositionArticle WHERE " +
                    $"ComArt_F_ARTICLE_SagId = {SageId}"), connectionPrestaconnect))
                {
                    using (SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader())
                    {
                        while (readerPrestaconnect.Read())
                        {
                            ListCA.Add(Construct(readerPrestaconnect));
                        }
                    }
                }
                return ListCA;
            }
        }

        public static CompositionArticle DeclinaisonGetNext(string connectionPrestaconnectString, int Art_Id, int SageId)
        {
            using (SqlConnection connectionPrestaconnect = new SqlConnection(connectionPrestaconnectString))
            {
                connectionPrestaconnect.Open();
                using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format("SELECT TOP 1 * FROM CompositionArticle WHERE " +
                    $"ComArt_F_ARTICLE_SagId = {SageId} AND ComArt_ArtId = " + Art_Id + " AND ComArt_Default != 1"), connectionPrestaconnect))
                {
                    SqlDataReader readerPrestaconnect = commandPrestaconnect.ExecuteReader();
                    if (readerPrestaconnect.Read())
                    {
                        if (readerPrestaconnect.GetInt32(0) > 0)
                        {
                            return Construct(readerPrestaconnect);
                        }
                        else { return null; }
                    }
                    else { return null; }
                }
            }
        }

        public static List<CompositionArticle> ListArticle(int ComArt_ArtId)
        {
            List<CompositionArticle> compositionArticles = new List<CompositionArticle>();

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.PrestaconnectConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM CompositionArticle WHERE ComArt_ArtId = {ComArt_ArtId}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            compositionArticles.Add(Construct(reader));
                        }
                    }
                }
            }
            return compositionArticles;
        }

        public static bool UpdateActive(CompositionArticle CompositionArticle, bool active)
        {
            try
            {
                if (CompositionArticle != null && active == false)
                {
                    Delete(CompositionArticle);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Core.Log.WriteLog(ex.ToString());
                return false;
            }
        }

        //public static bool CleanComposition(CompositionArticle CompositionArticle, int cbMarq, int NewArt_Id)
        //{
        //    try
        //    {
        //        if (CompositionArticle != null && cbMarq != 0 && DeclinaisonGet(Properties.Settings.Default.PrestaconnectConnection, CompositionArticle.ComArt_ArtId, cbMarq) != null)
        //        {
        //            int Art_Id = CompositionArticle.ComArt_ArtId;
        //            //Delete(CompositionArticle);
        //            if (CompositionArticle.ComArt_Default == true)
        //            {
        //                Model.Prestaconnect.CompositionArticle CompositionArticleNext = DeclinaisonGetNext(Properties.Settings.Default.PrestaconnectConnection, CompositionArticle.ComArt_ArtId, CompositionArticle.ComArt_F_ARTICLE_SagId);
        //                if (CompositionArticleNext != null)
        //                {
        //                    CompositionArticle.ComArt_Default = false;
        //                    CompositionArticleNext.ComArt_Default = true;
        //                    CompositionArticle.Save();
        //                    CompositionArticleNext.Save();
        //                }
        //            }
        //            Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
        //            if (CompositionArticle.Pre_Id != null && PsProductAttributeRepository.ExistProductAttribute((uint)CompositionArticle.Pre_Id))
        //            {
        //                Core.Log.WriteLog("Suppression de l'attribut" + CompositionArticle.Pre_Id + " dans Prestashop.");
        //                PsProductAttributeRepository.Delete(PsProductAttributeRepository.ReadProductAttribute((uint)CompositionArticle.Pre_Id));
        //                CompositionArticle.Pre_Id = null;
        //                CompositionArticle.Save();
        //            }
        //            else
        //            {
        //                Core.Log.WriteLog("Pas d'attribut " + CompositionArticle.Pre_Id + " dans Prestashop.");
        //                if (CompositionArticle.Pre_Id != null)
        //                {
        //                    CompositionArticle.Pre_Id = null;
        //                    CompositionArticle.Save();
        //                }
        //            }
        //            CompositionArticle.ComArt_ArtId = NewArt_Id;
        //            CompositionArticle.Save();
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        Core.Log.WriteLog(ex.ToString());
        //        return false;
        //    }
        //}

        public void Save()
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.PrestaconnectConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"UPDATE CompositionArticle SET ComArt_ArtId = {ComArt_ArtId}, ComArt_F_ARTICLE_SagId = {ComArt_F_ARTICLE_SagId},"
                    + $" ComArt_F_ARTENUMREF_SagId = {(ComArt_F_ARTENUMREF_SagId != null ? $"{ComArt_F_ARTENUMREF_SagId}" : "NULL")}, ComArt_F_CONDITION_SagId = {(ComArt_F_CONDITION_SagId != null ? $"{ComArt_F_CONDITION_SagId}" : "NULL")},"
                    + $" ComArt_Quantity = {ComArt_Quantity.ToString().Replace(",", ".")}, ComArt_Default = {(ComArt_Default ? 1 : 0)},"
                    + $" ComArt_Sync = {(ComArt_Sync ? 1 : 0)}, ComArt_Active = {(ComArt_Active ? 1 : 0)}, Pre_Id = {(Pre_Id != null ? $"{Pre_Id}" : "NULL")} WHERE ComArt_Id = {ComArt_Id}", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void Delete(CompositionArticle CompositionArticle)
        {
            if (CompositionArticle.ComArt_Default == true)
            {
                Model.Prestaconnect.CompositionArticle CompositionArticleNext = DeclinaisonGetNext(Properties.Settings.Default.PrestaconnectConnection, CompositionArticle.ComArt_ArtId, CompositionArticle.ComArt_F_ARTICLE_SagId);
                if (CompositionArticleNext != null)
                {
                    CompositionArticle.ComArt_Default = false;
                    CompositionArticleNext.ComArt_Default = true;
                    CompositionArticle.Save();
                    CompositionArticleNext.Save();
                }
            }
            Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
            if (CompositionArticle.Pre_Id != null && PsProductAttributeRepository.ExistProductAttribute((uint)CompositionArticle.Pre_Id))
            {
                Core.Log.WriteLog("Suppression de l'attribut" + CompositionArticle.Pre_Id + " dans Prestashop.");
                PsProductAttributeRepository.Delete(PsProductAttributeRepository.ReadProductAttribute((uint)CompositionArticle.Pre_Id));
            }
            else
            {
                Core.Log.WriteLog("Pas d'attribut " + CompositionArticle.Pre_Id + " dans Prestashop.");
            }
            foreach (CompositionArticleImage compositionArticleImage in CompositionArticleImage.ListCompositionArticle(Properties.Settings.Default.PrestaconnectConnection, CompositionArticle.ComArt_Id))
            {
                compositionArticleImage.Delete(Properties.Settings.Default.PrestaconnectConnection);
            }
            foreach (CompositionArticleAttribute compositionArticleAttribute in CompositionArticleAttribute.ListCompositionArticle(Properties.Settings.Default.PrestaconnectConnection, CompositionArticle.ComArt_Id))
            {
                compositionArticleAttribute.Delete(Properties.Settings.Default.PrestaconnectConnection);
            }
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.PrestaconnectConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"DELETE FROM CompositionArticle WHERE ComArt_Id = {CompositionArticle.ComArt_Id}", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static CompositionArticle Construct(SqlDataReader reader)
        {
            return new CompositionArticle()
            {
                ComArt_Active = (bool)reader["ComArt_Active"],
                ComArt_ArtId = (int)reader["ComArt_ArtId"],
                ComArt_Default = (bool)reader["ComArt_Default"],
                ComArt_F_ARTENUMREF_SagId = reader.IsDBNull(reader.GetOrdinal("ComArt_F_ARTENUMREF_SagId")) ? null : (int?)(int)reader["ComArt_F_ARTENUMREF_SagId"],
                ComArt_F_ARTICLE_SagId = (int)reader["ComArt_F_ARTICLE_SagId"],
                ComArt_F_CONDITION_SagId = reader.IsDBNull(reader.GetOrdinal("ComArt_F_CONDITION_SagId")) ? null : (int?)(int)reader["ComArt_F_CONDITION_SagId"],
                ComArt_Id = (int)reader["ComArt_Id"],
                ComArt_Quantity = (decimal)reader["ComArt_Quantity"],
                ComArt_Sync = (bool)reader["ComArt_Sync"],
                Pre_Id = reader.IsDBNull(reader.GetOrdinal("Pre_Id")) ? null : (int?)(int)reader["Pre_Id"],
            };
        }
    }

    public class CompositionArticleImage
    {
        public int ComArt_Id { get; set; }
        public int ImaArt_Id { get; set; }

        public void Delete(string connectionPrestaconnectString)
        {
            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"DELETE FROM CompositionArticleImage WHERE ComArt_Id = {ComArt_Id} AND ImaArt_Id = {ImaArt_Id}", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static IEnumerable<CompositionArticleImage> ListCompositionArticle(string connectionPrestaconnectString, int ComArt_Id)
        {
            List<CompositionArticleImage> compositionArticleImagees = new List<CompositionArticleImage>();

            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM CompositionArticleImage WHERE ComArt_Id = {ComArt_Id}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            compositionArticleImagees.Add(Construct(reader));
                        }
                    }
                }
            }

            return compositionArticleImagees;
        }

        public static IEnumerable<CompositionArticleImage> ListImageArticle(string connectionPrestaconnectString, int ImaArt_Id)
        {
            List<CompositionArticleImage> compositionArticleImagees = new List<CompositionArticleImage>();

            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM CompositionArticleImage WHERE ImaArt_Id = {ImaArt_Id}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            compositionArticleImagees.Add(Construct(reader));
                        }
                    }
                }
            }

            return compositionArticleImagees;
        }

        public static CompositionArticleImage ReadCompositionArticleImage(string connectionPrestaconnectString, int ComArt_Id, int ImaArt_Id)
        {
            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT TOP 1 * FROM CompositionArticleImage WHERE ComArt_Id = {ComArt_Id} AND ImaArt_Id = {ImaArt_Id}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Construct(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static CompositionArticleImage Construct(SqlDataReader reader)
        {
            return new CompositionArticleImage()
            {
                ComArt_Id = (int)reader["ComArt_Id"],
                ImaArt_Id = (int)reader["ImaArt_Id"],
            };
        }
    }

    public class CompositionArticleAttribute
    {
        public int Caa_ComArtId { get; set; }
        public int Caa_AttributeGroup_PreId { get; set; }
        public int Caa_Attribute_PreId { get; set; }

        public void Add(string connectionPrestaconnectString)
        {
            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"INSERT INTO CompositionArticleAttribute(Caa_ComArtId, Caa_AttributeGroup_PreId, Caa_Attribute_PreId)"
                    + $" VALUES({Caa_ComArtId}, {Caa_AttributeGroup_PreId}, {Caa_Attribute_PreId})", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Save(string connectionPrestaconnectString)
        {
            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"UPDATE CompositionArticleAttribute SET Caa_Attribute_PreId = {Caa_Attribute_PreId} WHERE Caa_ComArtId = {Caa_ComArtId} AND Caa_AttributeGroup_PreId = {Caa_AttributeGroup_PreId}", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(string connectionPrestaconnectString)
        {
            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"DELETE FROM CompositionArticleAttribute WHERE Caa_ComArtId = {Caa_ComArtId}"
                    + $" AND Caa_AttributeGroup_PreId = {Caa_AttributeGroup_PreId} AND Caa_Attribute_PreId = {Caa_Attribute_PreId}", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static IEnumerable<CompositionArticleAttribute> ListArticle(string connectionPrestaconnectString, int Art_Id)
        {
            List<CompositionArticleAttribute> compositionArticleAttributees = new List<CompositionArticleAttribute>();

            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM CompositionArticleAttribute caa " +
                                                           $" INNER JOIN CompositionArticle ca ON caa.Caa_ComArtId = ca.ComArt_Id " +
                                                           $" WHERE ca.ComArt_ArtId = {Art_Id}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            compositionArticleAttributees.Add(Construct(reader));
                        }
                    }
                }
            }

            return compositionArticleAttributees;
        }

        public static IEnumerable<CompositionArticleAttribute> ListCompositionArticle(string connectionPrestaconnectString, int Caa_ComArtId)
        {
            List<CompositionArticleAttribute> compositionArticleAttributees = new List<CompositionArticleAttribute>();

            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM CompositionArticleAttribute WHERE Caa_ComArtId = {Caa_ComArtId}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            compositionArticleAttributees.Add(Construct(reader));
                        }
                    }
                }
            }

            return compositionArticleAttributees;
        }

        public static IEnumerable<CompositionArticleAttribute> ListAttributeGroup(string connectionPrestaconnectString, int Caa_AttributeGroup_PreId)
        {
            List<CompositionArticleAttribute> compositionArticleAttributees = new List<CompositionArticleAttribute>();

            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM CompositionArticleAttribute WHERE Caa_AttributeGroup_PreId = {Caa_AttributeGroup_PreId}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            compositionArticleAttributees.Add(Construct(reader));
                        }
                    }
                }
            }

            return compositionArticleAttributees;
        }

        public static CompositionArticleAttribute ReadAttributeGroupCompositionArticle(string connectionPrestaconnectString, int Caa_AttributeGroup_PreId, int Caa_ComArtId)
        {
            using (SqlConnection connection = new SqlConnection(connectionPrestaconnectString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT TOP 1 * FROM CompositionArticleAttribute WHERE Caa_AttributeGroup_PreId = {Caa_AttributeGroup_PreId} AND Caa_ComArtId = {Caa_ComArtId}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Construct(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static CompositionArticleAttribute Construct(SqlDataReader reader)
        {
            return new CompositionArticleAttribute()
            {
                Caa_AttributeGroup_PreId = (int)reader["Caa_AttributeGroup_PreId"],
                Caa_Attribute_PreId = (int)reader["Caa_Attribute_PreId"],
                Caa_ComArtId = (int)reader["Caa_ComArtId"],
            };
        }
    }
}

