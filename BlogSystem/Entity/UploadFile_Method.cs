using BlogSystem.BussinessLogic;
using InfraStructure.DataBase;
using InfraStructure.Misc;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogSystem.Entity
{
    public partial class UploadFile : OwnerTable
    {
        /// <summary>
        /// 获得文章（包括评论）的所有图片
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static List<UploadFile> GetFileByArticleId(string articleId)
        {
            IMongoQuery x1 = Query.EQ(nameof(ArticleID), articleId);
            return MongoDbRepository.GetRecList<UploadFile>(x1);
        }
        /// <summary>
        /// 获得用户图片大小
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static double GetUseageByAccountId(string accountId)
        {
            IMongoQuery x1 = Query.EQ(nameof(OwnerId), accountId);
            var upfiles = MongoDbRepository.GetRecList<UploadFile>(x1);
            return upfiles.Sum((x) => { return x.Size; });
        }
        /// <summary>
        /// M的容量
        /// </summary>
        private static double Volumn_M = Math.Pow(2, 20);
        /// <summary>
        /// 获得限制值
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static double GetLimitByAccountId(string accountId)
        {
            var user = UserInfo.GetUserInfoBySn(accountId);
            //管理员和特约作者，上限1G
            if (user.Privilege == UserType.Admin || user.Privilege == UserType.Author) return 1024 * Volumn_M; 
            double Limit = 5 * Volumn_M;
            var AuthorQuery = Query.EQ(nameof(OwnerId), accountId);
            var articleCnt = MongoDbRepository.GetRecordCount<Article>(Query.And(AuthorQuery, ArticleListManager.FirstPageArticleQuery));
            Limit += articleCnt * 3 * Volumn_M;
            return Limit;
        }
        /// <summary>
        /// 获得剩余值
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetFreeVolumnByAccountId(string accountId)
        {
            var used = GetUseageByAccountId(accountId);
            var total = GetLimitByAccountId(accountId);
            var free = total - used;
            return StringExtend.GetSize((long)free);
        }
        /// <summary>
        /// 追加这个资源之后是否跨越了最大值
        /// </summary>
        /// <param name="NewResourceSize"></param>
        /// <returns></returns>
        public static bool IsAboveLimit(double NewResourceSize, string accountId)
        {
            var used = GetUseageByAccountId(accountId);
            var total = GetLimitByAccountId(accountId);
            return (NewResourceSize + used) > total;
        }
    }
}
