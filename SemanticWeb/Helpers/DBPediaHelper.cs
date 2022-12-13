using SemanticWeb.ViewModel;
using VDS.RDF;
using VDS.RDF.Query;
using System.Text.RegularExpressions;
using VDS.RDF.Nodes;

namespace SemanticWeb.Helpers
{
    public interface IDBPediaHelper
    {
        public IList<BaseRegionData> GetRegionsData();
        public IList<BaseParkData> GetRegionsParksData(string regionLink);
        public ParkData GetParkData(string parksLink);

    }

    public class DBPediaHelper : IDBPediaHelper
    {
        SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");

        public DBPediaHelper()
        {
            endpoint.RdfAcceptHeader = "application/turtle";
        }

        private string GetRegionSparqlQuery()
        {
            return "SELECT DISTINCT  ?location xsd:string(?locationName) as ?locationName xsd:integer(count(?park)) as ?countPark ?image  WHERE {   ?park dbo:wikiPageWikiLink dbr:National_Parks_of_Ukraine. ?park rdf:type dbo:Location;    dbo:location ?location. ?location rdf:type dbo:Location; foaf:name  ?locationName. optional {?location rdf:type dbo:Location; dbo:thumbnail ?image. }   filter(?location != dbr:Ukraine && ?location != dbr:Russia  &&  !regex(ucase(str(?location )), \"RAION\")  &&  !regex(ucase(str(?location )), \"REPUBLIC\")   && regex(ucase(str(?image)), \"FLAG\")). BIND( 0 AS ?CountOfParks ) } GROUP BY ?location ?locationName ?image ";
        }

        private string GetRegionsParksSparqlQuery(string regionLink)
        {
            var splitedData = regionLink.Split("/");
            string regionEndpoint = splitedData[splitedData.Length - 1];
            return "SELECT DISTINCT ?parkLink xsd:string(?name) as ?name ?image  WHERE {  ?parkLink dbo:wikiPageWikiLink dbr:National_Parks_of_Ukraine. ?parkLink rdf:type dbo:Location;    dbo:location ?location. ?parkLink foaf:name| dbp:name ?name .optional { ?parkLink dbo:thumbnail ?image. }filter(?location  = dbr:" + regionEndpoint + "). }GROUP BY(?parkLink )";
        }
        private string GetParkSparqlQuery(string parkLink)
        {
            var splitedData = parkLink.Split("/");
            string parkEndpoint = splitedData[splitedData.Length - 1];
            return "SELECT ?parkLink xsd:string(?name) as ?name ?regionLink xsd:string(?regionName) as ?regionName xsd:string( ?description) as ?description xsd:integer(?areaSize) as ?areaSize ?image  WHERE {   ?parkLink dbo:wikiPageWikiLink dbr:National_Parks_of_Ukraine.  ?parkLink foaf:name | dbo:name | dbp:name ?name; dbo:abstract ?description;  dbo:location ?regionLink. ?regionLink foaf:name ?regionName. optional{ ?parkLink  dbo:thumbnail ?image;  dbo:areaTotal ?areaSize. }  filter(  ?parkLink = dbr:" + parkEndpoint + ") }   LIMIT 1";
        }



        public IList<BaseRegionData> GetRegionsData()
        {
            IList<BaseRegionData> regions = new List<BaseRegionData>();
            BaseRegionData region;
            SparqlResultSet queryResult = endpoint.QueryWithResultSet(GetRegionSparqlQuery());


            foreach (var queryResultsItem in queryResult)
            {
                region = new BaseRegionData();
                region.Name = queryResultsItem["locationName"].ToString();
                region.Link = queryResultsItem["location"].ToString();
                region.ImageLink = queryResultsItem["image"].ToString();
                region.ParksCount = queryResultsItem["countPark"].AsValuedNode().AsInteger();
                regions.Add(region);
            }
            return regions;
        }

        public IList<BaseParkData> GetRegionsParksData(string regionLink)
        {

            IList<BaseParkData> parks = new List<BaseParkData>();
            BaseParkData park;
            SparqlResultSet queryResult = endpoint.QueryWithResultSet(GetRegionsParksSparqlQuery(regionLink));


            foreach (var queryResultsItem in queryResult)
            {
                park = new BaseParkData();
                park.Name = queryResultsItem["name"].ToString();
                park.Link = queryResultsItem["parkLink"].ToString();
                park.ImageLink = queryResultsItem["image"].ToString();
                parks.Add(park);
            }
            return parks;
        }

        public ParkData GetParkData(string parksLink)
        {
            ParkData park = new ParkData();
            SparqlResultSet queryResult = endpoint.QueryWithResultSet(GetParkSparqlQuery(parksLink));

            if (!queryResult.IsEmpty)
            {
                var parksData = queryResult[0]; 
                park.Name = parksData["name"].ToString();
                park.Link = parksData["parkLink"].ToString();
                park.LinkToRegion = parksData["regionLink"].ToString();
                park.Region = parksData["regionName"].ToString();
                park.AreaTotal = parksData["areaSize"].AsValuedNode().AsInteger();
                park.Description = parksData["description"].ToString();
                park.ImageLink = parksData["image"]?.ToString();

            }
            return park;
        }
    }
}
