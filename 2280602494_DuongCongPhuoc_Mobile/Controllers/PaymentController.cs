using Microsoft.AspNetCore.Mvc;
using _2280602494_DuongCongPhuoc_Mobile.Services;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly VnPayLibrary _vnPayLibrary;
        private readonly string _historyFilePath;

        public PaymentController(IConfiguration configuration, VnPayLibrary vnPayLibrary, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _vnPayLibrary = vnPayLibrary;
            _historyFilePath = Path.Combine(env.ContentRootPath, "payment_history.json");
        }

        [HttpPost("CreatePaymentUrl")]
        public IActionResult CreatePaymentUrl([FromBody] PaymentRequest request)
        {
            var timeNow = DateTime.Now;
            var tick = DateTime.Now.Ticks.ToString();
            
            _vnPayLibrary.AddRequestData("vnp_Version", "2.1.0");
            _vnPayLibrary.AddRequestData("vnp_Command", "pay");
            _vnPayLibrary.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            _vnPayLibrary.AddRequestData("vnp_Amount", ((long)request.Amount * 100).ToString());
            
            _vnPayLibrary.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            _vnPayLibrary.AddRequestData("vnp_CurrCode", "VND");
            _vnPayLibrary.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            _vnPayLibrary.AddRequestData("vnp_Locale", "vn");
            _vnPayLibrary.AddRequestData("vnp_OrderInfo", "Thanh toan chiphi " + tick);
            _vnPayLibrary.AddRequestData("vnp_OrderType", "other");
            _vnPayLibrary.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]);
            _vnPayLibrary.AddRequestData("vnp_TxnRef", tick);

            string paymentUrl = _vnPayLibrary.CreateRequestUrl(_configuration["VnPay:BaseUrl"], _configuration["VnPay:HashSecret"]);

            // Save Initial Pending Transaction
            var transaction = new PaymentTransaction
            {
                TxnRef = tick,
                Amount = request.Amount,
                Status = "Pending",
                CreatedDate = timeNow,
                OrderInfo = "Thanh toán chi phí",
                CustomerName = request.CustomerName ?? "Khách vãng lai",
                OrderDescription = request.OrderDescription ?? "Thanh toán dịch vụ"
            };
            SaveTransaction(transaction);

            return Ok(new { url = paymentUrl, txnRef = tick });
        }

        [HttpGet("Callback")]
        public IActionResult Callback()
        {
            try
            {
                if (Request.Query.Count > 0)
                {
                    string txnRef = Request.Query["vnp_TxnRef"].ToString();
                    string vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();
                    string amount = Request.Query["vnp_Amount"].ToString();
                    
                    var status = (vnp_ResponseCode == "00") ? "Success" : "Failed";
                    
                    // Update Transaction Status
                    UpdateTransactionStatus(txnRef, status);

                    // Beautiful UI
                    string color = (status == "Success") ? "#4CAF50" : "#F44336";
                    string icon = (status == "Success") ? "✔" : "✘";
                    string title = (status == "Success") ? "Thanh toán thành công!" : "Thanh toán thất bại";
                    string message = (status == "Success") 
                        ? "Cảm ơn bạn đã thanh toán. Giao dịch đã được ghi nhận." 
                        : "Có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại.";

                    string htmlContent = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Kết quả thanh toán</title>
                        <style>
                            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; }}
                            .card {{ background: white; padding: 40px; border-radius: 15px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); text-align: center; max-width: 400px; width: 90%; }}
                            .icon-box {{ width: 80px; height: 80px; border-radius: 50%; background-color: {color}; color: white; display: flex; justify-content: center; align-items: center; font-size: 40px; margin: 0 auto 20px auto; }}
                            h1 {{ color: #333; margin-bottom: 10px; font-size: 24px; }}
                            p {{ color: #666; margin-bottom: 30px; line-height: 1.5; }}
                            .btn {{ background-color: #2196F3; color: white; padding: 12px 30px; border: none; border-radius: 25px; text-decoration: none; font-size: 16px; font-weight: bold; transition: background 0.3s; display: inline-block; cursor: pointer; }}
                            .btn:hover {{ background-color: #1976D2; }}
                        </style>
                    </head>
                    <body>
                        <div class='card'>
                            <div class='icon-box'>{icon}</div>
                            <h1>{title}</h1>
                            <p>{message}</p>
                            <a href='#' onclick='window.close();' class='btn'>Quay lại ứng dụng</a>
                        </div>
                    </body>
                    </html>";

                    return Content(htmlContent, "text/html");
                }
            }
            catch (Exception ex)
            {
               return BadRequest($"Error: {ex.Message}");
            }
            return BadRequest();
        }

        [HttpGet("CheckStatus/{txnRef}")]
        public IActionResult CheckStatus(string txnRef)
        {
            var transactions = LoadTransactions();
            var txn = transactions.FirstOrDefault(t => t.TxnRef == txnRef);
            return Ok(new { status = txn?.Status ?? "NotFound" });
        }

        [HttpGet("History")]
        public IActionResult GetHistory()
        {
            var transactions = LoadTransactions().OrderByDescending(t => t.CreatedDate).ToList();
            return Ok(transactions);
        }

        // --- Helper Methods using JSON File persistence ---
        private List<PaymentTransaction> LoadTransactions()
        {
            if (!System.IO.File.Exists(_historyFilePath)) return new List<PaymentTransaction>();
            var json = System.IO.File.ReadAllText(_historyFilePath);
            return System.Text.Json.JsonSerializer.Deserialize<List<PaymentTransaction>>(json) ?? new List<PaymentTransaction>();
        }

        private void SaveTransaction(PaymentTransaction txn)
        {
            var transactions = LoadTransactions();
            if(!transactions.Any(t => t.TxnRef == txn.TxnRef))
            {
                transactions.Add(txn);
                System.IO.File.WriteAllText(_historyFilePath, System.Text.Json.JsonSerializer.Serialize(transactions));
            }
        }

        private void UpdateTransactionStatus(string txnRef, string status)
        {
            var transactions = LoadTransactions();
            var txn = transactions.FirstOrDefault(t => t.TxnRef == txnRef);
            if (txn != null)
            {
                txn.Status = status;
                System.IO.File.WriteAllText(_historyFilePath, System.Text.Json.JsonSerializer.Serialize(transactions));
            }
        }
    }

    public class PaymentTransaction
    {
        public string TxnRef { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; } // Pending, Success, Failed
        public DateTime CreatedDate { get; set; }
        public string OrderInfo { get; set; }
        public string CustomerName { get; set; }
        public string OrderDescription { get; set; }
    }

    public class PaymentRequest
    {
        public double Amount { get; set; }
        public string CustomerName { get; set; }
        public string OrderDescription { get; set; }
    }
}
