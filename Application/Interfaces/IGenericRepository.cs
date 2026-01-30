namespace Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // a Task "aszinkron Promise" háttérbe végez közbe más műveleteket is

        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();


        // Ez az egyik leggyakoribb félreértés az Entity Frameworknél.
        // Az EF Core-ban az Update és a Delete műveletek csak a memóriában
        // jelölik meg az entitást (átállítják az állapotát Modified vagy Deleted állapotra).
        // Mivel ez csak egy memória-művelet, ez nanoszekundumok alatt lefut.
        // Nincs értelme aszinkronná tenni, mert nincs mire várni (nem megy ki a hálózatba).

        // Az EF Core úgynevezett Unit of Work mintát követ. Ez azt jelenti,
        // hogy bármit csinálsz (Add, Update, Delete), az csak a memóriában történik meg,
        // amíg ki nem adod a parancsot: "Most már tényleg küldd el az összes változtatást
        // egyszerre az SQL szervernek!" Ez a SaveChangesAsync.
        // Miért jó ez? Mert ha 10 terméket adsz hozzá,
        // nem 10-szer küldünk egy-egy kérést az adatbázisnak, hanem egyetlen nagy csomagban,
        // ami sokkal gyorsabb.


        // 5. Miért nem ID alapú a törlés a Repository-ban?
        // Ez egy építészeti döntés.Két megközelítés van:ID alapú:
        // Delete(int id) -> Belül le kell kérdeznie az adatbázisból az objektumot,
        // majd törölni. (Ez +1 felesleges kör az adatbázishoz).
        // Objektum alapú: Delete(T entity) -> A hívó fél(a Service) általában már úgyis
        // lekérdezte az objektumot(pl.ellenőrizte, hogy létezik-e). Így a Repository-nak
        // csak átadjuk az objektumot, és ő tudja, mit kell tennie.
    }
}
