using System;
using System.ComponentModel;

namespace InfraStructure.Table
{
    public class AbstractRec : OwnerTable
    {
        [DisplayName("内容摘要")]
        public string Content { set; get; }

        [DisplayName("数据集")]
        public string CollectionDisplayName { set; get; }

        public string CollectionName { set; get; }

        public override string GetCollectionName()
        {
            throw new NotImplementedException();
        }

        public override string GetPrefix()
        {
            throw new NotImplementedException();
        }
    }
}
