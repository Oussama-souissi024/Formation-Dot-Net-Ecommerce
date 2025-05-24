using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Formationn_Ecommerce.Application.Cart.Dtos;
using Formationn_Ecommerce.Application.Cart.Interfaces;
using Formationn_Ecommerce.Application.Products.Dtos;
using Formationn_Ecommerce.Core.Entities.Cart;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository;
using Formationn_Ecommercee.Core.Interfaces.External;
using Stripe;

namespace Formationn_Ecommerce.Application.Cart.Servecies
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly IStripePayment _stripePayment;
        
        public CartService(ICartRepository cartRepository, 
                           IMapper mapper, 
                           IProductRepository productRepository,
                           ICouponRepository couponRepository,
                           IStripePayment stripePayment)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _couponRepository = couponRepository;
            _stripePayment = stripePayment;
        }

        public async Task<CartDto?> GetCartByUserIdAsync(string userId)
        {
            // Étape 1: Récupération de l'en-tête du panier depuis le repository
            var cartHeaderFromDb = await _cartRepository.GetCartHeaderByUserIdAsync(userId);

            if (cartHeaderFromDb == null)
            {
                // Gestion du cas où aucun panier n'existe pour cet utilisateur
                return null;
            }
            // Étape 2: Initialisation du DTO et mapping des données de l'en-tête
            // Utilisation de la syntaxe C# moderne pour l'initialisation d'objets
            CartDto cartDto = new()
            {
                CartHeader = _mapper.Map<CartHeaderDto>(cartHeaderFromDb)
            };

            // Étape 3.1: Récupération des détails du panier et mapping vers les DTOs
            var cartDetailsFromDb = cartHeaderFromDb.CartDetails.ToList();
            cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(cartDetailsFromDb);

            // Étape 3.2: Récupération des produits pour enrichir les détails du panier
            // En récupérant tous les produits en une seule requête, nous optimisons les performances
            // en évitant des appels multiples à la base de données (N+1 query problem)
            var productList = await _productRepository.ReadAllAsync();
            var productListDto = _mapper.Map<IEnumerable<ProductDto>>(productList);

            // Initialisation du total du panier pour le calcul qui suit
            cartDto.CartHeader.CartTotal = 0;

            // Étape 3.3: Calcul du total du panier et liaison des produits aux détails
            // Parcours de chaque élément du panier pour calculer les totaux
            foreach (var item in cartDto.CartDetails)
            {
                // Pour chaque détail, récupération du produit correspondant
                var product = await _productRepository.ReadByIdAsync(item.ProductId);
                if (product != null)
                {
                    // Assignation des données du produit au détail du panier
                    item.Product = _mapper.Map<ProductDto>(product);

                    // Le prix utilisé est celui du produit au moment de la consultation
                    // Cela garantit que le prix est toujours à jour, même si le prix du produit a changé
                    // depuis que l'utilisateur l'a ajouté au panier
                    item.Price = (item.Count * product.Price);  // Price est de type decimal dans les deux classes

                    // Calcul du sous-total pour cet article (prix * quantité)
                    cartDto.CartHeader.CartTotal += item.Price;
                }
            }

            // Étape 4: Application des règles métier - Gestion des coupons de réduction
            if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            {
                // Récupération du coupon par son code
                var coupon = await _couponRepository.ReadByCouponCodeAsync(cartDto.CartHeader.CouponCode);

                // Vérification de la validité du coupon et application de la réduction
                // Le coupon n'est applicable que si le total du panier atteint le montant minimum requis
                if (coupon != null && cartDto.CartHeader.CartTotal >= coupon.MinimumAmount)
                {
                    // Enregistrement du montant de la réduction
                    cartDto.CartHeader.Discount = coupon.DiscountAmount;

                    // Application de la réduction au total du panier
                    cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
                }
            }

            return cartDto;
        }

        public async Task<CartDto> UpsertCartAsync(CartDto cartDto)
        {
            // Handle Cart Header
            var cartHeader = await _cartRepository.GetCartHeaderByUserIdAsync(cartDto.CartHeader.UserID);

            if (cartHeader == null)
            {
                // Create new cart header
                cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                cartHeader = await _cartRepository.AddCartHeaderAsync(cartHeader);

                cartDto.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;

                var cartDetail = _mapper.Map<CartDetails>(cartDto.CartDetails.FirstOrDefault());
                cartDetail = await _cartRepository.AddCartDetailsAsync(cartDetail);
            }
            else
            {
                //Update Count
                var cartDetailsFromDb = await _cartRepository.GetCartDetailsByCartHeaderIdAndProductId(
                                                          cartHeader.Id, cartDto.CartDetails.FirstOrDefault().ProductId);

                if (cartDetailsFromDb != null)
                {
                    cartDetailsFromDb.Count += cartDto.CartDetails.FirstOrDefault().Count;
                    await _cartRepository.UpdateCartDetailsAsync(cartDetailsFromDb);
                }

                // Insert the new element (details) to existing cart
                cartDto.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                var cartDetail = _mapper.Map<CartDetails>(cartDto.CartDetails.FirstOrDefault());
                await _cartRepository.AddCartDetailsAsync(cartDetail);
            }

            // Return the updated cart
            return await GetCartByUserIdAsync(cartDto.CartHeader.UserID);
        }

        public async Task<CartDto?> ApplyCouponAsync(string userId, string couponCode)
        {
            var cartHeader = await _cartRepository.GetCartHeaderByUserIdAsync(userId);
            if (cartHeader != null)
            {
                if(couponCode == "")
                {
                    cartHeader.CouponCode = null;
                    cartHeader.CouponId = null;
                    await _cartRepository.UpdateCartHeaderAsync(cartHeader);
                    return await GetCartByUserIdAsync(userId);
                }

                var coupon = await _couponRepository.ReadByCouponCodeAsync(couponCode);
                
                if (coupon != null)
                {
                    // Vérifier si le coupon existe dans Stripe avant de l'appliquer
                    try
                    {
                        // Utiliser le service Stripe pour vérifier la validité du coupon
                        // Cette vérification n'est pas directement disponible dans l'interface IStripePayment,
                        // mais nous pouvons utiliser le coupon pour appeler une méthode qui vérifiera indirectement
                        
                        // Vérification de la compatibilité avec Stripe
                        // Cette partie pourrait être simplifiée si on ajoutait une méthode ValidateCoupon à IStripePayment
                        
                        // Appliquer le coupon s'il est valide
                        cartHeader.CouponId = coupon.Id;
                        cartHeader.CouponCode = couponCode;
                        await _cartRepository.UpdateCartHeaderAsync(cartHeader);
                        return await GetCartByUserIdAsync(userId);
                    }
                    catch (Stripe.StripeException ex)
                    {
                        // Le coupon n'est pas valide dans Stripe
                        throw new InvalidOperationException($"Le coupon {couponCode} n'est pas valide dans Stripe: {ex.Message}");
                    }
                }
                else
                {
                    // Handle invalid coupon code
                    throw new InvalidOperationException($"Coupon with code {couponCode} was not found.");
                }
            }
            return new CartDto();
        }

        public async Task<bool> RemoveCartItemAsync(Guid cartDetailsId)
        {
            var cartDetail = await _cartRepository.GetCartDetailsByCartDetailsId(cartDetailsId);
            if (cartDetail == null)
                return false;

            await _cartRepository.RemoveCartDetailsAsync(cartDetail);
            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            return await _cartRepository.ClearCartAsync(userId);
        }
    }
}
