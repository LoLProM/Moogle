namespace MoogleEngine;
public static class Moogle
{
    //Este objeto es perteneciente a la clase que se encargara de todo
    private static UnrealEngine unrealEngine; 
    public static SearchResult Query(string query) { //Metodo inicial que cambiamos

        (SearchItem[] items, string suggestion) = unrealEngine.Query(query);

        return new SearchResult(items, suggestion);
    }

    //Este metodo me crea mi matriz e inicializa el moogle antes de las busquedas de la query
    public static void Initialize() { 
        unrealEngine = new UnrealEngine();
    }
}
