using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace 资产管理系统.Controllers
{
    public class LoginController : Controller
    {
        Models.AssetDBEntities db = new Models.AssetDBEntities();
        /// <summary>
        /// 显示登录视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <returns></returns>
        public ActionResult isLogin(string userName, string pwd)
        {
            var list = (from i in db.EmpolyInfo
                        join j in db.RoleInfo on i.RoletId equals j.RoletId
                        where i.EmpolyNum == userName && i.EmpolyPwd == pwd
                        select new { i, j }).ToList();
            //登录成功
            if (list.Count > 0)
            {
                Session["EmpolyId"] = list.FirstOrDefault().i.EmpolyId; //员工ID
                Session["EmpolyNum"] = list.FirstOrDefault().i.EmpolyNum; //用户编号
                Session["RoleName"] = list.FirstOrDefault().j.RoleName; //权限名称
                Session["EmpolyName"] = list.FirstOrDefault().i.EmpolyName;//用户名
                Session["RoletId"] = list.FirstOrDefault().i.RoletId;//权限
                Session["EmpolyPwd"] = list.FirstOrDefault().i.EmpolyPwd;//密码
                FormsAuthentication.SetAuthCookie(userName, false);
            }
            return Json(list.Count.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}