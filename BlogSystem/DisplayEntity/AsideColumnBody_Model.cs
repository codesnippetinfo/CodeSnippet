using InfraStructure.DataBase;
using System.Collections.Generic;

namespace BlogSystem.DisplayEntity
{
    /// <summary>
    /// 侧表栏
    /// </summary>
    public partial class AsideColumnBody : EntityBase
    {
        #region "model"

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 链接基本地址
        /// </summary>
        public string HrefBase { get; set; }
        
        /// <summary>
        /// 标签信息
        /// </summary>
        public List<GenericItem> DetailItem { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "AsideColumnBody";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "AsideColumnBody";


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
        public static string MvcTitle = "侧表栏";

        /// <summary>
        /// 按照序列号查找侧表栏
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>侧表栏</returns>
        public static AsideColumnBody GetAsideColumnBodyBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<AsideColumnBody>(Sn);
        }

        /// <summary>
        /// 插入侧表栏
        /// </summary>
        /// <param name="Newasidecolumnbody"></param>
        /// <returns>序列号</returns>
        public static string InsertAsideColumnBody(AsideColumnBody NewAsideColumnBody)
        {
            return MongoDbRepository.InsertRec(NewAsideColumnBody);
        }

        /// <summary>
        /// 删除侧表栏
        /// </summary>
        /// <param name="DropAsideColumnBody"></param>
        public static void DropAsideColumnBody(AsideColumnBody DropAsideColumnBody)
        {
            MongoDbRepository.DeleteRec(DropAsideColumnBody);
        }

        /// <summary>
        /// 修改侧表栏
        /// </summary>
        /// <param name="OldAsideColumnBody"></param>
        public static void UpdateAsideColumnBody(AsideColumnBody OldAsideColumnBody)
        {
            MongoDbRepository.UpdateRec(OldAsideColumnBody);
        }

        #endregion
    }
}
