using System.Globalization;
using System.Net;
using CsvHelper;
using Newtonsoft.Json;
using RestSharp;
using TTNETFİBER;


public static class netspeed
{
    public  static void Run()
    { bool isFirstDepartment = true;
        List<ResultModel> resultList = new List<ResultModel>();
        RestClientOptions options = new RestClientOptions
        {
            BaseUrl = new Uri("https://www.netspeed.com.tr"),
        };
        RestClient client = new RestClient(options);

        var request =
            new RestRequest(
                "/Home/GetAddress", Method.Post);

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        request.AddParameter("type", "2");
        request.AddParameter("id", "1199");
        new RestRequest(
            "/Home/GetAddress", Method.Post);

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        request.AddParameter("type", "3");
        request.AddParameter("id", "6554");

        var getSemtlist =  client.ExecuteAsync<ResponseModel>(request).Result;


        foreach (var SemtItem in getSemtlist.Data.Data.Where(x=>x.Name.ToLower()=="merkez"))
        {
            
            request =
                new RestRequest(
                    "/Home/GetAddress", Method.Post);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddParameter("type", "4");
            request.AddParameter("id", SemtItem.Code);

            var getMahalleList =  client.ExecuteAsync<ResponseModel>(request).Result;

            foreach (var getMahalleItem in getMahalleList.Data.Data)
            {
                request =
                    new RestRequest(
                        "/Home/GetAddress", Method.Post);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.AddParameter("type", "3");
                request.AddParameter("id", getMahalleItem.Code);

                var getSokakList =  client.ExecuteAsync<ResponseModel>(request).Result;


                foreach (var sokakItem in getSokakList.Data.Data)
                {
                    request =
                        new RestRequest(
                            "/Home/GetAddress", Method.Post);

                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    request.AddParameter("type", "4");
                    request.AddParameter("id", sokakItem.Code);

                    var getApartmanList =  client.ExecuteAsync<ResponseModel>(request).Result;

                    foreach (var apartmentItem in getApartmanList.Data.Data)
                    {
                        request =
                            new RestRequest(
                                "/Home/GetAddress", Method.Post);

                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                        request.AddParameter("type", "5");
                        request.AddParameter("id", apartmentItem.Code);

                        var getKapiNoList =  client.ExecuteAsync<ResponseModel>(request).Result;

                        foreach (var daireItem in getKapiNoList.Data.Data)
                        {
                            request =
                                new RestRequest(
                                    "/Home/GetInfrastractureQueryResult", Method.Post);

                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                            request.AddParameter("GetInfrastractureQueryResult", daireItem.Code);


                            var getResult =  client.ExecuteAsync<string>(request).Result;

                            if (!string.IsNullOrEmpty(getResult.Data))
                            {
                                var ConvertedResult =
                                    Newtonsoft.Json.JsonConvert.DeserializeObject<ResultModel>(getResult.Data);

                                var lastResult = new BeautifiedModel("Bolu", "Merkez", SemtItem.Name, getMahalleItem.Name,
                                    getMahalleItem.Code, sokakItem.Name, sokakItem.Code, apartmentItem.Name,
                                    apartmentItem.Code, daireItem.Name, daireItem.Code, ConvertedResult);
                             Console.WriteLine(JsonConvert.SerializeObject(lastResult));
                             WriteToCsv(lastResult, "sonuc.csv",isFirstDepartment);
                             isFirstDepartment = false;
                            }
                            else
                            {
                                ResultModel bos = new ResultModel();
                                var lastResult = new BeautifiedModel("Bolu", "Merkez", SemtItem.Name, getMahalleItem.Name,
                                    getMahalleItem.Code, sokakItem.Name, sokakItem.Code, apartmentItem.Name,
                                    apartmentItem.Code, daireItem.Name, daireItem.Code, bos);
                                Console.WriteLine(JsonConvert.SerializeObject(lastResult));
                               

                                WriteToCsv(lastResult, "sonuc.csv",isFirstDepartment);
                                isFirstDepartment = false;

                               
                            }

                         
                        }
                    }
                }
            }
        }

        static void WriteToCsv(BeautifiedModel result, string filePath,bool writeHeader)
        {
            using (var writer = new StreamWriter(filePath, true)) // Append modunu true olarak ayarlayın
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if(writeHeader)
                {
                    csv.WriteHeader<BeautifiedModel>();
                    csv.NextRecord();
                }
                csv.WriteRecord(result);
                writer.WriteLine();
            }
        }
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