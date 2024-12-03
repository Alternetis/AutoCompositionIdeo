using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCompositionIdeo.Properties;
using PrestaconnectSql = AutoCompositionIdeo.Model.Prestaconnect.PrestaconnectRequeteSQL;
using PrestashopSql = AutoCompositionIdeo.Model.Prestashop.PrestashopRequeteSQL;
using SageSql = AutoCompositionIdeo.Model.Sage.SageRequeteSQL;

namespace AutoCompositionIdeo
{
    class Program
    {
        public class Composition
        {
            public int CL_No { get; set; }
            public string Groupe { get; set; }
            public string InfoProduit { get; set; }
            public List<Article> Articles { get; set; }
        }

        public class Article
        {
            public int cbMarq { get; set; }
            public string AR_Ref { get; set; }
            public string AR_Design { get; set; }
            public string Att1 { get; set; }
            public string Att2 { get; set; }
            public string Att3 { get; set; }
            public string Att4 { get; set; }
            public string Dec1 { get; set; }
            public string Dec2 { get; set; }
            public string Dec3 { get; set; }
            public string Dec4 { get; set; }
            public string Groupe { get; set; }
            public int CL_No1 { get; set; }
            public int CL_No2 { get; set; }
            public int CL_No3 { get; set; }
            public int CL_No4 { get; set; }
            public string InfoProduit { get; set; }
            public bool Active { get; set; }
        }

        public static string PrefixPrestashop = Settings.Default.PrefixPrestashop;

        static void Main(string[] args)
        {
            try
            {
                string connectionPrestashopString = Properties.Settings.Default.PrestashopConnection;
                string connectionPrestaconnectString = Properties.Settings.Default.PrestaconnectConnection;
                string connectionSageString = Properties.Settings.Default.SageConnection;

                Console.WriteLine("Debut du Programme..");
                Core.Log.WriteLog(DateTime.Now + " - Debut du Programme..");
                List<Composition> Compositions = new List<Composition>();
                List<Article> Articles = new List<Article>();

                // Récupération des Articles
                Console.WriteLine("récupération des articles de Sage...");
                Core.Log.WriteLog(DateTime.Now + " - récupération des articles de Sage...");
                SageSql.ListArticleDeclinaisons(Articles, Compositions);
                Console.WriteLine("compositions de Sage récupérés : " + Compositions.Count);
                Console.WriteLine("articles simples de Sage récupérés : " + Articles.Count);
                Core.Log.WriteLog(DateTime.Now + " - compositions de Sage récupérés : " + Compositions.Count);
                Core.Log.WriteLog(DateTime.Now + " - articles simples de Sage récupérés : " + Articles.Count);
                if (Compositions.Count() > 0)
                {
                    Console.WriteLine("traitement des compositions...");
                    Core.Log.WriteLog(DateTime.Now + " - traitement des compositions...");
                    traitementCompositions(Compositions, connectionPrestaconnectString, connectionSageString, connectionPrestashopString);
                }
                if (Articles.Count() > 0)
                {
                    Console.WriteLine("traitement des articles simples...");
                    Core.Log.WriteLog(DateTime.Now + " - traitement des compositions...");
                    TraitementArticlesSimples(Articles, connectionPrestaconnectString, connectionSageString, connectionPrestashopString);
                }
                Console.WriteLine("Fin du Programme.");
                Core.Log.WriteLog(DateTime.Now + " - Fin du Programme.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR GENERALE DE L'APPLICATION : " + ex.ToString());
                Core.Log.WriteLog(DateTime.Now + " - ERREUR GENERALE DE L'APPLICATION : " + ex.ToString());
            }
        }

