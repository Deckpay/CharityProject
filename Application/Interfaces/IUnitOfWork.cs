using Domain.Entities;

namespace Application.Interfaces
{
    // Az IDisposable biztosítja, hogy az adatbázis-kapcsolat lezáruljon, ha végeztünk
    public interface IUnitOfWork : IDisposable
    {
        // Elérhetővé tesszük a konkrét repository-kat
        IGenericRepository<Product> Products { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<ProductCategory> Categories { get; }
        IGenericRepository<County> Counties { get; }
        IGenericRepository<Chat> Chats { get; }
        IGenericRepository<ChatMessage> ChatMessages { get; }
        IGenericRepository<ProductRequest> ProductRequests { get; }

        // Ez a metódus menti el az összes változtatást (Tranzakció kezelés)
        Task<int> CompleteAsync();

    }
}
