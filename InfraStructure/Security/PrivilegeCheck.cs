using System.Collections.Generic;
using System.Linq;

namespace InfraStructure.Security
{
    public static class PrivilegeCheck
    {
        /// <summary>
        /// 访问权限列表
        /// </summary>
        public static Dictionary<string, int[]> AccessRightDic = new Dictionary<string, int[]>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public static string CreateActionKey(string areaName)
        {
            return areaName + ".*";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public static string CreateActionKey(string areaName, string controllerName)
        {
            return areaName + "." + controllerName + ".*" ;
        }
        /// <summary>
        /// 定义如何组合为主键
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static string CreateActionKey(string areaName, string controllerName, string actionName)
        {
            return areaName + "." + controllerName + "." + actionName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="privilege"></param>
        /// <returns></returns>
        public static bool Check(string areaName,int privilege)
        {
            var isPass = false;
            var actionKey = CreateActionKey(areaName);
            isPass = (AccessRightDic.ContainsKey(actionKey) && AccessRightDic[actionKey].Contains(privilege));
            return isPass;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="controllerName"></param>
        /// <param name="privilege"></param>
        /// <returns></returns>
        public static bool Check(string areaName, string controllerName, int privilege)
        {
            var isPass = false;
            var actionKey = CreateActionKey(areaName, controllerName);
            isPass = (AccessRightDic.ContainsKey(actionKey) && AccessRightDic[actionKey].Contains(privilege));
            return isPass;
        }
        /// <summary>
        /// 获取权限
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="privilege"></param>
        /// <returns></returns>
        public static bool Check(string areaName, string controllerName, string actionName, int privilege)
        {
            var isPass = false;
            var actionKey = CreateActionKey(areaName, controllerName, actionName);
            isPass = (AccessRightDic.ContainsKey(actionKey) && AccessRightDic[actionKey].Contains(privilege));
            return isPass;
        }
    }
}