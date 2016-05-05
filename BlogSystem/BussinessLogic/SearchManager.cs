using BlogSystem.Entity;
using InfraStructure.DataBase;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogSystem.BussinessLogic
{
    public static class SearchManager
    {
        /// <summary>
        /// NEST Client
        /// </summary>
        public static ElasticClient client = null;
        /// <summary>
        /// 索引名称
        /// </summary>
        const string indexName = "artical";
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            var node = new Uri("http://localhost:9200/");
            var settings = new ConnectionSettings(node);
            //必须是小写
            settings.DefaultIndex(indexName);
            client = new ElasticClient(settings);
        }

        /// <summary>
        /// 重新索引数据
        /// </summary>
        public static void ReIndex()
        {
            client.DeleteIndex(indexName);
            var res = client.CreateIndex(indexName);
            var articles = MongoDbRepository.GetRecList<Article>(ArticleListManager.PublicArticleQuery);
            System.Diagnostics.Debug.WriteLine("Total Record To ReIndex:" + articles.Count);
            foreach (var article in articles)
            {
                client.Index(article);
                System.Diagnostics.Debug.WriteLine("Process:" + article.Sn);
            }
        }
        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="article"></param>
        public static void Index(Article article)
        {
            client.Index(article);
        }
        /// <summary>
        /// 分词
        /// </summary>
        /// <param name="strSentence"></param>
        /// <returns></returns>
        public static List<string> GetTokenList(string strSentence)
        {
            AnalyzeRequest a = new AnalyzeRequest();
            a.Text = new string[] { strSentence };
            a.Tokenizer = "ik";
            var result = client.Analyze(a);
            if (result.Tokens == null) return null;
            return result.Tokens.Select(x=>x.Token).ToList<string>();
        }

        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="KeyWord"></param>
        /// <returns></returns>
        public static List<Article> Search(string KeyWord, string Field)
        {
            var KeyWordArray = KeyWord.Split(" ".ToArray());
            QueryContainer query = null;
            for (int i = 0; i < KeyWordArray.Length; i++)
            {
                if (query == null)
                {
                    query = new QueryStringQuery() { DefaultField = Field.ToLower(), Query = KeyWordArray[i], DefaultOperator = Operator.And };
                }
                else
                {
                    query = query && new QueryStringQuery() { DefaultField = Field.ToLower(), Query = KeyWordArray[i], DefaultOperator = Operator.And };
                }
            }
            ISearchRequest s = new SearchRequest();
            s.Query = query;
            s.Size = 1000;
            var searchResults = client.Search<Article>(s);
            return searchResults.Documents.ToList();
        }
    }
}
