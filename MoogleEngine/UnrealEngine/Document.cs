using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public class Document// Clase encargada de todo el procesamiento de los documentos
    {
        private Dictionary<string, (int frequency, int docIndex)> termsFrequencyAndIndexInDoc = new Dictionary<string, (int frecuency, int docIndex)>();
        //Con este diccionario guardamos todas las palabras unicas del documento ademas de que guardamos
        //cuantas veces se repite ese termino (valor necesario para el calculo de tfidf)
        //y almacenamos el indice de la primera vez que fue encontrada la palabra en el texto
        public string Title { get; } 
        public string DocumentText { get; }
        public Document(string filePath)
        {
            Title = Path.GetFileNameWithoutExtension(filePath);
            DocumentText = File.ReadAllText(filePath);
            termsFrequencyAndIndexInDoc = GetTermsFrequency();
        }
        private Dictionary<string, (int frecuency, int docIndex)> GetTermsFrequency()
        {
            //Metodo para obtener cuantas veces se repite una palabra por documento y el indice en el texto
            Dictionary<string, (int frecuency, int docIndex)> termsFreq = new Dictionary<string, (int frecuency, int docIndex)>();
            var matches = Regex.Matches(DocumentText, @"\w+", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            //Regex.Matches recibe el texto del documento y mediante el patron definido guardamos todas las
            //palabras q coincidan (Tokenizamos el texto y guardamos las palabras en el diccionario)
            foreach (Match match in matches)
            {
                //Por cada coincidencia la llevamos a minuscula y almacenamos en el diccionario
                //si ya la tenemos aumentamos su frecuencia
                string term = match.Value.ToLower();
                if (termsFreq.ContainsKey(term))
                    termsFreq[term] = (termsFreq[term].frecuency + 1, termsFreq[term].docIndex);
                else
                    termsFreq.Add(term, (1, match.Index));
            }
            return termsFreq;
        }
        public string[] Terms => GetTerms();
        private string[] GetTerms()
        {
            string[] terms = new string[termsFrequencyAndIndexInDoc.Count];
            int count = 0;

            foreach (var term in termsFrequencyAndIndexInDoc.Keys)
            {
                terms[count] = term;
                count++;
            }
            return terms;
        }
        public double[] GetVector(Dictionary<string, (int frequency, int index)> documentsFrequencyAndIndexes, int docsCount)
        {
            //Obtenemos el vector TFIDF de ese documento
            //Recibimos como parametros el diccionario general de palabras y el total de documentos
            //El TF no es mas que la cantidad de veces que se repite una palabra sobre la cantidad de palabras
            //(vemos la importancia de esa palabra en el documento)
            //El IDF no es mas que la cantidad de documentos sobre la cantidad de documentos en las que sale ese
            //termino
            double[] vect = new double[documentsFrequencyAndIndexes.Count];

            foreach (var term in Terms)
            {
                double tf = termsFrequencyAndIndexInDoc[term].frequency / (double)Terms.Length;
                double idf = Math.Log(docsCount / (double)documentsFrequencyAndIndexes[term].frequency);
                vect[documentsFrequencyAndIndexes[term].index] = tf * idf;
            }
            return vect;
        }
        public string GetSnipet(string[] queryTerms)//Obtenemos una porcion del texto donde aparezca el termino de la query
        {
            string snipet = "";

            for (int i = 0; i < queryTerms.Length; i++)
            {
                if (termsFrequencyAndIndexInDoc.ContainsKey(queryTerms[i]))
                {
                    int indexTerm = termsFrequencyAndIndexInDoc[queryTerms[i]].docIndex;

                    snipet = DocumentText.Substring(Math.Max(0, indexTerm - 30), Math.Min(DocumentText.Length - Math.Max(0, indexTerm - 30), 60 + queryTerms[i].Length));
                    return snipet;
                }
            }
            return snipet;
        }
    }
}