using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OnlinePayment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OnlinePayment
{
    public class OnlinePayment
    {
        private readonly PaymentDbContext _context;
        public string RedirectUrl;

        public OnlinePayment(PaymentDbContext context)
        {
            _context = context;
        }



        public async Task<string> RequestSepPayment(Invoice invoice)
        {
            if (RedirectUrl == null)
            {
                throw new InvalidOperationException("Redirect Url for Request Payment Not Set");
            }
            var payment = new Payment()
            {
                Amount = invoice.Amount,
                Payload = invoice.Payload,
                Description = invoice.Description,
                Status = PaymentStatus.Pending,
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //get token
            using (HttpClient client = new HttpClient())
            {
                var uri = new Uri("https://sep.shaparak.ir/onlinepg/onlinepg");

                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                request.Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    action = "token",
                    TerminalId = invoice.MerchantID,
                    Amount = invoice.Amount,
                    ResNum = invoice.Payload,
                    RedirectUrl = RedirectUrl,
                    CellNumber = invoice.PhoneNumber
                }), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sepResponce = JsonConvert.DeserializeObject<SepResponce>(content);

                    if (sepResponce.Status == 1)
                    {
                        //set GetMethod value in the query string to true to get the payment details in the return from gateway like this &GetMethod=true
                        return sepResponce.Token;
                    }
                    else
                    {
                        payment.Status = PaymentStatus.Failed;
                        await _context.SaveChangesAsync();
                        throw new Exception("Request failed: " + sepResponce.ErrorDesc);
                    }

                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                    await _context.SaveChangesAsync();
                    throw new Exception("Request failed with status code: " + response.StatusCode);
                }
            }
        }

        public async Task<string> VerifyPayment(HttpContext httpContext)
        {
            var form = await httpContext.Request.ReadFormAsync();
            var status = int.Parse(form["Status"]);
            //state is status in english
            var state = form["State"][0];
            //resNumber to find the payment
            var resNumber = form["ResNum"][0];
            var payment = _context.Payments.Where(p=>p.Payload == resNumber).SingleOrDefault();

            //refNumber to verify the payment
            var refNumber = form["RefNum"][0];
            if (payment.RefNumber == refNumber)
            {
                throw new Exception("this payment have been made");
            }
            payment.RefNumber = refNumber;

            if (status == 1)
            {
                payment.Status = PaymentStatus.Cancelled;
                await _context.SaveChangesAsync();
                throw new Exception(state);
            }
            else if (status == 2)
            {
                //verify the payment with sep

                var mid = form["MID"][0];
                //traceNumber for the user to have
                var traceNumber = form["TraceNo"][0];

                using (HttpClient client = new HttpClient())
                {
                    var uri = new Uri("https://sep.shaparak.ir/verifyTxnRandomSessionkey/ipg/VerifyTransaction");

                    var request = new HttpRequestMessage(HttpMethod.Post, uri);

                    request.Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        RefNum = refNumber,
                        TerminalNumber = mid,
                    }), Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var sepResponce = JsonConvert.DeserializeObject<SepVerifyResponse>(content);

                        if (sepResponce.Success)
                        {
                            payment.Status = PaymentStatus.Payed;
                            await _context.SaveChangesAsync();
                            return traceNumber;
                        }
                        else
                        {
                            payment.Status = PaymentStatus.Failed;
                            await _context.SaveChangesAsync();
                            throw new Exception(sepResponce.ResultDescription);
                        }

                    }
                    else
                    {
                        payment.Status = PaymentStatus.PendingVerify;
                        await _context.SaveChangesAsync();
                        throw new Exception("Request failed with status code: " + response.StatusCode);
                    }
                }
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                await _context.SaveChangesAsync();
                throw new Exception(state);
            }
        }

        static Dictionary<string, string> ParsePostBody(string postBody)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            // Split the POST body into key-value pairs using '&' as the separator
            string[] pairs = postBody.Split('&');

            foreach (string pair in pairs)
            {
                // Split each key-value pair into key and value using '=' as the separator
                string[] parts = pair.Split('=');

                if (parts.Length == 2)
                {
                    string key = parts[0];
                    string value = parts[1];

                    // Handle URL-encoded values (if necessary)
                    value = HttpUtility.UrlDecode(value);

                    keyValuePairs[key] = value;
                }
            }

            return keyValuePairs;
        }
    }
}
