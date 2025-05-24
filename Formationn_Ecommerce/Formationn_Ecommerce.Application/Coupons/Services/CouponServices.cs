using AutoMapper;
using Formationn_Ecommerce.Application.Coupons.Dtos;
using Formationn_Ecommerce.Application.Coupons.Interfaces;
using Formationn_Ecommerce.Core.Entities.Coupon;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommercee.Core.Interfaces.External;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V133.CSS;

namespace Formationn_Ecommerce.Application.Coupons.Services
{
    public class CouponServices : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly IStripePayment _stripePayment;

        public CouponServices(ICouponRepository couponRepository, IMapper mapper, IStripePayment stripePayment)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
            _stripePayment = stripePayment;
        }
        public async Task<CouponDto> AddAsync(CouponDto couponDto)
        {
            //Mapper le DTO vers l'entité Coupon
            var coupon = _mapper.Map<Coupon>(couponDto);

            // Créer le coupon dans Stripe d'abord
            string stripeResult = await _stripePayment.AddStripeCoupon(coupon);
            
            // Vérifier si la création a réussi dans Stripe
            if (!stripeResult.StartsWith("Error") && !stripeResult.StartsWith("Stripe Error") && !stripeResult.StartsWith("Failed"))
            {
                // Appeler le repository avec l'entité
                var addedCoupon = await _couponRepository.AddAsync(coupon);
                
                //Retourner le résultat mapé en Dto
                return _mapper.Map<CouponDto>(addedCoupon);
            }
            
            // Si la création a échoué dans Stripe, renvoyer l'erreur
            throw new InvalidOperationException($"Échec de création du coupon dans Stripe: {stripeResult}");
        }

        public async Task<CouponDto?> ReadByIdAsync(Guid couponId)
        {
            var coupon = await _couponRepository.ReadByIdAsync(couponId);
            if(coupon == null)
            {
                return null;
            }
            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string couponCode)
        {
            var coupon = await _couponRepository.ReadByCouponCodeAsync(couponCode);
            if(coupon == null)
            {
                return null;
            }
            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<IEnumerable<CouponDto>> ReadAllAsync()
        {
            var couponList = await _couponRepository.ReadAllAsync();
            return _mapper.Map<IEnumerable<CouponDto>>(couponList);
        }

        public async Task UpdateAsync(UpdateCouponDto updateCouponDto)
        {
            // Vérifier que le coupon existe
            var existingCoupon = await _couponRepository.ReadByIdAsync(updateCouponDto.Id);
            if(existingCoupon == null)
            {
                throw new NotFoundException("Coupon Not Found");
            }
            
            // Mapper le DTO vers l'entité Coupon
            var couponToUpdate = _mapper.Map<Coupon>(updateCouponDto);
            
            // Pour mettre à jour un coupon dans Stripe, nous devons d'abord le supprimer puis le recréer
            // car Stripe ne permet pas de modifier directement un coupon existant
            string deleteResult = await _stripePayment.DeleteStripeCoupon(existingCoupon);
            
            if (!deleteResult.StartsWith("Error") && !deleteResult.StartsWith("Stripe Error"))
            {
                // Créer un nouveau coupon dans Stripe avec les valeurs mises à jour
                string createResult = await _stripePayment.AddStripeCoupon(couponToUpdate);
                
                if (!createResult.StartsWith("Error") && !createResult.StartsWith("Stripe Error") && !createResult.StartsWith("Failed"))
                {
                    // Mettre à jour l'entité dans le repository local
                    await _couponRepository.UpdateAsync(couponToUpdate);
                }
                else
                {
                    throw new InvalidOperationException($"Échec de création du coupon mis à jour dans Stripe: {createResult}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Échec de suppression du coupon existant dans Stripe: {deleteResult}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            // Vérifier que le coupon existe
            var coupon = await _couponRepository.ReadByIdAsync(id);
            if(coupon == null)
            {
                throw new NotFoundException("Coupon Not Found");
            }
            
            // Supprimer le coupon dans Stripe d'abord
            string stripeResult = await _stripePayment.DeleteStripeCoupon(coupon);
            
            // Vérifier si la suppression a réussi dans Stripe
            if (!stripeResult.StartsWith("Error") && !stripeResult.StartsWith("Stripe Error"))
            {
                // Supprimer le coupon localement
                await _couponRepository.DeleteAsync(id);
            }
            else
            {
                throw new InvalidOperationException($"Échec de suppression du coupon dans Stripe: {stripeResult}");
            }
        }
    }
}
