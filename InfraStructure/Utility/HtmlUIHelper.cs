
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using InfraStructure.DataBase;
using InfraStructure.Helper;
using InfraStructure.Security;
using InfraStructure.Table;

namespace InfraStructure.Utility
{
    public static class HtmlUiHelper
    {
        #region Controller
        /// <summary>
        /// 验证并输出菜单项目的MvcHtmlString
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="display"></param>
        /// <param name="areaName"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="privilege"></param>
        /// <returns></returns>
        public static MvcHtmlString GetMenuItemWithPrivilegeCheck(HtmlHelper htmlhelper, string display,
                                    string areaName, string controllerName, string actionName, int privilege)
        {
            var html = string.Empty;
            if (PrivilegeCheck.Check(areaName, controllerName, actionName, privilege))
            {
                html = "<li>" + htmlhelper.ActionLink(display, actionName, controllerName, new { area = areaName }, null) + "</li>";
            }
            return new MvcHtmlString(html);
        }

        /// <summary>
        ///     获取BootStrap标准情景色
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="selectType"></param>
        /// <returns></returns>
        public static MvcHtmlString GetTableCellColorPick(string fieldName, WarningType selectType)
        {
            var html = string.Empty;
            html = "<table class=\"table\">" + Environment.NewLine;
            foreach (WarningType value in Enum.GetValues(typeof(WarningType)))
            {
                if (value != WarningType.Primary)
                {
                    html += "<tr>" + Environment.NewLine;
                    if (value != WarningType.None)
                    {
                        html += "    <td class=\"" + value + "\">" + Environment.NewLine;
                    }
                    else
                    {
                        html += "    <td>" + Environment.NewLine;
                    }
                    html += "        <input type=\"radio\" id=\"" + fieldName + "\" name=\"" + fieldName + "\" " +
                            (selectType.GetHashCode() == value.GetHashCode() ? "checked" : "") + " value=\"" +
                            value.GetHashCode() + "\"/> " + Environment.NewLine;
                    html += value + Environment.NewLine;
                    html += "    </td>" + Environment.NewLine;
                    html += "</tr>" + Environment.NewLine;
                }
            }
            html += "</table>" + Environment.NewLine;
            return new MvcHtmlString(html);
        }

        /// <summary>
        /// 泛用ActionLink
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString NoEncodeActionLink(this HtmlHelper htmlHelper,
                                     string text, string title, string action,
                                     string controller,
                                     object routeValues = null,
                                     object htmlAttributes = null)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var builder = new TagBuilder("a");
            builder.InnerHtml = text;
            builder.Attributes["title"] = title;
            builder.Attributes["href"] = urlHelper.Action(action, controller, routeValues);
            builder.MergeAttributes(new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));

