namespace Formationn_Ecommerce.Core.Common
{
    // Classe abstraite de base dont héritent toutes les entités du système
    public abstract class BaseEntity
    {
        // Identifiant unique de l'entité (clé primaire)
        public Guid Id { get; set; }
        
        // Date de création de l'entité dans la base de données
        public DateTime CreatedDate { get; set; }
        
        // Date de dernière modification de l'entité (nullable)
        public DateTime? LastModifiedDate { get; set; }
    }
}
