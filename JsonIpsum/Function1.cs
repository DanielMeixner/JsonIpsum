using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JsonIpsum
{
    public static class Function1
    {
        [FunctionName("LoremFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");

            string nrOfElems = req.Query["nrOfElems"].ToString();
            string objSizeMin = req.Query["objSizeMin"].ToString();
            string objSizeMax = req.Query["objSizeMax"].ToString();
            string depth = req.Query["depth"].ToString();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
         
            depth = depth ?? data?.depth;
            nrOfElems = nrOfElems ?? data?.nrOfElems;
            objSizeMin = objSizeMin ?? data?.objSizeMin;
            objSizeMax = objSizeMax ?? data?.objSizeMax;

            int iDepth, iNrOfElems , iObjSizeMin , iObjSizeMax ;
            if(!Int32.TryParse(depth, out iDepth))
            {
                iDepth = 5;
            }
            if (!Int32.TryParse(nrOfElems, out iNrOfElems))
            {
                iNrOfElems = 5;
            }
            if (!Int32.TryParse(objSizeMin, out iObjSizeMin))
            {
                iObjSizeMin = 1;
            }
            if (!Int32.TryParse(objSizeMax, out iObjSizeMax))
            {
                iObjSizeMax = 1;
            }

            return (ActionResult)new OkObjectResult(GenerateResultJson(iNrOfElems,iDepth, iObjSizeMin, iObjSizeMax));
        
        }
        static Random r = new Random();
        static private String GenerateResultJson(int nrOfElems = 5, int depth = 5, int objSizeMin=1, int objSizeMax=4)
        {

            if(nrOfElems<=0 || nrOfElems> 10)
            {
                nrOfElems = 10;
            }

            if (depth <= 0 || depth > 10)
            {
                depth = 10;
            }

            if (objSizeMin <= 0 || objSizeMin > 10)
            {
                objSizeMin = 5;
            }

            if (objSizeMax <= 0 || objSizeMax > 10)
            {
                objSizeMax = 10;
            }

            var resString = String.Empty;
            string ipsumString = File.ReadAllText("lorem.txt");

            string[] ipArr = ipsumString.Split(" ");
            for(int i=0; i< ipArr.Length; i++)
            {
                ipArr[i] = ipArr[i].Trim();
            }

           
            var rand = r.Next(objSizeMin,objSizeMax);
            var ix = 0;

            string o = "{}";

            var resob = CreateIpsumObject(nrOfElems, depth, objSizeMin, objSizeMax,  ix, ipArr);

            resString = JsonConvert.SerializeObject(resob);
            return resString;
        }

        private static dynamic CreateIpsumObject(int nrOfElems, int depth, int objSizeMin, int objSizeMax, int ix, string [] ipArr) 
        {
            dynamic myob = null;
            string o = "{}";
            myob = JsonConvert.DeserializeObject(o);
            for (int elemsCounter = 0; elemsCounter < nrOfElems; elemsCounter++)
            {

                for (int depthCounter = 0; depthCounter < depth; depthCounter++)
                {
                    var num = r.Next(objSizeMin, objSizeMax);
                    int n = 0;
                    while (n++ < num)
                    {
                        int randy = r.Next(1, 10);
                        bool createObject = (randy < 5);
                        if (createObject)
                        {
                            myob[ipArr[ix++]] = CreateIpsumObject(1, 1, 1, 1, ix, ipArr);
                        }
                        else
                        {
                            myob[ipArr[ix++]] = ipArr[ix++];
                        }
                    }

                }
            }
            return myob;
        }
    }
}
