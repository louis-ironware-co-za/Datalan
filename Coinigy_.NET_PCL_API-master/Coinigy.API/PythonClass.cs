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

//namespace Coinigy.API

//DDA9EE9BEC1F58CB805003DD44E5CA024FBEE5CDEB01E671BED2FD82C3FC6D96
//200 OK b'{"success":true,"error":null,"result":[{"balanceId":15018228430,"balanceAuthId":456582,"fiat":false,"balanceCurrCode":"XRP","balanceCurrId":642,"balanceAmountAvailable":27.0,"balanceAmountHeld":50.0,"balanceAmountTotal":77.0,"quoteBalance":0.00138292,"lastTrade":0.0000180300,"balanceQuoteCurrCode":"BTC","lastPrice":0.00001796,"balanceDate":"2022-04-14T21:53:07Z","balanceHidden":false,"hasImage":true,"balanceCurrSlug":"xrp-ripple","balanceCurrName":"Ripple","balanceCurrImgName":"XRP"}]}' https://api.coinigy.com/api/v2/private/user/balances

    
    public class Module
    {

        //public static object BASE_URL = "https://api.coinigy.com";

        //public static object ENDPOINT = "/api/v2/private/user/balances";

        //public static object X_API_KEY = "8d3e1cbe41f747e2ad5d4b9510ecd55f";

        //public static object SECRET = "12db3e29366b490e8737b53ef7763afc";

        //public static object METHOD = "GET";

        //public static object UNIXTIME = datetime.fromtimestamp(time.time(), timezone.utc);

        //public static object PARAMS = new Dictionary<object, object> {
        //    {
        //        "StartDate",
        //        "2019-01-01T00:00:00.000Z"},
        //    {
        //        "EndDate",
        //        "2019-01-02T00:00:00.000Z"}};

        //public static object BODY = "";

        //public static object X_API_TIMESTAMP = Convert.ToInt32(datetime.timestamp(UNIXTIME)).ToString();

        //public static object query_string = "?" + "&".join((from key in PARAMS.keys()
        //                                                    select (key + "=" + urllib.parse.quote_plus(PARAMS[key]))).ToList());
        ////#'?StartDate=2019-01-01T00%3A00%3A00.000Z&EndDate=2019-01-02T00%3A00%3A00.000Z'

        //public static object msg = X_API_KEY + X_API_TIMESTAMP + METHOD + ENDPOINT + query_string.Count > 1 ? query_string : "" + BODY;
        ////#'8d3e1cbe41f747e2ad5d4b9510ecd55f1649973293GET/api/v2/private/user?StartDate=2019-01-01T00%3A00%3A00.000Z&EndDate=2019-01-02T00%3A00%3A00.000Z'

        //public static object signature_bytes = hmac.@new(SECRET.encode("ascii"), msg.encode("ascii"), digestmod: hashlib.sha256).digest();
        ////#b'\x91\xcf\xf4\xd9+\x85\x04(\xd4t\x7fe\xa7\\9\x83\x02\x98\r\xa92\xdb\xfa)b\x15.T\xbf\x11zU'

        //public static object signature_hex = map("{:02X}".format, signature_bytes);
        ////#<map object at 0x0000021AD98AE108>

        //public static object X_API_SIGN = "".join(signature_hex);
        ////#'91CFF4D92B850428D4747F65A75C398302980DA932DBFA2962152E54BF117A55'

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
    
        //    //#{'Accept': 'application/json', 'Content-Type': 'application/json', 'X-API-KEY': '8d3e1cbe41f747e2ad5d...9510ecd55f', 'X-API-SIGN': '91CFF4D92B850428D474...54BF117A55', 'X-API-TIMESTAMP': '1649973293'}
     

        //public static object r = requests.get(BASE_URL + ENDPOINT, headers: headers, params: PARAMS, data: BODY);

    }


//}
