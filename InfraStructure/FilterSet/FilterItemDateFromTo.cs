using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InfraStructure.FilterSet
{
    public class FilterItemDateFromTo : FilterItemBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        public FilterItemDateFromTo(string fieldName)
        {
            FieldName = fieldName;
            IsActive = false;
            From = DateTime.MinValue;
            To = DateTime.MaxValue;
        }

        /// <summary>
        ///     开始
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime From { set; get; }

        /// <summary>
        ///     结束
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime To { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IMongoQuery GetQuery()
        {
            var filterItemQuery = Query.And(Query.GTE(FieldName, From), Query.LTE(FieldName, To));
            return filterItemQuery;
        }
    }
}