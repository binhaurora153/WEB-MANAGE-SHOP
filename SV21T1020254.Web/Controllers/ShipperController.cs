using _21T1020254.BusinessLayers;
using _21T1020254.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020254.Web.Models;

namespace SV21T1020254.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
    public class ShipperController : Controller
    {
        public const int PAGE_SIZE = 5;

        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";
        public IActionResult Index()

        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data

            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {

            ViewBag.Title = "Bổ sung người giao hàng";
            var data = new Shipper()
            {
                ShipperID = 0,

            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhập thông tin người giao hàng";
            var data = CommonDataService.GetShipper(id);

            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Shipper data)
        {

            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhập thông tin người giao hàng";
            //Todo:Kiem soat du lieu dau vao, nếu không hợp lệ thì ta tạo ra thông báo lỗi và lưu giữ nó trong ModelState
            //ModelState.addmodelError(key,message)

            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại của người giao hàng");

            if (!ModelState.IsValid)
            {
                return View("Edit", data);//trả dữ liệu về cho View kèm theo các thông báo lỗi

            }

            //Kiem soat du lieu dau vao
            if (data.ShipperID == 0)
            {
               int id= CommonDataService.AddShipper(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại đã tồn tại");
                    return View("Edit", data);
                }
            }
            else
            {
              bool result=  CommonDataService.UpdateShipper(data);
                if (!result)
                {
                   ModelState.AddModelError(nameof(data.Phone), "Số điện thoại đã tồn tại");
                    return View("Edit", data);
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
