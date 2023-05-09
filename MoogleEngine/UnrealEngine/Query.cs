using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public class Query//Clase encargada del procesamiento de la query 
    {
        private Dictionary<string, int> termsFrequency = new Dictionary<string, int>();
        public Query(string query)
        {
            termsFrequency = GetTermsFrequency(query);
        }
        private Dictionary<string, int> GetTermsFrequency(string query)
        {
            //Aqui procesamos la query y guardamos los terminos sin rep
            Dictionary<string,int> termsFreq = new Dictionary<string, int>();
            var matches = Regex.Matches(query, @"\w+", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            var eliminate = Regex.Matches(query, @"!\w+");
            var need = Regex.Matches(query, @"\^\w+");
            var importance = Regex.Matches(query, @"\*+\w+");
            //Aun no estan implementados los operadores
            
            foreach (Match match in matches)
            {
                string term = match.Value.ToLower();
                if (termsFreq.ContainsKey(term))
                    termsFreq[term]++;
                else
                    termsFreq.Add(term,1);
            }
            return termsFreq;
        }
        public string[] Terms => GetTerms();
        private string[] GetTerms()
        {
            string[] terms = new string[termsFrequency.Count];
            int count = 0;

            foreach (var term in termsFrequency.Keys){
                terms[count] = term;
                count++;
            }
            return terms;
        }
        public double[] GetVector(Dictionary<string, (int frequency, int index)> documentsFrequencyAndIndexes, int docsCount)
        {
            //Obtenemos el vector TFIDF de la query 
            double[] vect = new double[documentsFrequencyAndIndexes.Count];
            
            foreach (var term in Terms){
                if (!documentsFrequencyAndIndexes.ContainsKey(term)){
                    continue;
                }
                double tf = termsFrequency[term] / (double)Terms.Length;
                double idf = Math.Log(docsCount / (double)documentsFrequencyAndIndexes[term].frequency);
                vect[documentsFrequencyAndIndexes[term].index] = tf*idf;
            }
            return vect; 
        }
    }
}