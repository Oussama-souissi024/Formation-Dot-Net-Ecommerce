# Formation-Dot-Net-Ecommerce

Projet de formation complet couvrant la création d'une application e-commerce en **ASP.NET Core 8** avec une architecture en couches (Clean Architecture).

## Sommaire
- [Aperçu](#aperçu)
- [Fonctionnalités](#fonctionnalités)
- [Architecture](#architecture)
- [Prérequis](#prérequis)
- [Configuration](#configuration)
- [Lancement de l'application](#lancement-de-lapplication)
- [Tests](#tests)
- [Structure du dépôt](#structure-du-dépôt)
- [Contribuer](#contribuer)
- [Licence](#licence)

## Aperçu
Cette application illustre la plupart des fonctionnalités essentielles d'une plateforme e-commerce : gestion des produits et catégories, panier, coupons de réduction, passage de commandes, intégration du paiement **Stripe** et authentification sécurisée avec **ASP.NET Core Identity**.

## Fonctionnalités
- Authentification / autorisation (inscription, connexion, rôles)  
- CRUD Produits & Catégories  
- Panier d'achat et calcul dynamique des totaux  
- Coupons de réduction (création, application, suppression)  
- Passage de commande & historique  
- Paiement Stripe : Checkout Session, validation, remboursement  
- API RESTful avec DTOs & AutoMapper  
- Couche Tests unitaires (xUnit)  
- Architecture Clean / DDD  

## Architecture
Le projet suit une structure en couches afin de séparer clairement les responsabilités :

```text
┌────────────────────────────┐
│        Presentation        │  Web (Controllers, Views/API, Program)
└────────────┬───────────────┘
             │
┌────────────▼───────────────┐
│         Application        │  Services, DTOs, Profiles
└────────────┬───────────────┘
             │
┌────────────▼───────────────┐
│           Core             │  Entités, Interfaces, Domain Logic
└────────────┬───────────────┘
             │
┌────────────▼───────────────┐
│        Infrastructure      │  Persistence (EF Core), Repositories,
│                            │  External services (Stripe…)
└────────────────────────────┘
```

### Couche Presentation (Formationn_Ecommerce)

Responsable de l'interface utilisateur et de l'API Web :

- **Controllers/** : Gestion des requêtes HTTP et coordination des actions
  - `AuthController` : Inscription, connexion et gestion des rôles
  - `ProductController` : CRUD pour les produits avec filtrage par catégorie
  - `CartController` : Gestion du panier d'achat 
  - `CouponController` : Application des codes promo
  - `OrderController` : Traitement des commandes
- **Views/** : Vues Razor MVC et composants partiels
- **Models/** : ViewModels spécifiques à la présentation
- **Program.cs** : Configuration de l'application, DI, middleware
- **wwwroot/** : Ressources statiques (CSS, JS, images)

### Couche Application (Formationn_Ecommerce.Application)

Orchestre la logique métier :

- **Authentication/** : Services d'authentification
  - `AuthService` : Encapsule la logique métier liée à l'authentification
  - `Interfaces/IAuthService` : Contrat pour les opérations d'authentification
  - `DTOs/` : LoginRequestDto, RegistrationRequestDto, etc.
- **Products/** : Gestion des produits
  - `Services/ProductServices` : Manipulation des produits
  - `DTOs/ProductDto` : Transfert des données produits
  - `Profiles/ProductProfile` : Mappage entre entités et DTOs
- **Categories/**, **Cart/**, **Coupons/**, **Order/** : Structure similaire pour chaque domaine
- **Common/** : DTOs et services partagés

### Couche Core (Formationn_Ecommerce.Core)

Contient les entités et règles métier fondamentales :

- **Entities/** : Classes de domaine
  - `Product`, `Category`, `Cart`, `CartItem`
  - `Coupon`, `OrderHeader`, `OrderDetails`
  - `Identity/ApplicationUser` : Utilisateur personnalisé
- **Interfaces/** : Contrats pour les repositories et services
  - `Repositories/` : IProductRepository, ICategoryRepository, etc.
  - `Services/` : IStripePayment et autres interfaces de service
- **Not Mapped Entities/** : Objets de transfert non persistés
- **Utility/** : Classes utilitaires et constantes

### Couche Infrastructure (Formationn_Ecommerce.Infrastucture)

Implémente l'accès aux données et services externes :

- **Persistence/** : Accès aux données
  - `ApplicationDbContext` : Contexte EF Core
  - `Repositories/` : Implémentations des repositories
  - `ProductRepository` : Inclut Category dans les requêtes produits
  - `AuthRepository` : Utilise UserManager/SignInManager d'Identity
- **External/** : Services tiers
  - `Payment/StripePayment` : Intégration avec l'API Stripe
    - Sessions de paiement, coupons, remboursements
- **Migrations/** : Migrations Entity Framework 
- **Extension/** : Méthodes d'extension et services d'infrastructure

## Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/)  
- **SQL Server** (LocalDB ou instance)  
- Compte **Stripe** (clés *test* Publishable & Secret)  
- (Optionnel) Node.js pour assets front-end  

## Configuration

1. Dupliquer `appsettings.json` → `appsettings.Development.json`.
2. Adapter la chaîne de connexion et les clés Stripe :

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\\\MSSQLLocalDB;Database=EcommerceDB;Trusted_Connection=True;"
  },
  "Stripe": {
    "PublishableKey": "pk_test_xxxxx",
    "SecretKey": "sk_test_xxxxx"
  }
}
```

3. Générer la base de données :

```bash
dotnet ef database update --project Formationn_Ecommerce.Infrastucture
```

## Lancement de l'application

```bash
# Compilation
dotnet build

# Exécution
dotnet run --project Formationn_Ecommerce
```

L'application est alors accessible sur `https://localhost:5001` (ou selon votre profile `launchSettings.json`).

## Tests

```bash
dotnet test
```

## Structure du dépôt

```text
Formation-Dot-Net-Ecommerce
│
├─ Formationn_Ecommerce/                # Projet Web (API / MVC)
├─ Formationn_Ecommerce.Application/    # Logique métier
├─ Formationn_Ecommerce.Core/           # Entités & Interfaces
├─ Formationn_Ecommerce.Infrastucture/  # Accès aux données, Stripe, …
├─ Formationn_Ecommerce.Test/           # Tests unitaires
└─ README.md
```

## Contribuer
Les contributions sont les bienvenues !  
1. Forker le dépôt  
2. Créer une branche feature (`git checkout -b feature/ma-feature`)  
3. Commit (`git commit -m 'Ajout …'`)  
4. Push (`git push origin feature/ma-feature`)  
5. Ouvrir une *Pull Request*

## Objectifs pédagogiques et acquis de la formation

Cette formation vise à transmettre des compétences avancées dans le développement d'applications .NET en suivant les principes de la Clean Architecture et du Domain-Driven Design.

### Objectifs généraux

- **Compréhension de la Clean Architecture** : principes SOLID, séparation des préoccupations, indépendance des frameworks
- **Décomposition structurée d'un projet** : organisation en couches avec responsabilités distinctes
- **Conception d'applications évolutives** : bases solides pour une maintenabilité à long terme
- **Intégration concrète avec des services tiers** : implémentation des paiements en ligne

### Compétences acquises

À l'issue de la formation, les étudiants seront capables de :

1. **Architecturer une application** :
   - Découpler le code en couches indépendantes et testables
   - Implémenter les principes SOLID dans une application concrète
   - Assurer l'indépendance du domaine métier vis-à-vis des frameworks

2. **Implémenter des patterns essentiels** :
   - Repository Pattern pour l'abstraction de l'accès aux données
   - Unit of Work pour la gestion des transactions
   - CQRS simplifié via les DTOs et Services
   - Dependency Injection native d'ASP.NET Core

3. **Développer des fonctionnalités e-commerce** :
   - Système d'authentification sécurisé avec Identity
   - Panier d'achat avec sessions & persistance
   - Gestion de coupons et réductions
   - Intégration de passerelles de paiement (Stripe)

4. **Maîtriser les outils .NET modernes** :
   - Entity Framework Core et migrations 
   - AutoMapper pour la transformation de données
   - ASP.NET Core MVC & API
   - Tests unitaires avec xUnit

Ces acquis permettent aux étudiants de construire des applications professionnelles maintenables, évolutives et robustes, en appliquant les meilleures pratiques de l'industrie du développement logiciel.

## Licence
Ce projet est distribué sous licence **MIT**. Voir le fichier `LICENSE` pour plus d'informations.
