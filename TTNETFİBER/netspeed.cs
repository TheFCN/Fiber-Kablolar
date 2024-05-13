using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;
using TTNETFİBER;

public static class netspeed
{
    public static void Run()
    {
        string connectionString = "mongodb://localhost:27017";
        var mongoclient = new MongoClient(connectionString);
        var database = mongoclient.GetDatabase("Fiber");
        var collection = database.GetCollection<BeautifiedModel>("Netspeed");

        bool isFirstDepartment = true;
        List<ResultModel> resultList = new List<ResultModel>();
        RestClientOptions options = new RestClientOptions
        {
            BaseUrl = new Uri("https://www.netspeed.com.tr"),
        };
        RestClient client = new RestClient(options);

        var request = new RestRequest("/Home/GetAddress", Method.Post);
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        request.AddParameter("type", "3");
        request.AddParameter("id", "6554");

        var getMahList = client.ExecuteAsync<ResponseModel>(request).Result;

        Parallel.ForEach(getMahList.Data.Data, mahItem =>
        {
           var  request = new RestRequest("/Home/GetAddress", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddParameter("type", "4");
            request.AddParameter("id", mahItem.Code);

            var getSokakList = client.ExecuteAsync<ResponseModel>(request).Result;

            Parallel.ForEach(getSokakList.Data.Data, getSokakItem =>
            {
                var request = new RestRequest("/Home/GetAddress", Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.AddParameter("type", "5");
                request.AddParameter("id", getSokakItem.Code);

                var getBinaList = client.ExecuteAsync<ResponseModel>(request).Result;

                Parallel.ForEach(getBinaList.Data.Data, BinaItem =>
                {
                   var request = new RestRequest("/Home/GetAddress", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    request.AddParameter("type", "6");
                    request.AddParameter("id", BinaItem.Code);

                    var getDaire = client.ExecuteAsync<ResponseModel>(request).Result;

                    Parallel.ForEach(getDaire.Data.Data, daireItem =>
                    {
                        var request = new RestRequest("/Home/GetInfrastractureQueryResult", Method.Post);
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                        request.AddParameter("searchKey", daireItem.Code);

                        var getResult = client.ExecuteAsync<string>(request).Result;

                        if (!string.IsNullOrEmpty(getResult.Data))
                        {
                            var ConvertedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultModel>(getResult.Data);

                            var lastResult = new BeautifiedModel("Bolu", "Merkez", "Merkez", mahItem.Name, mahItem.Code,
                                getSokakItem.Name, getSokakItem.Code, BinaItem.Name, BinaItem.Code, daireItem.Name,
                                daireItem.Code, ConvertedResult);
                            Console.WriteLine(JsonConvert.SerializeObject(lastResult));

                            collection.InsertOne(lastResult);
                            isFirstDepartment = false;
                        }
                        else
                        {
                            ResultModel bos = new ResultModel();
                            var lastResult = new BeautifiedModel("Bolu", "Merkez", "Merkez", mahItem.Name, mahItem.Code,
                                getSokakItem.Name, getSokakItem.Code, BinaItem.Name, BinaItem.Code, daireItem.Name,
                                daireItem.Code, bos);
                            Console.WriteLine(JsonConvert.SerializeObject(lastResult));

                            collection.InsertOne(lastResult);
                            isFirstDepartment = false;
                        }
                    });
                });
            });
        });
    }

    private record BeautifiedModel(
        string Il,
        string Ilce,
        string Semt,
        string Mahalle,
        string MahalleKodu,
        string Sokak,
        string SokakKodu,
        string Apartman,
        string ApartmanKodu,
        string Daire,
        string DaireKodu,
        ResultModel Sonuc)
    {
        public override string ToString()
        {
            return
                $"{{ Il = {Il}, Ilce = {Ilce}, Semt = {Semt}, Mahalle = {Mahalle}, MahalleKodu = {MahalleKodu}, Sokak = {Sokak}, SokakKodu = {SokakKodu}, Apartman = {Apartman}, ApartmanKodu = {ApartmanKodu}, Daire = {Daire}, DaireKodu = {DaireKodu}, Sonuc = {Sonuc} }}";
        }
    }
}