        public static void traitementCompositions(List<Composition> Compositions, string connectionPrestaconnectString, string connectionSageString, string connectionPrestashopString)
        {
            try
            {
                foreach (Composition composition in Compositions)
                {
                    Console.WriteLine("traitement de la composition de référence " + composition.InfoProduit);
                    Core.Log.WriteLog(DateTime.Now + " - traitement de la composition de référence " + composition.InfoProduit);
                    int Cat_Id = 0;
                    if (composition.CL_No != 0)
                    {
                        Cat_Id = PrestaconnectSql.SearchCatIdArticle(connectionSageString, composition.CL_No, connectionPrestaconnectString);
                    }
                    else
                    {
                        Cat_Id = Settings.Default.Catalogue;
                    }

                    if (Cat_Id != 0)
                    {
                        Console.WriteLine("composition " + composition.InfoProduit + " (groupe " + composition.Groupe + ") : assignation à la catégorie n° " + Cat_Id);
                        Core.Log.WriteLog(DateTime.Now + " - composition " + composition.InfoProduit + " : assignation à la catégorie n° " + Cat_Id);
                        //création de la composition si indisponible
                        int Art_Id = PrestaconnectSql.CreationComposition(connectionSageString, connectionPrestaconnectString, composition, Cat_Id);

                        if (Art_Id != -1)
                        {
                            Console.WriteLine("composition " + composition.InfoProduit + " : identifiant Prestaconnect " + Art_Id);
                            Core.Log.WriteLog(DateTime.Now + " - composition " + composition.InfoProduit + " : identifiant Prestaconnect " + Art_Id);
                            //création des liens attribut à partir du premier article de la composition
                            Article article = composition.Articles[0];
                            // Vérifier que le nom du groupe d'attribut Existe dans Prestashop
                            // Si elle n'existe pas la créer
                            int InfoLibre1IdAttributeGroup = -1;
                            int InfoLibre2IdAttributeGroup = -1;
                            int InfoLibre3IdAttributeGroup = -1;
                            int InfoLibre4IdAttributeGroup = -1;
                            if (!string.IsNullOrWhiteSpace(article.Att1))
                            {
                                PrestashopSql.CreationGroupeAttribut(connectionPrestashopString, article.Att1);
                                Console.WriteLine($"récupération intitulé Groupe Attribut {article.Att1}");
                                Core.Log.WriteLog(DateTime.Now + $"récupération intitulé Groupe Attribut {article.Att1}", false);
                                InfoLibre1IdAttributeGroup = PrestashopSql.RecupererG_IdAttributeFROM_Name(PrefixPrestashop, article.Att1, connectionPrestashopString);
                                Console.WriteLine($"{article.Att1} = " + InfoLibre1IdAttributeGroup);
                                Core.Log.WriteLog(DateTime.Now + $"{ article.Att1} = " + InfoLibre1IdAttributeGroup, false);
                                if (InfoLibre1IdAttributeGroup != -1)
                                {
                                    Console.WriteLine($"Création du groupe d'attribut {article.Att1} pour la composition");
                                    Core.Log.WriteLog(DateTime.Now + $"Création du groupe d'attribut {article.Att1} pour la composition");
                                    PrestaconnectSql.InsertG_AttributeGroup(connectionPrestaconnectString, Art_Id, InfoLibre1IdAttributeGroup);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(article.Att2))
                            {
                                PrestashopSql.CreationGroupeAttribut(connectionPrestashopString, article.Att2);
                                Console.WriteLine($"récupération intitulé Groupe Attribut {article.Att2}");
                                Core.Log.WriteLog(DateTime.Now + $"récupération intitulé Groupe Attribut {article.Att2}", false);
                                InfoLibre2IdAttributeGroup = PrestashopSql.RecupererG_IdAttributeFROM_Name(PrefixPrestashop, article.Att2, connectionPrestashopString);
                                Console.WriteLine($"{article.Att2} = " + InfoLibre2IdAttributeGroup);
                                Core.Log.WriteLog(DateTime.Now + $"{ article.Att2} = " + InfoLibre2IdAttributeGroup, false);
                                if (InfoLibre2IdAttributeGroup != -1)
                                {
                                    Console.WriteLine($"Création du groupe d'attribut {article.Att2} pour la composition");
                                    Core.Log.WriteLog(DateTime.Now + $"Création du groupe d'attribut {article.Att2} pour la composition");
                                    PrestaconnectSql.InsertG_AttributeGroup(connectionPrestaconnectString, Art_Id, InfoLibre2IdAttributeGroup);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(article.Att3))
                            {
                                PrestashopSql.CreationGroupeAttribut(connectionPrestashopString, article.Att3);
                                Console.WriteLine($"récupération intitulé Groupe Attribut {article.Att3}");
                                Core.Log.WriteLog(DateTime.Now + $"récupération intitulé Groupe Attribut {article.Att3}", false);
                                InfoLibre3IdAttributeGroup = PrestashopSql.RecupererG_IdAttributeFROM_Name(PrefixPrestashop, article.Att1, connectionPrestashopString);
                                Console.WriteLine($"{article.Att3} = " + InfoLibre3IdAttributeGroup);
                                Core.Log.WriteLog(DateTime.Now + $"{article.Att3} = " + InfoLibre3IdAttributeGroup, false);
                                if (InfoLibre3IdAttributeGroup != -1)
                                {
                                    Console.WriteLine($"Création du groupe d'attribut {article.Att3} pour la composition");
                                    Core.Log.WriteLog(DateTime.Now + $"Création du groupe d'attribut {article.Att3} pour la composition");
                                    PrestaconnectSql.InsertG_AttributeGroup(connectionPrestaconnectString, Art_Id, InfoLibre3IdAttributeGroup);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(article.Att4))
                            {
                                PrestashopSql.CreationGroupeAttribut(connectionPrestashopString, article.Att4);
                                Console.WriteLine($"récupération intitulé Groupe Attribut {article.Att4}");
                                Core.Log.WriteLog(DateTime.Now + $"récupération intitulé Groupe Attribut {article.Att4}", false);
                                InfoLibre4IdAttributeGroup = PrestashopSql.RecupererG_IdAttributeFROM_Name(PrefixPrestashop, article.Att4, connectionPrestashopString);
                                Console.WriteLine($"{article.Att4} = " + InfoLibre4IdAttributeGroup);
                                Core.Log.WriteLog(DateTime.Now + $"{article.Att4} = " + InfoLibre4IdAttributeGroup, false);
                                if (InfoLibre4IdAttributeGroup != -1)
                                {
                                    Console.WriteLine($"Création du groupe d'attribut {article.Att4} pour la composition");
                                    Core.Log.WriteLog(DateTime.Now + $"Création du groupe d'attribut {article.Att4} pour la composition");
                                    PrestaconnectSql.InsertG_AttributeGroup(connectionPrestaconnectString, Art_Id, InfoLibre4IdAttributeGroup);
                                }
                            }

                            foreach (Article articleAdd in composition.Articles)
                            {
                                if (!PrestaconnectSql.DeclinaisonExist(connectionPrestaconnectString, articleAdd.cbMarq))
                                {
                                    Console.WriteLine($"Création de l'article de composition {composition.InfoProduit}");
                                    Core.Log.WriteLog(DateTime.Now + $"Création de l'article de composition {composition.InfoProduit}", false);
                                    int NbrDeclinaison = PrestaconnectSql.NombreDeclinaison(connectionPrestaconnectString, Art_Id);
                                    if (NbrDeclinaison == 0)
                                    {
                                        PrestaconnectSql.InsertDeclinaisonArticle(connectionPrestaconnectString, Art_Id, articleAdd.cbMarq, 1);
                                    }
                                    else
                                    {
                                        PrestaconnectSql.InsertDeclinaisonArticle(connectionPrestaconnectString, Art_Id, articleAdd.cbMarq, 0);
                                    }

                                    if (!string.IsNullOrWhiteSpace(articleAdd.Att1) && !string.IsNullOrWhiteSpace(articleAdd.Dec1))
                                    {
                                        PrestashopSql.CreationAttribut(connectionPrestashopString, InfoLibre1IdAttributeGroup, articleAdd.Dec1, 0);
                                        Console.WriteLine($"Assignation de la composition {composition.InfoProduit} à l'attribut {articleAdd.Att1} - {articleAdd.Dec1}");
                                        Core.Log.WriteLog(DateTime.Now + $"Assignation de la composition {composition.InfoProduit} à l'attribut {articleAdd.Att1} - {articleAdd.Dec1}");
                                        int idAttributeInfo1 = PrestashopSql.Recuperer_IdAttributeByName(articleAdd.Dec1, PrefixPrestashop, connectionPrestashopString, InfoLibre1IdAttributeGroup);
                                        if (idAttributeInfo1 != 0)
                                        {
                                            PrestaconnectSql.InsertAttribute(connectionPrestaconnectString, InfoLibre1IdAttributeGroup, idAttributeInfo1, articleAdd.cbMarq, Art_Id);
                                        }
                                    }
                                    if (!string.IsNullOrWhiteSpace(articleAdd.Att2) && !string.IsNullOrWhiteSpace(articleAdd.Dec2))
                                    {
                                        Console.WriteLine($"Assignation de la composition {composition.InfoProduit} à l'attribut {articleAdd.Att2} - {articleAdd.Dec2}");
                                        Core.Log.WriteLog(DateTime.Now + $"Assignation de la composition {composition.InfoProduit} à l'attribut {articleAdd.Att2} - {articleAdd.Dec2}");
                                        PrestashopSql.CreationAttribut(connectionPrestashopString, InfoLibre2IdAttributeGroup, articleAdd.Dec2, 0);
                                        int idAttributeInfo2 = PrestashopSql.Recuperer_IdAttributeByName(articleAdd.Dec2, PrefixPrestashop, connectionPrestashopString, InfoLibre2IdAttributeGroup);
                                        if (idAttributeInfo2 != 0)
                                        {
                                            PrestaconnectSql.InsertAttribute(connectionPrestaconnectString, InfoLibre2IdAttributeGroup, idAttributeInfo2, articleAdd.cbMarq, Art_Id);
                                        }
                                    }
                                    if (!string.IsNullOrWhiteSpace(articleAdd.Att3) && !string.IsNullOrWhiteSpace(articleAdd.Dec3))
                                    {
                                        Console.WriteLine($"Assignation de la composition {composition.InfoProduit} à l'attribut {articleAdd.Att3} - {articleAdd.Dec3}");
                                        Core.Log.WriteLog(DateTime.Now + $"Assignation de la composition {composition.InfoProduit} à l'attribut {articleAdd.Att3} - {articleAdd.Dec3}");
                                        PrestashopSql.CreationAttribut(connectionPrestashopString, InfoLibre3IdAttributeGroup, articleAdd.Dec3, 0);
                                        int idAttributeInfo3 = PrestashopSql.Recuperer_IdAttributeByName(articleAdd.Dec3, PrefixPrestashop, connectionPrestashopString, InfoLibre3IdAttributeGroup);
                                        if (idAttributeInfo3 != 0)
                                        {
                                            PrestaconnectSql.InsertAttribute(connectionPrestaconnectString, InfoLibre3IdAttributeGroup, idAttributeInfo3, articleAdd.cbMarq, Art_Id);
                                        }
                                    }
                                    if (!string.IsNullOrWhiteSpace(articleAdd.Att4) && !string.IsNullOrWhiteSpace(articleAdd.Dec4))
                                    {
                                        Console.WriteLine($"Assignation de la composition {composition.InfoProduit} à l'attribut {articleAdd.Att4} - {articleAdd.Dec4}");
                                        Core.Log.WriteLog(DateTime.Now + $"Assignation de la composition {composition.InfoProduit} à l'attribut {articleAdd.Att4} - {articleAdd.Dec4}");
                                        PrestashopSql.CreationAttribut(connectionPrestashopString, InfoLibre4IdAttributeGroup, articleAdd.Dec4, 0);
                                        int idAttributeInfo4 = PrestashopSql.Recuperer_IdAttributeByName(articleAdd.Dec4, PrefixPrestashop, connectionPrestashopString, InfoLibre4IdAttributeGroup);
                                        if (idAttributeInfo4 != 0)
                                        {
                                            PrestaconnectSql.InsertAttribute(connectionPrestaconnectString, InfoLibre4IdAttributeGroup, idAttributeInfo4, articleAdd.cbMarq, Art_Id);
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("La déclinaison " + articleAdd.AR_Ref + " pour la composition " + composition.InfoProduit + " existe déjà sur Prestaconnect.");
                                    Core.Log.WriteLog(DateTime.Now + " - La déclinaison " + articleAdd.AR_Ref + " pour la composition " + composition.InfoProduit + " existe déjà sur Prestaconnect.");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Le catalogue Sage de la composition " + composition.InfoProduit + " n'a pas de lien Prestaconnect !");
                        Core.Log.WriteLog(DateTime.Now + " - la composition " + composition.InfoProduit + " n'a pas de lien Prestaconnect !");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR GENERALE LORS DU TRAITEMENT DES COMPOSITIONS : " + ex.ToString());
                Core.Log.WriteLog(DateTime.Now + " - ERREUR GENERALE LORS DU TRAITEMENT DES COMPOSITIONS : " + ex.ToString());
            }
        }

        public static void TraitementArticlesSimples(List<Article> Articles, string connectionPrestaconnectString, string connectionSageString, string connectionPrestashopString)
        {
            try
            {
                foreach (Article article in Articles)
                {
                    Console.WriteLine("traitement de l'article simple de référence " + article.AR_Ref);
                    Core.Log.WriteLog(DateTime.Now + " - traitement de l'article simple de référence " + article.AR_Ref);
                    int Cat_Id = 0;
                    int CL_No = article.CL_No4 != 0 ? article.CL_No4 : article.CL_No3 != 0 ? article.CL_No3 : article.CL_No2 != 0 ? article.CL_No2 : article.CL_No1 != 0 ? article.CL_No1 : 0;
                    if (CL_No != 0)
                    {
                        Cat_Id = PrestaconnectSql.SearchCatIdArticle(connectionSageString, CL_No, connectionPrestaconnectString);
                    }
                    else
                    {
                        Cat_Id = Settings.Default.Catalogue;
                    }
                    if (Cat_Id != 0)
                    {
                        if (PrestaconnectSql.SearchSagIdPrestaconnect(connectionPrestaconnectString, article.cbMarq) == -1)// L'article n'existe pas, le créer
                        {
                            Console.WriteLine("création de l'article simple " + article.AR_Ref);
                            Core.Log.WriteLog(DateTime.Now + " - création de l'article simple " + article.AR_Ref);
                            PrestaconnectSql.CreationSimple(connectionSageString, connectionPrestaconnectString, article, Cat_Id);
                        }
                        else
                        {
                            Console.WriteLine("l'article simple " + article.AR_Ref + " existe déjà sur prestaconnect.");
                            Core.Log.WriteLog(DateTime.Now + " - l'article simple " + article.AR_Ref + " existe déjà sur prestaconnect.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Le catalogue Sage de l'article simple " + article.AR_Ref + " n'a pas de lien Prestaconnect !");
                        Core.Log.WriteLog(DateTime.Now + " - la composition " + article.AR_Ref + " n'a pas de lien Prestaconnect !");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR GENERALE LORS DU TRAITEMENT DES ARTICLES SIMPLES : " + ex.ToString());
                Core.Log.WriteLog(DateTime.Now + " - ERREUR GENERALE LORS DU TRAITEMENT DES ARTICLES SIMPLES : " + ex.ToString());
            }
        }
    }
}
