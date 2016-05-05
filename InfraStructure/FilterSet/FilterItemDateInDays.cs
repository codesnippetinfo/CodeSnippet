using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InfraStructure.FilterSet
{
    public class FilterItemDateInDays : FilterItemBase
    {
        public FilterItemDateInDays(string fieldName)
        {
            FieldName = fieldName;
            IsActive = false;
            Days = 0;
            BaseDate = DateTime.Now;
        }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BaseDate { get; set; }
        /// <summary>
        ///     在多少天内
        /// </summary>
        public int Days { set; get; }

        public override IMongoQuery GetQuery()
        {
            IMongoQuery filterItemQuery;
            var limitDate = BaseDate.AddDays(Days);
            filterItemQuery = Query.LTE(FieldName, limitDate);
            return filterItemQuery;
        }
    }
}
