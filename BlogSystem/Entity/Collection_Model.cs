using InfraStructure.DataBase;
using InfraStructure.Table;
using System;
using System.Collections.Generic;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 文集
    /// </summary>
    public partial class Collection : OwnerTable
    {
        #region "model"

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否为系列教程
        /// </summary>
        public bool IsSerie { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 标签列表
        /// </summary>
        public string CustomTagList { get; set; }

        /// <summary>
        /// 标签表示文字
        /// </summary>
        public List<string> TagName { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "Collection";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "Collection";


        /// <summary>
        /// 数据主键前缀
        /// </summary>
        public override string GetPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        /// 数据主键前缀静态字段
        /// </summary>
        public static string Prefix = string.Empty;

        /// <summary>
        /// Mvc画面的标题
        /// </summary>
        public static string MvcTitle = "文集";

        /// <summary>
        /// 按照序列号查找文集
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>文集</returns>
        public static Collection GetCollectionBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<Collection>(Sn);
        }

        /// <summary>
        /// 插入文集
        /// </summary>
        /// <param name="Newcollection"></param>
        /// <param name="OwnerId"></param>
        /// <returns>序列号</returns>
        public static string InsertCollection(Collection NewCollection, string OwnerId)
        {
            return OwnerTableOperator.InsertRec(NewCollection, OwnerId);
        }

        /// <summary>
        /// 删除文集
        /// </summary>
        /// <param name="DropCollection"></param>
        public static void DropCollection(Collection DropCollection)
        {
            MongoDbRepository.DeleteRec(DropCollection);
        }

        /// <summary>
        /// 修改文集
        /// </summary>
        /// <param name="OldCollection"></param>
        public static void UpdateCollection(Collection OldCollection)
        {
            MongoDbRepository.UpdateRec(OldCollection);
        }

        #endregion
    }
}
