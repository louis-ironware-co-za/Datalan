using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using time;

//using hmac;

//using hashlib;

//using requests;

//using urllib.parse;

//using datetime = datetime.datetime;

//using timezone = datetime.timezone;

//using System.Collections.Generic;

//using System;

//using System.Linq;


namespace Coinigy.API
{
    class PythonExchanges
    {


        public static class Module
        {

            public static object BASE_URL = "https://api.coinigy.com";

            public static object ENDPOINT = "/api/v2/private/exchanges";

            public static object X_API_KEY = "keykeykeykeykeykeykeykeykeykeyke";

            public static object SECRET = "secretsecretsecretsecretsecretse";

            public static object METHOD = "GET";

            public static object UNIXTIME = DateTime.Now;  //datetime.fromtimestamp(1532718830, timezone.utc);

            public static object PARAMS = new Dictionary<object, object> {
            {
                "pythagoreanTheorem",
                "a^2+b^2=c^2"}};

            public static object BODY = "";

            public static object X_API_TIMESTAMP = ""; //Convert.ToInt32(datetime.timestamp(UNIXTIME)).ToString();

            public static object query_string = "?"; //+ "&".join((from key in PARAMS.keys()
                                                     //          select (key + "=" + urllib.parse.quote_plus(PARAMS[key]))).ToList());

            public static object msg = "";  //X_API_KEY + X_API_TIMESTAMP + METHOD + ENDPOINT + query_string.Count > 1 ? query_string : "" + BODY;

            //public static object signature_bytes = hmac.@new(SECRET.encode("ascii"), msg.encode("ascii"), digestmod: hashlib.sha256).digest();

            //public static object signature_hex = map("{:02X}".format, signature_bytes);

            //public static object X_API_SIGN = "".join(signature_hex);

        }
    }
}
  
    //public static object headers = new Dictionary<object, object> {
    //    {
    //        "Accept",
    //        "application/json"},
    //    {
    //        "Content-Type",
    //        "application/json"},
    //    {
    //        "X-API-SIGN",
    //        X_API_SIGN},
    //    {
    //        "X-API-TIMESTAMP",
    //        X_API_TIMESTAMP},
    //    {
    //        "X-API-KEY",
    //        X_API_KEY}};

    //public static object r = requests.get(BASE_URL + ENDPOINT, headers: headers, params: PARAMS, data: BODY);



    //import time
    //import hmac
    //import hashlib
    //import requests

    //BASE_URL = 'https://api.coinigy.com'
    //ENDPOINT = '/api/v2/private/exchanges'
    //X_API_KEY = 'your_api_v2_key_goes_here'
    //SECRET = 'your_corresponding_api_v2_secret_goes_here'
    //METHOD = 'GET'
    //UNIXTIME = time.time()
    //PARAMS = { } # = {'StartDate':'2018-07-30T00:00:00.000Z','EndDate':'2018-07-30T23:59:59.000Z'}
    //BODY = ''

    //X_API_TIMESTAMP = str(int(UNIXTIME))
    //query_string = "?" + "&".join([key + '=' + urllib.parse.quote_plus(PARAMS[key]) for key in PARAMS.keys()] )
    //msg = X_API_KEY + X_API_TIMESTAMP + METHOD + ENDPOINT + (query_string if len(query_string) > 1 else '') + BODY
    //signature_bytes = hmac.new(SECRET.encode("ascii"), msg.encode("ascii"), digestmod=hashlib.sha256).digest()
    //signature_hex = map("{:02X}".format, signature_bytes)
    //X_API_SIGN = ''.join(signature_hex)

    //headers = {'Accept': 'application/json', 'Content-Type': 'application/json', 'X-API-SIGN': X_API_SIGN, 'X-API-TIMESTAMP' : X_API_TIMESTAMP, 'X-API-KEY': X_API_KEY
    //    }
    //    r = requests.get(BASE_URL + ENDPOINT, headers=headers, params=PARAMS, data=BODY)

    //print(r.status_code, r.reason, r.content, BASE_URL + ENDPOINT)

    //    }

