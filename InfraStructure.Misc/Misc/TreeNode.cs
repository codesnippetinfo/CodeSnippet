
using System.Collections.Generic;

namespace InfraStructure.Misc
{
    public class TreeNode
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Code;
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentCode;
        /// <summary>
        /// 子节点（按照Rank排序）
        /// </summary>
        public List<TreeNode> ChildList;
        /// <summary>
        /// 等级（兄弟之间排序用）
        /// </summary>
        public int Rank;
        /// <summary>
        /// 深度
        /// </summary>
        public int NestLevel;
    }
}
