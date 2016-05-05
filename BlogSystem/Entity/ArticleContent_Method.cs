using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BlogSystem.Entity
{
    public partial class ArticleContent : OwnerTable
    {

        //MarkDown
        //Draft:草稿
        //First:第一次审核通过后的MarkDown，防止首页纠纷
        //Current：当前MarkDown，展示用
        //HTML
        //Current：PDF用

        /// <summary>
        /// MarkDown
        /// </summary>
        public const string MarkDown = "MarkDown";

        /// <summary>
        /// HTML
        /// </summary>
        public const string HTML = "HTML";

        /// <summary>
        /// 保存MarkDown
        /// </summary>
        /// <param name="strArticleId"></param>
        /// <param name="strContent"></param>
        /// <param name="OwnerId"></param>
        public static void SaveMarkDownVersion(string strArticleId, string strContent, string OwnerId, RevisionType step)
        {
            IMongoQuery ArticleIdQuery = Query.EQ(nameof(ArticleID), strArticleId);
            IMongoQuery RevisionQuery = Query.EQ(nameof(Revision), step);
            IMongoQuery MarkDownExistQuery = Query.EQ(nameof(ContentType), MarkDown);
            var FirstFind = MongoDbRepository.GetFirstRec<ArticleContent>(Query.And(ArticleIdQuery, RevisionQuery, MarkDownExistQuery));
            if (FirstFind != null)
            {
                FirstFind.Content = strContent;
                FirstFind.ContentType = MarkDown;
                MongoDbRepository.UpdateRec(FirstFind);
            }
            else
            {
                ArticleContent NewFirst = new ArticleContent();
                NewFirst.ArticleID = strArticleId;
                NewFirst.Revision = step;
                NewFirst.Content = strContent;
                NewFirst.ContentType = MarkDown;
                InsertArticleContent(NewFirst, OwnerId);
            }
        }

        /// <summary>
        /// 保存HTML
        /// </summary>
        /// <param name="strArticleId"></param>
        /// <param name="strContent"></param>
        /// <param name="OwnerId"></param>
        public static void SaveHTMLVersion(string strArticleId, string strContent, string OwnerId)
        {
            //HTML只保存当前版本，为了制作PDF用
            IMongoQuery ArticleIdQuery = Query.EQ(nameof(ArticleID), strArticleId);
            IMongoQuery HTMLExistQuery = Query.EQ(nameof(ContentType), HTML);
            var FirstFind = MongoDbRepository.GetFirstRec<ArticleContent>(Query.And(ArticleIdQuery, HTMLExistQuery));
            if (FirstFind != null)
            {
                FirstFind.Content = strContent;
                FirstFind.ContentType = HTML;
                MongoDbRepository.UpdateRec(FirstFind);
            }
            else
            {
                ArticleContent NewFirst = new ArticleContent();
                NewFirst.ArticleID = strArticleId;
                NewFirst.Revision = RevisionType.Current;
                NewFirst.Content = strContent;
                NewFirst.ContentType = HTML;
                InsertArticleContent(NewFirst, OwnerId);
            }
        }

        /// <summary>
        /// 获得MarkDown
        /// </summary>
        /// <param name="strArticleId"></param>
        /// <returns></returns>
        public static ArticleContent GetMarkDown(string strArticleId, RevisionType Rtype)
        {
            IMongoQuery ArticleIdQuery = Query.EQ(nameof(ArticleID), strArticleId);
            IMongoQuery RevisionQuery = Query.EQ(nameof(Revision), Rtype);
            IMongoQuery MarkDownExistQuery = Query.EQ(nameof(ContentType), MarkDown);
            var Draft = MongoDbRepository.GetFirstRec<ArticleContent>(Query.And(ArticleIdQuery, RevisionQuery, MarkDownExistQuery));
            return Draft;
        }

        /// <summary>
        /// 获得MD文字
        /// </summary>
        /// <param name="strArticleId"></param>
        /// <param name="Rtype"></param>
        /// <returns></returns>
        public static string GetMarkDownString(string strArticleId, RevisionType Rtype)
        {
            var Document = GetMarkDown(strArticleId, Rtype);
            if (Document == null)
            {
                InfraStructure.Log.ExceptionLog.Log("SYSTEM", "GetHtmlString", "strArticleId=" + strArticleId, "NULL");
                return string.Empty;
            }
            return Document.Content;
        }
        /// <summary>
        /// 获得HTML文字
        /// </summary>
        /// <param name="strArticleId"></param>
        /// <returns></returns>
        public static string GetHtmlString(string strArticleId)
        {
            IMongoQuery ArticleIdQuery = Query.EQ(nameof(ArticleID), strArticleId);
            IMongoQuery HtmlExistQuery = Query.EQ(nameof(ContentType), HTML);
            var Draft = MongoDbRepository.GetFirstRec<ArticleContent>(Query.And(ArticleIdQuery, HtmlExistQuery));
            if (Draft == null) return string.Empty;
            return Draft.Content;
        }
    }
}
