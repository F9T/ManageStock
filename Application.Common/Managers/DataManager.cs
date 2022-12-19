using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Managers.DatabaseManagerBase.Conditions;
using Application.Common.Models;
using Application.Common.Models.Articles;
using Application.Common.Models.Devises;
using Application.Common.Models.Groups;
using Application.Common.Models.History;
using Application.Common.Models.Providers;
using Application.Common.Notifications;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.Common.Managers
{
    public class DataManager
    {
        private static ArticleManager s_ArticleManager;
        private static ProviderManager s_ProviderManager;
        private static GroupProviderManager s_GroupProviderManager;
        private static GroupArticleManager s_GroupArticleManager;
        private static CurrencyManager s_CurrencyManager;
        private static HistoryManager s_HistoryManager;

        private DataManager()
        {

        }

        public static void SetNotificationManager(CustomNotificationsManager _NotificationManager)
        {
            ProviderManager.NotificationManager = _NotificationManager;
            ArticleManager.NotificationManager = _NotificationManager;
            GroupProviderManager.NotificationManager = _NotificationManager;
            CurrencyManager.NotificationManager = _NotificationManager;
            GroupArticleManager.NotificationManager = _NotificationManager;
            HistoryManager.NotificationManager = _NotificationManager;
        }
        public static bool? Execute(EnumDatabaseAction _Action, IDatabaseModel _Item)
        {
            bool? result = null;

            if (_Item is Article article)
            {
                return ArticleManager.Execute(_Action, article);
            }

            if (_Item is Provider provider)
            {
                return ProviderManager.Execute(_Action, provider);
            }

            if (_Item is GroupProvider groupProvider)
            {
                return GroupProviderManager.Execute(_Action, groupProvider);
            }

            if (_Item is GroupArticle groupArticle)
            {
                return GroupArticleManager.Execute(_Action, groupArticle);
            }

            if (_Item is Currency currency)
            {
                return CurrencyManager.Execute(_Action, currency);
            }

            if (_Item is History history)
            {
                return HistoryManager.Execute(_Action, history);
            }

            return result;
        }

        public static ProviderManager ProviderManager => s_ProviderManager ?? (s_ProviderManager = new ProviderManager());
        public static ArticleManager ArticleManager => s_ArticleManager ?? (s_ArticleManager = new ArticleManager());
        public static GroupProviderManager GroupProviderManager => s_GroupProviderManager ?? (s_GroupProviderManager = new GroupProviderManager());
        public static GroupArticleManager GroupArticleManager => s_GroupArticleManager ?? (s_GroupArticleManager = new GroupArticleManager());
        public static CurrencyManager CurrencyManager => s_CurrencyManager ?? (s_CurrencyManager = new CurrencyManager());
        public static HistoryManager HistoryManager => s_HistoryManager ?? (s_HistoryManager = new HistoryManager());
    }

    public abstract class DataManagerBase<T> where T : IDatabaseModel
    {
        public event EventHandler<object> ItemOnChanged;

        public CustomNotificationsManager NotificationManager { get; set; }

        public abstract List<T> FetchAll(bool _Reload = false);

        protected abstract bool Insert(T _Item);

        protected abstract bool Update(T _Item);

        protected abstract bool Delete(T _Item);

        public bool? Execute(EnumDatabaseAction _Action, T _Item)
        {
            Func<T, bool> func = null;
            switch (_Action)
            {
                case EnumDatabaseAction.Insert:
                    func = Insert;
                    break;
                case EnumDatabaseAction.Update:
                    func = Update;
                    break;
                case EnumDatabaseAction.Delete:
                    func = Delete;
                    break;
            }

            return func?.Invoke(_Item);
        }

        protected virtual void OnItemOnChanged(T _Item)
        {
            ItemOnChanged?.Invoke(this, _Item.GetID());
        }
    }


    public class CurrencyManager : DataManagerBase<Currency>
    {
        private List<Currency> m_Currencies;

        internal CurrencyManager()
        {
        }

        protected override bool Insert(Currency _Currency)
        {
            // add other currencies
            foreach (Currency currency in m_Currencies)
            {
                // add other currency exchange
                _Currency.ExchangeRates.Add(new ExchangeRate
                {
                    Currency = currency,
                    Rate = 1.0
                });

                // add new currency in all other currency exchange
                ExchangeRate exchangeRate = new ExchangeRate
                {
                    Currency = _Currency,
                    Rate = 1.0
                };
                currency.ExchangeRates.Add(exchangeRate);

                InsertExchange(currency, exchangeRate);
            }


            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("devise", _Currency.Name, _Currency.Name.GetType()),
                new ColumnResult("ratecurrencyid", _Currency.RateID.ToString(), typeof(string))
            };

            bool result = DBManager.InstanceOf.Insert("Currency", columns, out int id);

            if (!result)
            {
                NotificationManager.Show("La devise n'a pas pu être ajouté.", NotificationType.Error);
                return false;
            }

            _Currency.ID = id;
            m_Currencies.Add(_Currency);

            foreach (ExchangeRate exchangeRate in _Currency.ExchangeRates)
            {
                ColumnsList rateColumns = new ColumnsList
                {
                    new ColumnResult("idcurrencyfirst", _Currency.RateID.ToString(), typeof(string)),
                    new ColumnResult("idcurrencysecond", exchangeRate.Currency.RateID.ToString(), typeof(string)),
                    new ColumnResult("rate", exchangeRate.Rate, exchangeRate.Rate.GetType())
                };

                bool insertResult = DBManager.InstanceOf.Insert("RateCurrency", rateColumns, out int _);
                if (!insertResult)
                {
                }
            }

            if (result)
            {
                NotificationManager.Show("La devise a été ajouté avec succès.", NotificationType.Success);
            }

            return true;
        }

        private void InsertExchange(Currency _Currency, ExchangeRate _ExchangeRate)
        {
            ColumnsList rateColumns = new ColumnsList
            {
                new ColumnResult("idcurrencyfirst", _Currency.RateID.ToString(), typeof(string)),
                new ColumnResult("idcurrencysecond", _ExchangeRate.Currency.RateID.ToString(), typeof(string)),
                new ColumnResult("rate", _ExchangeRate.Rate, _ExchangeRate.Rate.GetType())
            };

            DBManager.InstanceOf.Insert("RateCurrency", rateColumns, out int _);
        }

        public void UpdateAll()
        {
            NotificationManager.SuspendNotification();
            foreach (Currency currency in m_Currencies)
            {
                Update(currency);
            }
            NotificationManager.ResumeNotification();
        }

        protected override bool Update(Currency _Currency)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("devise", _Currency.Name, _Currency.Name.GetType())
            };

            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("id", "=", _Currency.ID)
            };

            bool result = DBManager.InstanceOf.Update("Currency", columns, conditions);

            if (!result)
            {
                NotificationManager.Show("Les informations n'ont pas pu être enregistrées.", NotificationType.Error);
                return false;
            }

            foreach (ExchangeRate exchangeRate in _Currency.ExchangeRates)
            {
                ColumnsList rateColumns = new ColumnsList
                {
                    new ColumnResult("rate", exchangeRate.Rate, exchangeRate.Rate.GetType())
                };

                List<ConditionBase> rateConditions = new List<ConditionBase>
                {
                    new Condition("idcurrencyfirst", "=", _Currency.RateID.ToString()),
                    new ConditionSeparator(EnumConditionSeparator.AND),
                    new Condition("idcurrencysecond", "=", exchangeRate.Currency.RateID.ToString())
                };

                bool insertResult = DBManager.InstanceOf.Update("RateCurrency", rateColumns, rateConditions);
                if (!insertResult)
                {
                }
            }

            if (result)
            {
                NotificationManager.Show("Les informations ont été enregistrées avec succès.", NotificationType.Success);
            }

            return true;
        }

        protected override bool Delete(Currency _Currency)
        {
            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("id", "=", _Currency.ID)
            };

            bool result = DBManager.InstanceOf.Delete("Currency", conditions);

            if (!result)
            {
                NotificationManager.Show("La devise n'a pas pu être supprimé.", NotificationType.Error);
                return false;
            }

            foreach (ExchangeRate exchangeRate in _Currency.ExchangeRates)
            {
                List<ConditionBase> rateConditions = new List<ConditionBase>
                {
                    new Condition("idcurrencyfirst", "=", _Currency.RateID.ToString()),
                    new ConditionSeparator(EnumConditionSeparator.AND),
                    new Condition("idcurrencysecond", "=", exchangeRate.Currency.RateID.ToString())
                };

                DBManager.InstanceOf.Delete("RateCurrency", rateConditions);
            }

            _Currency.ExchangeRates.Clear();

            m_Currencies.Remove(_Currency);

            // remove from other currency
            foreach (Currency currency in m_Currencies)
            {
                var exchangeRate = currency.ExchangeRates.First(_ => _.Currency == _Currency);

                List<ConditionBase> rateConditions = new List<ConditionBase>
                {
                    new Condition("idcurrencyfirst", "=", currency.RateID.ToString()),
                    new ConditionSeparator(EnumConditionSeparator.AND),
                    new Condition("idcurrencysecond", "=", exchangeRate.Currency.RateID.ToString())
                };

                bool removeResult = DBManager.InstanceOf.Delete("RateCurrency", rateConditions);

                if (removeResult)
                {
                    currency.ExchangeRates.Remove(exchangeRate);
                }
            }

            NotificationManager.Show("La devise a été supprimé avec succès.", NotificationType.Success);

            return true;
        }


        public override List<Currency> FetchAll(bool _Reload = false)
        {
            if (m_Currencies == null || _Reload == true)
            {
                m_Currencies = new List<Currency>();

                var currencies = DBManager.InstanceOf.SelectQuery("SELECT * FROM Currency;");
                foreach (RowResult currencyResult in currencies)
                {
                    Currency currency = new Currency
                    {
                        ID = int.Parse(currencyResult.ColumnsList[0].Value.ToString()),
                        Name = currencyResult.ColumnsList[1].Value.ToString(),
                        RateID = Guid.Parse(currencyResult.ColumnsList[2].Value.ToString())
                    };

                    m_Currencies.Add(currency);
                }

                var rateCurrencies = DBManager.InstanceOf.SelectQuery("SELECT * FROM RateCurrency;");
                foreach (RowResult rateCurrencyResult in rateCurrencies)
                {
                    Guid idFirst = Guid.Parse(rateCurrencyResult.ColumnsList[0].Value.ToString());
                    Guid idSecond = Guid.Parse(rateCurrencyResult.ColumnsList[1].Value.ToString());
                    double rate = double.Parse(rateCurrencyResult.ColumnsList[2].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

                    var firstCurrency = m_Currencies.FirstOrDefault(_ => _.RateID == idFirst);
                    var secondCurrency = m_Currencies.FirstOrDefault(_ => _.RateID == idSecond);

                    if (firstCurrency != null && secondCurrency != null)
                    {
                        firstCurrency.ExchangeRates.Add(new ExchangeRate
                        {
                            Currency = secondCurrency,
                            Rate = rate
                        });
                    }
                }
            }

            return m_Currencies;
        }
    }
    public class GroupArticleManager : DataManagerBase<GroupArticle>
    {
        private List<GroupArticle> m_GroupArticles;

        internal GroupArticleManager()
        {
        }

        protected override bool Insert(GroupArticle _GroupArticle)
        {
            var articles = DataManager.ArticleManager.FetchAll();

            var article = articles.FirstOrDefault(_ => _.GroupArticleID == _GroupArticle.ID);

            if (article == null)
            {
                return false;
            }

            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("groupaaid", _GroupArticle.ID.ToString(), typeof(string)),
                new ColumnResult("idarticle", _GroupArticle.ArticleId, _GroupArticle.ArticleId.GetType()),
                new ColumnResult("quantityuse", _GroupArticle.QuantityUse, _GroupArticle.QuantityUse.GetType())
            };

            bool result = DBManager.InstanceOf.Insert("GroupAA", columns, out int _);

            if (result)
            {
                // if already add with undo/redo
                if (!article.GroupArticles.Contains(_GroupArticle))
                {
                    article.GroupArticles.Add(_GroupArticle);
                }
                NotificationManager.Show("Le sous-article a été ajouté avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Le sous-article n'a pas pu être ajouté.", NotificationType.Error);
            }

            return true;
        }

        protected override bool Update(GroupArticle _GroupArticle)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("quantityuse", _GroupArticle.QuantityUse, _GroupArticle.QuantityUse.GetType())
            };

            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("groupaaid", "=", _GroupArticle.ID.ToString()),
                new ConditionSeparator(EnumConditionSeparator.AND),
                new Condition("idarticle", "=", _GroupArticle.ArticleId)
            };

            bool result = DBManager.InstanceOf.Update("GroupAA", columns, conditions);

            if (result)
            {
                NotificationManager.Show("Les informations ont été enregistrées avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Les informations n'ont pas pu être enregistrées.", NotificationType.Error);
            }

            return true;
        }

        protected override bool Delete(GroupArticle _GroupArticle)
        {
            var articles = DataManager.ArticleManager.FetchAll();

            var article = articles.FirstOrDefault(_ => _.GroupArticleID == _GroupArticle.ID);

            if (article == null)
            {
                return false;
            }

            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("groupaaid", "=", _GroupArticle.ID.ToString()),
                new ConditionSeparator(EnumConditionSeparator.AND),
                new Condition("idarticle", "=", _GroupArticle.ArticleId)
            };
            bool result = DBManager.InstanceOf.Delete("GroupAA", conditions);

            if (result)
            {
                article.GroupArticles.Remove(_GroupArticle);
                NotificationManager.Show("Le sous-article a été supprimé avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Le sous-article n'a pas pu être supprimé.", NotificationType.Error);
            }

            return true;
        }


        public override List<GroupArticle> FetchAll(bool _Reload = false)
        {
            if (m_GroupArticles == null || _Reload == true)
            {
                m_GroupArticles = new List<GroupArticle>();

                var groupArticles = DBManager.InstanceOf.SelectQuery("SELECT * FROM GroupAA;");
                foreach (RowResult groupArticleResult in groupArticles)
                {
                    int articleId = int.Parse(groupArticleResult.ColumnsList[1].Value.ToString());

                    GroupArticle groupArticle = new GroupArticle
                    {
                        ID = Guid.Parse(groupArticleResult.ColumnsList[0].Value.ToString()),
                        QuantityUse = double.Parse(groupArticleResult.ColumnsList[2].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        ArticleId = articleId
                    };

                    m_GroupArticles.Add(groupArticle);
                }
            }

            return m_GroupArticles;
        }
    }

    public class GroupProviderManager : DataManagerBase<GroupProvider>
    {
        private List<GroupProvider> m_GroupProviders;

        internal GroupProviderManager()
        {
        }

        protected override bool Insert(GroupProvider _GroupProvider)
        {
            var articles = DataManager.ArticleManager.FetchAll();

            var article = articles.FirstOrDefault(_ => _.GroupProviderID == _GroupProvider.ID);

            if (article == null)
            {
                return false;
            }

            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("groupapid", _GroupProvider.ID.ToString(), typeof(string)),
                new ColumnResult("idprovider", _GroupProvider.IDProvider, _GroupProvider.IDProvider.GetType()),
                new ColumnResult("priceht", _GroupProvider.PriceHT, _GroupProvider.PriceHT.GetType()),
                new ColumnResult("noarticle", _GroupProvider.ArticleNumber, _GroupProvider.ArticleNumber.GetType())
            };

            int currencyId = _GroupProvider.Currency != null ? _GroupProvider.Currency.ID : -1;
            columns.Add(new ColumnResult("devise", currencyId, currencyId.GetType()));

            bool result = DBManager.InstanceOf.Insert("GroupAP", columns, out int _);

            if (result)
            {
                // if already add with undo/redo
                if (!article.GroupProviders.Contains(_GroupProvider))
                {
                    article.GroupProviders.Add(_GroupProvider);
                }
                NotificationManager.Show("Le fournisseur a été ajouté à cet article avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Le fournisseur n'a pas pu être ajouté à cet article.", NotificationType.Error);
            }

            return true;
        }

        protected override bool Update(GroupProvider _GroupProvider)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("priceht", _GroupProvider.PriceHT, _GroupProvider.PriceHT.GetType()),
                new ColumnResult("noarticle", _GroupProvider.ArticleNumber, _GroupProvider.ArticleNumber.GetType())
            };

            int currencyId = _GroupProvider.Currency != null ? _GroupProvider.Currency.ID : -1;
            columns.Add(new ColumnResult("currencyid", currencyId, currencyId.GetType()));

            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("groupapid", "=", _GroupProvider.ID.ToString()),
                new ConditionSeparator(EnumConditionSeparator.AND),
                new Condition("idprovider", "=", _GroupProvider.IDProvider)
            };

            bool result = DBManager.InstanceOf.Update("GroupAP", columns, conditions);

            if (result)
            {
                NotificationManager.Show("Les informations ont été enregistrées avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Les informations n'ont pas pu être enregistrées.", NotificationType.Error);
            }

            return true;
        }

        protected override bool Delete(GroupProvider _GroupProvider)
        {
            var articles = DataManager.ArticleManager.FetchAll();

            var article = articles.FirstOrDefault(_ => _.GroupProviderID == _GroupProvider.ID);

            if (article == null)
            {
                return false;
            }

            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("groupapid", "=", _GroupProvider.ID.ToString()),
                new ConditionSeparator(EnumConditionSeparator.AND),
                new Condition("idprovider", "=", _GroupProvider.IDProvider)
            };
            bool result = DBManager.InstanceOf.Delete("GroupAP", conditions);

            if (result)
            {
                article.GroupProviders.Remove(_GroupProvider);
                NotificationManager.Show("Le fournisseur a été supprimé de cet article avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Le fournisseur n'a pas pu être supprimé de cet article.", NotificationType.Error);
            }

            return true;
        }


        public override List<GroupProvider> FetchAll(bool _Reload = false)
        {
            if (m_GroupProviders == null || _Reload == true)
            {
                m_GroupProviders = new List<GroupProvider>();

                // get currency
                var currencies = DataManager.CurrencyManager.FetchAll(_Reload);

                var groupProviders = DBManager.InstanceOf.SelectQuery("SELECT * FROM GroupAP;");
                foreach (RowResult groupProviderResult in groupProviders)
                {
                    int providerId = int.Parse(groupProviderResult.ColumnsList[1].Value.ToString());

                    GroupProvider groupProvider = new GroupProvider
                    {
                        ID = Guid.Parse(groupProviderResult.ColumnsList[0].Value.ToString()),
                        IDProvider = providerId,
                        PriceHT = double.Parse(groupProviderResult.ColumnsList[2].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        ArticleNumber = groupProviderResult.ColumnsList[3].Value.ToString()
                    };
                    if (groupProviderResult.ColumnsList[4].Value != null && !string.IsNullOrEmpty(groupProviderResult.ColumnsList[4].Value.ToString()))
                    {
                        int currencyId = int.Parse(groupProviderResult.ColumnsList[4].Value.ToString());

                        var currency = currencies.FirstOrDefault(_ => _.ID == currencyId);
                        groupProvider.Currency = currency;
                    }

                    m_GroupProviders.Add(groupProvider);
                }
            }

            return m_GroupProviders;
        }
    }


    public class ArticleManager : DataManagerBase<Article>
    {
        private List<Article> m_Articles;

        internal ArticleManager()
        {
        }

        public override List<Article> FetchAll(bool _Reload = false)
        {
            if (m_Articles == null || _Reload == true)
            {
                m_Articles = new List<Article>();

                var results = DBManager.InstanceOf.SelectQuery("SELECT * FROM Article;");

                var groupProviders = DataManager.GroupProviderManager.FetchAll(_Reload);
                var providers = DataManager.ProviderManager.FetchAll(_Reload);
                var history = DataManager.HistoryManager.FetchAll(_Reload);

                foreach (RowResult articleRowResult in results)
                {
                    Article article = new Article
                    {
                        ID = int.Parse(articleRowResult.ColumnsList[0].Value.ToString()),
                        Number = articleRowResult.ColumnsList[1].Value.ToString(),
                        Name = articleRowResult.ColumnsList[2].Value.ToString(),
                        Description = articleRowResult.ColumnsList[3].Value.ToString(),
                        PriceHT = double.Parse(articleRowResult.ColumnsList[4].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        PriceTTC = double.Parse(articleRowResult.ColumnsList[5].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        Quantity = double.Parse(articleRowResult.ColumnsList[6].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        CriticalQuantity = double.Parse(articleRowResult.ColumnsList[7].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        RubricPage = articleRowResult.ColumnsList[8].Value.ToString(),
                        Commentary = articleRowResult.ColumnsList[9].Value.ToString(),
                        GroupArticleID = Guid.Parse(articleRowResult.ColumnsList[12].Value.ToString()),
                        GroupProviderID = Guid.Parse(articleRowResult.ColumnsList[13].Value.ToString()),
                        OrderFileName = articleRowResult.ColumnsList[14].Value.ToString()
                    };

                    if (Enum.TryParse(articleRowResult.ColumnsList[15].Value.ToString(), out EnumArticleAssemblyType assemblyType))
                    {
                        article.AssemblyType = assemblyType;
                    }

                    if (string.IsNullOrEmpty(article.OrderFileName))
                    {
                        article.OrderFileName = Guid.NewGuid().ToString();
                        article.MustSaved = true;
                    }

                    var groups = groupProviders.Where(_ => _.ID == article.GroupProviderID);

                    foreach (GroupProvider groupProvider in groups)
                    {
                        var provider = providers.FirstOrDefault(_ => _.ID == groupProvider.IDProvider);

                        if (provider != null)
                        {
                            groupProvider.Item = provider;
                            article.GroupProviders.Add(groupProvider);
                        }
                    }

                    foreach (History h in history.Where(_ => _.ArticleID == article.ID))
                    {
                        article.History.Add(h);
                    }

                    m_Articles.Add(article);
                }

                // get sub-articles
                var groupArticles = DataManager.GroupArticleManager.FetchAll(_Reload);

                foreach (Article article in m_Articles)
                {
                    var groupA = groupArticles.Where(_ => _.ID == article.GroupArticleID);
                    foreach (GroupArticle groupArticle in groupA)
                    {
                        var subArticle = m_Articles.FirstOrDefault(_ => _.ID == groupArticle.ArticleId);

                        if (subArticle != null)
                        {
                            groupArticle.Item = subArticle;
                            article.GroupArticles.Add(groupArticle);
                        }
                    }
                }
            }

            return m_Articles;
        }

        protected override bool Insert(Article _Article)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("no", _Article.Number, _Article.Number.GetType()),
                new ColumnResult("name", _Article.Name, _Article.Name.GetType()),
                new ColumnResult("description", _Article.Description, _Article.Description.GetType()),
                new ColumnResult("priceht", _Article.PriceHT, _Article.PriceHT.GetType()),
                new ColumnResult("pricettc", _Article.PriceTTC, _Article.PriceTTC.GetType()),
                new ColumnResult("quantity", _Article.Quantity, _Article.Quantity.GetType()),
                new ColumnResult("quantitymin", _Article.CriticalQuantity, _Article.CriticalQuantity.GetType()),
                new ColumnResult("rubric", _Article.RubricPage, _Article.RubricPage.GetType()),
                new ColumnResult("commentary", _Article.Commentary, _Article.Commentary.GetType()),
                new ColumnResult("isCommand", 0, typeof(int)),
                new ColumnResult("urlphoto", "", typeof(string)),
                new ColumnResult("groupaaid", _Article.GroupArticleID.ToString(), typeof(string)),
                new ColumnResult("groupapid", _Article.GroupProviderID.ToString(), typeof(string)),
                new ColumnResult("order_file", _Article.OrderFileName.ToString(), typeof(string)),
                new ColumnResult("assembly_type", (int)_Article.AssemblyType, typeof(int))
            };

            bool result = DBManager.InstanceOf.Insert("Article", columns, out int id);
            if (result)
            {
                _Article.ID = id;
                m_Articles.Add(_Article);
                NotificationManager.Show("L'article a été ajouté avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("L'article n'a pas pu être ajouté.", NotificationType.Error);
            }

            return result;
        }

        protected override bool Update(Article _Article)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("no", _Article.Number, _Article.Number.GetType()),
                new ColumnResult("name", _Article.Name, _Article.Name.GetType()),
                new ColumnResult("description", _Article.Description, _Article.Description.GetType()),
                new ColumnResult("priceht", _Article.PriceHT, _Article.PriceHT.GetType()),
                new ColumnResult("pricettc", _Article.PriceTTC, _Article.PriceTTC.GetType()),
                new ColumnResult("quantity", _Article.Quantity, _Article.Quantity.GetType()),
                new ColumnResult("quantitymin", _Article.CriticalQuantity, _Article.CriticalQuantity.GetType()),
                new ColumnResult("rubric", _Article.RubricPage, _Article.RubricPage.GetType()),
                new ColumnResult("commentary", _Article.Commentary, _Article.Commentary.GetType()),
                new ColumnResult("isCommand", 0, typeof(int)),
                new ColumnResult("urlphoto", "", typeof(string)),
                new ColumnResult("groupaaid", _Article.GroupArticleID.ToString(), typeof(string)),
                new ColumnResult("groupapid", _Article.GroupProviderID.ToString(), typeof(string)),
                new ColumnResult("order_file", _Article.OrderFileName.ToString(), typeof(string)),
                new ColumnResult("assembly_type", (int)_Article.AssemblyType, typeof(int)),
            };

            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("id", "=", _Article.ID)
            };

            bool result = DBManager.InstanceOf.Update("Article", columns, conditions);
            if (result)
            {
                NotificationManager.Show("Les changements ont été sauvegardés avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Les changements n'ont pas pu être sauvegardés.", NotificationType.Error);
            }

            return result;
        }

        protected override bool Delete(Article _Article)
        {
            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("id", "=", _Article.ID)
            };

            // remove all sub-articles
            DataManager.GroupArticleManager.NotificationManager.SuspendNotification();
            for (int i = 0; i < _Article.GroupArticles.Count;)
            {
                GroupArticle groupArticle = _Article.GroupArticles[i];
                if (DataManager.Execute(EnumDatabaseAction.Delete, groupArticle) == false)
                {
                    i++;
                }
            }
            DataManager.GroupArticleManager.NotificationManager.ResumeNotification();


            // remove all article-provider
            DataManager.GroupProviderManager.NotificationManager.SuspendNotification();
            for (int i = 0; i < _Article.GroupProviders.Count;)
            {
                GroupProvider groupProvider = _Article.GroupProviders[i];
                if (DataManager.Execute(EnumDatabaseAction.Delete, groupProvider) == false)
                {
                    i++;
                }
            }
            DataManager.GroupProviderManager.NotificationManager.ResumeNotification();

            // remove file order
            if (File.Exists(_Article.OrderFileName))
            {
                File.Delete(_Article.OrderFileName);
            }

            bool result = DBManager.InstanceOf.Delete("Article", conditions);
            if (result)
            {
                m_Articles.Remove(_Article);
                NotificationManager.Show("L'article a été supprimé avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("L'article n'a pas pu être supprimé.", NotificationType.Error);
            }

            return result;
        }
    }

    public class ProviderManager : DataManagerBase<Provider>
    {
        private List<Provider> m_Providers;

        internal ProviderManager()
        {
        }

        protected override bool Insert(Provider _Provider)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("no", _Provider.Number, _Provider.Number.GetType()),
                new ColumnResult("name", _Provider.Name, _Provider.Name.GetType()),
                new ColumnResult("address", _Provider.Address, _Provider.Address.GetType()),
                new ColumnResult("locality", _Provider.Locality, _Provider.Locality.GetType()),
                new ColumnResult("npa", _Provider.NPA, _Provider.NPA.GetType()),
                new ColumnResult("country", _Provider.Country, _Provider.Country.GetType()),
                new ColumnResult("fax", _Provider.Fax, _Provider.Fax.GetType()),
                new ColumnResult("phone", _Provider.Phone, _Provider.Phone.GetType()),
                new ColumnResult("privatephone", _Provider.PrivatePhone, _Provider.PrivatePhone.GetType()),
                new ColumnResult("mail", _Provider.Mail, _Provider.Mail.GetType()),
                new ColumnResult("web", _Provider.SiteWeb, _Provider.SiteWeb.GetType()),
                new ColumnResult("noclient", _Provider.ClientNumber, _Provider.ClientNumber.GetType()),
                new ColumnResult("manager", _Provider.Manager, _Provider.Manager.GetType()),
                new ColumnResult("pricetransport", _Provider.PriceTransport, _Provider.PriceTransport.GetType()),
                new ColumnResult("commentary", _Provider.Commentary, _Provider.Commentary.GetType()),
                new ColumnResult("groupapid", _Provider.GroupProviderID.ToString(), typeof(string))
            };

            bool result = DBManager.InstanceOf.Insert("Provider", columns, out int id);
            if (result)
            {
                _Provider.ID = id;
                m_Providers.Add(_Provider);
                NotificationManager.Show("Le fournisseur a été ajouté avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Le fournisseur n'a pas pu être ajouté.", NotificationType.Error);
            }

            return result;
        }

        protected override bool Update(Provider _Provider)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("no", _Provider.Number, _Provider.Number.GetType()),
                new ColumnResult("name", _Provider.Name, _Provider.Name.GetType()),
                new ColumnResult("address", _Provider.Address, _Provider.Address.GetType()),
                new ColumnResult("locality", _Provider.Locality, _Provider.Locality.GetType()),
                new ColumnResult("npa", _Provider.NPA, _Provider.NPA.GetType()),
                new ColumnResult("country", _Provider.Country, _Provider.Country.GetType()),
                new ColumnResult("fax", _Provider.Fax, _Provider.Fax.GetType()),
                new ColumnResult("phone", _Provider.Phone, _Provider.Phone.GetType()),
                new ColumnResult("privatephone", _Provider.PrivatePhone, _Provider.PrivatePhone.GetType()),
                new ColumnResult("mail", _Provider.Mail, _Provider.Mail.GetType()),
                new ColumnResult("web", _Provider.SiteWeb, _Provider.SiteWeb.GetType()),
                new ColumnResult("noclient", _Provider.ClientNumber, _Provider.ClientNumber.GetType()),
                new ColumnResult("manager", _Provider.Manager, _Provider.Manager.GetType()),
                new ColumnResult("pricetransport", _Provider.PriceTransport, _Provider.PriceTransport.GetType()),
                new ColumnResult("commentary", _Provider.Commentary, _Provider.Commentary.GetType()),
                new ColumnResult("groupapid", _Provider.GroupProviderID.ToString(), typeof(string))
            };

            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("id", "=", _Provider.ID)
            };

            bool result = DBManager.InstanceOf.Update("Provider", columns, conditions);
            if (result)
            {
                NotificationManager.Show("Les changements ont été sauvegardés avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Les changements n'ont pas pu être sauvegardés.", NotificationType.Error);
            }

            return result;
        }

        protected override bool Delete(Provider _Provider)
        {
            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("id", "=", _Provider.ID)
            };

            bool result = DBManager.InstanceOf.Delete("Provider", conditions);
            if (result)
            {
                m_Providers.Remove(_Provider);
                NotificationManager.Show("Le fournisseur a été supprimé avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Le fournisseur n'a pas pu être supprimé.", NotificationType.Error);
            }

            return result;
        }

        public override List<Provider> FetchAll(bool _Reload = false)
        {
            if (m_Providers == null || _Reload == true)
            {
                m_Providers = new List<Provider>();

                var results = DBManager.InstanceOf.SelectQuery("SELECT * FROM Provider;");

                foreach (RowResult rowResult in results)
                {
                    Provider provider = new Provider
                    {
                        ID = int.Parse(rowResult.ColumnsList[0].Value.ToString()),
                        Number = rowResult.ColumnsList[1].Value.ToString(),
                        Name = rowResult.ColumnsList[2].Value.ToString(),
                        Address = rowResult.ColumnsList[3].Value.ToString(),
                        Locality = rowResult.ColumnsList[4].Value.ToString(),
                        NPA = rowResult.ColumnsList[5].Value.ToString(),
                        Country = rowResult.ColumnsList[6].Value.ToString(),
                        Fax = rowResult.ColumnsList[7].Value.ToString(),
                        Phone = rowResult.ColumnsList[8].Value.ToString(),
                        PrivatePhone = rowResult.ColumnsList[9].Value.ToString(),
                        Mail = rowResult.ColumnsList[10].Value.ToString(),
                        SiteWeb = rowResult.ColumnsList[11].Value.ToString(),
                        ClientNumber = rowResult.ColumnsList[12].Value.ToString(),
                        Manager = rowResult.ColumnsList[13].Value.ToString(),
                        PriceTransport = double.Parse(rowResult.ColumnsList[14].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        Commentary = rowResult.ColumnsList[15].Value.ToString(),
                        GroupProviderID = Guid.Parse(rowResult.ColumnsList[16].Value.ToString())
                    };

                    m_Providers.Add(provider);
                }
            }

            return m_Providers;
        }
    }

    public class HistoryManager : DataManagerBase<History>
    {
        private List<History> m_Histories;

        internal HistoryManager()
        {
        }

        protected override bool Insert(History _History)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("articleid", _History.ArticleID, _History.ArticleID.GetType()),
                new ColumnResult("datetime", _History.Date.ToUniversalTime(), typeof(string)),
                new ColumnResult("quantity", _History.Quantity, _History.Quantity.GetType()),
                new ColumnResult("balance", _History.Balance, _History.Balance.GetType()),
                new ColumnResult("actiontype", (int)_History.ActionType, typeof(int))
            };

            bool result = DBManager.InstanceOf.Insert("History", columns, out int id);
            if (result)
            {
                _History.ID = id;
                m_Histories.Add(_History);
                NotificationManager.Show("L'historique a été ajouté avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("L'historique n'a pas pu être ajouté.", NotificationType.Error);
            }

            return result;
        }

        public bool InsertAll(List<History> _Histories)
        {
            NotificationManager.SuspendNotification();

            foreach (History history in _Histories)
            {
                Insert(history);
            }

            NotificationManager.ResumeNotification();
            return true;
        }

        protected override bool Update(History _History)
        {
            ColumnsList columns = new ColumnsList
            {
                new ColumnResult("articleid", _History.ArticleID, _History.ArticleID.GetType()),
                new ColumnResult("datetime", _History.Date.ToUniversalTime(), typeof(string)),
                new ColumnResult("quantity", _History.Quantity, _History.Quantity.GetType()),
                new ColumnResult("balance", _History.Balance, _History.Balance.GetType()),
                new ColumnResult("actiontype", (int)_History.ActionType, typeof(int))
            };

            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("id", "=", _History.ID)
            };

            bool result = DBManager.InstanceOf.Update("History", columns, conditions);
            if (result)
            {
                NotificationManager.Show("Les changements ont été sauvegardés avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("Les changements n'ont pas pu être sauvegardés.", NotificationType.Error);
            }

            return result;
        }

        protected override bool Delete(History _History)
        {
            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("id", "=", _History.ID)
            };

            bool result = DBManager.InstanceOf.Delete("History", conditions);
            if (result)
            {
                m_Histories.Remove(_History);
                NotificationManager.Show("L'historique a été supprimé avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("L'historique n'a pas pu être supprimé.", NotificationType.Error);
            }

            return result;
        }

        public override List<History> FetchAll(bool _Reload = false)
        {
            if (m_Histories == null || _Reload == true)
            {
                m_Histories = new List<History>();

                var results = DBManager.InstanceOf.SelectQuery("SELECT * FROM History;");

                foreach (RowResult rowResult in results)
                {
                    History history = new History
                    {
                        ID = int.Parse(rowResult.ColumnsList[0].Value.ToString()),
                        ArticleID = int.Parse(rowResult.ColumnsList[1].Value.ToString()),
                        Date = DateTime.Parse(rowResult.ColumnsList[2].Value.ToString()).ToLocalTime(),
                        Quantity = double.Parse(rowResult.ColumnsList[3].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        Balance = double.Parse(rowResult.ColumnsList[4].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture),
                        ActionType = (EnumStockAction)int.Parse(rowResult.ColumnsList[5].Value.ToString()),
                    };

                    m_Histories.Add(history);
                }
            }

            return m_Histories;
        }

        public bool DeleteHistoryArticle(Article _Article)
        {
            List<ConditionBase> conditions = new List<ConditionBase>
            {
                new Condition("articleid", "=", _Article.ID)
            };

            bool result = DBManager.InstanceOf.Delete("History", conditions);
            if (result)
            {
                m_Histories.RemoveAll(_ => _.ArticleID == _Article.ID);
                NotificationManager.Show("L'historique a été supprimé avec succès.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Show("L'historique n'a pas pu être supprimé.", NotificationType.Error);
            }

            return result;
        }
    }
}
