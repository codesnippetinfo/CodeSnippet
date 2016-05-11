using BlogSystem.BussinessLogic;
using BlogSystem.DisplayEntity;
using BlogSystem.Entity;
using BlogSystem.TagSystem;
using InfraStructure.DataBase;
using InfraStructure.Log;
using InfraStructure.Utility;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CodeSnippet.Controllers
{
    public class AdminController : Controller
    {

        /// <summary>
        /// 首页审核
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            if ((UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin &&
                (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Editor) return Redirect("/");
            ViewBag.Title = "首页审核";
            ViewData.Model = Article.GetPendingArticleList();
            return View();
        }
        
        /// <summary>
        /// 文章察看
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult ViewArticle(string ArticleId)
        {
            if (string.IsNullOrEmpty(ArticleId)) return Redirect("/");
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            if ((UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin &&
                (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Editor) return Redirect("/");
            ViewData.Model = ArticleListManager.GetArticleBodyById(ArticleId);
            return View();
        }

        /// <summary>
        /// 接受文章首页申请
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult Accept(string ArticleId)
        {
            if (string.IsNullOrEmpty(ArticleId)) return Redirect("/");
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            if ((UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin &&
                (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Editor) return Redirect("/");

            if (Article.GetArticleBySn(ArticleId) == null) return Redirect("/");
            Article.Accept(ArticleId);
            Article article = Article.GetArticleBySn(ArticleId);
            var articleurl = "<a href = '/Article/Index?ArticleId=" + article.Sn + "'>" + article.Title + "</a>";
            SiteMessage.CreateNotify(article.OwnerId, "您的文章[" + articleurl + "]通过审核");
            if (article.IsPutToMyTopic)
            {
                //发布后则加入到自己专题
                var topic = Topic.GetTopicByAccountId(article.OwnerId);
                if (topic != null)
                {
                    TopicArticle.InsertTopicArticle(new TopicArticle()
                    {
                        ArticleID = article.Sn,
                        TopicID = topic.Sn,
                        PublishStatus = ApproveStatus.NotNeed
                    });
                }
            }
            return Redirect("/Admin");
        }

        /// <summary>
        /// 拒绝文章首页申请
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult Reject()
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            if ((UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin &&
                (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Editor) return Redirect("/");

            string strMessage = Request.Form["message"];
            string ArticleId = Request.Form["ArticleID"];
            if (string.IsNullOrEmpty(ArticleId)) return Redirect("/");
            if (Article.GetArticleBySn(ArticleId) == null) return Redirect("/");
            if (string.IsNullOrEmpty(strMessage)) return Redirect("/");
            Article.Reject(ArticleId, strMessage);
            Article article = Article.GetArticleBySn(ArticleId);
            var articleurl = "<a href = '/PostEdit/MarkDownEditor?ArticleId=" + article.Sn + "'>" + article.Title + "</a>";
            SiteMessage.CreateNotify(article.OwnerId, "您的文章[" + articleurl + "]没有通过审核，理由：" + strMessage);
            return Redirect("/Admin");
        }

        /// <summary>
        /// 所有文章一览（公开的）
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public ActionResult ArticleList(int PageNo = 1)
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            ViewBag.Title = "文章管理";
            //公开的审核通过的所有文章
            Pages p = new Pages(ArticleListManager.GetArticleCnt(ArticleListManager.PublicArticleQueryFilter), 50);
            p.CurrentPageNo = PageNo;
            var currentpageList = ArticleListManager.GetPublicListForArticleByPage(p, ArticleListManager.PublicArticleQueryFilter);
            ViewData.Model = currentpageList;
            ViewBag.Pages = p;
            return View();
        }
        /// <summary>
        /// 作者一览（普通和特约）
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public ActionResult AuthorList(int PageNo = 1)
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            ViewBag.Title = "用户管理";
            Pages p = new Pages(UserInfo.GetNoBlockUserCnt(), 50);
            p.CurrentPageNo = PageNo;
            var currentpageList = UserInfo.GetNoBlockUserInfo(p);
            var UserItemBodyList = new List<UserItemBody>();
            foreach (var item in currentpageList)
            {
                UserItemBodyList.Add(UserManager.GetUserItemBody(item.Sn));
            }
            ViewData.Model = UserItemBodyList;
            ViewBag.Pages = p;
            return View();
        }
        /// <summary>
        /// 改变权限
        /// </summary>
        /// <param name="Group"></param>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public ActionResult Privilege(string Group, string AccountId)
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            if (Group.Equals(UserType.Author.ToString())) UserInfo.ChangePrivilege(AccountId, UserType.Author);
            if (Group.Equals(UserType.Block.ToString())) UserInfo.ChangePrivilege(AccountId, UserType.Block);
            if (Group.Equals(UserType.Editor.ToString())) UserInfo.ChangePrivilege(AccountId, UserType.Editor);
            UserManager.RemoveUserItemBody(AccountId);
            return Redirect("/Admin/AuthorList");
        }

        /// <summary>
        /// 设定首页推荐文章
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult SetTopArticle(string ArticleId)
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            if (string.IsNullOrEmpty(ArticleId)) return Redirect("/");
            ViewBag.Title = "设定首页推荐文章";
            ArticleListManager.SetTopArticle(ArticleId);
            var config = SiteConfig.GetSiteConfigBySn("00000001");
            if (config != null)
            {
                config.TopArticleID = ArticleId;
                SiteConfig.UpdateSiteConfig(config);
            }
            return Redirect("/");
        }
        /// <summary>
        /// 移出首页
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult RemoveFromFirstPage(string ArticleId)
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            if (string.IsNullOrEmpty(ArticleId)) return Redirect("/");
            Article.Reject(ArticleId, "移出首页");
            Article article = Article.GetArticleBySn(ArticleId);
            var articleurl = "<a href = '/PostEdit/MarkDownEditor?ArticleId=" + article.Sn + "'>" + article.Title + "</a>";
            SiteMessage.CreateNotify(article.OwnerId, "您的文章[" + articleurl + "]被移出首页");
            return Redirect("/Admin/ArticleList?PageNo=1");
        }
        /// <summary>
        /// 导入标签
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportTag()
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            ViewData.Model = Tag.GetAllTags();
            ViewBag.CustomTag = TagUtility.CustomNewTags;
            if (Request.Files.Count == 1 && Request.Files[0].ContentLength > 0)
            {
                ReloadTagListFromExcel(Request.Files[0]);
                TagUtility.Init();
                ViewData.Model = Tag.GetAllTags();
                ViewBag.CustomTag = TagUtility.CustomNewTags;
            }
            ViewBag.Title = "导入标签";
            return View();
        }

        /// <summary>
        /// 重新建立索引
        /// </summary>
        /// <returns></returns>
        public ActionResult ReIndexTextSearch()
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            SearchManager.ReIndex();
            return Redirect("/");
        }

        /// <summary>
        /// 刷新所有首页文章
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshAllFirstPagePDF()
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            FileSystemController.RefreshAllPDF();
            return Redirect("/");
        }

        /// <summary>
        /// Tag文件名称
        /// </summary>
        public static string TagFilename = string.Empty;

        /// <summary>
        /// 从Excel文件重新加载标签列表
        /// </summary>
        /// <param name="file">上传的Excel文件</param>
        private static void ReloadTagListFromExcel(HttpPostedFileBase file)
        {
            var excelFileStream = file.InputStream;
            file.SaveAs(TagFilename);
            //下面的代码会读取完流，所以先保存为文件
            InsertExcelTagInfo(new FileStream(TagFilename, FileMode.Open));
        }

        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <param name="excelFileStream"></param>
        public static void InsertExcelTagInfo(Stream excelFileStream)
        {
            //正常的流程是先插入再删除，现在暂时不这样考虑
            MongoDbRepository.DeleteAllRecPhysical(Tag.CollectionName);
            IWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(0);
            int CurrentRowIndex = 3;
            //这里表示第一个可用格子的下标？？？
            int StartColIndex = 3;
            IRow row = sheet.GetRow(CurrentRowIndex);
            while (row != null && !string.IsNullOrEmpty(row.GetCell(StartColIndex).StringCellValue))
            {
                Tag t = new Tag();
                t.TagName = row.GetCell(StartColIndex).StringCellValue;
                StartColIndex += 5;
                t.Catalog = NpoiExtend.GetCellText(row.GetCell(StartColIndex));
                StartColIndex += 5;
                t.IsCaseSensitive = !string.IsNullOrEmpty(NpoiExtend.GetCellText(row.GetCell(StartColIndex)));
                StartColIndex += 5;
                t.BaseTagName = NpoiExtend.GetCellText(row.GetCell(StartColIndex));
                StartColIndex += 5;
                t.Comment = NpoiExtend.GetCellText(row.GetCell(StartColIndex));
                StartColIndex += 5;
                t.IsOnlyContain = !string.IsNullOrEmpty(NpoiExtend.GetCellText(row.GetCell(StartColIndex)));
                MongoDbRepository.InsertRec(t);
                CurrentRowIndex++;
                StartColIndex = 3;
                row = sheet.GetRow(CurrentRowIndex);
            }
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        /// <returns></returns>
        public ActionResult ExceptionLogIndex()
        {
            if (Session[ConstHelper.Session_USERID] == null || (UserType)Session[ConstHelper.Session_PRIVILEGE] != UserType.Admin) return Redirect("/");
            ViewBag.Title = "异常日志";
            ViewData.Model = ExceptionLog.GetLog();
            return View();
        }
    }
}