using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace 资产管理系统.Controllers
{
    [Authorize]
    public class IndexMianController : Controller
    {
        public ActionResult Mian()
        {
            return View();
        }


        Models.AssetDBEntities db = new Models.AssetDBEntities();

        public ActionResult CheckLoginStatus()
        {
            if (Session["EmpolyId"] != null)
            {
                int EmpolyId = int.Parse(Session["EmpolyId"].ToString());
                var list = from i in db.EmpolyInfo
                           join j in db.RoleInfo on i.RoletId equals j.RoletId
                           where i.EmpolyId == EmpolyId
                           select new { i.EmpolyName, j.RoleName };

                return Json(list.FirstOrDefault(), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(-1, JsonRequestBehavior.AllowGet);
        }


        public ActionResult OutLogin()
        {
            Session.Abandon();
            return null;
        }

        public ActionResult SelPersonInfo()
        {
            db.Configuration.ProxyCreationEnabled = false;
            string userEmpolyNum = Session["EmpolyNum"].ToString();
            var list = (from ei in db.EmpolyInfo
                        join di in db.DeptInfo on ei.DeptId equals di.DeptId
                        where ei.EmpolyNum == userEmpolyNum
                        select new { ei.EmpolyImg, ei.EmpolyName, ei.EmpolyIdCard, ei.EmpolyLevel, ei.InductionDate, ei.DepartureDate, ei.EmpolyReMark, di.DeptName }).Single();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdPersonInfoPwd(string oldEmpolypwd, string userEmpolyPwd)
        {
            string re = "";
            if (oldEmpolypwd == Session["EmpolyPwd"].ToString())
            {
                int EmpolyId = int.Parse(Session["EmpolyId"].ToString());

                var info = (from i in db.EmpolyInfo where i.EmpolyId == EmpolyId select i).Single();

                info.EmpolyPwd = userEmpolyPwd;

                db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                re = db.SaveChanges() > 0 ? "修改成功" : "修改失败";
            }
            else re = "原始密码错误";

            return Json(re, JsonRequestBehavior.AllowGet);
        }
    }
}