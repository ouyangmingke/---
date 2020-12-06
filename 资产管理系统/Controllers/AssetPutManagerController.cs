using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 资产管理系统.Models;

namespace 资产管理系统.Controllers
{
    [Authorize]
    public class AssetPutManagerController : Controller
    {
        Models.AssetDBEntities db = new Models.AssetDBEntities();
        // GET: AssetPutManager
        public ActionResult Index()
        {
            return View();
        }
        #region 资产入库分页

        /// <summary>
        ///  资产入库分页
        /// </summary>
        /// <returns></returns>

        public ActionResult PutmanagerPage()
        {
            var put = from i in db.AssetPutInfo
                      join j in db.EmpolyInfo on i.EmpolyId equals j.EmpolyId
                      join k in db.AssetInfo on i.AssetId equals k.AssetId
                      select new { k.AssetId, k.AssetName, k.AssetTypeID, i.AssetPutNum, j.EmpolyName, i.AssetPutDate, i.AssetPutReMark, i.AssetPutID };

            string name = Request["name"];
            if (!string.IsNullOrEmpty(name))
            {
                put = put.Where(j => j.EmpolyName.Contains(name));
            }
            string putname = Request["putname"];
            if (!string.IsNullOrEmpty(putname))
            {
                put = put.Where(k => k.AssetName.Contains(putname));
            }
            int typeid = Convert.ToInt32(Request["puttype"]);
            if (typeid != 0)
            {
                put = put.Where(k => k.AssetTypeID == typeid);
            }
            int row = 3, num = 0;
            if (!string.IsNullOrEmpty(Request["limit"]))
            {
                row = Convert.ToInt32(Request["limit"]);
            }
            if (!string.IsNullOrEmpty(Request["offset"]))
            {
                num = Convert.ToInt32(Request["offset"]);
            }
            var date = new
            {
                total = put.Count(),
                rows = put.ToList().Skip(num).Take(row)
            };

            return Json(date, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  二级联动
        /// <summary>
        ///  二级联动
        /// </summary>
        /// <param name="context"></param>
        public ActionResult dropnamechange()
        {
            int id = Convert.ToInt32(Request["droptypeid"]);
            var typename = from i in db.AssetInfo select new { i.AssetName, i.AssetTypeID, i.AssetId };
            if (id != 0)
            {
                typename = typename.Where(i => i.AssetTypeID == id);
            }

            return Json(typename, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region  添加入库信息
        /// <summary>
        ///   添加入库信息
        /// </summary>
        /// <param name=""></param>
        public int Addput(AssetPutInfo put)
        {
            put.EmpolyId = 1;// Convert.ToInt32( Session["EmpolyId"]);
            db.AssetPutInfo.Add(put);
            return db.SaveChanges();
        }


        #endregion
        #region 修改资产入库信息
        /// <summary>
        ///   修改资产入库信息
        /// </summary>
        /// <param name=""></param>
        public int Updateput(AssetPutInfo put)
        {
            var puts = from i in db.AssetPutInfo where i.AssetPutID == put.AssetPutID select i;

            if (puts.Count() > 0)
            {
                put.EmpolyId = 1;// Convert.ToInt32( Session["EmpolyId"]);
                db.Entry(put).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
            else
            {
                return -1;
            }


        }


        #endregion
    }
}