using _21T1020254.BusinessLayers;
using _21T1020254.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020254.Web.Models;

namespace SV21T1020254.Web.Controllers
{
 [Authorize(Roles =$"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
        public class CatagoryController : Controller
    {
        public const int PAGE_SIZE = 5;

        private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";

        public IActionResult Index()

        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfCategories(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            CategorySearchResult model = new CategorySearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data

            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {

            ViewBag.Title = "Bổ sung loại hàng";

            var data = new Category()
            {
                CategoryID = 0,
               
            };
            return View("Edit",data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhập thông tin loại hàng";

            var data = CommonDataService.GetCategory(id);

            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Category data)
        {

            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhập thông tin loại hàng";
            //Todo:Kiem soat du lieu dau vao, nếu không hợp lệ thì ta tạo ra thông báo lỗi và lưu giữ nó trong ModelState
            //ModelState.addmodelError(key,message)

            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Vui lòng nhập thông tin loại hàng");

            if (!ModelState.IsValid)
            {
                return View("Edit", data);//trả dữ liệu về cho View kèm theo các thông báo lỗi

            }
            //Todo:Kiem soat du lieu dau vao
            if (data.CategoryID == 0)
            {
               int id =  CommonDataService.AddCategory(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên loại đã tồn tại");
                    return View("Edit", data);
                }
            }
            else
            {
              bool result =  CommonDataService.UpdateCategory(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên loại đã tồn tại");
                    return View("Edit", data);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCategory(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
