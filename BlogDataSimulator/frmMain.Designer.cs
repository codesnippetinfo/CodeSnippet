namespace BlogDataSimulator
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnSimulate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NumPortDataBase = new System.Windows.Forms.NumericUpDown();
            this.txtCnblogFilename = new System.Windows.Forms.TextBox();
            this.Cnblogs = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMDContent = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.chkVisitor = new System.Windows.Forms.CheckBox();
            this.chkArticle = new System.Windows.Forms.CheckBox();
            this.chkFocus = new System.Windows.Forms.CheckBox();
            this.chkStock = new System.Windows.Forms.CheckBox();
            this.chkTopic = new System.Windows.Forms.CheckBox();
            this.chkComment = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCommentMarkDown = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.NumArticleCount = new System.Windows.Forms.NumericUpDown();
            this.btnCnblogFilename = new System.Windows.Forms.Button();
            this.btnMDContent = new System.Windows.Forms.Button();
            this.btnNest = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCommentHTML = new System.Windows.Forms.TextBox();
            this.txtHTMLContent = new System.Windows.Forms.TextBox();
            this.btnHTMLContent = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSentence = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnAnlyze = new System.Windows.Forms.Button();
            this.btnCompareTagResult = new System.Windows.Forms.Button();
            this.btnGetTitleToken = new System.Windows.Forms.Button();
            this.btnReIndex = new System.Windows.Forms.Button();
            this.NumPortFileStroage = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.btnBackFileStroage = new System.Windows.Forms.Button();
            this.txtBackUpFolder = new System.Windows.Forms.TextBox();
            this.btnGetBackUpFolder = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnOAuth = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnMarkDownFile = new System.Windows.Forms.Button();
            this.txtMarkDownFile = new System.Windows.Forms.TextBox();
            this.btnAnlyzeMarkDown = new System.Windows.Forms.Button();
            this.btnPdfCreator = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtArticleIdForPDF = new System.Windows.Forms.TextBox();
            this.chkIsArticleRandom = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumPortDataBase)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumArticleCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumPortFileStroage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSimulate
            // 
            this.btnSimulate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnSimulate.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSimulate.Location = new System.Drawing.Point(219, 355);
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.Size = new System.Drawing.Size(100, 36);
            this.btnSimulate.TabIndex = 0;
            this.btnSimulate.Text = "模拟数据";
            this.btnSimulate.UseVisualStyleBackColor = false;
            this.btnSimulate.Click += new System.EventHandler(this.btnSimulate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(210, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "数据库端口";
            // 
            // NumPortDataBase
            // 
            this.NumPortDataBase.Location = new System.Drawing.Point(286, 77);
            this.NumPortDataBase.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NumPortDataBase.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NumPortDataBase.Name = "NumPortDataBase";
            this.NumPortDataBase.Size = new System.Drawing.Size(71, 21);
            this.NumPortDataBase.TabIndex = 2;
            this.NumPortDataBase.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumPortDataBase.Value = new decimal(new int[] {
            28030,
            0,
            0,
            0});
            // 
            // txtCnblogFilename
            // 
            this.txtCnblogFilename.Location = new System.Drawing.Point(123, 107);
            this.txtCnblogFilename.Name = "txtCnblogFilename";
            this.txtCnblogFilename.Size = new System.Drawing.Size(412, 21);
            this.txtCnblogFilename.TabIndex = 3;
            this.txtCnblogFilename.Text = "E:\\WorkSpace\\CodeSnippet\\BlogDataSimulator\\Sample\\ArticleList.txt";
            // 
            // Cnblogs
            // 
            this.Cnblogs.AutoSize = true;
            this.Cnblogs.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Cnblogs.Location = new System.Drawing.Point(17, 110);
            this.Cnblogs.Name = "Cnblogs";
            this.Cnblogs.Size = new System.Drawing.Size(96, 12);
            this.Cnblogs.TabIndex = 4;
            this.Cnblogs.Text = "文章和标题列表";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(56, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "文章内容";
            // 
            // txtMDContent
            // 
            this.txtMDContent.Location = new System.Drawing.Point(123, 135);
            this.txtMDContent.Name = "txtMDContent";
            this.txtMDContent.Size = new System.Drawing.Size(412, 21);
            this.txtMDContent.TabIndex = 3;
            this.txtMDContent.Text = "E:\\WorkSpace\\CodeSnippet\\BlogDataSimulator\\Sample\\Markdown.txt";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSelectAll);
            this.groupBox1.Controls.Add(this.chkVisitor);
            this.groupBox1.Controls.Add(this.chkArticle);
            this.groupBox1.Controls.Add(this.chkFocus);
            this.groupBox1.Controls.Add(this.chkStock);
            this.groupBox1.Controls.Add(this.chkTopic);
            this.groupBox1.Controls.Add(this.chkComment);
            this.groupBox1.Location = new System.Drawing.Point(128, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(501, 48);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "模拟项";
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnSelectAll.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelectAll.Location = new System.Drawing.Point(412, 16);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 19;
            this.btnSelectAll.Text = "全选";
            this.btnSelectAll.UseVisualStyleBackColor = false;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // chkVisitor
            // 
            this.chkVisitor.AutoSize = true;
            this.chkVisitor.Location = new System.Drawing.Point(282, 20);
            this.chkVisitor.Name = "chkVisitor";
            this.chkVisitor.Size = new System.Drawing.Size(48, 16);
            this.chkVisitor.TabIndex = 10;
            this.chkVisitor.Text = "访问";
            this.chkVisitor.UseVisualStyleBackColor = true;
            // 
            // chkArticle
            // 
            this.chkArticle.AutoSize = true;
            this.chkArticle.Location = new System.Drawing.Point(11, 20);
            this.chkArticle.Name = "chkArticle";
            this.chkArticle.Size = new System.Drawing.Size(48, 16);
            this.chkArticle.TabIndex = 6;
            this.chkArticle.Text = "文章";
            this.chkArticle.UseVisualStyleBackColor = true;
            // 
            // chkFocus
            // 
            this.chkFocus.AutoSize = true;
            this.chkFocus.Location = new System.Drawing.Point(120, 20);
            this.chkFocus.Name = "chkFocus";
            this.chkFocus.Size = new System.Drawing.Size(48, 16);
            this.chkFocus.TabIndex = 7;
            this.chkFocus.Text = "关注";
            this.chkFocus.UseVisualStyleBackColor = true;
            // 
            // chkStock
            // 
            this.chkStock.AutoSize = true;
            this.chkStock.Location = new System.Drawing.Point(66, 20);
            this.chkStock.Name = "chkStock";
            this.chkStock.Size = new System.Drawing.Size(48, 16);
            this.chkStock.TabIndex = 8;
            this.chkStock.Text = "收藏";
            this.chkStock.UseVisualStyleBackColor = true;
            // 
            // chkTopic
            // 
            this.chkTopic.AutoSize = true;
            this.chkTopic.Location = new System.Drawing.Point(228, 20);
            this.chkTopic.Name = "chkTopic";
            this.chkTopic.Size = new System.Drawing.Size(48, 16);
            this.chkTopic.TabIndex = 9;
            this.chkTopic.Text = "专题";
            this.chkTopic.UseVisualStyleBackColor = true;
            // 
            // chkComment
            // 
            this.chkComment.AutoSize = true;
            this.chkComment.Location = new System.Drawing.Point(174, 20);
            this.chkComment.Name = "chkComment";
            this.chkComment.Size = new System.Drawing.Size(48, 16);
            this.chkComment.TabIndex = 9;
            this.chkComment.Text = "评论";
            this.chkComment.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(56, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "评论内容";
            // 
            // txtCommentMarkDown
            // 
            this.txtCommentMarkDown.Location = new System.Drawing.Point(123, 209);
            this.txtCommentMarkDown.Multiline = true;
            this.txtCommentMarkDown.Name = "txtCommentMarkDown";
            this.txtCommentMarkDown.Size = new System.Drawing.Size(506, 64);
            this.txtCommentMarkDown.TabIndex = 6;
            this.txtCommentMarkDown.Text = "Welcome To CodeInfo\r\n```csharp\r\nif (Session[ConstHelper.Session_USERID] == null) " +
    "return Redirect(\"/Home/Index\");\r\n```";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(71, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "文章数";
            // 
            // NumArticleCount
            // 
            this.NumArticleCount.Location = new System.Drawing.Point(126, 77);
            this.NumArticleCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NumArticleCount.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumArticleCount.Name = "NumArticleCount";
            this.NumArticleCount.Size = new System.Drawing.Size(71, 21);
            this.NumArticleCount.TabIndex = 7;
            this.NumArticleCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumArticleCount.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // btnCnblogFilename
            // 
            this.btnCnblogFilename.Location = new System.Drawing.Point(541, 109);
            this.btnCnblogFilename.Name = "btnCnblogFilename";
            this.btnCnblogFilename.Size = new System.Drawing.Size(75, 23);
            this.btnCnblogFilename.TabIndex = 8;
            this.btnCnblogFilename.Text = "浏览...";
            this.btnCnblogFilename.UseVisualStyleBackColor = true;
            this.btnCnblogFilename.Click += new System.EventHandler(this.btnCnblogFilename_Click);
            // 
            // btnMDContent
            // 
            this.btnMDContent.Location = new System.Drawing.Point(541, 135);
            this.btnMDContent.Name = "btnMDContent";
            this.btnMDContent.Size = new System.Drawing.Size(75, 23);
            this.btnMDContent.TabIndex = 9;
            this.btnMDContent.Text = "浏览...";
            this.btnMDContent.UseVisualStyleBackColor = true;
            this.btnMDContent.Click += new System.EventHandler(this.btnMDContent_Click);
            // 
            // btnNest
            // 
            this.btnNest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnNest.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnNest.Location = new System.Drawing.Point(123, 355);
            this.btnNest.Name = "btnNest";
            this.btnNest.Size = new System.Drawing.Size(90, 36);
            this.btnNest.TabIndex = 10;
            this.btnNest.Text = "初始化NEST";
            this.btnNest.UseVisualStyleBackColor = false;
            this.btnNest.Click += new System.EventHandler(this.btnNest_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(58, 290);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "评论内容";
            // 
            // txtCommentHTML
            // 
            this.txtCommentHTML.Location = new System.Drawing.Point(123, 290);
            this.txtCommentHTML.Multiline = true;
            this.txtCommentHTML.Name = "txtCommentHTML";
            this.txtCommentHTML.Size = new System.Drawing.Size(506, 59);
            this.txtCommentHTML.TabIndex = 11;
            this.txtCommentHTML.Text = resources.GetString("txtCommentHTML.Text");
            // 
            // txtHTMLContent
            // 
            this.txtHTMLContent.Location = new System.Drawing.Point(123, 162);
            this.txtHTMLContent.Name = "txtHTMLContent";
            this.txtHTMLContent.Size = new System.Drawing.Size(411, 21);
            this.txtHTMLContent.TabIndex = 12;
            this.txtHTMLContent.Text = "E:\\WorkSpace\\CodeSnippet\\BlogDataSimulator\\Sample\\Html.txt";
            // 
            // btnHTMLContent
            // 
            this.btnHTMLContent.Location = new System.Drawing.Point(540, 160);
            this.btnHTMLContent.Name = "btnHTMLContent";
            this.btnHTMLContent.Size = new System.Drawing.Size(75, 23);
            this.btnHTMLContent.TabIndex = 9;
            this.btnHTMLContent.Text = "浏览...";
            this.btnHTMLContent.UseVisualStyleBackColor = true;
            this.btnHTMLContent.Click += new System.EventHandler(this.btnHTMLContent_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(60, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "文章内容";
            // 
            // txtSentence
            // 
            this.txtSentence.Location = new System.Drawing.Point(713, 64);
            this.txtSentence.Name = "txtSentence";
            this.txtSentence.Size = new System.Drawing.Size(389, 21);
            this.txtSentence.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(669, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "词语";
            // 
            // btnAnlyze
            // 
            this.btnAnlyze.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnAnlyze.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAnlyze.Location = new System.Drawing.Point(1108, 62);
            this.btnAnlyze.Name = "btnAnlyze";
            this.btnAnlyze.Size = new System.Drawing.Size(97, 23);
            this.btnAnlyze.TabIndex = 15;
            this.btnAnlyze.Text = "分析Token";
            this.btnAnlyze.UseVisualStyleBackColor = false;
            this.btnAnlyze.Click += new System.EventHandler(this.btnAnlyze_Click);
            // 
            // btnCompareTagResult
            // 
            this.btnCompareTagResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnCompareTagResult.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCompareTagResult.Location = new System.Drawing.Point(713, 91);
            this.btnCompareTagResult.Name = "btnCompareTagResult";
            this.btnCompareTagResult.Size = new System.Drawing.Size(224, 41);
            this.btnCompareTagResult.TabIndex = 16;
            this.btnCompareTagResult.Text = "对比标签结果";
            this.btnCompareTagResult.UseVisualStyleBackColor = false;
            this.btnCompareTagResult.Click += new System.EventHandler(this.btnCompareTagResult_Click);
            // 
            // btnGetTitleToken
            // 
            this.btnGetTitleToken.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnGetTitleToken.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGetTitleToken.Location = new System.Drawing.Point(961, 91);
            this.btnGetTitleToken.Name = "btnGetTitleToken";
            this.btnGetTitleToken.Size = new System.Drawing.Size(245, 41);
            this.btnGetTitleToken.TabIndex = 17;
            this.btnGetTitleToken.Text = "获得标题热门词汇";
            this.btnGetTitleToken.UseVisualStyleBackColor = false;
            this.btnGetTitleToken.Click += new System.EventHandler(this.btnGetTitleToken_Click);
            // 
            // btnReIndex
            // 
            this.btnReIndex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnReIndex.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReIndex.Location = new System.Drawing.Point(327, 355);
            this.btnReIndex.Name = "btnReIndex";
            this.btnReIndex.Size = new System.Drawing.Size(145, 36);
            this.btnReIndex.TabIndex = 18;
            this.btnReIndex.Text = "重新索引 ElasticSearch";
            this.btnReIndex.UseVisualStyleBackColor = false;
            this.btnReIndex.Click += new System.EventHandler(this.btnReIndex_Click);
            // 
            // NumPortFileStroage
            // 
            this.NumPortFileStroage.Location = new System.Drawing.Point(790, 190);
            this.NumPortFileStroage.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NumPortFileStroage.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NumPortFileStroage.Name = "NumPortFileStroage";
            this.NumPortFileStroage.Size = new System.Drawing.Size(71, 21);
            this.NumPortFileStroage.TabIndex = 20;
            this.NumPortFileStroage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumPortFileStroage.Value = new decimal(new int[] {
            28031,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(719, 194);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "文件端口";
            // 
            // btnBackFileStroage
            // 
            this.btnBackFileStroage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnBackFileStroage.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBackFileStroage.Location = new System.Drawing.Point(713, 219);
            this.btnBackFileStroage.Name = "btnBackFileStroage";
            this.btnBackFileStroage.Size = new System.Drawing.Size(488, 55);
            this.btnBackFileStroage.TabIndex = 21;
            this.btnBackFileStroage.Text = "备份文件";
            this.btnBackFileStroage.UseVisualStyleBackColor = false;
            this.btnBackFileStroage.Click += new System.EventHandler(this.btnBackFileStroage_Click);
            // 
            // txtBackUpFolder
            // 
            this.txtBackUpFolder.Location = new System.Drawing.Point(790, 163);
            this.txtBackUpFolder.Name = "txtBackUpFolder";
            this.txtBackUpFolder.Size = new System.Drawing.Size(326, 21);
            this.txtBackUpFolder.TabIndex = 24;
            this.txtBackUpFolder.Text = "E:\\WorkSpace\\Domino\\BlogDataSimulator\\Sample\\Html.txt";
            // 
            // btnGetBackUpFolder
            // 
            this.btnGetBackUpFolder.Location = new System.Drawing.Point(1137, 163);
            this.btnGetBackUpFolder.Name = "btnGetBackUpFolder";
            this.btnGetBackUpFolder.Size = new System.Drawing.Size(75, 23);
            this.btnGetBackUpFolder.TabIndex = 23;
            this.btnGetBackUpFolder.Text = "浏览...";
            this.btnGetBackUpFolder.UseVisualStyleBackColor = true;
            this.btnGetBackUpFolder.Click += new System.EventHandler(this.btnGetBackUpFolder_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(719, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "文章内容";
            // 
            // btnOAuth
            // 
            this.btnOAuth.Location = new System.Drawing.Point(966, 28);
            this.btnOAuth.Name = "btnOAuth";
            this.btnOAuth.Size = new System.Drawing.Size(75, 23);
            this.btnOAuth.TabIndex = 25;
            this.btnOAuth.Text = "OAuth";
            this.btnOAuth.UseVisualStyleBackColor = true;
            this.btnOAuth.Click += new System.EventHandler(this.btnOAuth_Click);
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(737, 28);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(213, 21);
            this.txtCode.TabIndex = 26;
            this.txtCode.Text = "无法跨域";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(668, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 12);
            this.label10.TabIndex = 27;
            this.label10.Text = "Code:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(49, 487);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 12);
            this.label11.TabIndex = 28;
            this.label11.Text = "分析MD文件";
            // 
            // btnMarkDownFile
            // 
            this.btnMarkDownFile.Location = new System.Drawing.Point(478, 476);
            this.btnMarkDownFile.Name = "btnMarkDownFile";
            this.btnMarkDownFile.Size = new System.Drawing.Size(75, 23);
            this.btnMarkDownFile.TabIndex = 9;
            this.btnMarkDownFile.Text = "浏览...";
            this.btnMarkDownFile.UseVisualStyleBackColor = true;
            this.btnMarkDownFile.Click += new System.EventHandler(this.btnMarkDownFile_Click);
            // 
            // txtMarkDownFile
            // 
            this.txtMarkDownFile.Location = new System.Drawing.Point(124, 478);
            this.txtMarkDownFile.Name = "txtMarkDownFile";
            this.txtMarkDownFile.Size = new System.Drawing.Size(348, 21);
            this.txtMarkDownFile.TabIndex = 12;
            this.txtMarkDownFile.Text = "E:\\WorkSpace\\CodeSnippet\\BlogDataSimulator\\Sample\\MarkDownForAnlyze.txt";
            // 
            // btnAnlyzeMarkDown
            // 
            this.btnAnlyzeMarkDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnAnlyzeMarkDown.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAnlyzeMarkDown.Location = new System.Drawing.Point(571, 476);
            this.btnAnlyzeMarkDown.Name = "btnAnlyzeMarkDown";
            this.btnAnlyzeMarkDown.Size = new System.Drawing.Size(75, 23);
            this.btnAnlyzeMarkDown.TabIndex = 29;
            this.btnAnlyzeMarkDown.Text = "分析";
            this.btnAnlyzeMarkDown.UseVisualStyleBackColor = false;
            this.btnAnlyzeMarkDown.Click += new System.EventHandler(this.btnAnlyzeMarkDown_Click);
            // 
            // btnPdfCreator
            // 
            this.btnPdfCreator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnPdfCreator.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPdfCreator.Location = new System.Drawing.Point(327, 411);
            this.btnPdfCreator.Name = "btnPdfCreator";
            this.btnPdfCreator.Size = new System.Drawing.Size(145, 28);
            this.btnPdfCreator.TabIndex = 18;
            this.btnPdfCreator.Text = "生成PDF";
            this.btnPdfCreator.UseVisualStyleBackColor = false;
            this.btnPdfCreator.Click += new System.EventHandler(this.btnPdfCreator_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(17, 416);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(103, 12);
            this.label12.TabIndex = 30;
            this.label12.Text = "文章编号（8位）";
            // 
            // txtArticleIdForPDF
            // 
            this.txtArticleIdForPDF.Location = new System.Drawing.Point(123, 416);
            this.txtArticleIdForPDF.Name = "txtArticleIdForPDF";
            this.txtArticleIdForPDF.Size = new System.Drawing.Size(196, 21);
            this.txtArticleIdForPDF.TabIndex = 31;
            // 
            // chkIsArticleRandom
            // 
            this.chkIsArticleRandom.AutoSize = true;
            this.chkIsArticleRandom.Location = new System.Drawing.Point(376, 79);
            this.chkIsArticleRandom.Name = "chkIsArticleRandom";
            this.chkIsArticleRandom.Size = new System.Drawing.Size(108, 16);
            this.chkIsArticleRandom.TabIndex = 32;
            this.chkIsArticleRandom.Text = "文章完全随机化";
            this.chkIsArticleRandom.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(658, 535);
            this.Controls.Add(this.chkIsArticleRandom);
            this.Controls.Add(this.txtArticleIdForPDF);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnAnlyzeMarkDown);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.btnOAuth);
            this.Controls.Add(this.txtBackUpFolder);
            this.Controls.Add(this.btnGetBackUpFolder);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnBackFileStroage);
            this.Controls.Add(this.NumPortFileStroage);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnPdfCreator);
            this.Controls.Add(this.btnReIndex);
            this.Controls.Add(this.btnGetTitleToken);
            this.Controls.Add(this.btnCompareTagResult);
            this.Controls.Add(this.btnAnlyze);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtSentence);
            this.Controls.Add(this.txtMarkDownFile);
            this.Controls.Add(this.txtHTMLContent);
            this.Controls.Add(this.txtCommentHTML);
            this.Controls.Add(this.btnMarkDownFile);
            this.Controls.Add(this.btnNest);
            this.Controls.Add(this.btnHTMLContent);
            this.Controls.Add(this.btnMDContent);
            this.Controls.Add(this.btnCnblogFilename);
            this.Controls.Add(this.NumArticleCount);
            this.Controls.Add(this.txtCommentMarkDown);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NumPortDataBase);
            this.Controls.Add(this.Cnblogs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMDContent);
            this.Controls.Add(this.txtCnblogFilename);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnSimulate);
            this.Name = "frmMain";
            this.Text = "数据模拟器";
            ((System.ComponentModel.ISupportInitialize)(this.NumPortDataBase)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumArticleCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumPortFileStroage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NumPortDataBase;
        private System.Windows.Forms.TextBox txtCnblogFilename;
        private System.Windows.Forms.Label Cnblogs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMDContent;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkArticle;
        private System.Windows.Forms.CheckBox chkFocus;
        private System.Windows.Forms.CheckBox chkStock;
        private System.Windows.Forms.CheckBox chkComment;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCommentMarkDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown NumArticleCount;
        private System.Windows.Forms.CheckBox chkTopic;
        private System.Windows.Forms.Button btnCnblogFilename;
        private System.Windows.Forms.Button btnMDContent;
        private System.Windows.Forms.Button btnNest;
        private System.Windows.Forms.CheckBox chkVisitor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCommentHTML;
        private System.Windows.Forms.TextBox txtHTMLContent;
        private System.Windows.Forms.Button btnHTMLContent;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSentence;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAnlyze;
        private System.Windows.Forms.Button btnCompareTagResult;
        private System.Windows.Forms.Button btnGetTitleToken;
        private System.Windows.Forms.Button btnReIndex;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.NumericUpDown NumPortFileStroage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnBackFileStroage;
        private System.Windows.Forms.TextBox txtBackUpFolder;
        private System.Windows.Forms.Button btnGetBackUpFolder;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnOAuth;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnMarkDownFile;
        private System.Windows.Forms.TextBox txtMarkDownFile;
        private System.Windows.Forms.Button btnAnlyzeMarkDown;
        private System.Windows.Forms.Button btnPdfCreator;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtArticleIdForPDF;
        private System.Windows.Forms.CheckBox chkIsArticleRandom;
    }
}