            return MvcHtmlString.Create(builder.ToString());
        }
        /// <summary>
        /// 带图标的ActionLink
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="glyphiconName"></param>
        /// <param name="title"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString NoEncodeActionLinkForIcon(this HtmlHelper htmlHelper,
                             string glyphiconName, string title, string action,
                             string controller,
                             object routeValues = null,
                             object htmlAttributes = null)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var builder = new TagBuilder("a");
            builder.InnerHtml = "<span class=\"glyphicon glyphicon-" + glyphiconName + "\"></span>&nbsp;" + title;
            builder.Attributes["href"] = urlHelper.Action(action, controller, routeValues);
            builder.MergeAttributes(new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            return MvcHtmlString.Create(builder.ToString());
        }
        /// <summary>
        /// GetInput TagBuilder
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetInput(string fieldName, string type, string value)
        {
            var orgHtml =
                "<input class=\"form-control text-box single-line\"  id=\"%FieldName%\" name=\"%FieldName%\" type=\"%Type%\" value=\"%value%\" />";
            orgHtml = orgHtml.Replace("%FieldName%", fieldName);
            orgHtml = orgHtml.Replace("%Type%", type);
            orgHtml = orgHtml.Replace("%value%", value);
            return orgHtml;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static MvcHtmlString GetDatePicker(string fieldName)
        {
            var strDate = "<input style=\"width:80%;width:240px;\" class=\"form-control form_date\" type=\"text\" id=\"@FIELD@\" name=\"@FIELD@\" />";
            strDate = strDate.Replace("@FIELD@", fieldName) + Environment.NewLine;
            strDate = "<div class=\"input-group\">" + Environment.NewLine + strDate + "</div>" + Environment.NewLine;

            return new MvcHtmlString(strDate);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static MvcHtmlString GetDatePicker(string fieldName, DateTime Default)
        {
            var strDate = "<input style=\"width:80%;width:240px;\" class=\"form-control form_date\" type=\"text\" id=\"@FIELD@\" name=\"@FIELD@\" value=\"@VALUE@\" />";
            strDate = strDate.Replace("@FIELD@", fieldName);
            strDate = strDate.Replace("@VALUE@", Default.ToString("yyyy-MM-dd"));
            strDate = "<div class=\"input-group\">" + Environment.NewLine + strDate + "</div>" + Environment.NewLine;
            return new MvcHtmlString(strDate);
        }
        /// <summary>
        /// 这里是普通的CheckBox
        /// </summary>
        /// <param name="display"></param>
        /// <param name="fieldName"></param>
        /// <param name="ischecked"></param>
        /// <param name="withBr"></param>
        /// <returns></returns>
        public static string GetCheckBox(string display, string fieldName, bool ischecked, bool withBr = true)
        {
            var html = string.Empty;
            if (ischecked)
            {
                html += "<input checked type=\"checkbox\" name=\"" + fieldName + "\" id=\"" + fieldName + "\" />" + display +
                        (withBr ? "<br />" : string.Empty) + Environment.NewLine;
            }
            else
            {
                html += "<input type=\"checkbox\" name=\"" + fieldName + "\" id=\"" + fieldName + "\" />" + display +
                        (withBr ? "<br />" : string.Empty) + Environment.NewLine;
            }
            return html;
        }
        /// <summary>
        /// 这里是MVC的CheckBox
        /// </summary>
        /// <param name="display"></param>
        /// <param name="fieldName"></param>
        /// <param name="ischecked"></param>
        /// <param name="withBr"></param>
        /// <returns></returns>
        public static MvcHtmlString GetMvcCheckBox(string display, string fieldName, bool ischecked, bool withBr = true)
        {
            //CheckBox如果选中的话，则值是 on 
            //但是这样的话，MVC则认为 on 不是一个正确的值，在UpdateModel的时候会出错
            //MS的做法如下，一个CheckBox的值是true，同时还有一个hidden的值为false的项目

            var html = string.Empty;
            if (ischecked)
            {
                html += "<input checked type=\"checkbox\" name=\"" + fieldName + "\" id=\"" + fieldName + "\" value=\"true\" />" + display +
                        (withBr ? "<br />" : string.Empty) + Environment.NewLine;
            }
            else
            {
                html += "<input type=\"checkbox\" name=\"" + fieldName + "\" id=\"" + fieldName + "\"  value=\"true\" />" + display +
                        (withBr ? "<br />" : string.Empty) + Environment.NewLine;
            }
            html += "<input name=\"" + fieldName + "\" type=\"hidden\" value=\"false\" />";
            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// 获得布尔值图标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MvcHtmlString GetBooleanValue(bool value)
        {
            var html = string.Empty;

            if (value)
            {
                html = "<span class=\"glyphicon glyphicon-ok\" style=\"color:green\" />";
            }
            else
            {
                html = "<span class=\"glyphicon glyphicon-remove\" style=\"color:red\" />";
            }
            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// 获得标题
        /// </summary>
        /// <param name="strTitle"></param>
        /// <returns></returns>
        public static MvcHtmlString GetTitle(string strTitle, string strType = "info")
        {
            var html = string.Empty;
            html += "<div class=\"alert alert-" + strType + "\" >" + Environment.NewLine;
            html += "<strong>" + strTitle + "</strong>" + Environment.NewLine;
            html += "</div>" + Environment.NewLine;
            return new MvcHtmlString(html);
        }
        /// <summary>
        /// 限制宽度的标题
        /// </summary>
        /// <param name="strTitle"></param>
        /// <param name="strType"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static MvcHtmlString GetTitleWidth(string strTitle, string strType = "info", int width = 200)
        {
            var html = "<div style=\"width:" + width + "px;height:30px;\">" + Environment.NewLine;
            html += "<div class=\"alert alert-" + strType + "\" >" + Environment.NewLine;
            html += strTitle + Environment.NewLine;
            html += "</div>" + Environment.NewLine;
            html += "</div>" + Environment.NewLine;
            return new MvcHtmlString(html);
        }
        /// <summary>
        /// 获得一个面板
        /// </summary>
        /// <param name="id"></param>
        /// <param name="strTitle"></param>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public static MvcHtmlString GetPanel(string id, string strTitle, string strContent)
        {
            var html = "<div class=\"panel panel-info\">" + Environment.NewLine;
            html += "<div class=\"panel-heading\">" + Environment.NewLine;
            html += "<h3 class=\"panel-title\">" + Environment.NewLine;
            html += "<a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#collapse" + id + "\">" + Environment.NewLine;
            html += strTitle + Environment.NewLine;
            html += "</a>" + Environment.NewLine;
            html += "</h3>" + Environment.NewLine;
            html += "</div>" + Environment.NewLine;
            html += "<div id=\"collapse" + id + "\" class=\"panel-collapse collapse out\">" + Environment.NewLine;
            html += strContent + Environment.NewLine;
            html += "</div>" + Environment.NewLine;
            html += "</div>" + Environment.NewLine;
            return new MvcHtmlString(html);
        }

        #endregion

        #region Picker

        /// <summary>
        ///     获得用于单选的列表UI
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="dic"></param>
        /// <param name="selectKey"></param>
        /// <returns></returns>
        public static MvcHtmlString GetSingleValueUi<T>(string fieldName, List<T> list, string selectKey = "", string onEvent = "")
            where T : MasterTable
        {
            var html = string.Empty;
            html = "<select " + onEvent + "class = \"form-control\" name= \"" + fieldName + "\" id= \"" + fieldName + "\" >" + Environment.NewLine;
            html += "<option value=\"" + MasterTable.UnKnownCode + "\">未选择</option>" + Environment.NewLine;
            foreach (var item in list)
            {
                if (item.Code == selectKey)
                {
                    html += "<option selected value=\"" + item.Code + "\">" + item.Name +
                            ((string.IsNullOrEmpty(item.Description) || item.Name == item.Description) ? "" : "(" + item.Description + ")")
                            + "</option>" + Environment.NewLine;
                }
                else
                {
                    html += "<option value=\"" + item.Code + "\">" + item.Name +
                            ((string.IsNullOrEmpty(item.Description) || item.Name == item.Description) ? "" : "(" + item.Description + ")")
                            + "</option>" + Environment.NewLine;
                }
            }
            html += "</select>";
            return new MvcHtmlString(html);
        }

        /// <summary>
        ///     枚举UI（泛型）
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="EnumType"></param>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public static MvcHtmlString GetEnumUI<T>(string fieldName, int selectValue)
        {
            var enumType = typeof(T);
            return GetEnumUI(fieldName, enumType, selectValue);
        }

        /// <summary>
        ///  枚举UI
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="enumType"></param>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public static MvcHtmlString GetEnumUI(string fieldName, Type enumType, int selectValue)
        {
            var html = string.Empty;
            html = "<select class = \"form-control\" name= \"" + fieldName + "\" id= \"" + fieldName + "\" >" + Environment.NewLine;
            var list = Enum.GetValues(enumType);
            foreach (var value in list)
            {
                var field = enumType.GetField(value.ToString());
                var display = field.GetCustomAttributes(typeof(EnumDisplayNameAttribute), false).FirstOrDefault() as EnumDisplayNameAttribute;
                html += "<option" + (value.GetHashCode() == selectValue ? " selected" : string.Empty) +
                        " value=\"" + value.GetHashCode() + "\">" + display.DisplayName + "</option>" + Environment.NewLine;
            }
            html += "</select>";
            return new MvcHtmlString(html);
        }

        /// <summary>
        ///     获得用于带有级别的多选列表
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="dic"></param>
        /// <param name="selectItems"></param>
        /// <returns></returns>
        public static MvcHtmlString GetMultiValueWithGradeUi<T>(string fieldName, List<T> masterTableList, List<ItemWithGrade> selectItems) where T : MasterTable
        {
            var html = string.Empty;
            var selectDic = new Dictionary<string, CommonGrade>();
            foreach (var item in selectItems)
            {
                selectDic.Add(item.MasterCode, item.Grade);
            }
            foreach (var item in masterTableList)
            {
                html += "<div class=\"input-group\">";
                html += "<span class=\"input-group-addon\" style=\"width:80px;\" >";

                if (selectDic.ContainsKey(item.Code))
                {
                    html += GetCheckBox(item.Name + ((string.IsNullOrEmpty(item.Description) || item.Name == item.Description) ? "" : "(" + item.Description + ")"), fieldName + "." + item.Code, true, false);
                    html += "</span>";
                    html += GetCommonGrade(fieldName + "." + item.Code, selectDic[item.Code]);
                }
                else
                {
                    html += GetCheckBox(item.Name + ((string.IsNullOrEmpty(item.Description) || item.Name == item.Description) ? "" : "(" + item.Description + ")"), fieldName + "." + item.Code, false, false);
                    html += "</span>";
                    html += GetCommonGrade(fieldName + "." + item.Code, CommonGrade.NotAvaliable);
                }
                html += "</div>";
            }
            return new MvcHtmlString(html);
        }

        private static MvcHtmlString GetCommonGrade(string fieldName, CommonGrade grade)
        {
            var html = string.Empty;
            var enumType = typeof(CommonGrade);
            html = "<select class = \"form-control\" name= \"" + fieldName + ".Grade" + "\" id= \"" + fieldName + ".Grade" + "\" >" + Environment.NewLine;

            foreach (var value in Enum.GetValues(enumType))
            {
                var field = enumType.GetField(value.ToString());
                var display = field.GetCustomAttributes(typeof(EnumDisplayNameAttribute), false).FirstOrDefault() as EnumDisplayNameAttribute;
                if (grade.GetHashCode() == value.GetHashCode())
                {
                    html += "<option selected value=\"" + value.GetHashCode() + "\">" + display.DisplayName + "</option>" + Environment.NewLine;
                }
                else
                {
                    html += "<option  value=\"" + value.GetHashCode() + "\">" + display.DisplayName + "</option>" + Environment.NewLine;
                }

            }
            html += "</select>" + Environment.NewLine;
            return new MvcHtmlString(html);
        }

        /// <summary>
        ///     获得值列表
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<ItemWithGrade> GetMultiValueWithGradeFromUi(string fieldName, FormCollection collection)
        {
            var valueList = new List<ItemWithGrade>();
            foreach (var item in collection.AllKeys)
            {
                if (item.StartsWith(fieldName + "."))
                {
                    //LanguageList.00001.Grade NG
                    if (item.Split(".".ToCharArray()).Count() == 2)
                    {
                        //LanguageList.00001 OK
                        valueList.Add(new ItemWithGrade
                        {
                            MasterCode = item.Substring(fieldName.Length + 1),
                            Grade = (CommonGrade)(int.Parse(collection.GetValue(item + ".Grade").AttemptedValue))
                        });
                    }
                }
            }
            return valueList;
        }

        /// <summary>
        ///     获得用于多选的列表UI
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="dic"></param>
        /// <param name="selectKey"></param>
        /// <returns></returns>
        public static MvcHtmlString GetMultiValueUi<T>(string fieldName, List<T> list, List<string> selectKey)
            where T : MasterTable
        {
            var html = string.Empty;
            foreach (var item in list)
            {
                if (selectKey != null && selectKey.Contains(item.Code))
                {
                    html += GetCheckBox(item.Name + ((string.IsNullOrEmpty(item.Description) || item.Name == item.Description) ? "" : "(" + item.Description + ")"), fieldName + "." + item.Code, true);
                }
                else
                {
                    html += GetCheckBox(item.Name + ((string.IsNullOrEmpty(item.Description) || item.Name == item.Description) ? "" : "(" + item.Description + ")"), fieldName + "." + item.Code, false);
                }
            }
            return new MvcHtmlString(html);
        }

        /// <summary>
        ///     获得值列表
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<string> GetMultiValueFromUi(string fieldName, FormCollection collection)
        {
            var valueList = new List<string>();
            foreach (var item in collection.AllKeys)
            {
                if (item.StartsWith(fieldName + "."))
                {
                    valueList.Add(item.Substring(fieldName.Length + 1));
                }
            }
            return valueList;
        }

        #endregion

        #region Page
        /// <summary>
        /// 获得页数导航组件
        /// </summary>
        /// <param name="p">
        /// 表示开始分页器
        /// </param>
        /// <param name="link">
        /// 页数的Link链接字符生成器，
        /// link函数的数字参数表示从1开始的页数
        /// link函数结果预想是自带 a 标签的
        /// </param>
        ///<param name="PageCnt">显示多少页</param>
        /// <returns></returns>
        public static string GetPageNavi(Pages p, Func<int, string> link, int PageCnt = 15)
        {
            var html = string.Empty;
            html += "<nav>" + Environment.NewLine;
            html += "    <ul class=\"pagination\">" + Environment.NewLine;

            if (p.CurrentPageNo != 1)
            {
                html += "        <li>" + Environment.NewLine;
                html += "            <a href=\"#\" aria-label=\"Previous\">" + Environment.NewLine;
                html += "                <span aria-hidden=\"true\">&laquo;</span>" + Environment.NewLine;
                html += "            </a>" + Environment.NewLine;
                html += "        </li>" + Environment.NewLine;
            }
            int MaxPage = p.TotalPage();
            if (MaxPage <= PageCnt)
            {
                for (var pageNo = 1; pageNo <= MaxPage; pageNo++)
                {
                    if (pageNo == p.CurrentPageNo)
                    {
                        html += "<li class=\"active\">" + Environment.NewLine;
                    }
                    else
                    {
                        html += "<li>" + Environment.NewLine;
                    }
                    html += link(pageNo) + Environment.NewLine;
                    html += "</li>" + Environment.NewLine;
                }
            }
            else
            {
                //第一页和最后一页，当前页必须显示
                var ShowPage = new List<int>();
                ShowPage.Add(1);
                ShowPage.Add(MaxPage);
                if (!ShowPage.Contains(p.CurrentPageNo)) ShowPage.Add(p.CurrentPageNo);
                //以当前页为中心，前后展开
                for (int i = 1; i < PageCnt; i++)
                {
                    int prePage = p.CurrentPageNo - i;
                    int NextPage = p.CurrentPageNo + i;
                    if (prePage > 1)
                    {
                        if (!ShowPage.Contains(prePage))
                        {
                            ShowPage.Add(prePage);
                            if (ShowPage.Count == PageCnt) break;
                        }
                    }
                    if (NextPage < MaxPage)
                    {
                        if (!ShowPage.Contains(NextPage))
                        {
                            ShowPage.Add(NextPage);
                            if (ShowPage.Count == PageCnt) break;
                        }
                    }
                }
                ShowPage.Sort();
                for (var PageIndex = 0; PageIndex < PageCnt; PageIndex++)
                {
                    int pageNo = ShowPage[PageIndex];
                    if (pageNo == p.CurrentPageNo)
                    {
                        html += "<li class=\"active\">" + Environment.NewLine;
                    }
                    else
                    {
                        html += "<li>" + Environment.NewLine;
                    }
                    html += link(pageNo) + Environment.NewLine;
                    html += "</li>" + Environment.NewLine;
                }

            }


            if (p.CurrentPageNo != MaxPage)
            {
                html += "        <li>" + Environment.NewLine;
                html += "            <a href=\"#\" aria-label=\"Next\">" + Environment.NewLine;
                html += "                <span aria-hidden=\"true\">&raquo;</span>" + Environment.NewLine;
                html += "            </a>" + Environment.NewLine;
                html += "        </li>" + Environment.NewLine;
            }
            html += "    </ul>" + Environment.NewLine;
            html += "</nav>" + Environment.NewLine;
            return html;
        }
        #endregion

    }
}