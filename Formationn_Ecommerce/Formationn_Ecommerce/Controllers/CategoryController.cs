using AutoMapper;
using Formationn_Ecommerce.Application.Categories.Dtos;
using Formationn_Ecommerce.Application.Categories.Interfaces;
using Formationn_Ecommerce.Models.Category;
using Microsoft.AspNetCore.Mvc;

namespace Formationn_Ecommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> CategoryIndex()
        {
            try
            {
                IEnumerable<CategoryDto> categoryDtos = await _categoryService.ReadAllAsync();
                IEnumerable<CategoryViewModel> viewModels = _mapper.Map<IEnumerable<CategoryViewModel>>(categoryDtos);
                return View(viewModels);
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                return View(new List<CategoryViewModel>());
            }
        }

        public IActionResult CreateCategory()
        {
            return View(new CreateCategoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel createCategoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createCategoryViewModel);
            }

            try
            {
                var createCategoryDto = _mapper.Map<CreateCategoryDto>(createCategoryViewModel);
                var result = await _categoryService.AddAsync(createCategoryDto);

                if (result != null)
                {
                    TempData["success"] = "Category created successfully!";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                return View(createCategoryViewModel);
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                ModelState.AddModelError("", "An error occurred while creating the category.");
                TempData["error"] = "Failed to create category.";
                return View(createCategoryViewModel);
            }
        }

        public async Task<IActionResult> EditCategory(Guid id)
        {
            try
            {
                var categoryDto = await _categoryService.ReadByIdAsync(id);
                if (categoryDto == null)
                {
                    TempData["error"] = "Category not found.";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                return View(_mapper.Map<CategoryViewModel>(categoryDto));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error loading category.";
                return RedirectToAction(nameof(CategoryIndex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryViewModel);
            }

            try
            {
                var updateCategoryDto = _mapper.Map<UpdateCategoryDto>(categoryViewModel);
                await _categoryService.UpdateAsync(updateCategoryDto);
                TempData["success"] = "Category updated successfully!";
                return RedirectToAction(nameof(CategoryIndex));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the category.");
                TempData["error"] = "Failed to update category.";
                return View(categoryViewModel);
            }
        }

        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var categoryDto = await _categoryService.ReadByIdAsync(id);
                if (categoryDto == null)
                {
                    TempData["error"] = "Category not found.";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                return View(_mapper.Map<DeleteCategoryViewModel>(categoryDto));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error loading category for deletion.";
                return RedirectToAction(nameof(CategoryIndex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryConfirmed(DeleteCategoryViewModel deleteCategoryViewModel)
        {
            try
            {
                await _categoryService.DeleteAsync(deleteCategoryViewModel.Id);
                TempData["success"] = "Category deleted successfully!";
                return RedirectToAction(nameof(CategoryIndex));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to delete category.";
                return RedirectToAction(nameof(CategoryIndex));
            }
        }
    }
}
