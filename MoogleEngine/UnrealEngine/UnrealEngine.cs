namespace MoogleEngine;
public class UnrealEngine
{
    public static string path = Directory.GetCurrentDirectory() + "..//..//Content";
    private Document[] documents;
    private Dictionary<string, (int frequency, int index)> documentsFrequencyAndIndexes;
    private double[,] TFIDF;

    public UnrealEngine() //Con este constructor iniciamos lo que es toda la estructura antes de la busqueda
    {
        documents = GetDocuments();
        documentsFrequencyAndIndexes = GetDocsFrecuencyAndIndex();
        TFIDF = GetTFIDFMatrix();
    }
    private Document[] GetDocuments() //Obtener todos los documentos en path
    {
        string[] documentPaths = Directory.GetFiles(path);
        Document[] docs = new Document[documentPaths.Length];

        for (int i = 0; i < docs.Length; i++)
        {
            docs[i] = new Document(documentPaths[i]);
        }
        return docs;
    }
    private Dictionary<string, (int, int)> GetDocsFrecuencyAndIndex() //Obtener cada termino con su frecuencia entre todos los documento y un indice asociados
    {
        Dictionary<string, (int frequency, int index)> docsFrequencyAndIndex = new Dictionary<string, (int frequency, int index)>();

        foreach (var doc in documents)//Vamos recorriendo cada documento 
        {
            foreach (var term in doc.Terms)//Aqui vamos recorriendo cada termino de ese documento
            {
                if (docsFrequencyAndIndex.ContainsKey(term))//Vamos llenando nuestro diccionario general
                {
                    docsFrequencyAndIndex[term] = (docsFrequencyAndIndex[term].frequency + 1, docsFrequencyAndIndex[term].index);
                }
                else
                {
                    docsFrequencyAndIndex.Add(term, (1, docsFrequencyAndIndex.Count));
                }
            }
        }
        return docsFrequencyAndIndex;
    }
    private double[,] GetTFIDFMatrix()//Aqui vamos a crear la Matriz de documentos
    {
        //Va a tener tamaño cantidad de documentos por cantidad de palabras del corpus
        double[,] tfIdf = new double[documents.Length, documentsFrequencyAndIndexes.Count];
        for (int i = 0; i < documents.Length; i++)//vamos recorriendo los documentos
        {
            //Por cada documento vamos a sacar su vector TFIDF e ir rellenando la matriz
            //Este proceso de obtener el vector del documento lo realiza mi clase documento
            //Clase encargada de todo lo necesario para los documentos
            double[] docVector = documents[i].GetVector(documentsFrequencyAndIndexes, documents.Length);
            for (int j = 0; j < docVector.Length; j++)
            {
                tfIdf[i, j] = docVector[j];
            }
        }
        return tfIdf;
    }
    public (SearchItem[] searchItems, string suggestion) Query(string query)
    {
        //Con este metodo ya estamos asegurando el retorno de los valores necesarios para el usuario
        Query queryDoc = new Query(query);
        //Con la clase Query ya tenemos lo necesario para la query q entre el usuario (aun falta el trabajo segun operadores)
        double[] queryVector = queryDoc.GetVector(documentsFrequencyAndIndexes, documents.Length);
        //Guardamos el vector de la query 
        double[] cosSimilarity = GetCosSimilarityVector(queryVector);
        //Guardamos la similitud del coseno entre la query y los documentos
        int[] indexVect = Enumerable.Range(0, cosSimilarity.Length).ToArray();
        //Creo un array de indices para poder ordenar mi vector similitud y saber q indices se movieron
        //Asi mantengo un control sobre los documentos mas importantes
        Array.Sort(cosSimilarity, indexVect);

        string[] queryTerms = SortQueryTerms(queryDoc, queryVector);

        SearchItem[] searchItems = GetSearchItems(cosSimilarity, indexVect, queryTerms);//Guardamos los searchItems

        string suggestion = string.Empty;

        if (NeedSuggestion(queryDoc.Terms))
        {
            suggestion = GetSuggestions(queryDoc.Terms);
        }//Verificamos si hay necesidad de sugerencia segun los terminos de la query

        return (searchItems, suggestion);
    }
    public bool NeedSuggestion(string[] queryTerms)
    {
        int count = 0;
        for (int i = 0; i < queryTerms.Length; i++)
        {
            if (documentsFrequencyAndIndexes.ContainsKey(queryTerms[i]))
            {
                count++;
            }
        }
        if (count < queryTerms.Length) return true;
        return false;
    }
    private string GetSuggestions(string[] queryTerms)//Metodo para la sugerencia en caso de que fuese necesaria
    {
        string suggestion = "";

        for (int i = 0; i < queryTerms.Length; i++)
        {
            suggestion += GetSuggestion(queryTerms[i], documentsFrequencyAndIndexes) + " ";
        }
        return suggestion;
    }
    private SearchItem[] GetSearchItems(double[] cosSimilarity, int[] indexVect, string[] queryTerms)
    {//Elegimos los mejores documentos y los devolvemos 
        List<SearchItem> searchItems = new List<SearchItem>();
        for (int i = cosSimilarity.Length - 1; i >= cosSimilarity.Length - 5; i--)
        {
            if (cosSimilarity[i] > 0)
            {
                string snipet = documents[indexVect[i]].GetSnipet(queryTerms);
                searchItems.Add(new SearchItem(documents[indexVect[i]].Title, snipet, (float)cosSimilarity[i]));
            }
            else break;
        }
        return searchItems.ToArray();
    }
    public string GetSuggestion(string queryTerm, Dictionary<string, (int frequency, int index)> documentsFrequencyAndIndexes)
    {
        //Una vez aqui se utiliza el metodo de la distancia de levenshtein este metodo se encarga de ver
        //cual es la minima cantidad de pasos que hay q hacer para transformar una palabra en otra
        //recorremos todos nuestros terminos en nuestro diccionario general y vamos guardando en una
        //variable la mejor de las distancias (seria la menor) vamos guardando esos terminos en una lista
        //y tenemos la sugerencia
        List<string> suggestions = new List<string>();
        double best = int.MaxValue;
        foreach (var term in documentsFrequencyAndIndexes.Keys)
        {
            double levenshteinDistance = LevenshteinDistance(queryTerm, term);
            if (levenshteinDistance < best)
            {
                best = levenshteinDistance;
                suggestions.Add(term);
            }
        }
        return suggestions[suggestions.Count - 1];
    }
    private string[] SortQueryTerms(Query queryDoc, double[] queryVector)
    {
        string[] terms = queryDoc.Terms;

        for (int i = 0; i < terms.Length - 1; i++)
        {
            for (int j = i + 1; j < terms.Length; j++)
            {
                //Aqui ordenamos segun el valor de los elementos de la query en su vector query
                double leftValue = documentsFrequencyAndIndexes.ContainsKey(terms[i]) ? queryVector[documentsFrequencyAndIndexes[terms[i]].index] : 0;
                double rigthValue = documentsFrequencyAndIndexes.ContainsKey(terms[j]) ? queryVector[documentsFrequencyAndIndexes[terms[j]].index] : 0;
                if (leftValue < rigthValue)
                {
                    string temp = terms[j];
                    terms[j] = terms[i];
                    terms[i] = temp;
                }
            }
        }
        return terms;
    }
    private double[] GetCosSimilarityVector(double[] queryVector)//Obtenemos el vector similitud del coseno
    {
        double[] cosSimilarityVector = new double[documents.Length];

        for (int i = 0; i < documents.Length; i++)
        {
            cosSimilarityVector[i] = GetCosSimilarity(queryVector, i);
            //Vamos obteniendo la similitud del coseno por documento y la vamos guardando
        }
        return cosSimilarityVector;
    }

