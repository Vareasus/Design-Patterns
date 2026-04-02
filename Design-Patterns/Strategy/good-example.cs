// Strategy Design Pattern - Behavioral Category
// Source: salihcantekin/youtube_DesignPatterns_Builder

interface IPaymentService
{
    bool Pay(PaymentOptions paymentOptions);
}

public class PaymentOptions
{
    public string CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public string ExpirationDate { get; set; }
    public string Cvv { get; set; }
    public decimal Amount { get; set; }
}

// === Concrete Strategies ===

public class GarantiBankPaymentService : IPaymentService
{
    public bool Pay(PaymentOptions paymentOptions)
    {
        Console.WriteLine("Garanti Bankası ile ödeme yapıldı.");
        return true;
    }
}

public class YapiKrediBankPaymentService : IPaymentService
{
    public bool Pay(PaymentOptions paymentOptions)
    {
        Console.WriteLine("Yapı Kredi Bankası ile ödeme yapıldı.");
        return true;
    }
}

public class IsBankasiBankPaymentService : IPaymentService
{
    public bool Pay(PaymentOptions paymentOptions)
    {
        Console.WriteLine("İş Bankası ile ödeme yapıldı.");
        return true;
    }
}

// === Context ===

class PaymentService
{
    private IPaymentService paymentService;

    public PaymentService() { }

    public PaymentService(IPaymentService paymentService)
    {
        this.paymentService = paymentService;
    }

    public void SetPaymentService(IPaymentService paymentService)
    {
        this.paymentService = paymentService;
    }

    public bool PayViaStrategy(PaymentOptions options)
    {
        return paymentService.Pay(options);
    }
}

// === Usage ===
// var paymentService = new PaymentService();
// paymentService.SetPaymentService(new GarantiBankPaymentService());
// paymentService.PayViaStrategy(paymentOptions);
//
// Runtime'da banka değiştirilir, PaymentService hiç değişmez.
