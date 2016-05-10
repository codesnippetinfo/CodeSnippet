using BlogSystem.BussinessLogic;
using BlogSystem.DisplayEntity;
using BlogSystem.Entity;
using BlogSystem.TagSystem;
using InfraStructure.DataBase;
using InfraStructure.Log;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BlogDataSimulator
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        private void SetCache()
        {
            MongoDbRepository.DrapCollection(UserBody.CollectionName);
            MongoDbRepository.DrapCollection(UserItemBody.CollectionName);
            MongoDbRepository.DrapCollection(ArticleItemBody.CollectionName);
            MongoDbRepository.DrapCollection(ArticleBody.CollectionName);

            MongoDbRepository.SetCacheTime(ArticleBody.CollectionName, 15);
            MongoDbRepository.SetCacheTime(ArticleItemBody.CollectionName, 15);
            MongoDbRepository.SetCacheTime(UserBody.CollectionName, 15);
            MongoDbRepository.SetCacheTime(UserItemBody.CollectionName, 15);

            MongoDbRepository.SetIndex(UserBody.CollectionName, nameof(UserBody.GitInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(UserBody.CollectionName, nameof(UserBody.QQInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(UserItemBody.CollectionName, nameof(UserItemBody.UserInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(ArticleItemBody.CollectionName, nameof(ArticleItemBody.ArticleInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(ArticleItemBody.CollectionName, nameof(ArticleItemBody.AuthorInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(ArticleBody.CollectionName, nameof(ArticleBody.ArticleInfo) + "." + MongoDbRepository.MongoKeyField);
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        private void SetIndex()
        {
            MongoDbRepository.SetIndex(Article.CollectionName, nameof(Article.OwnerId));
            MongoDbRepository.SetIndex(Collection.CollectionName, nameof(Collection.OwnerId));
            MongoDbRepository.SetIndex(Stock.CollectionName, nameof(Stock.OwnerId));
            MongoDbRepository.SetIndex(Stock.CollectionName, nameof(Stock.ArticleID));
            MongoDbRepository.SetIndex(Stock.CollectionName, nameof(Stock.AuthorID));
            MongoDbRepository.SetIndex(BlogSystem.Entity.Focus.CollectionName, nameof(BlogSystem.Entity.Focus.OwnerId));
            MongoDbRepository.SetIndex(BlogSystem.Entity.Focus.CollectionName, nameof(BlogSystem.Entity.Focus.AccountID));
            MongoDbRepository.SetIndex(Comment.CollectionName, nameof(Comment.ArticleID));
            MongoDbRepository.SetIndex(Comment.CollectionName, nameof(Comment.OwnerId));
            MongoDbRepository.SetIndex(Visitor.CollectionName, nameof(Visitor.ArticleID));
            MongoDbRepository.SetIndex(Topic.CollectionName, nameof(Topic.OwnerId));
            MongoDbRepository.SetIndex(TopicArticle.CollectionName, nameof(TopicArticle.TopicID));
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            chkArticle.Checked = true;
            chkStock.Checked = true;
            chkFocus.Checked = true;
            chkComment.Checked = true;
            chkTopic.Checked = true;
            chkVisitor.Checked = true;
        }
        private void btnSimulate_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                if (MessageBox.Show("NEST服务器没有链接，是否继续", "警告", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;
            }

            //数据库准备
            MongoDbRepository.Init(new[] { "Bussiness" }, "Bussiness", NumPortDataBase.Value.ToString());

            SetCache();
            SetIndex();

            GenerateCnblogsDate.titles.Clear();
            GenerateCnblogsDate.userColdic.Clear();
            GenerateCnblogsDate.userdic.Clear();
            MongoDbRepository.DrapCollection(SiteMessage.CollectionName);

            btnSimulate.Enabled = false;
            if (chkArticle.Checked)
            {
                var strMDContent = new StreamReader(txtMDContent.Text).ReadToEnd();
                var strHTMLContent = new StreamReader(txtHTMLContent.Text).ReadToEnd();
                GenerateCnblogsDate.InsertCnblogs(txtCnblogFilename.Text, strMDContent, strHTMLContent, (int)NumArticleCount.Value, client);
            }
            if (chkStock.Checked) GenerateCnblogsDate.SimulateStock(5, 50);
            if (chkFocus.Checked) GenerateCnblogsDate.SimulateFocus(5, 50);
            if (chkComment.Checked) GenerateCnblogsDate.SimulateComment(txtCommentMarkDown.Text, txtCommentHTML.Text, 5, 20);
            if (chkTopic.Checked) GenerateCnblogsDate.SimulateTopic();
            if (chkVisitor.Checked) GenerateCnblogsDate.SimulateVisitor();
            btnSimulate.Enabled = true;
            MessageBox.Show("操作完成！");
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCnblogFilename_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtCnblogFilename.Text = fd.FileName;
            }
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMDContent_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtMDContent.Text = fd.FileName;
            }
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHTMLContent_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtHTMLContent.Text = fd.FileName;
            }
        }
        /// <summary>
        /// NEST Client
        /// </summary>
        static ElasticClient client = null;
        /// <summary>
        /// Index Name
        /// </summary>
        static string indexname = "Artical".ToLower();
        /// <summary>
        /// Nest Init
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNest_Click(object sender, EventArgs e)
        {
            var node = new Uri("http://localhost:9200/");
            var settings = new ConnectionSettings(node);
            //必须是小写
            settings.DefaultIndex(indexname);
            client = new ElasticClient(settings);
            client.DeleteIndex(indexname);
            var res = client.CreateIndex(indexname);
            MessageBox.Show("操作完成！");
        }
        /// <summary>
        /// 重新索引
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReIndex_Click(object sender, EventArgs e)
        {
            SearchManager.Init();
            SearchManager.ReIndex();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyWord = "测试";
            var searchResults = client.Search<Article>(s => s
                .Index(indexname).Query(q => q.QueryString(qs => qs.Query(keyWord).DefaultOperator(Operator.And)))
            );
        }
        /// <summary>
        /// 模拟获得Token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnlyze_Click(object sender, EventArgs e)
        {
            SearchManager.Init();
            var x = SearchManager.GetTokenList(txtSentence.Text);
            if (x == null)
            {
                MessageBox.Show("Error!!!");
            }
            else
            {
                MessageBox.Show(string.Join(",", x.ToArray()));
            }
        }
        /// <summary>
        /// 新旧两种标签结果的对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompareTagResult_Click(object sender, EventArgs e)
        {
            SearchManager.Init();
            MongoDbRepository.Init(new[] { "Bussiness" }, "Bussiness");
            TagUtility.Tags = BlogSystem.TagSystem.Tag.GetAllTags();
            var writer = new StreamWriter("TagCompare.txt", false);
            var articles = MongoDbRepository.GetRecList<Article>(ArticleListManager.FirstPageArticleQuery);
            foreach (var item in articles)
            {
                var TRTag = TagUtility.getTagsFromTitleOld(item.Title);
                var ELTag = TagUtility.getTagsFromTitle(item.Title);
                bool isSame = true;
                if (TRTag.Count == ELTag.Count)
                {
                    for (int i = 0; i < TRTag.Count; i++)
                    {
                        if (!TRTag[i].Equals(ELTag[i]))
                        {
                            isSame = false;
                            break;
                        }
                    }
                }
                else
                {
                    isSame = false;
                }
                if (!isSame)
                {
                    if (TRTag.Contains("View")) continue;
                    if (TRTag.Contains("Firefox")) continue;
                    if (TRTag.Contains("ASP.NET")) continue;
                    if (TRTag.Contains("HTML5")) continue;
                    if (TRTag.Contains("Unity3D")) continue;
                    writer.Write(item.Title + "`");
                    writer.Write(string.Join(",", TRTag.ToArray()) + "`");
                    writer.WriteLine(string.Join(",", ELTag.ToArray()) + "");
                }
            }
            writer.Close();
        }
        /// <summary>
        /// 获得最热门的词语
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetTitleToken_Click(object sender, EventArgs e)
        {
            SearchManager.Init();
            MongoDbRepository.Init(new[] { "Bussiness" }, "Bussiness");
            var articles = MongoDbRepository.GetRecList<Article>(ArticleListManager.FirstPageArticleQuery);
            var TokenCnt = new Dictionary<string, int>();
            foreach (var item in articles)
            {
                var x = SearchManager.GetTokenList(item.Title);
                if (x != null)
                {
                    foreach (var token in x)
                    {
                        if (TokenCnt.ContainsKey(token))
                        {
                            TokenCnt[token]++;
                        }
                        else
                        {
                            TokenCnt.Add(token, 1);
                        }
                    }
                }
            }
            var writer = new StreamWriter("TokenCnt.txt", false);
            foreach (var item in TokenCnt)
            {
                writer.WriteLine(item.Key + "`" + item.Value);
            }
            writer.Close();
        }

        private void btnGetBackUpFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtBackUpFolder.Text = fd.SelectedPath;
            }
        }
        /// <summary>
        /// BackUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackFileStroage_Click(object sender, EventArgs e)
        {
            Misc.BackFile("Image", txtBackUpFolder.Text);
        }
        /// <summary>
        /// OAuth
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOAuth_Click(object sender, EventArgs e)
        {
            MongoDbRepositoryLog.Init("28030");
            GithubAccount.AppName = "codesnippet";
            GithubAccount.ClientID = "01a8bf26baecfa8db577";
            GithubAccount.ClientSecret = "79137611b498051330a1695248b572b18f5983e0";

            //var userInfo = GithubAccount.GetUserInfo(txtCode.Text);
            //if (userInfo == null)
            //{
            //    System.Diagnostics.Debug.WriteLine("Ooo My LadyGaga!!!");
            //}
        }

        private void btnMarkDownFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtMarkDownFile.Text = fd.FileName;
            }
        }
        /// <summary>
        /// 分析MD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnlyzeMarkDown_Click(object sender, EventArgs e)
        {
            MongoDbRepository.Init(new[] { "Bussiness" }, "Bussiness");
            MongoDbRepositoryLog.Init();
            var r = new StreamReader(txtMarkDownFile.Text);
            var md = r.ReadToEnd();
            var result = MarkDownAnlyzer.Anlyze(md);
        }
        /// <summary>
        /// 生成PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPdfCreator_Click(object sender, EventArgs e)
        {
            foreach (var file in new DirectoryInfo(@"C:\Users\scs\Desktop\DD").GetFiles())
            {
                System.Diagnostics.Debug.Print(file.Name);
                foreach (var c in file.Name.ToCharArray())
                {
                    System.Diagnostics.Debug.Print(c.ToString() + ":" + (int)c);
                    var cx1 = (char)160;
                    var cx2 = (char)32;
                }
                break;
            }
            return;

            //callback( { "client_id":"YOUR_APPID","openid":"YOUR_OPENID"} );
            var res = "callback( { \"client_id\":\"YOUR_APPID\",\"openid\":\"YOUR_OPENID\"} );";
            string json = res.Substring(res.IndexOf("(") + 1);
            json = json.Substring(0, json.IndexOf(")"));
            dynamic obj = JsonConvert.DeserializeObject(json);
            string openid = obj.openid;
            string articleId = txtArticleIdForPDF.Text;
            articleId = "00000575";
            Misc.ConvertUrlToPdf("http://localhost:60907/Article/SimplePdf?ArticleId=" + articleId, articleId,true);
        }
    }
}