    private double GetCosSimilarity(double[] queryVector, int docIndex)
    {
        // La similitud del coseno nos dice q tanto se "parecen" dos vectores mediante operaciones algebraicas
        //La similitud del coseno es el producto punto entre los dos vectores sobre la multiplicacion de las normas
        //de cada vector
        //Mientras mas cercano a 1 es ese valor mas parecidos serian los documentos
        double dotProduct = 0;
        double docNorm = 0;
        double queryNorm = GetNorm(queryVector);
        if (queryNorm == 0) return 0;

        for (int i = 0; i < TFIDF.GetLength(1); i++)
        {
            dotProduct += queryVector[i] * TFIDF[docIndex, i];
            docNorm += TFIDF[docIndex, i] * TFIDF[docIndex, i];
            //Norma del documento (docNorm) que no es mas que la suma de los cuadrados de cada valor
            //TFIDF por palabra del documento
        }
        docNorm = Math.Sqrt(docNorm);//Y la raiz cuadrada de todo ese valor
        double cosSimilarity = dotProduct / docNorm * queryNorm;
        return cosSimilarity;
    }
    private double GetNorm(double[] vector)//QUERYVECTOR
    {
        double norm = 0;

        for (int i = 0; i < vector.Length; i++)
        {
            norm += vector[i] * vector[i];
        }
        return Math.Sqrt(norm);
    }
    public int LevenshteinDistance(string query, string word)
    {
        double porcentaje = 0;
        // d es una tabla con m+1 renglones y n+1 columnas
        int costo = 0;
        int m = query.Length;
        int n = word.Length;
        int[,] d = new int[m + 1, n + 1];

        // Verifica que exista algo que comparar
        if (n == 0) return m;
        if (m == 0) return n;

        // Llena la primera columna y la primera fila.
        for (int i = 0; i <= m; d[i, 0] = i++) ;
        for (int j = 0; j <= n; d[0, j] = j++) ;

        /// recorre la matriz llenando cada unos de los pesos.
        /// i columnas, j renglones
        for (int i = 1; i <= m; i++)
        {
            // recorre para j
            for (int j = 1; j <= n; j++)
            {
                /// si son iguales en posiciones equidistantes el peso es 0
                /// de lo contrario el peso suma a uno.
                costo = (query[i - 1] == word[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1,  //Eliminacion
                d[i, j - 1] + 1),                             //Insercion 
                d[i - 1, j - 1] + costo);                     //Sustitucion
            }
        }
        /// Calculamos el porcentaje de cambios en la palabra.
        if (query.Length > word.Length)
            porcentaje = ((double)d[m, n] / (double)query.Length);
        else
            porcentaje = ((double)d[m, n] / (double)word.Length);
        return d[m, n];
    }
}