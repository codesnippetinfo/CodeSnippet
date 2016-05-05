using System;
using System.Collections.Generic;
using System.Linq;

namespace InfraStructure.DataBase
{
    /// <summary>
    ///     分页处理
    /// </summary>
    public class Pages
    {
        /// <summary>
        ///     当前显示的页编号
        /// </summary>
        public int CurrentPageNo = 1;

        /// <summary>
        ///     每页项目数
        /// </summary>
        /// <value>The page item count.</value>
        public int PageItemCount;

        /// <summary>
        ///     满足条件的总数目
        /// </summary>
        public int TotalItemCount;

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="totalItems"></param>
        /// <param name="itemsPerPage"></param>
        public Pages(int totalItems, int itemsPerPage = 50)
        {
            PageItemCount = itemsPerPage;
            TotalItemCount = totalItems;
        }

        /// <summary>
        ///     总页数
        /// </summary>
        public int TotalPage()
        {
            var page = (int) Math.Ceiling((double) TotalItemCount/PageItemCount);
            return page == 0 ? 1 : page;
        }
        /// <summary>
        ///     跳过条数
        /// </summary>
        /// <returns></returns>
        public int SkipCount()
        {
            return (CurrentPageNo - 1) * PageItemCount;
        }

        /// <summary>
        ///     获得对象列表(适合轻量数据)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ItemList"></param>
        /// <returns></returns>
        public List<T> GetList<T>(List<T> ItemList)
        {
            //内部自动屏蔽错误，可以放心使用
            return ItemList.Skip(SkipCount()).Take(PageItemCount).ToList();
        }
    }
}