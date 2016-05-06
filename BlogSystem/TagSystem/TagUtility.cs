using BlogSystem.BussinessLogic;
using BlogSystem.Entity;
using InfraStructure.DataBase;
using InfraStructure.Misc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BlogSystem.TagSystem
{
    public static class TagUtility
    {
        /// <summary>
        /// 官方标签库
        /// </summary>
        public static List<Tag> Tags;

        /// <summary>
        /// Gets the tags from title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static List<string> getTagsFromTitle(string title)
        {
            //如果没有ElasticSearch的时候，使用传统方式
            if (SearchManager.client == null) return getTagsFromTitleOld(title);

            List<string> tags = new List<string>();
            TagStringComparer comparer = new TagStringComparer();
            //ElasticSearch
            var Tokens = new List<string>();
            Tokens = SearchManager.GetTokenList(title);
            if (Tokens == null) return getTagsFromTitleOld(title);
            foreach (var tag in Tags)
            {
                var MainTag = string.IsNullOrEmpty(tag.BaseTagName) ? tag.TagName : tag.BaseTagName;
                if (tag.IsOnlyContain)
                {
                    if (tag.IsCaseSensitive)
                    {
                        if (title.Contains(tag.TagName)) tags.Add(MainTag);
                    }
                    else
                    {
                        if (title.ToUpper().Contains(tag.TagName.ToUpper())) tags.Add(MainTag);
                    }
                }
                else
                {
                    //Token没有大小写
                    if (Tokens.Contains(tag.TagName, comparer)) tags.Add(MainTag);
                }
            }
            FilterTag(ref tags);
            return tags;
        }
        /// <summary>
        /// 比较器
        /// </summary>
        private class TagStringComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return (x.ToUpper().Equals(y.ToUpper()));
            }

            public int GetHashCode(string obj)
            {
                return int.Parse(obj.ToUpper()).ToString().GetHashCode();
            }
        }

        /// <summary>
        /// 长字符
        /// </summary>
        private static string[] LongWord = new string[] {
            "代码重构",
            "服务器端",
            "机器学习",
            "箭头函数",
            "解决方案",
            "开发工具",
            "垃圾回收",
            "理论学习",
            "内存管理",
            "设计模式",
            "社交网站",
            "生命周期",
            "数据结构",
            "数据类型",
            "数据挖掘",
            "微信支付",
            "性能优化",
            "游戏开发",
        };

        /// <summary>
        /// 整理标签结果
        /// </summary>
        /// <param name="Tags"></param>
        private static void FilterTag(ref List<string> Tags)
        {
            Tags = Tags.Distinct().ToList();
            if (Tags.Contains("ASP.NET") && Tags.Contains(".NET")) Tags.Remove(".NET");
            if (Tags.Contains("HTML5") && Tags.Contains("HTML")) Tags.Remove("HTML");
            if (Tags.Contains("Unity3D") && Tags.Contains("Unity")) Tags.Remove("Unity");
            foreach (var word in LongWord)
            {
                if (Tags.Contains(word) && Tags.Contains(word.Substring(0, 2)) && Tags.Contains(word.Substring(2)))
                {
                    Tags.Remove(word.Substring(0, 2));
                    Tags.Remove(word.Substring(2));
                }
            }
            if (Tags.Contains("集成开发环境") && Tags.Contains("集成") && Tags.Contains("开发") && Tags.Contains("环境"))
            {
                Tags.Remove("集成");
                Tags.Remove("开发");
                Tags.Remove("环境");
            }
            if (Tags.Contains("正则表达式") && Tags.Contains("正则") && Tags.Contains("表达式"))
            {
                Tags.Remove("正则");
                Tags.Remove("表达式");
            }
        }

        /// <summary>
        /// Gets the tags from title.
        /// </summary>
        /// <returns>The tags from title.</returns>
        /// <param name="UpperTitle">Title.</param>
        public static List<string> getTagsFromTitleOld(string title)
        {
            List<string> tags = new List<string>();
            //普通策略
            string UpperTitle = title.ToUpper();
            foreach (var tag in Tags)
            {
                var MainTag = string.IsNullOrEmpty(tag.BaseTagName) ? tag.TagName : tag.BaseTagName;
                var UpperTagName = tag.TagName.ToUpper();
                if (tag.IsCaseSensitive)
                {
                    //区分大小写，如果不匹配，直接退出
                    if (title.Contains(tag.TagName))
                    {
                        if (!CheckContains(title, tag.TagName)) tags.Add(MainTag);
                    }
                }
                else
                {
                    //标签名
                    if (UpperTitle.Contains(UpperTagName))
                    {
                        if (!CheckContains(title, tag.TagName)) tags.Add(MainTag);
                    }
                }
            }
            FilterTag(ref tags);
            return tags;
        }


        /// <summary>
        /// 解决标签包含问题
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="tag">待检查标签</param>
        /// <returns></returns>
        private static bool CheckContains(string title, string tag)
        {
            //该标签是否是其他标签的一部分
            //IsContain表示该标签包含于其他标签的列表,并且该标签出现在标题中。
            //例如DOCKER包含OC，则评价OC的时候，必须考虑标题中是否含有DOCKER关键字
            bool IsContain = false;
            foreach (var chkTags in Tags)
            {
                //例如Docker在评价自己的时候
                if (chkTags.TagName.Equals(tag)) continue;
                if (chkTags.IsCaseSensitive)
                {
                    if (chkTags.TagName.Contains(tag))
                    {
                        //某标签包含缩写：例如DOCKER包含OC
                        if (title.Contains(chkTags.TagName))
                        {
                            //标题中是否包含DOCKER标签
                            IsContain = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (chkTags.TagName.ToUpper().Contains(tag.ToUpper()))
                    {
                        //某标签包含缩写：例如DOCKER包含OC
                        if (title.ToUpper().Contains(chkTags.TagName.ToUpper()))
                        {
                            //标题中是否包含DOCKER标签
                            IsContain = true;
                            break;
                        }
                    }
                }
            }
            return IsContain;
        }


        /// <summary>
        /// 获得标签注解
        /// </summary>
        /// <param name="TagName"></param>
        /// <returns></returns>
        public static string GetTagCommentByTagName(string TagName)
        {
            var tags = Tags.Where(x => { return x.TagName == TagName || x.BaseTagName == TagName; });
            if (tags.Count() > 0) return tags.First().Comment;
            return string.Empty;
        }
        /// <summary>
        /// 用户新标签
        /// </summary>
        public static List<string> CustomNewTags = new List<string>();
        /// <summary>
        /// 排名管理器
        /// </summary>
        public static RankContain TagRankContain = null;
        /// <summary>
        /// 双标签
        /// </summary>
        public static RankContain TagTwoComboRankContain = null;
        /// <summary>
        /// 加载标签
        /// </summary>
        public static void Init()
        {
            Tags = Tag.GetAllTags();
            List<string> officeTag = new List<string>();
            //必须先初始化
            CustomNewTags.Clear();
            foreach (var tag in Tags)
            {
                officeTag.Add(tag.TagName);
            }
            //统计文章，通过首页审核文章
            var articles = MongoDbRepository.GetRecList<Article>();
            TagRankContain = Statistics(articles);
            TagTwoComboRankContain = StatisticsTwoCombo(articles);
            foreach (var article in articles)
            {
                //防御代码
                if (string.IsNullOrEmpty(article.CustomTagList)) article.CustomTagList = string.Empty;
                article.TagName = GetTagNameList(article.Title, article.CustomTagList);
                foreach (var tag in article.TagName)
                {
                    if (!officeTag.Contains(tag) && !CustomNewTags.Contains(tag)) CustomNewTags.Add(tag);
                }
                Article.UpdateArticle(article);
            }
            var topics = Topic.getAllTopic();
            foreach (var topic in topics)
            {
                //防御代码
                if (string.IsNullOrEmpty(topic.CustomTagList)) topic.CustomTagList = string.Empty;
                topic.TagName = GetTagNameList(topic.Title, topic.CustomTagList);
                foreach (var tag in topic.TagName)
                {
                    if (!officeTag.Contains(tag) && !CustomNewTags.Contains(tag)) CustomNewTags.Add(tag);
                }
                Topic.UpdateTopic(topic);
            }
            var collections = Collection.getAllSerial();
            foreach (var collection in collections)
            {
                //防御代码
                if (string.IsNullOrEmpty(collection.CustomTagList)) collection.CustomTagList = string.Empty;
                collection.TagName = GetTagNameList(collection.Title, collection.CustomTagList);
                foreach (var tag in collection.TagName)
                {
                    if (!officeTag.Contains(tag) && !CustomNewTags.Contains(tag)) CustomNewTags.Add(tag);
                }
                Collection.UpdateCollection(collection);
            }
        }
        /// <summary>
        /// 获得标签列表
        /// </summary>
        /// <param name="Title">标题</param>
        /// <param name="CustomTagList">自定义标签</param>
        /// <returns></returns>
        public static List<string> GetTagNameList(string Title, string CustomTagList)
        {
            var totalTags = getTagsFromTitle(Title);
            //这里注意，如果作者的自定义标签已经被系统收录了，但是这个自定义标签又归属于一个基础词
            //这个时候就会发生用户自定义标签统计为0的问题
            var customTags = Tag.GetCustomTag(CustomTagList);
            foreach (var customTag in customTags)
            {
                if (!totalTags.Contains(customTag))
                {
                    totalTags.Add(customTag);
                }
            }
            return totalTags;
        }

        /// <summary>
        /// 文章标签统计
        /// </summary>
        /// <param name="articles">文章</param>
        /// <param name="CustomNewTags">新标签</param>
        public static RankContain Statistics(List<Article> articles)
        {
            Dictionary<string, int> TagCntDic = new Dictionary<string, int>();
            foreach (var article in articles)
            {
                var tags = getTagsFromTitle(article.Title);
                foreach (var tag in tags)
                {
                    if (TagCntDic.ContainsKey(tag))
                    {
                        TagCntDic[tag]++;
                    }
                    else
                    {
                        TagCntDic.Add(tag, 1);
                    }
                }
            }
            return new RankContain(TagCntDic);
        }
        /// <summary>
        /// 获得两个组合标签
        /// </summary>
        /// <param name="articles"></param>
        /// <returns></returns>
        public static RankContain StatisticsTwoCombo(List<Article> articles)
        {
            Dictionary<string, int> TagCntDic = new Dictionary<string, int>();
            foreach (var article in articles)
            {
                var tags = getTagsFromTitle(article.Title);
                var twoCombo = GetTwoCombo(tags);
                foreach (var tag in twoCombo)
                {
                    if (TagCntDic.ContainsKey(tag))
                    {
                        TagCntDic[tag]++;
                    }
                    else
                    {
                        TagCntDic.Add(tag, 1);
                    }
                }
            }
            return new RankContain(TagCntDic);
        }

        /// <summary>
        /// 获得组合代码
        /// </summary>
        /// <param name="Tags"></param>
        /// <returns></returns>
        public static List<string> GetTwoCombo(List<string> Tags)
        {
            List<string> Combo = new List<string>();
            switch (Tags.Count)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    Combo.Add(Tags[0] + "|" + Tags[1]);
                    break;
                default:
                    for (int i = 0; i < Tags.Count; i++)
                    {
                        for (int j = i + 1; j < Tags.Count; j++)
                        {
                            Combo.Add(Tags[i] + "|" + Tags[j]);
                        }
                    }
                    break;
            }
            return Combo;
        }
    }
}
