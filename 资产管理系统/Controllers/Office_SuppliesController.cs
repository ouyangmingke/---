using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 资产管理系统.Models;

namespace 资产管理系统.Controllers
{
    [Authorize]
    public class Office_SuppliesController : Controller
    {
        Models.AssetDBEntities db = new Models.AssetDBEntities();

        // GET: Office_Supplies
        /// <summary>
        /// 办公用品审批
        /// </summary>
        /// <returns></returns>
        public ActionResult Collection_office_supplies()
        {
            return View();
        }

        /// <summary>
        /// 办公用品统计
        /// </summary>
        /// <returns></returns>
        public ActionResult Statistics_Office_supplies()
        {
            return View();
        }

        /// <summary>
        /// 办公用品使用申请
        /// </summary>
        /// <returns></returns>
        public ActionResult Register_office_supplies()
        {
            Session["EmpolyName"] = "小雄";
            Session["EmpolyId"] = 1;
            return View();
        }
        #region//获取办公用品统计
        /// <summary>
        /// 获取办公用品统计
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStatistics()
        {
            //获取入库总数量
            var AssetPutNumSum = from t1 in db.AssetInfo
                                 where t1.AssetTypeID == 2
                                 join t2 in db.AssetPutInfo on t1.AssetId equals t2.AssetId into right1
                                 from t3 in right1.DefaultIfEmpty()
                                 group t3 by t1.AssetName into tt
                                 select new { AssetName = tt.Key, AssetPutNumSum = tt.Sum(t => t.AssetPutNum) };
            //获取使用总数量
            var OfficeReceiveNumSum = from t1 in db.AssetInfo
                                      where t1.AssetTypeID == 2
                                      join t2 in db.OfficeSuppliesRecordInfo on t1.AssetId equals t2.AssetId into right1
                                      from t3 in right1.DefaultIfEmpty()
                                      group t3 by t1.AssetName into tt
                                      select new { AssetName = tt.Key, OfficeReceiveNumSum = tt.Sum(t => t.OfficeReceiveNum) };


            var list = from t1 in AssetPutNumSum
                       join t2 in OfficeReceiveNumSum on t1.AssetName equals t2.AssetName
                       join t3 in db.AssetInfo on t1.AssetName equals t3.AssetName
                       select new { t3.AssetId, t1.AssetName, t1.AssetPutNumSum, t2.OfficeReceiveNumSum, surplusSum = t1.AssetPutNumSum - t2.OfficeReceiveNumSum };
            if (!string.IsNullOrEmpty(Request["AssetName"]))
            {
                string AssetName = Request["AssetName"];
                list = list.Where(left3 => left3.AssetName.Contains(AssetName));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//获取办公用品申领信息
        /// <summary>
        /// 获取办公用品申领信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCollection()
        {
            int limit = 5, beginIndex = 0;
            if (!string.IsNullOrEmpty(Request["limit"]))
                limit = int.Parse(Request["limit"].ToString());//每页显示的数量

            if (!string.IsNullOrEmpty(Request["offset"]))
                beginIndex = int.Parse(Request["offset"].ToString());// SQL语句起始索引

            var list = from t1 in db.OfficeSuppliesRecordInfo
                       join t2 in db.EmpolyInfo on t1.OfficeApplyID equals t2.EmpolyId
                       join t3 in db.AssetInfo on t1.AssetId equals t3.AssetId
                       select new { t1.OfficeId, t1.AssetId, t3.AssetName, t2.EmpolyName, t1.OfficeApplyDate, t1.OfficeApplyNum, t1.OfficeApplyState, t1.OfficeReceiveNum, t1.OfficeReceiveDate, t1.OfficeRemark };
            if (!string.IsNullOrEmpty(Request["AssetName"]))
            {
                string AssetName = Request["AssetName"];
                list = list.Where(left3 => left3.AssetName.Contains(AssetName));
            }
            if (!string.IsNullOrEmpty(Request["EmpolyName"]))
            {

                string EmpolyName = Request["EmpolyName"];
                if (EmpolyName == "Session")
                {
                    EmpolyName = Session["EmpolyName"].ToString();
                }
                list = list.Where(left3 => left3.EmpolyName.Contains(EmpolyName));
            }
            if (!string.IsNullOrEmpty(Request["OfficeApplyState"]))
            {
                int OfficeApplyState = int.Parse(Request["OfficeApplyState"]);
                if (OfficeApplyState != -1)
                    list = list.Where(t1 => t1.OfficeApplyState == OfficeApplyState);
            }
            var data = new
            {
                total = list.Count(),
                rows = list.ToList().Skip((beginIndex)).Take(limit)
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//获取库存数量
        /// <summary>
        /// 获取库存数量
        /// </summary>
        /// <returns></returns>
        public ActionResult GetstockNum(int AssetId)
        {
            //获取入库总数量
            int AssetPutNumSum = (from t1 in db.AssetPutInfo
                                  where t1.AssetId == AssetId
                                  select t1.AssetPutNum).Sum();
            //获取使用总数量
            var list = from t1 in db.OfficeSuppliesRecordInfo
                       where t1.AssetId == AssetId
                       select t1.OfficeReceiveNum;
            int AssetNumSum = 0;
            if (list.Count() > 0)
            {
                AssetNumSum = Convert.ToInt32((list).Sum());
            }
            return Json(AssetPutNumSum - AssetNumSum, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//审批办公用品申请
        /// <summary>
        /// 审批办公用品申请
        /// </summary>
        /// <returns></returns>
        public ActionResult Approval(int OfficeId, int OfficeReceiveNum, DateTime OfficeReceiveDate)
        {
            var update = (from t1 in db.OfficeSuppliesRecordInfo where t1.OfficeId == OfficeId select t1).FirstOrDefault();
            if (update != null)
            {
                update.OfficeReceiveNum = OfficeReceiveNum;
                update.OfficeApplyState = 1;
                update.OfficeReceiveDate = OfficeReceiveDate;
                db.Entry(update).State = System.Data.Entity.EntityState.Modified;
                return Json(db.SaveChanges(), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(-1, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region//办公用品申请
        /// <summary>
        /// 办公用品申请
        /// </summary>
        /// <returns></returns>
        public ActionResult Apply(int AssetId, int OfficeApplyNum, DateTime OfficeApplyDate,string OfficeRemark)
        {
            OfficeSuppliesRecordInfo officeSupplies = new OfficeSuppliesRecordInfo();
            officeSupplies.AssetId = AssetId;
            officeSupplies.OfficeApplyNum = OfficeApplyNum;
            officeSupplies.OfficeApplyDate = OfficeApplyDate;
            officeSupplies.OfficeApplyState = 0;
            officeSupplies.OfficeApplyID = int.Parse(Session["EmpolyId"].ToString());
            officeSupplies.OfficeRemark = OfficeRemark;
            db.Configuration.ProxyCreationEnabled = false;
            db.OfficeSuppliesRecordInfo.Add(officeSupplies);
            return Json(db.SaveChanges(), JsonRequestBehavior.AllowGet);

        }
        #endregion

    }
}