using AutoMapper;
using Formationn_Ecommerce.Application.Coupons.Dtos;
using Formationn_Ecommerce.Application.Coupons.Interfaces;
using Formationn_Ecommerce.Models.Coupon;
using Microsoft.AspNetCore.Mvc;

namespace Formationn_Ecommerce.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper; 
        public CouponController(ICouponService couponService, IMapper mapper)
        {
            _couponService = couponService;
            _mapper = mapper;
        }
        public async Task<IActionResult> CouponIndex()
        {
            try
            {
                IEnumerable<CouponDto> couponDtos = await _couponService.ReadAllAsync();
                IEnumerable<CouponViewModel> viewModels = _mapper.Map<IEnumerable<CouponViewModel>>(couponDtos);
                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error loading coupons.";
                return View();
            }          
        }

        public IActionResult CreateCoupon()
        {
            return View(new CreateCouponViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CreateCouponViewModel createCouponViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(createCouponViewModel);
            }
            try
            {
                var couponDto = _mapper.Map<CouponDto>(createCouponViewModel);

                var result = await _couponService.AddAsync(couponDto);

                if(result != null)
                {
                    TempData["success"] = "Coupon created successfully!";
                    return RedirectToAction(nameof(CouponIndex));
                }

                return View(createCouponViewModel);
            }
            catch (InvalidOperationException ex)
            {
                // This will catch Stripe-specific errors from our service
                TempData["error"] = ex.Message;
                return View(createCouponViewModel);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to create coupon.";
                return View(createCouponViewModel);
            }
        }

        public async Task<IActionResult> DeleteCoupon(Guid couponId)
        {
            try
            {
                var couponDto = await _couponService.ReadByIdAsync(couponId);
                if(couponDto == null)
                {
                    TempData["error"] = "Coupon not found.";
                    return RedirectToAction(nameof(CouponIndex));
                }
                var couponToDelete = _mapper.Map<DeleteCouponViewModel>(couponDto);
                return View(couponToDelete);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error loading coupon for deletion.";
                return RedirectToAction(nameof(CouponIndex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCouponConfirmed(DeleteCouponViewModel deleteCouponViewModel)
        {
            try
            {
                await _couponService.DeleteAsync(deleteCouponViewModel.Id);
                TempData["success"] = "Coupon deleted successfully!";
                return RedirectToAction(nameof(CouponIndex));
            }
            catch (InvalidOperationException ex)
            {
                // This will catch Stripe-specific errors from our service
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(CouponIndex));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to delete coupon.";
                return RedirectToAction(nameof(CouponIndex));
            }
        }
    }
}
