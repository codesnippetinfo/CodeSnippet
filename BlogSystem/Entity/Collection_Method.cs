using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace BlogSystem.Entity
{
    public partial class Collection : OwnerTable
    {
        /// <summary>
        /// 获得某人文集列表
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<Collection> GetCollectionListByOwnerId(string ownerId)
        {
            return OwnerTableOperator.GetRecListByOwnerId<Collection>(CollectionName, ownerId);
        }
        /// <summary>
        /// 某人是否拥有了某个标题的文集
        /// </summary>
        /// <param name="title"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static bool ExistCollectionByTitle(string title, string ownerId)
        {
            IMongoQuery TitleQuery = Query.EQ(nameof(Title), title);
            var cnt = OwnerTableOperator.GetRecListByOwnerId<Collection>(CollectionName, ownerId, TitleQuery).Count;
            return cnt != 0;
        }

        /// <summary>
        /// 获得所有系列文章
        /// </summary>
        /// <returns></returns>
        public static List<Collection> getAllSerial()
        {
            return MongoDbRepository.GetRecList<Collection>(Query.EQ(nameof(IsSerie), true));
        }


    }
}
