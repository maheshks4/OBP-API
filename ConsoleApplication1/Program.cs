using Newtonsoft.Json.Linq;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
// You will have to add the System.Web to your References if you are using a
// standard console application template.  

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            OBPHelper ObtainToken = new OBPHelper();

            //step1
           /* Dictionary<string,string> token =  ObtainToken.ObtainRequestToken(oauth_callback:"http://www.bbc.co.uk/news", 
                oauth_consumer_key:"q4vfomz3peqmtoxg0dgs0mg3aafa22a0ku3yei2v",
                oauth_nonce:Guid.NewGuid().ToString(),
                oauth_consumer_secret:"xygpxhs01smnphljh32p4n3b3oud0jclzoy3stbq",                 
                uri:"https://apisandbox.openbankproject.com/oauth/initiate");
            Debug.WriteLine("oauth_token:" + token["oauth_token"] + " oauth_token_secret:" + token["oauth_token_secret"]);
            */
            //oauth_token:IOUAAUIWMTHEW0C0RRQLLWKZTKYRYIIFOTHSJBQL oauth_token_secret:RFCY5KVEFHZFAKIFOLHAQE3HFU4XPJPYTDPW0JMM
            

            //step2: get oauth_verifier from browser
            //https://apisandbox.openbankproject.com/oauth/authorize?oauth_token=IOUAAUIWMTHEW0C0RRQLLWKZTKYRYIIFOTHSJBQL
            //https://apisandbox.openbankproject.com/oauth/thanks?redirectUrl=http%253A%252F%252Fwww.bbc.co.uk%252Fnews%253Foauth_token%253DIOUAAUIWMTHEW0C0RRQLLWKZTKYRYIIFOTHSJBQL%2526oauth_verifier%253D81748

            //step 3 : Converting the request token to an access token
           /* Dictionary<string, string> token = ObtainToken.ConvertRequestTokentoAccessToken(oauth_callback :null, 
                oauth_consumer_key:"q4vfomz3peqmtoxg0dgs0mg3aafa22a0ku3yei2v", 
                oauth_nonce:Guid.NewGuid().ToString(),
                oauth_consumer_secret:"xygpxhs01smnphljh32p4n3b3oud0jclzoy3stbq", 
                oauth_token_secret:"RFCY5KVEFHZFAKIFOLHAQE3HFU4XPJPYTDPW0JMM", 
                uri:"https://apisandbox.openbankproject.com/oauth/token", 
                oauth_verifier:"81748", 
                oauth_token: "IOUAAUIWMTHEW0C0RRQLLWKZTKYRYIIFOTHSJBQL");
            Debug.WriteLine("oauth_token:" + token["oauth_token"] + " oauth_token_secret:" + token["oauth_token_secret"]);
           */
            //oauth_token:VQFOSWCWDYDOHRBJGTQSYXAQTXEN4BOZBSDJFTGK oauth_token_secret:S4GJUNYMBIK1ZXLUC1C1WF2V1WORQWYF3PWYMKND

            //step 4 : Accessing protected resources (GET)
            string Json = ObtainToken.AccessingProtectedResources(oauth_callback: null, 
                oauth_consumer_key: "q4vfomz3peqmtoxg0dgs0mg3aafa22a0ku3yei2v", 
                oauth_nonce: Guid.NewGuid().ToString(),
                oauth_consumer_secret: "xygpxhs01smnphljh32p4n3b3oud0jclzoy3stbq", 
                oauth_token_secret: "S4GJUNYMBIK1ZXLUC1C1WF2V1WORQWYF3PWYMKND", 
                uri:"https://apisandbox.openbankproject.com/obp/v1.2.1/accounts/private", 
                oauth_verifier:null, 
                oauth_token: "VQFOSWCWDYDOHRBJGTQSYXAQTXEN4BOZBSDJFTGK",
                method:"GET");
            Debug.WriteLine(Json);
            //{"accounts":[{"id":"MAH0001","label":null,"views_available":[{"id":"owner","short_name":"Owner","description":"Owner View","is_public":false,"alias":"","hide_metadata_if_alias_used":false,"can_add_comment":true,"can_add_corporate_location":true,"can_add_image":true,"can_add_image_url":true,"can_add_more_info":true,"can_add_open_corporates_url":true,"can_add_physical_location":true,"can_add_private_alias":true,"can_add_public_alias":true,"can_add_tag":true,"can_add_url":true,"can_add_where_tag":true,"can_delete_comment":true,"can_delete_corporate_location":true,"can_delete_image":true,"can_delete_physical_location":true,"can_delete_tag":true,"can_delete_where_tag":true,"can_edit_owner_comment":true,"can_see_bank_account_balance":true,"can_see_bank_account_bank_name":true,"can_see_bank_account_currency":true,"can_see_bank_account_iban":true,"can_see_bank_account_label":true,"can_see_bank_account_national_identifier":true,"can_see_bank_account_number":true,"can_see_bank_account_owners":true,"can_see_bank_account_swift_bic":true,"can_see_bank_account_type":true,"can_see_comments":true,"can_see_corporate_location":true,"can_see_image_url":true,"can_see_images":true,"can_see_more_info":true,"can_see_open_corporates_url":true,"can_see_other_account_bank_name":true,"can_see_other_account_iban":true,"can_see_other_account_kind":true,"can_see_other_account_metadata":true,"can_see_other_account_national_identifier":true,"can_see_other_account_number":true,"can_see_other_account_swift_bic":true,"can_see_owner_comment":true,"can_see_physical_location":true,"can_see_private_alias":true,"can_see_public_alias":true,"can_see_tags":true,"can_see_transaction_amount":true,"can_see_transaction_balance":true,"can_see_transaction_currency":true,"can_see_transaction_description":true,"can_see_transaction_finish_date":true,"can_see_transaction_metadata":true,"can_see_transaction_other_bank_account":true,"can_see_transaction_start_date":true,"can_see_transaction_this_bank_account":true,"can_see_transaction_type":true,"can_see_url":true,"can_see_where_tag":true}],"bank_id":"hsbc-test"}]}


            //step 5 : Accessing protected resources (POST)
            /*string transcation = ObtainToken.AccessingProtectedResources(oauth_callback: null,
                oauth_consumer_key: "q4vfomz3peqmtoxg0dgs0mg3aafa22a0ku3yei2v",
                oauth_nonce: Guid.NewGuid().ToString(),
                oauth_consumer_secret: "xygpxhs01smnphljh32p4n3b3oud0jclzoy3stbq",
                oauth_token_secret: "S4GJUNYMBIK1ZXLUC1C1WF2V1WORQWYF3PWYMKND",
                uri: "https://apisandbox.openbankproject.com/obp/v1.2.1/banks/hsbc-test/accounts/MAH0001/owner/transactions",
                oauth_verifier: null,
                oauth_token: "VQFOSWCWDYDOHRBJGTQSYXAQTXEN4BOZBSDJFTGK",
                method: "POST");
            Debug.WriteLine(transcation);*/


//            string url = "https://apisandbox.openbankproject.com/oauth/authorize?oauth_token=IOUAAUIWMTHEW0C0RRQLLWKZTKYRYIIFOTHSJBQL";
//WebRequest myReq = WebRequest.Create(url);
//string credentials = "mahesh.ks4@gmail.com:TestBank1234@";
//CredentialCache mycache = new CredentialCache();
//myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
//WebResponse wr = myReq.GetResponse();
//Stream receiveStream = wr.GetResponseStream();
//StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
//string content = reader.ReadToEnd();
//Console.WriteLine(content);
//var json = "[" + content + "]"; // change this to array
//var objects = JArray.Parse(json); // parse as array  
//foreach (JObject o in objects.Children<JObject>())
//{
//    foreach (JProperty p in o.Properties())
//    {
//        string name = p.Name;
//        string value = p.Value.ToString();
//        Console.Write(name + ": " + value);
//    }
//}

     

     
            
            Console.ReadLine();
        }     
     
    }
}